using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private Entity player;
    [SerializeField]
    private Transform FacingVector;
    [SerializeField]
    private Transform LeftArm, RightArm, LeftLeg, RightLeg, Head, Body;
    [SerializeField]
    private GameObject RightArmVisual;
    public ItemDisplay HeldItem;
    private float moveCounter = 0;
    private float currentBodyTilt = 0;
    private float itemUseAnimationPercent = 0;
    private const float MinSpeedConsideredMoving = 0.2f;
    private const float JointSpeedMultiplier = 3f;
    private const float DegreesOfJointMovementLegs = 60;
    private const float DegreesOfJointMovementArms = 45;
    private const float DegreesOfPassiveArmSway = 5;
    private const float ResetRotationJoints = 0.1f;
    private const float ResetRotationBody = 0.07f;
    public bool IsFirstPerson = false;
    public void SetActive(bool FirstPerson = true)
    {
        IsFirstPerson = FirstPerson;
        if (FirstPerson)
        {
            LeftArm.gameObject.SetActive(false);
            RightArm.gameObject.SetActive(true);
            LeftLeg.gameObject.SetActive(false);
            RightLeg.gameObject.SetActive(false);
            Head.gameObject.SetActive(false);
            Body.gameObject.SetActive(false);
            RightArm.gameObject.layer = HeldItem.gameObject.layer = RightArmVisual.layer = 10;
            RightArm.transform.localScale = Vector3.one * 0.5f;
            RightArm.transform.localPosition = new Vector3(0.25f, 0.375f, 0f);
        }
        else
        {
            LeftArm.gameObject.SetActive(true);
            RightArm.gameObject.SetActive(true);
            LeftLeg.gameObject.SetActive(true);
            RightLeg.gameObject.SetActive(true);
            Head.gameObject.SetActive(true);
            Body.gameObject.SetActive(true);
            RightArm.gameObject.layer = HeldItem.gameObject.layer = RightArmVisual.layer = 6;
            RightArm.transform.localScale = Vector3.one;
            RightArm.transform.localPosition = new Vector3(0.25f, 0.375f, 0f);
        }
    }
    private void FixedUpdate()
    {
        Vector3 velo = player.Velocity.Value;
        float desiredBodyTilt = FacingVector.eulerAngles.y;
        float tiltMultiplier = player.MovingBackward ? -1 : 1;

        if (!IsFirstPerson)
        {
            if (player.MovingLeft)
                desiredBodyTilt += -45 * tiltMultiplier;
            if (player.MovingRight)
                desiredBodyTilt += 45 * tiltMultiplier;

            if (desiredBodyTilt < 0)
            {
                desiredBodyTilt = 360 + desiredBodyTilt;
            }
            desiredBodyTilt %= 360;
        }

        float bonusLerpSpeed = Mathf.Clamp(Mathf.Abs(Mathf.DeltaAngle(currentBodyTilt, desiredBodyTilt)) / 90f, 0, 2) + 1; //Increase head rotation speed the farther away from the facing angle the player model is
        float changeSpeed = ResetRotationBody * bonusLerpSpeed;
        if (IsFirstPerson)
            changeSpeed = 1;
        currentBodyTilt = Mathf.LerpAngle(currentBodyTilt, desiredBodyTilt, changeSpeed);
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

        itemUseAnimationPercent = 0f;
        Vector3 armUseAnimation = Vector3.zero;
        if(player is Player p)
        {
            //if (!player.IsOwner)
            //    Debug.Log(p.FacingVector.transform.forward);

            HeldItem.item = p.HeldItem();
            float maxTime = HeldItem.item.Firerate;
            if (p.ItemUseTimeMax != 0)
                maxTime = p.ItemUseTimeMax;

            float animationPercent = (1 + p.ItemUseTime / maxTime) / 2f;
            animationPercent = Mathf.Clamp(animationPercent, 0, 1);
            armUseAnimation = p.FacingVector.transform.rotation.eulerAngles;
            armUseAnimation.x -= 80;
            armUseAnimation.x += 25 * MathF.Sin(animationPercent * Mathf.PI * 2f);
            armUseAnimation.y += 12 * MathF.Cos(animationPercent * Mathf.PI);
            itemUseAnimationPercent = animationPercent;

            ///This controls how the player holds the item out, rather than when it is used. It might benefit from being moved into ItemDisplay, instead of being here...
            if (IsFirstPerson)
            {
                itemUseAnimationPercent = Mathf.Clamp(itemUseAnimationPercent, 1f, 1);
            }
            else if (HeldItem.item != null)
            {
                if (HeldItem.item is PlaceableBlock)
                {
                    //Debug.Log(p.ItemUseTime);
                    itemUseAnimationPercent = Mathf.Clamp(itemUseAnimationPercent, 0.3f, 1);
                }
                else if (HeldItem.item is Weapon)
                    itemUseAnimationPercent = Mathf.Clamp(itemUseAnimationPercent, 0.8f, 1);
            }
        }
        float slerpPoint = Mathf.Sin(Mathf.PI * Mathf.Pow(itemUseAnimationPercent, 2));
        if(itemUseAnimationPercent < 0.7f)
            RightArm.localEulerAngles = new Vector3(sinusoid * DegreesOfJointMovementArms, 0, 0);
        RightArm.rotation = math.slerp(RightArm.rotation, armUseAnimation.ToQuaternion(), slerpPoint); 
        LeftArm.localEulerAngles = new Vector3(sinusoid * -DegreesOfJointMovementArms, 0, 0);
        IdleArmSwaying();
    }
    private float IdleSwayCounter = 0;
    private void IdleArmSwaying()
    {
        IdleSwayCounter++;
        float sinusoid = Mathf.Sin(IdleSwayCounter * Mathf.Deg2Rad / 2f) * 0.5f + 0.5f; //Multiplying by 0.5f and then adding 0.5f locks the sinusoid to the range [0, 1]
        RightArm.Rotate(0, 0, sinusoid * DegreesOfPassiveArmSway);
        LeftArm.Rotate(0, 0, -sinusoid * DegreesOfPassiveArmSway);
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
    public void SetSprite()
    {
        bool duck = UnityEngine.Random.Range(1, 3) == 2;
        bool suit = UnityEngine.Random.Range(1, 11) == 10;
        Material sprite = Spritesheet;
        if (duck)
        {
            sprite = suit ? Spritesheet4 : Spritesheet3;
        }
        else if (suit)
        {
            sprite = Spritesheet2;
        }

        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            if(r.GetComponent<ItemDisplay>() == null)
            {
                r.material = sprite;
            }
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
