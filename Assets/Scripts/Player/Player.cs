using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    #region public
    ///These are public because they need to be accessed outside the class, and they cannot be serialized as properties.
    public GameObject BasicProjectileTest;
    public Rigidbody RB;
    public GameObject FacingVector;
    #endregion

    /// <summary>
    /// These classes/structs manage the players current control state. This allows us to check for controls more consistently in Fixed Update, and do more precise things with controls
    /// </summary>
    [SerializeField] private PlayerControls ControlManager;
    public PlayerControls.ControlDown Control => ControlManager.Control;
    public PlayerControls.ControlDown LastControl => ControlManager.LastControl;

    [SerializeField] private float Sensitivity = 1;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private ScreenBlocker ScreenBlocker;
    public const int EntityLayer = 6;
    [SerializeField] private float BlockRange = 4;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float WalkSpeed = 2f;
    [SerializeField] private float WalkSpeedMax = 5;
    [SerializeField] private float SprintMult = 2;
    [SerializeField] private float MoveDeacceleration = 0.5f;
    [SerializeField] private float MoveAcceleration = 0.1f;
    [SerializeField] private float JumpDrag = 0.95f;
    private bool OnTheFloor = false;

    private void Start()
    {
        Inventory = new Inventory(30);
        for(int i = 0; i < Inventory.Count; i++)
        {
            if(i == 0)
                Inventory.Set(i, new BasicBlaster());
            else if(i <= BlockID.Max)
                Inventory.Set(i, new PlaceableBlock(i, 20));
            else
                Inventory.Set(i, new PlaceableBlock(BlockID.Dirt));
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        RB.maxDepenetrationVelocity = 0;
    }
    public int SelectedItem { get; private set; }
    public Item HeldItem()
    {
        return Inventory.Get(SelectedItem);
    }
    private float PreviousYVelocity = 0;
    private void Update()
    {
        ControlManager.OnUpdate();
        HotbarControls();
        MouseControls(); //Should be updated immediately so players see the effects of their rotation at the same pace as their refresh rate.
        BlockCollisionCheck(); //Should be updated in here so collision is always up to date.
    }
    /// <summary>
    /// This field is only serialized so it can display in the editor. It is not actually meant to be modified
    /// </summary>
    [SerializeField] private Vector2 perpendicularVelocity;
    public void FixedUpdate()
    {
        //Movement should be updated in fixed update so it works probably on all systems
        perpendicularVelocity = new Vector2(RB.velocity.x, RB.velocity.z).RotatedBy(Direction.y * Mathf.Deg2Rad);
        float speed = WalkSpeed * MoveAcceleration;
        float maxSpeed = WalkSpeedMax;
        if (Control.Shift)
        {
            maxSpeed *= SprintMult;
        }
        if (Control.Forward) 
        {
            perpendicularVelocity.y += speed;
        }
        else
        {
            if (perpendicularVelocity.y > 0)
                perpendicularVelocity.y *= MoveDeacceleration;
        }
        if (Control.Left)
        {
            perpendicularVelocity.x -= speed;
        }
        else
        {
            if (perpendicularVelocity.x < 0)
                perpendicularVelocity.x *= MoveDeacceleration;
        }
        if (Control.Back)
        {
            perpendicularVelocity.y -= speed;
        }
        else
        {
            if (perpendicularVelocity.y < 0)
                perpendicularVelocity.y *= MoveDeacceleration;
        }
        if (Control.Right)
        {
            perpendicularVelocity.x += speed;
        }
        else
        {
            if (perpendicularVelocity.x > 0)
                perpendicularVelocity.x *= MoveDeacceleration;
        }
        perpendicularVelocity = perpendicularVelocity.RotatedBy(Direction.y * -Mathf.Deg2Rad);
        
        Vector3 velo = new Vector3(perpendicularVelocity.x, RB.velocity.y, perpendicularVelocity.y);
        Vector2 velocityXZ = new Vector2(velo.x, velo.z);

        if(velocityXZ.magnitude > maxSpeed)
        {
            velocityXZ = velocityXZ.normalized * maxSpeed;
        }
        if (Control.Jump && !LastControl.Jump && OnTheFloor)
        {
            //RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y + 1, RB.velocity.z);
            velo.y *= 0.0f;
            velo.y += jumpForce;
            OnTheFloor = false;
        }
        velo.x = velocityXZ.x;
        velo.z = velocityXZ.y;
        perpendicularVelocity = velocityXZ;
        RB.velocity = velo;

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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if(collision.impulse.y > 0)
                OnTheFloor = true;
        }
    }

    private Vector2 Direction = Vector2.zero;
    /// <summary>
    /// Updates the rotation of the camera to match with the movement of the mouse
    /// </summary>
    private void MouseControls()
    {
        float mouseX = Control.XAxis * Time.deltaTime * Sensitivity;
        float mouseY = Control.YAxis * Time.deltaTime * Sensitivity;
        Direction.y += mouseX;
        Direction.x -= mouseY;

        Direction.x = Mathf.Clamp(Direction.x, -90f, 90f);

        CameraTransform.rotation = FacingVector.transform.rotation = Quaternion.Euler(Direction.x, Direction.y, 0f);
        CameraTransform.position = FacingVector.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        //transform.rotation = Quaternion.Euler(0, Direction.y, 0f);
    }
    public void HotbarControls()
    {
        if(Control.Hotkey1 && !LastControl.Hotkey1)
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
    [SerializeField] private GameObject BlockOutline;
    private void HeldItemUpdate() 
    {
        Item heldItem = HeldItem();
        bool left = Control.LeftClick && !LastControl.LeftClick;
        bool right = Control.RightClick && !LastControl.RightClick;
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
            if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hitInfo, BlockRange * 2)) //Twice the block range so we can hit all blocks within the range
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
            if (left)
            {
                blocksWereModified = World.SetBlock(TargetPosition, 0);
                updateBlockOutline = blocksWereModified;
            } 
            else if (holdingPlaceableBlock && heldItem.OnSecondaryUse(this) && right && World.Block(TargetPosition) == BlockID.Air && blockToPlace != BlockID.Air)
            {
                Collider[] inBlockPosition = Physics.OverlapBox(centerOfBlock, new Vector3(0.49f, 0.49f, 0.49f));
                if (inBlockPosition.Count(item => item.gameObject.layer == EntityLayer) <= 0)
                {
                    bool placedBlocks = World.SetBlock(TargetPosition, blockToPlace);
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
            if (updateBlockOutline)
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
        else
        {
            BlockOutline.SetActive(false);
        }
        if(!blocksWereModified)
        {
            if(left)
            {
                if(heldItem.OnPrimaryUse(this))
                {
                    itemUsed = true;
                }
            }
            if(right && !holdingPlaceableBlock)
            {
                if (heldItem.OnSecondaryUse(this))
                {
                    itemUsed = true;
                }
            }
        }
        if(itemUsed)
        {
            if(heldItem.IsConsumedOnUse(this))
            {
                heldItem.ModifyCount(-1);
                if(heldItem.Count <= 0)
                {
                    Inventory.Set(SelectedItem, Item.Empty);
                }
            }
        }
    }
    [SerializeField] private GameObject InBlockColliderTop;
    [SerializeField] private GameObject InBlockColliderBottom;
    /// <summary>
    /// Updates the colliders that surround the player to make sure they can't fall out of the world if they clip out of the chunk's mesh
    /// </summary>
    private void BlockCollisionCheck()
    {
        Vector3 topAsInt = new Vector3(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y + 0.5f), Mathf.FloorToInt(transform.position.z));
        InBlockColliderTop.transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y + 0.5f), Mathf.FloorToInt(transform.position.z) + 0.5f);
        InBlockColliderBottom.transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y + 0.5f) - 1, Mathf.FloorToInt(transform.position.z) + 0.5f);
        InBlockColliderTop.GetComponent<BarrierBlock>().UpdateCollision();
        InBlockColliderBottom.GetComponent<BarrierBlock>().UpdateCollision();
        ScreenBlocker.UpdateUVS(World.Block(topAsInt));
    }
}
