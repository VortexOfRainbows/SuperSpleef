using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float Sensitivity = 1;
    public Rigidbody RB;
    public Transform CameraTransform;
    public ScreenBlocker ScreenBlocker;
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
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
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y * 0.95f, RB.velocity.z);

        CameraControls();
        MouseControls();
        BlockCollisionCheck();
    }
    Vector2 Direction = Vector2.zero;
    private void CameraControls()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * Sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Sensitivity;
        Direction.y += mouseX;
        Direction.x -= mouseY;

        Direction.x = Mathf.Clamp(Direction.x, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, Direction.y, 0f);
        CameraTransform.rotation = Quaternion.Euler(Direction.x, Direction.y, 0f);

        CameraTransform.position = transform.position + new Vector3(0, 0.5f, 0);
    }
    private void MouseControls()
    {
        bool left = Input.GetMouseButtonDown(0);
        bool right = Input.GetMouseButtonDown(1);
        if(left || right)
        {
            //Debug.Log("attemptRaycast");
            RaycastHit hitInfo;
            Vector3 pointOfTargetBlock = transform.position + new Vector3(0, 0.5f, 0);
            bool activate = false;
            int blockType = World.Block(pointOfTargetBlock);
            if (blockType != BlockID.Air) //Checks if the player is inside a block
            {
                activate = true;
            }
            else
            {
                if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hitInfo, 128f))
                {
                    if (left && hitInfo.collider.gameObject.tag != "InverseCube")
                    {
                        pointOfTargetBlock = hitInfo.point + CameraTransform.forward * 0.01f;
                    }
                    else
                    {
                        pointOfTargetBlock = hitInfo.point - CameraTransform.forward * 0.01f;
                    }
                    activate = true;
                }
            }
            if (activate)
            {
                if (left)
                    World.SetBlock(pointOfTargetBlock, 0);
                else if(World.Block(pointOfTargetBlock) == BlockID.Air)
                    World.SetBlock(pointOfTargetBlock, 1);
            }
        }
    }
    [SerializeField] private GameObject InBlockColliderTop;
    [SerializeField] private GameObject InBlockColliderBottom;
    private void BlockCollisionCheck()
    {
        Vector3 topAsInt = new Vector3((int)transform.position.x + 0.5f, (int)(transform.position.y + 0.5f), (int)transform.position.z + 0.5f);
        InBlockColliderTop.transform.position = topAsInt;
        InBlockColliderBottom.transform.position = new Vector3((int)transform.position.x + 0.5f, (int)(transform.position.y + 0.5f) - 1, (int)transform.position.z + 0.5f);
        InBlockColliderTop.GetComponent<BarrierBlock>().UpdateCollision();
        InBlockColliderBottom.GetComponent<BarrierBlock>().UpdateCollision();
        ScreenBlocker.UpdateUVS(World.Block(topAsInt));
    }
}
