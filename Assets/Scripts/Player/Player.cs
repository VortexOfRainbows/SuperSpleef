using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class Player : Entity ///Team members that contributed to this script: Ian Bunnell, Sehun Heo, Samuel Gines
{
    #region public
    ///These are public because they need to be accessed outside the class, and they cannot be serialized as properties.
    public GameObject FacingVector;
    public int MyID { get; set; }
    [SerializeField] private Rigidbody RB;
    [SerializeField] private BoxCollider JumpHitbox;
    [SerializeField] private GameObject PlayerVisual;
    #endregion
    private Camera MainCamera => ClientManager.GetCamera(ControlManager.UsingGamepad);
    private Transform CameraTransform => MainCamera.transform;
    private ScreenBlocker ScreenBlocker => ClientManager.GetBlocker(ControlManager.UsingGamepad);
    private GameObject BlockOutline => ClientManager.GetOutline(ControlManager.UsingGamepad);
    /// <summary>
    /// These classes/structs manage the players current control state. This allows us to check for controls more consistently in Fixed Update, and do more precise things with controls.
    /// This is public because it needs to be accessible by other classes
    /// </summary>
    public PlayerControls ControlManager;
    public PlayerControls.ControlDown Control => ControlManager.Control;
    public PlayerControls.ControlDown LastControl => ControlManager.LastControl;
    public const int EntityLayer = 6;
    [SerializeField] private float BlockRange = 4;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float WalkSpeed = 2f;
    [SerializeField] private float WalkSpeedMax = 5;
    [SerializeField] private float SprintMult = 2;
    [SerializeField] private float MoveDeacceleration = 0.5f;
    [SerializeField] private float MoveAcceleration = 0.1f;
    [SerializeField] private float JumpDrag = 0.95f;
    [SerializeField] private bool OnTheFloor = false;
    /// <summary>
    /// How long until the player can use an item. 
    /// Public float since it is used inside ITEM.
    /// </summary>
    private NetworkVariable<float> useTime = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> useTimeMax = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private float itemTime;
    private float itemTimeMax;
    public float ItemUseTime 
    {
        get 
        { 
            return itemTime; 
        }
        set 
        { 
            if (IsOwner)
                useTime.Value = value;
            else
                itemTime = value;
        }
    }
    public float ItemUseTimeMax
    {
        get
        {
            return itemTimeMax;
        }
        set
        {
            if (IsOwner)
                useTimeMax.Value = value;
            else
                itemTimeMax = value;
        }
    }
    public override void OnNetworkSpawn()
    {
        OnStart();
    }
    private void Start()
    {
        OnStart();
    }
    private void OnStart()
    {
        useTime.OnValueChanged += delegate { itemTime = useTime.Value; };
        useTimeMax.OnValueChanged += delegate { itemTimeMax = useTimeMax.Value; };
        ItemUseTimeMax = Item.DefaultItemFirerate;
        bool apocalypse = GameStateManager.Mode == GameModeID.Apocalypse || GameStateManager.Mode == GameModeID.LaserBattleApocalypse;
        bool giveLaser = GameStateManager.LocalMultiplayer || GameStateManager.Mode == GameModeID.LaserBattleApocalypse || GameStateManager.Mode == GameModeID.LaserBattle;
        currentPlayerHP = maxPlayerHP;
        Inventory = new Inventory(30);
        int StartingItemCount = 20;
        if(GameStateManager.Mode == GameModeID.Creative)
        {
            StartingItemCount = Item.DefaultMaxCount;
        }
        if(giveLaser) //These if statements are very deliberately placed, since i want items to be in the hotbar in a certain order...
        {
            StartingItemCount = 100;
        }
        Inventory.AddItem(new BasicBlaster());
        if (giveLaser) 
        {
            Inventory.AddItem(new LaserCannon());
        }
        if (apocalypse)
        {
            Inventory.AddItem(new BlockGun());
        }
        if (giveLaser) //That is why these if statements are seperate, despite involving the same boolean expression.
        {
            Inventory.AddItem(new PlaceableBlock(ControlManager.UsingGamepad ? BlockID.BlueBricks : BlockID.YellowBricks, StartingItemCount));
        }
        else
        {
            for (int i = 1; i < BlockID.Max; i++) //Initializies a basic inventory for the purpose of testing
            {
                Inventory.AddItem(new PlaceableBlock(i, StartingItemCount));
            }
        }
        RB.maxDepenetrationVelocity = 0;
        transform.position = new Vector3(Chunk.Width * GameStateManager.WorldSizeOverride / 2f, transform.position.y, Chunk.Width * GameStateManager.WorldSizeOverride / 2f); //Centers the player in teh world when they spawn in

        //PlayerVisual.GetComponent<PlayerAnimator>().SetSprite();
    }
    private NetworkVariable<int> selectedItem = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public int SelectedItem 
    { 
        get 
        {
            return selectedItem.Value;
        }
        set 
        {
            if (IsOwner)
                selectedItem.Value = value;
        }
    }
    public Item HeldItem()
    {
        return Inventory.Get(SelectedItem);
    }
    private float PreviousYVelocity = 0;
    private bool HasBeenAddedToPlayerList = false;
    private bool ThirdPersonCamera = false;
    private void Update()
    {
        if(!HasBeenAddedToPlayerList && !GameStateManager.Players.Contains(this))
        {
            MyID = GameStateManager.Players.Count;
            GameStateManager.Players.Add(this);
            HasBeenAddedToPlayerList = true;
        }
        ControlManager.OnUpdate();
        if (!GameStateManager.GameIsPausedOrOver)
        {
            HotbarControls();
            MouseControls(); //Should be updated immediately so players see the effects of their rotation at the same pace as their refresh rate.
            BlockCollisionCheck(); //Should be updated in here so collision is always up to date.
        }
        if (IsOwner || !NetHandler.Active)
        {
            if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftAlt)) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.LeftControl)))
            {
                ThirdPersonCamera = !ThirdPersonCamera;
            }
            bool doNotChangeCamera = ThirdPersonCamera && Input.GetKey(KeyCode.LeftControl);
            FacingVector.transform.rotation = Quaternion.Euler(Direction.x, Direction.y, 0f);
            FacingVector.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            if (!doNotChangeCamera)
            {
                CameraTransform.rotation = FacingVector.transform.rotation;
                CameraTransform.position = FacingVector.transform.position;
                if (ThirdPersonCamera)
                {
                    CameraTransform.position -= FacingVector.transform.forward * 5;
                }
            }
            //Can only see your own player model if in third person camera
            PlayerVisual.SetActive(ThirdPersonCamera);
        }
        else
        {
            PlayerVisual.SetActive(true);
            //Visual is always visible to other players
        }
        if (currentPlayerHP <= 0f) // If the player's current HP reaches zero, or if the player falls too far down the world...
        {
            OnDeath(); // Trigger the Death Behavior of the character
        }
        else
        {
            if (transform.position.y < World.OutOfBounds)
            {
                currentPlayerHP -= DamageFromVoid * Time.deltaTime;
            }
        }

        if (IsOwner || !NetHandler.Active)
            ClientManager.GetInventoryInterface().SetActive(true);
    }
    /// <summary>
    /// This field is only serialized so it can display in the editor. It is not actually meant to be modified
    /// </summary>
    [SerializeField] private Vector2 perpendicularVelocity;
    public override void OnFixedUpdate()
    {
        MovingForward = MovingLeft = MovingRight = MovingBackward = false;
        //Movement should be updated in fixed update so it works probably on all systems
        perpendicularVelocity = new Vector2(RB.velocity.x, RB.velocity.z).RotatedBy(Direction.y * Mathf.Deg2Rad); //Speed is modified as a perpendicular value to make stopping and switching directions more smooth and consistent. Also allows modifying speed more easily.
        float speed = WalkSpeed * MoveAcceleration;
        float maxSpeed = WalkSpeedMax;
        if (Control.Shift)
        {
            maxSpeed *= SprintMult;
        }
        if (Control.Forward) 
        {
            MovingForward = true;
            perpendicularVelocity.y += speed * Mathf.Abs(Control.YMove);
        }
        else if (perpendicularVelocity.y > 0)
                perpendicularVelocity.y *= MoveDeacceleration;
        if (Control.Left)
        {
            MovingLeft = true;
            perpendicularVelocity.x -= speed * Mathf.Abs(Control.XMove);
        }
        else if (perpendicularVelocity.x < 0)
                perpendicularVelocity.x *= MoveDeacceleration;
        if (Control.Back)
        {
            MovingBackward = true;
            perpendicularVelocity.y -= speed * Mathf.Abs(Control.YMove);
        }
        else if (perpendicularVelocity.y < 0)
                perpendicularVelocity.y *= MoveDeacceleration;
        if (Control.Right)
        {
            MovingRight = true;
            perpendicularVelocity.x += speed * Mathf.Abs(Control.XMove);
        }
        else if (perpendicularVelocity.x > 0)
                perpendicularVelocity.x *= MoveDeacceleration;
        perpendicularVelocity = perpendicularVelocity.RotatedBy(Direction.y * -Mathf.Deg2Rad);
        
        Vector3 velo = new Vector3(perpendicularVelocity.x, RB.velocity.y, perpendicularVelocity.y);
        Vector2 velocityXZ = new Vector2(velo.x, velo.z);

        if(velocityXZ.magnitude > maxSpeed)
        {
            velocityXZ = velocityXZ.normalized * maxSpeed;
        }
        if (Control.Jump && OnTheFloor)
        {
            //RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y + 1, RB.velocity.z);
            velo.y *= 0.0f;
            velo.y += jumpForce;
        }
        OnTheFloor = false;
        velo.x = velocityXZ.x;
        velo.z = velocityXZ.y;
        perpendicularVelocity = velocityXZ;
        RB.velocity = velo;
        if (IsOwner)
            Velocity.Value = velo;
        if (RB.velocity.y > 0)
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y * JumpDrag, RB.velocity.z);
            RB.maxDepenetrationVelocity = 10;
        }
        else if (RB.velocity.y < 0 || (RB.velocity.y == 0 && PreviousYVelocity > 0))
        {
            RB.maxDepenetrationVelocity = 10;
        }
        PreviousYVelocity = RB.velocity.y;
        HeldItemUpdate(); //Item updates should be considered on fixed updated so they update in time with physics

        //RB.MovePosition(RB.velocity * Time.fixedDeltaTime);

        ControlManager.OnFixedUpdate();
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerCollision(other);
    }
    private void OnTriggerStay(Collider other)
    {
        OnTriggerCollision(other);
    }
    private void OnCollision(Collision collision)
    {
        //This method of checking for ground collision sometimes bugs out... Making you unable to jump for some time
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("InverseCubeBottom"))
        {
            if (collision.impulse.y > 0 && RB.velocity.y < 0) //This checks if the player is touching the top surface of a block (so the player can't jump off of walls)
                OnTheFloor = true;
        }
    }
    private void OnTriggerCollision(Collider other)
    {
        if ((other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("InverseCubeBottom"))) //There is a trigger collider on the bottom of the player to check for ground collision. This collider is smaller than the player
        {
            if(RB.velocity.y == 0)
                OnTheFloor = true;
        }
    }
    public Vector2 GetFacingDirection()
    {
        return Direction;
    }
    private Vector2 Direction = Vector2.zero;
    /// <summary>
    /// Updates the rotation of the camera to match with the movement of the mouse
    /// </summary>
    private void MouseControls()
    {
        float mouseX = Control.XAxis * Time.deltaTime * PlayerControls.DefaultMouseSensitivity;
        float mouseY = Control.YAxis * Time.deltaTime * PlayerControls.DefaultMouseSensitivity;
        Direction.y += mouseX;
        Direction.x -= mouseY;
        Direction.x = Mathf.Clamp(Direction.x, -90f, 90f);
    }
    /// <summary>
    /// Manages which item is selected by the player
    /// </summary>
    public void HotbarControls()
    {
        if(Control.Hotkey1 && !LastControl.Hotkey1) //Structs can't store arrays. This means that it cannot store hotkey numbers, so the only way I can do a pattern like this is through a tone of if statements. I would use a for loop, if I could. I might refactor later in order to fix this.
            SelectedItem = 0;
        if (Control.Hotkey2 && !LastControl.Hotkey2)
            SelectedItem = 1;
        if (Control.Hotkey3 && !LastControl.Hotkey3)
            SelectedItem = 2;
        if (Control.Hotkey4 && !LastControl.Hotkey4)
            SelectedItem = 3;
        if (Control.Hotkey5 && !LastControl.Hotkey5)
            SelectedItem = 4;
        if (Control.Hotkey6 && !LastControl.Hotkey6)
            SelectedItem = 5;
        if (Control.Hotkey7 && !LastControl.Hotkey7)
            SelectedItem = 6;
        if (Control.Hotkey8 && !LastControl.Hotkey8)
            SelectedItem = 7;
        if (Control.Hotkey9 && !LastControl.Hotkey9)
            SelectedItem = 8;
        if (Control.Hotkey0 && !LastControl.Hotkey0)
            SelectedItem = 9;
        int newSelectedItem = SelectedItem - Mathf.RoundToInt(Control.ScrollDelta);
        newSelectedItem = newSelectedItem % 10;
        if (newSelectedItem < 0)
            newSelectedItem = 10 + newSelectedItem;
        SelectedItem = newSelectedItem;
    }
    /// <summary>
    /// Manages the players item usage
    /// </summary>
    private void HeldItemUpdate()
    {
        Item heldItem = HeldItem();
        if (heldItem.Count <= 0)
        {
            Inventory.Set(SelectedItem, Item.Empty);
        }
        bool left = Control.LeftClick;
        bool right = Control.RightClick;
        if (ItemUseTime > 0)
        {
            left = right = false; //Do not consider an input if the item timer is up
        }
        bool holdingClick = (Control.LeftClick && LastControl.LeftClick) || (Control.RightClick && LastControl.RightClick);
        ///Debug.Log(ItemUseTimeMax);
        if(ItemUseTime > -ItemUseTimeMax)
            ItemUseTime--;
        //This is to give the same effect as in minecraft, where you can shoot faster while spamming than while holding...
        //Might have to reimplement this system in a way that does not interfere with animation smoothness
        //if (holdingClick)
        //    ItemUseTime--;
        //else
        //    ItemUseTime = -1;
        bool itemUsed = false;
        RaycastHit hitInfo;
        Vector3 TargetPosition = transform.position + new Vector3(0, 0.5f, 0);
        bool blocksWereModified = false;
        bool DoBlockCheck = false;
        bool holdingPlaceableBlock = false;
        int blockToPlace = BlockID.Air;
        if (heldItem is PlaceableBlock block)
        {
            blockToPlace = block.PlaceID;
            holdingPlaceableBlock = true;
        }
        int blockType = World.Block(TargetPosition);
        if (blockType != BlockID.Air) //Checks if the player is inside a block
        {
            DoBlockCheck = true;
        }
        else
        {
            if (Physics.Raycast(FacingVector.transform.position, FacingVector.transform.forward, out hitInfo, BlockRange * 2, -1, QueryTriggerInteraction.Ignore)) //Twice the block range so we can hit all blocks within the range
            {
                Vector3 hitPoint = hitInfo.point;
                Vector3 InsideBlock = hitPoint - hitInfo.normal * 0.1f;
                Vector3 ToHitPoint = new Vector3(Mathf.FloorToInt(InsideBlock.x) + 0.5f, Mathf.FloorToInt(InsideBlock.y) + 0.5f, Mathf.FloorToInt(InsideBlock.z) + 0.5f) - transform.position;
                if (ToHitPoint.magnitude <= BlockRange)
                {
                    if (!right && hitInfo.collider.gameObject.tag != "InverseCube")
                    {
                        TargetPosition = InsideBlock;
                    }
                    else
                    {
                        TargetPosition = hitPoint + hitInfo.normal * 0.1f;
                    }
                    DoBlockCheck = true;
                }
            }
        }
        if (DoBlockCheck)
        {
            Vector3 centerOfBlock = new Vector3(Mathf.FloorToInt(TargetPosition.x) + 0.5f, Mathf.FloorToInt(TargetPosition.y) + 0.5f, Mathf.FloorToInt(TargetPosition.z) + 0.5f);
            bool updateBlockOutline = true;
            if (left) //Destroy a block with left click
            {
                blocksWereModified = World.SetBlock(TargetPosition, BlockID.Air, generateSound: true);
                updateBlockOutline = blocksWereModified;
            } 
            else if (holdingPlaceableBlock && right && World.Block(TargetPosition) == BlockID.Air && blockToPlace != BlockID.Air) //Place a block with right click
            {
                if(heldItem.UseSecondary(this))
                {
                    Collider[] inBlockPosition = Physics.OverlapBox(centerOfBlock, new Vector3(0.48f, 0.48f, 0.48f));
                    bool NoEntities = inBlockPosition.Count(item => item.gameObject.layer == EntityLayer) <= 0;
                    if (NoEntities)
                    {
                        bool placedBlocks = World.SetBlock(TargetPosition, blockToPlace, generateSound: true);
                        updateBlockOutline = placedBlocks;
                        if (placedBlocks)
                        {
                            itemUsed = true;
                            blocksWereModified = true;
                        }
                    }
                    else
                        updateBlockOutline = false;
                }
            }
            if (updateBlockOutline && (IsOwner || !NetHandler.Active))
            {
                if (World.Block(TargetPosition) == BlockID.Air)
                {
                    BlockOutline.SetActive(false);
                }
                else
                {
                    BlockOutline.transform.position = centerOfBlock;
                    BlockOutline.SetActive(true);
                }
            }
        }
        else if (IsOwner || !NetHandler.Active)
        {
            BlockOutline.SetActive(false);
        }
        if(!blocksWereModified) //Moves the block outline to the block the player is facing
        {
            if(left)
            {
                if(heldItem.UsePrimary(this))
                {
                    itemUsed = true;
                }
            }
            if(right && !holdingPlaceableBlock)
            {
                if (heldItem.UseSecondary(this))
                {
                    itemUsed = true;
                }
            }
        }
        else //if blocks WERE modified
        {
            if(left)
            {
                ItemUseTime = ItemUseTimeMax = Item.DefaultItemFirerate;
            }
        }
        if(itemUsed)
        {
            if(heldItem.IsConsumedOnUse(this))
            {
                heldItem.ModifyCount(-1);
                if(IsOwner)
                {
                    GameStateManager.NetData.SyncInventoryItemRpc(MyID, SelectedItem, heldItem.Count);
                }
            }
            if (heldItem.Firerate <= 0)
            {
                ItemUseTime = ItemUseTimeMax = Item.DefaultItemFirerate;
            }
            else
                ItemUseTime = ItemUseTimeMax = heldItem.Firerate;
        }
    }
    [SerializeField] private GameObject InBlockColliderTemplate;
    private GameObject InBlockColliderTop, InBlockColliderBottom;
    private void InitializeInBlockColliders()
    {
        InBlockColliderBottom = Instantiate(InBlockColliderTemplate, null);
        InBlockColliderTop = Instantiate(InBlockColliderTemplate, null);
        InBlockColliderBottom.name = "BottomInBlockCollider";
        InBlockColliderTop.name = "TopInBlockCollider";
        InBlockColliderBottom.GetComponent<BarrierBlock>().IgnoreTop = true;
        InBlockColliderTop.GetComponent<BarrierBlock>().IgnoreBot = true;
    }
    /// <summary>
    /// Updates the colliders that surround the player to make sure they can't fall out of the world if they clip out of the chunk's mesh
    /// </summary>
    private void BlockCollisionCheck()
    {
        if(InBlockColliderTop == null && InBlockColliderBottom == null)
        {
            InitializeInBlockColliders();
        }
        Vector3 topAsInt = new Vector3(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y + 0.5f), Mathf.FloorToInt(transform.position.z));
        InBlockColliderTop.transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y + 0.5f), Mathf.FloorToInt(transform.position.z) + 0.5f);
        InBlockColliderBottom.transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y + 0.5f) - 1, Mathf.FloorToInt(transform.position.z) + 0.5f);
        InBlockColliderTop.GetComponent<BarrierBlock>().UpdateCollision();
        InBlockColliderBottom.GetComponent<BarrierBlock>().UpdateCollision();
        if (!NetHandler.Active || IsOwner)
        {
            ScreenBlocker.UpdateUVS(World.Block(topAsInt));
        }
    }
    public const float DamageFromVoid = 200f;
    private const float maxPlayerHP = 100; // Assigns the Max HP of the player
    private float currentPlayerHP = maxPlayerHP; // Assigns the current HP of the character
    private float deathAnimTimer = 0.0f; // Artifical Stopwatch
    private void OnDeath()
    {
        deathAnimTimer += Time.deltaTime; // Start the stopwatch
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 90), deathAnimTimer * deathAnimTimer * 9); // Interpolate the player and Rotate the character 90 degrees
        if(!NetHandler.Active)
        {
            if (Time.timeScale >= 1)
            {
                //Debug.Log(currentPlayerHP);
                string DeathText = GameStateManager.DefaultGameOverText;
                Color deathColor = Color.white;
                if (GameStateManager.LocalMultiplayer)
                {
                    if (ControlManager.UsingGamepad)
                    {
                        //blue player uses gamepad
                        DeathText = "Yellow Wins";
                        deathColor = Color.yellow;
                    }
                    else
                    {
                        //yellow player uses gamepad
                        DeathText = "Blue Wins";
                        deathColor = Color.blue;
                    }
                }
                GameStateManager.EndGame(DeathText, deathColor);
            }
        }
        else
        {
            if(NetworkManager.IsServer)
            {
                Destroy(gameObject);
                Destroy(InBlockColliderTop);
                Destroy(InBlockColliderBottom);
            }
        }
    }
    public override void OnDestroy()
    {
        GameStateManager.Players.Remove(this);
    }
}
