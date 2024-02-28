using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCube : Projectile ///Team members that contributed to this script: Ian Bunnell
{
    [SerializeField] private float ColorLerpTime = 2f;
    [SerializeField] private float GravityMultiplier = 0.5f;
    [SerializeField] private ParticleSystem pSystem;
    private float ColorMod = 0f;
    private Rigidbody rb;
    private Rigidbody pRB;
    public override void OnSpawn()
    {
        rb = GetComponent<Rigidbody>();
        pRB = pSystem.GetComponent<Rigidbody>();
    }
    public override void OnFixedUpdate()
    {
        transform.LookAt(transform.position + rb.velocity);
        ColorMod += Time.fixedDeltaTime;
        pRB.velocity = rb.velocity;
        rb.velocity -= Physics.gravity * (1 - GravityMultiplier) * Time.fixedDeltaTime;
    }
    public override void OnDeath(bool OutBoundDeath)
    {
        Vector3 scale = pSystem.transform.localScale;
        pSystem.gameObject.transform.parent = null;
        pSystem.transform.localScale = scale; //For some reason, detaching the parent bugs out the scale. This just sets it back to what it is before
        pSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        Destroy(pSystem.gameObject, pSystem.main.startLifetime.constant);
        if (!OutBoundDeath)
        {
            World.FillBlock(transform.position - Vector3.one, transform.position + Vector3.one, BlockID.Air, 0.75f);
        }
    }
    public override Color DrawColor()
    {
        Color c = Color.Lerp(Color.red, Color.yellow, ColorMod / ColorLerpTime);
        c.a = 0.5f;
        return c;
    }
}
