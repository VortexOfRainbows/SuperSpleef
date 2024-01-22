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
        InsideBlockCheck();
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
            if(Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hitInfo, 128f))
            {
                Vector3 pointOfTargetBlock;
                if(left && hitInfo.collider.gameObject.tag != "InverseCube")
                {
                    pointOfTargetBlock = hitInfo.point + CameraTransform.forward * 0.01f;
                }
                else
                {
                    pointOfTargetBlock = hitInfo.point - CameraTransform.forward * 0.01f;
                }
                GameObject chunkObj = World.Instance.BoundingChunk(pointOfTargetBlock.x, pointOfTargetBlock.z);
                if(chunkObj != null)
                {
                    int blockX = Mathf.FloorToInt(pointOfTargetBlock.x) - (int)chunkObj.transform.position.x;
                    int blockY = Mathf.FloorToInt(pointOfTargetBlock.y);
                    int blockZ = Mathf.FloorToInt(pointOfTargetBlock.z) - (int)chunkObj.transform.position.z;
                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    if(blockY < Chunk.Height && blockY >= 0)
                    {
                        if (left)
                            chunk.blocks[blockX, blockY, blockZ] = 0;
                        else
                            chunk.blocks[blockX, blockY, blockZ] = 1;
                        if (blockX <= 0)
                        {
                            GameObject adjacentChunk = World.Instance.GetChunk(chunk.Index.x - 1, chunk.Index.y);
                            if(adjacentChunk != null)
                                adjacentChunk.GetComponent<Chunk>().BuildMesh();
                        }
                        if (blockZ <= 0)
                        {
                            GameObject adjacentChunk = World.Instance.GetChunk(chunk.Index.x, chunk.Index.y - 1);
                            if (adjacentChunk != null)
                                adjacentChunk.GetComponent<Chunk>().BuildMesh();
                        }
                        if (blockX >= Chunk.Width - 1)
                        {
                            GameObject adjacentChunk = World.Instance.GetChunk(chunk.Index.x + 1, chunk.Index.y);
                            if (adjacentChunk != null)
                                adjacentChunk.GetComponent<Chunk>().BuildMesh();
                        }
                        if (blockZ >= Chunk.Width - 1)
                        {
                            GameObject adjacentChunk = World.Instance.GetChunk(chunk.Index.x, chunk.Index.y + 1);
                            if (adjacentChunk != null)
                                adjacentChunk.GetComponent<Chunk>().BuildMesh();
                        }
                        chunk.BuildMesh();
                    }
                }
            }
        }
    }
    [SerializeField] private GameObject InBlockColliderTop;
    [SerializeField] private GameObject InBlockColliderBottom;
    private void InsideBlockCheck()
    {
        Vector3 pointOfTargetBlock = transform.position;
        pointOfTargetBlock.y -= 0.5f;
        bool insideTop = false;
        bool insideBot = false;
        for (int j = 0; j < 2; j++)
        {
            if(pointOfTargetBlock.y >= 0 || pointOfTargetBlock.y < Chunk.Height)
            {
                GameObject chunkObj = World.Instance.BoundingChunk(pointOfTargetBlock.x, pointOfTargetBlock.z);
                if (chunkObj != null)
                {
                    int blockX = Mathf.FloorToInt(pointOfTargetBlock.x) - (int)chunkObj.transform.position.x;
                    int blockY = Mathf.FloorToInt(pointOfTargetBlock.y);
                    int blockZ = Mathf.FloorToInt(pointOfTargetBlock.z) - (int)chunkObj.transform.position.z;
                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    if (chunk.blocks[blockX, blockY, blockZ] != 0)
                    {
                        if (j == 0)
                            insideBot = true;
                        else
                            insideTop = true;
                    }
                }
            }
            pointOfTargetBlock.y += 1;
        }
        if(insideTop)
        {
            InBlockColliderTop.SetActive(true);
            InBlockColliderTop.transform.position = new Vector3((int)transform.position.x + 0.5f, (int)transform.position.y, (int)transform.position.z + 0.5f);
        }
        else
        {
            InBlockColliderTop.SetActive(false);
        }
        /*if (insideBot)
        {
            InBlockColliderBottom.SetActive(true);
            InBlockColliderBottom.transform.position = new Vector3((int)transform.position.x + 0.5f, (int)transform.position.y - 1, (int)transform.position.z + 0.5f);
        }
        else
        {
            InBlockColliderBottom.SetActive(false);
        }*/
    }
}
