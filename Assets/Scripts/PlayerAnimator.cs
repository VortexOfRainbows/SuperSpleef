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
    private float crouchFunny = 0;
    public float crouchPercent = 0;
    public bool isCrouched = false;
    public void SetActive(bool FirstPerson = true)
    {
        IsFirstPerson = HeldItem.FirstPerson = FirstPerson;
        if (FirstPerson)
        {
            LeftArm.gameObject.SetActive(false);
            RightArm.gameObject.SetActive(true);
            LeftLeg.gameObject.SetActive(false);
            RightLeg.gameObject.SetActive(false);
            Head.gameObject.SetActive(false);
            Body.gameObject.SetActive(false);
            RightArm.gameObject.layer = HeldItem.gameObject.layer = RightArmVisual.layer = 10;
            RightArm.transform.localScale = Vector3.one * 0.45f;
            RightArm.transform.localPosition = new Vector3(0.15f, 0.51f + crouchPercent * -0.3125f, 0f);
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
    private void Update()
    {
        if (IsFirstPerson)
        {
            BodyRotationUpdate();
            MainArmUpdate();
        }
    }
    private void BodyRotationUpdate()
    {
        float desiredBodyTilt = FacingVector.eulerAngles.y;
        float tiltMultiplier = player.MovingBackward ? -1 : 1;

        isCrouched = false;
        crouchFunny = 0;
        if (player is Player p)
        {
            if (p.Crouched.Value)
            {
                if(!IsFirstPerson)
                    crouchFunny = 30;
                isCrouched = true;
            }
        }
        if (isCrouched)
        {
            crouchPercent += 0.075f;
        }
        else
            crouchPercent -= 0.075f;
        crouchPercent = Mathf.Clamp(crouchPercent, 0, 1);
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
        transform.eulerAngles = new Vector3(crouchFunny, currentBodyTilt, 0);
        transform.localPosition = new Vector3(0, -0.25f * crouchFunny / 30f, 0);
    }
    private void FixedUpdate()
    {
        Vector3 velo = player.Velocity.Value;
        if (!IsFirstPerson)
            BodyRotationUpdate();

        Head.eulerAngles = new Vector3(FacingVector.eulerAngles.x, FacingVector.eulerAngles.y, 0);

        float speed = new Vector2(velo.x, velo.z).magnitude;
       
        bool isMoving = speed > MinSpeedConsideredMoving;
        if(isMoving)
        {
            float direction = (player.MovingForward || player.MovingRight) ? 1 : -1;
            if (speed < 1)
                speed = speed * speed;
            else
                speed = Mathf.Sqrt(speed);
            moveCounter += speed * JointSpeedMultiplier * direction;
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

        float degrees = DegreesOfJointMovementLegs * (1 - 0.5f * crouchFunny / 30f);
        LeftLeg.localEulerAngles = new Vector3(sinusoid * degrees - crouchFunny, LeftLeg.localEulerAngles.y, LeftLeg.localEulerAngles.z);
        RightLeg.localEulerAngles = new Vector3(sinusoid * -degrees - crouchFunny, RightLeg.localEulerAngles.y, RightLeg.localEulerAngles.z);
        LeftLeg.localPosition = new Vector3(0, -0.25f + 0.125f * crouchFunny / 30f, 0);
        RightLeg.localPosition = new Vector3(0, -0.25f + 0.125f * crouchFunny / 30f, 0);
        degrees = DegreesOfJointMovementArms * (1 - 0.5f * crouchFunny / 30f);
        LeftArm.localEulerAngles = new Vector3(sinusoid * -degrees, 0, 0);
        if (!IsFirstPerson)
            MainArmUpdate();
    }
    private void MainArmUpdate()
    {
        float sinusoid = Mathf.Sin(moveCounter * Mathf.Deg2Rad);
        itemUseAnimationPercent = 0f;
        Vector3 armUseAnimation = Vector3.zero;
        if (player is Player p)
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
            armUseAnimation.x += 25 * MathF.Sin(animationPercent * Mathf.PI * 2f) * MathF.Sin(animationPercent * Mathf.PI);
            armUseAnimation.y += 12 * MathF.Cos(animationPercent * Mathf.PI);
            itemUseAnimationPercent = animationPercent;

            ///This controls how the player holds the item out, rather than when it is used. It might benefit from being moved into ItemDisplay, instead of being here...
            if (IsFirstPerson)
            {
                itemUseAnimationPercent = 0.707106781f;
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
                else if(HeldItem.item is Drill)
                    itemUseAnimationPercent = Mathf.Clamp(itemUseAnimationPercent, 0.4f, 1);
            }
        }
        float slerpPoint = Mathf.Sin(Mathf.PI * Mathf.Pow(itemUseAnimationPercent, 2));
        float degrees = DegreesOfJointMovementArms * (1 - 0.5f * crouchFunny / 30f);
        if (itemUseAnimationPercent < 0.7f)
            RightArm.localEulerAngles = new Vector3(sinusoid * degrees, 0, 0);
        RightArm.rotation = math.slerp(RightArm.rotation, armUseAnimation.ToQuaternion(), slerpPoint);

        IdleArmSwaying();
    }
    private float IdleSwayCounter = 0;
    private void IdleArmSwaying()
    {
        if(!IsFirstPerson)
        {
            IdleSwayCounter++;
            float sinusoid = Mathf.Sin(IdleSwayCounter * Mathf.Deg2Rad / 2f) * 0.5f + 0.5f; //Multiplying by 0.5f and then adding 0.5f locks the sinusoid to the range [0, 1]
            RightArm.Rotate(0, 0, sinusoid * DegreesOfPassiveArmSway);
            LeftArm.Rotate(0, 0, -sinusoid * DegreesOfPassiveArmSway);
        }
        else
        {
            //This creates an ARM BOBBING EFFECT.. Which may be better simulated by adjusting the camera instead (potentially the main camera for a entire VIEW BOBBING effect, or the Arm Camera for ARM BOBBING).
            float sinusoid = Mathf.Sin(moveCounter * Mathf.Deg2Rad);
            RightArm.Rotate(sinusoid * 4f, 0, sinusoid * 1.6f);
        }
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
    private Material Spritesheet5;

    [SerializeField]
    private Material Spritesheet6;

    [SerializeField]
    private PlayerModel Model;
    [SerializeField]
    private MeshFilter HeadMesh, BodyMesh, LeftLegMesh, RightLegMesh, LeftArmMesh, RightArmMesh;
    public void SetSprite()
    {
        int rand = UnityEngine.Random.Range(0, 3);
        bool mario = rand == 1;
        bool duck = rand == 2;
        bool suit = UnityEngine.Random.Range(1, 11) == 10;
        Material sprite = Spritesheet;
        if(mario)
        {
            sprite = suit ? Spritesheet6 : Spritesheet5;
        }
        else if(duck)
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
