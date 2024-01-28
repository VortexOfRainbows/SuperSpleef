using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float Sensitivity = 1;
    [SerializeField] private Rigidbody RB;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private ScreenBlocker ScreenBlocker;
    public const int EntityLayer = 6;
    [SerializeField] private float BlockRange = 4;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        RB.maxDepenetrationVelocity = 0;
    }
    private float PreviousYVelocity = 0;
    private void Update()
    {
        Vector2 moveDir = Vector2.zero;
        if(Input.GetKey(KeyCode.W))
        {
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x -= 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x += 1;
        }
        moveDir = moveDir.RotatedBy(Direction.y * -Mathf.Deg2Rad);
        RB.velocity = new Vector3(RB.velocity.x + moveDir.x, RB.velocity.y, RB.velocity.z + moveDir.y);
        if (Input.GetKey(KeyCode.Space))
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y + 1, RB.velocity.z);
        }
        RB.velocity = new Vector3(RB.velocity.x * 0.9f, RB.velocity.y, RB.velocity.z * 0.9f);
        if(RB.velocity.y > 0)
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y * 0.95f, RB.velocity.z);
            RB.maxDepenetrationVelocity = 0;
        }
        else if(RB.velocity.y < 0 || (RB.velocity.y == 0 && PreviousYVelocity > 0))
        {
            RB.maxDepenetrationVelocity = 10;
        }
        PreviousYVelocity = RB.velocity.y;
        CameraControls();
        MouseControls();
        BlockCollisionCheck();
    }
    private Vector2 Direction = Vector2.zero;
    /// <summary>
    /// Updates the rotation of the camera to match with the movement of the mouse
    /// </summary>
    private void CameraControls()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * Sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Sensitivity;
        Direction.y += mouseX;
        Direction.x -= mouseY;

        Direction.x = Mathf.Clamp(Direction.x, -90f, 90f);

        //transform.rotation = Quaternion.Euler(0, Direction.y, 0f);
        CameraTransform.rotation = Quaternion.Euler(Direction.x, Direction.y, 0f);

        CameraTransform.position = transform.position + new Vector3(0, 0.5f, 0);
    }
    [SerializeField] GameObject BlockOutline;
    private void MouseControls()
    {
        bool left = Input.GetMouseButtonDown(0);
        bool right = Input.GetMouseButtonDown(1);
        RaycastHit hitInfo;
        Vector3 TargetPosition = transform.position + new Vector3(0, 0.5f, 0);
        bool activate = false;
        int blockType = World.Block(TargetPosition);
        if (blockType != BlockID.Air) //Checks if the player is inside a block
        {
            activate = true;
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
                    activate = true;
                }
            }
        }
        if (activate)
        {
            Vector3 centerOfBlock = new Vector3(Mathf.FloorToInt(TargetPosition.x) + 0.5f, Mathf.FloorToInt(TargetPosition.y) + 0.5f, Mathf.FloorToInt(TargetPosition.z) + 0.5f);
            bool ShouldUpdateBlockOutline = true;
            if (left)
            {
                ShouldUpdateBlockOutline = World.SetBlock(TargetPosition, 0);
            }
            else if (right && World.Block(TargetPosition) == BlockID.Air)
            {
                Collider[] inBlockPosition = Physics.OverlapBox(centerOfBlock, new Vector3(0.49f, 0.49f, 0.49f));
                if (inBlockPosition.Count(item => item.gameObject.layer == EntityLayer) <= 0)
                {
                    ShouldUpdateBlockOutline = World.SetBlock(TargetPosition, 1);
                }
                else
                    ShouldUpdateBlockOutline = false;
            }
            if(ShouldUpdateBlockOutline)
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
