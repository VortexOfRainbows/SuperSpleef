using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private Entity player;
    [SerializeField]
    private Transform FacingVector;
    [SerializeField]
    private Transform LeftArm, RightArm, LeftLeg, RightLeg, Head;

    private float moveCounter = 0;
    private float currentBodyTilt = 0;
    private const float MinSpeedConsideredMoving = 0.2f;
    private const float JointSpeedMultiplier = 3f;
    private const float DegreesOfJointMovementLegs = 60;
    private const float DegreesOfJointMovementArms = 45;
    private const float DegreesOfPassiveArmSway = 5;
    private const float ResetRotationJoints = 0.1f;
    private const float ResetRotationBody = 0.07f;
    private void FixedUpdate()
    {
        Vector3 velo = player.Velocity.Value;
        float desiredBodyTilt = FacingVector.eulerAngles.y;
        float tiltMultiplier = player.MovingBackward ? -1 : 1;

        if (player.MovingLeft)
            desiredBodyTilt += -45 * tiltMultiplier;
        if (player.MovingRight)
            desiredBodyTilt += 45 * tiltMultiplier;

        if (desiredBodyTilt < 0)
        {
            desiredBodyTilt = 360 + desiredBodyTilt;
        }
        desiredBodyTilt %= 360;

        float bonusLerpSpeed = Mathf.Clamp(Mathf.Abs(Mathf.DeltaAngle(currentBodyTilt, desiredBodyTilt)) / 90f, 0, 2) + 1; //Increase head rotation speed the farther away from the facing angle the player model is
        currentBodyTilt = Mathf.LerpAngle(currentBodyTilt, desiredBodyTilt, ResetRotationBody * bonusLerpSpeed);
        transform.eulerAngles = new Vector3(0, currentBodyTilt, 0);
        Head.eulerAngles = new Vector3(FacingVector.eulerAngles.x, FacingVector.eulerAngles.y, 0);

        float speed = new Vector2(velo.x, velo.z).magnitude;
       
        bool isMoving = speed > MinSpeedConsideredMoving;
        if(isMoving)
        {
            float direction = (player.MovingForward || player.MovingRight) ? 1 : -1;
            moveCounter += Mathf.Sqrt(speed) * JointSpeedMultiplier * direction;
        }
        if (moveCounter < 0)
        {
            moveCounter = 360 + moveCounter;
        }
        moveCounter %= 360;
        if (!isMoving) //Move limbs to a default position when not moving
        {
            bool isCloserToZero = moveCounter <= 90 || moveCounter > 270;
            if(isCloserToZero)
                moveCounter = Mathf.Lerp(moveCounter, moveCounter <= 90 ? 0 : 360, ResetRotationJoints);
            else
                moveCounter = Mathf.Lerp(moveCounter, 180, ResetRotationJoints);
        }

        float sinusoid = Mathf.Sin(moveCounter * Mathf.Deg2Rad);

        LeftLeg.localEulerAngles = new Vector3(sinusoid * DegreesOfJointMovementLegs, LeftLeg.localEulerAngles.y, LeftLeg.localEulerAngles.z);
        RightLeg.localEulerAngles = new Vector3(sinusoid * -DegreesOfJointMovementLegs, RightLeg.localEulerAngles.y, RightLeg.localEulerAngles.z);

        RightArm.localEulerAngles = new Vector3(sinusoid * DegreesOfJointMovementArms, RightArm.localEulerAngles.y, RightArm.localEulerAngles.z);
        LeftArm.localEulerAngles = new Vector3(sinusoid * -DegreesOfJointMovementArms, LeftArm.localEulerAngles.y, LeftArm.localEulerAngles.z);

        IdleArmSwaying();
    }
    private float IdleSwayCounter = 0;
    private void IdleArmSwaying()
    {
        IdleSwayCounter++;
        float sinusoid = Mathf.Sin(IdleSwayCounter * Mathf.Deg2Rad / 2f) * 0.5f + 0.5f; //Multiplying by 0.5f and then adding 0.5f locks the sinusoid to the range [0, 1]
        RightArm.localEulerAngles = new Vector3(RightArm.localEulerAngles.x, RightArm.localEulerAngles.y, sinusoid * DegreesOfPassiveArmSway);
        LeftArm.localEulerAngles = new Vector3(LeftArm.localEulerAngles.x, LeftArm.localEulerAngles.y, -sinusoid * DegreesOfPassiveArmSway);
    }

    [SerializeField]
    private Material Spritesheet;

    [SerializeField]
    private Material Spritesheet2;

    [SerializeField]
    private Material Spritesheet3;

    [SerializeField]
    private Material Spritesheet4;

    [SerializeField]
    private PlayerModel Model;
    [SerializeField]
    private MeshFilter HeadMesh, BodyMesh, LeftLegMesh, RightLegMesh, LeftArmMesh, RightArmMesh;
    private void SetSprite()
    {
        int duck = 0;
        var sprite = Spritesheet;
        if (Random.Range(1, 3) == 2)
        {
            sprite = Spritesheet3;
            duck = 1;
        }
        if (duck == 0)
        {
            if (Random.Range(1, 11) == 10)
            {
                sprite = Spritesheet2;
            }
        }
        if (duck == 1)
        {
            if (Random.Range (1, 11) == 10)
            {
                sprite = Spritesheet4;
            }
        }
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            r.material = sprite;
        }
    }
    private void Start()
    {
        //Model.Setup();
        HeadMesh.mesh = Model.HeadMesh;
        BodyMesh.mesh = Model.BodyMesh;
        LeftLegMesh.mesh = Model.LeftLegMesh;
        RightLegMesh.mesh = Model.RightLegMesh;
        LeftArmMesh.mesh = Model.LeftArmMesh;
        RightArmMesh.mesh = Model.RightArmMesh;
        SetSprite();
    }
}
