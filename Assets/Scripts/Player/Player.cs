using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    /// <summary>
    /// These classes/structs manage the players current control state. This allows us to check for controls more consistently in Fixed Update, and do more precise things with controls
    /// </summary>
    [SerializeField] private PlayerControls ControlManager;
    public PlayerControls.ControlDown Control => ControlManager.Control;
    public PlayerControls.ControlDown LastControl => ControlManager.LastControl;

    [SerializeField] private float Sensitivity = 1;
    [SerializeField] private Rigidbody RB;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private ScreenBlocker ScreenBlocker;
    public const int EntityLayer = 6;
    [SerializeField] private float BlockRange = 4;
    private void Awake()
    {
        Inventory = new Inventory(30);
        for(int i = 0; i < Inventory.Count; i++)
        {
            Inventory.Set(i, new PlaceableBlock(BlockID.Grass));
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
    public void FixedUpdate()
    {
        //Movement should be updated in fixed update so it works probably on all systems
        Vector2 moveDir = Vector2.zero;
        if (Control.Forward) 
        {
            moveDir.y += 1;
        }
        if (Control.Left)
        {
            moveDir.x -= 1;
        }
        if (Control.Back)
        {
            moveDir.y -= 1;
        }
        if (Control.Right)
        {
            moveDir.x += 1;
        }
        moveDir = moveDir.RotatedBy(Direction.y * -Mathf.Deg2Rad);
        RB.velocity = new Vector3(RB.velocity.x + moveDir.x, RB.velocity.y, RB.velocity.z + moveDir.y);
        if (Control.Jump)
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y + 1, RB.velocity.z);
        }
        RB.velocity = new Vector3(RB.velocity.x * 0.9f, RB.velocity.y, RB.velocity.z * 0.9f);
        if (RB.velocity.y > 0)
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y * 0.95f, RB.velocity.z);
            RB.maxDepenetrationVelocity = 0;
        }
        else if (RB.velocity.y < 0 || (RB.velocity.y == 0 && PreviousYVelocity > 0))
        {
            RB.maxDepenetrationVelocity = 10;
        }
        PreviousYVelocity = RB.velocity.y;
        HeldItemUpdate(); //Item updates should be considered on fixed updated so they update in time with physics
        ControlManager.OnFixedUpdate();
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

        //transform.rotation = Quaternion.Euler(0, Direction.y, 0f);
        CameraTransform.rotation = Quaternion.Euler(Direction.x, Direction.y, 0f);

        CameraTransform.position = transform.position + new Vector3(0, 0.5f, 0);

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
            if(holdingPlaceableBlock)
            {
                if (left)
                {
                    updateBlockOutline = World.SetBlock(TargetPosition, 0);
                }
                else if (right && World.Block(TargetPosition) == BlockID.Air && blockToPlace != BlockID.Air)
                {
                    Collider[] inBlockPosition = Physics.OverlapBox(centerOfBlock, new Vector3(0.49f, 0.49f, 0.49f));
                    if (inBlockPosition.Count(item => item.gameObject.layer == EntityLayer) <= 0)
                    {
                        bool successfullyPlacedBlock = World.SetBlock(TargetPosition, blockToPlace);
                        updateBlockOutline = successfullyPlacedBlock;
                        if(successfullyPlacedBlock)
                        {
                            itemUsed = true;
                        }
                    }
                    else
                        updateBlockOutline = false;
                }
            }
            if(updateBlockOutline)
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
        if(itemUsed)
        {
            if(heldItem.IsConsumedOnUse())
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
