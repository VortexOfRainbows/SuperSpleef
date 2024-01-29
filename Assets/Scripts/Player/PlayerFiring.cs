using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFiring : MonoBehaviour
{
    public Rigidbody shot;
    public Player player;
    
    public float minLaunchForce;
    public float maxLaunchForce;

    private float currentLaunchForce;

    private bool fired;
    
    // Start is called before the first frame update
    void Start()
    {
        currentLaunchForce = minLaunchForce;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Fire();

        //Debug.Log(player.velocity);
    }


    private void Fire() 
    { 
        fired = true;
        Transform shotTransform = player.FacingVector.transform;
        Rigidbody shotInstance =
            Instantiate(shot, shotTransform.position, shotTransform.rotation) as Rigidbody;

        shotInstance.velocity = currentLaunchForce * shotTransform.forward + player.RB.velocity;
    }
}
