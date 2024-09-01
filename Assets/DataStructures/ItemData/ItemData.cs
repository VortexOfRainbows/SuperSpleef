using System.Collections.Generic;
using UnityEngine;

//public static class ProjectileID
//{
//    /// Why am I using a static class instead of ENUMS?
//    /// Besides being a preference thing, there are some mathemetical advantages to this (such as being able to use IDs in integer functions)
//    /// It also means I don't need to cast back in forth between int and enum when different circumstances call for different things.
//    public const int BouncyDeathBall = 0;
//    public const int BlockProjectile = 1;
//    public const int FragBall = 2;
//    public const int SuperFragBall = 3;
//    public const int TunnelBore = 4;
//    public const int FragFragBall = 5;
//    public const int LaserCube = 6;

//    /// <summary>
//    /// Whenever a new projectile is added, this number should be increased to the highest one + 1
//    /// </summary>
//    public const int Max = 7;
//}

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public Mesh Cube;
    public GameObject Hammer;
    //public GameObject BouncyDeathBall;
    //public GameObject BlockProjectile;
    //public GameObject FragBall;
    //public GameObject SuperFragBall;
    //public GameObject TunnelBore;
    //public GameObject FragFragBall;
    //public GameObject LaserCube;

    //private Dictionary<int, GameObject> registry = null;
    //private Dictionary<int, GameObject> InitDict()
    //{
    //    return new Dictionary<int, GameObject>() {
    //        {ProjectileID.BouncyDeathBall, BouncyDeathBall},
    //        {ProjectileID.BlockProjectile, BlockProjectile},
    //        {ProjectileID.FragBall, FragBall},
    //        {ProjectileID.SuperFragBall, SuperFragBall},
    //        {ProjectileID.TunnelBore, TunnelBore},
    //        {ProjectileID.FragFragBall, FragFragBall},
    //        {ProjectileID.LaserCube, LaserCube},
    //    };
    //}
    //public GameObject GetProjectile(int type)
    //{
    //    if (registry == null)
    //        registry = InitDict();
    //    return registry[type];
    //}
}
