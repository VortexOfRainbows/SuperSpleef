using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private Player myPlayer = null;
    [SerializeField] private Rigidbody RB;
    [SerializeField] private PlayerControls ControlManager;
    public PlayerControls.ControlDown Control => ControlManager.Control;
    public PlayerControls.ControlDown LastControl => ControlManager.LastControl;
    private int PlayerToWatchID = -1;
    private Transform CameraTransform => ClientManager.Camera.transform;
    public string Username => MyName.Value.ToString();
    private NetworkVariable<FixedString512Bytes> MyName = new NetworkVariable<FixedString512Bytes>("Username", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    void Start()
    {
        NetHandler.LoggedPlayers.Add(this);
    }
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            MyName.Value = GameStateManager.LocalUsername;
        }
    }
    public override void OnNetworkDespawn()
    {

    }
    private void Update()
    {
        if(GameStateManager.Players.Count > 0)
        {
            myPlayer = null;
            foreach (Player player in GameStateManager.Players)
            {
                if (player.OwnerClientId == OwnerClientId)
                {
                    myPlayer = player;
                    break;
                }
            }
            if(myPlayer != null)
            {
                transform.position = myPlayer.transform.position;
                Direction = myPlayer.GetFacingDirection();
            }
            else
            {
                ControlManager.OnUpdate();
                if (Input.GetMouseButtonDown(0))
                {
                    PlayerToWatchID++;
                    if (PlayerToWatchID >= GameStateManager.Players.Count)
                    {
                        PlayerToWatchID = -1;
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    PlayerToWatchID--;
                    if (PlayerToWatchID < -1)
                    {
                        PlayerToWatchID = GameStateManager.Players.Count - 1;
                    }
                }
                if(PlayerToWatchID >= 0)
                {
                    Player toWatch = GameStateManager.Players[PlayerToWatchID];
                    transform.position = toWatch.FacingVector.transform.position;
                    transform.rotation = toWatch.FacingVector.transform.rotation;
                }
                if (!GameStateManager.GameIsPausedOrOver)
                {
                    MouseControls();
                }
                CameraTransform.position = transform.position + Vector3.up * 2f;
                if (IsOwner)
                {
                    CameraTransform.rotation = Quaternion.Euler(Direction.x, Direction.y, 0f);
                }
            }
        }
    }
    [SerializeField] private float Speed = 2f;
    [SerializeField] private float SpeedMax = 5;
    [SerializeField] private float MoveDeacceleration = 0.25f;
    [SerializeField] private float MoveAcceleration = 0.125f;
    private void FixedUpdate()
    {
        if (GameStateManager.Players.Count > 0)
        {
            if (myPlayer == null) //Usually the update is done in player. Do the update here if we can't find a player.
            {
                //Movement should be updated in fixed update so it works probably on all systems
                Vector2 perpendicularVelocity = new Vector2(RB.velocity.x, RB.velocity.z).RotatedBy(Direction.y * Mathf.Deg2Rad); //Speed is modified as a perpendicular value to make stopping and switching directions more smooth and consistent. Also allows modifying speed more easily.
                float speed = Speed * MoveAcceleration;
                float maxSpeed = SpeedMax;

                if (Control.Forward && !Control.Back)
                    perpendicularVelocity.y += speed * Mathf.Abs(Control.YMove);
                else if (perpendicularVelocity.y > 0)
                    perpendicularVelocity.y *= MoveDeacceleration;
                if (Control.Left && !Control.Right)
                    perpendicularVelocity.x -= speed * Mathf.Abs(Control.XMove);
                else if (perpendicularVelocity.x < 0)
                    perpendicularVelocity.x *= MoveDeacceleration;
                if (Control.Back && !Control.Forward)
                    perpendicularVelocity.y -= speed * Mathf.Abs(Control.YMove);
                else if (perpendicularVelocity.y < 0)
                    perpendicularVelocity.y *= MoveDeacceleration;
                if (Control.Right && !Control.Left)
                    perpendicularVelocity.x += speed * Mathf.Abs(Control.XMove);
                else if (perpendicularVelocity.x > 0)
                    perpendicularVelocity.x *= MoveDeacceleration;

                perpendicularVelocity = perpendicularVelocity.RotatedBy(Direction.y * -Mathf.Deg2Rad);

                Vector3 velo = new Vector3(perpendicularVelocity.x, RB.velocity.y, perpendicularVelocity.y);
                Vector2 velocityXZ = new Vector2(velo.x, velo.z);

                if (Control.Jump && !Control.Shift)
                    velo.y += speed;
                else if(velo.y > 0)
                    velo.y *= MoveDeacceleration;
                if (Control.Shift && !Control.Jump)
                    velo.y -= speed;
                else if (velo.y < 0)
                    velo.y *= MoveDeacceleration;

                velo.x = velocityXZ.x;
                velo.z = velocityXZ.y;
                if (velo.magnitude > maxSpeed)
                {
                    velo = velo.normalized * maxSpeed;
                }
                RB.velocity = velo;

                ControlManager.OnFixedUpdate();
            }
        }
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
}
