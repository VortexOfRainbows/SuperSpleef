using UnityEngine;
using UnityEngine.UI;

///Team members that contributed to this script: Ian Bunnell
public class SpawnCubes : MonoBehaviour
{
    public const int OutOfScreenPadding = 1;
    private const string FallingBlockTag = "FallingBlock";
    [SerializeField]
    private ParticleSystem BlockParticles;
    [SerializeField]
    private GameObject FallingCubePrefab;
    [SerializeField]
    private GameObject BlockOutline;
    [SerializeField]
    private GameObject PopupNumber;
    [SerializeField]
    private Camera Camera;
    private float CubeTimer;
    private float CubeSpawnTime = 1f;
    private void Awake()
    {
        ///make sure this value is assigned so we can generate block breaking particles in the menu
        World.BlockParticleRef = BlockParticles; 
    }
    private void FixedUpdate()
    {
        CubeTimer += Time.fixedDeltaTime;
        float SpawnTime = CubeSpawnTime / FallingCube.AdditionalSpeedBasedOnBlocksBrokenInARow();
        if (CubeTimer > SpawnTime)
        {
            Vector3 spawnPositionPixels = new Vector3(Random.Range(0, (float)Camera.scaledPixelWidth), Camera.scaledPixelHeight, 0);
            spawnPositionPixels.z = -Camera.transform.position.z;
            Vector3 spawnPos = Camera.ScreenToWorldPoint(spawnPositionPixels) + Vector3.up * OutOfScreenPadding;
            FallingCube cube = Instantiate(FallingCubePrefab, spawnPos, Quaternion.identity, transform).GetComponent<FallingCube>();
            cube.SetDeathScreenBound(-spawnPos.y + 1);
            CubeTimer -= SpawnTime;
        }
    }
    private void Update()
    {
        MouseRaycast();
        BlockOutline.SetActive(true);
    }
    private void MouseRaycast()
    {
        BlockOutline.transform.position = Camera.transform.position + Vector3.back; //Put the block outline outside of view
        RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.transform.position.z;
        Vector3 spawnpos = Camera.ScreenToWorldPoint(mousePos);
        spawnpos.z = Camera.transform.position.z;
        if (Physics.Raycast(spawnpos, Vector3.forward, out hit, 15))
        {
            if(hit.collider.CompareTag(FallingBlockTag))
            {
                if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    hit.collider.gameObject.GetComponent<FallingCube>().DestroyEffects();

                    ///Moves the display number to a position behind the block.
                    Vector3 positionToGoTo = hit.collider.transform.position;
                    Vector3 goToPos = Camera.WorldToScreenPoint(positionToGoTo);
                    goToPos.z = PopupNumber.transform.position.z - Camera.transform.position.z;
                    goToPos = Camera.ScreenToWorldPoint(goToPos);
                    PopupNumber.transform.position = goToPos;
                    PopupNumber.GetComponent<Text>().text = FallingCube.BlocksBrokenInARow.ToString();
                    PopupNumber.SetActive(true);
                }
                else
                {
                    BlockOutline.transform.position = hit.collider.gameObject.transform.position;
                    BlockOutline.transform.rotation = hit.collider.gameObject.transform.rotation;
                }
            }
        }
    }
}
