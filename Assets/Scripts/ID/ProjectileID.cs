public static class ProjectileID
{
    /// Why am I using a static class instead of ENUMS?
    /// Besides being a preference thing, there are some mathemetical advantages to this (such as being able to use IDs in integer functions)
    /// It also means I don't need to cast back in forth between int and enum when different circumstances call for different things.
    public static int BouncyDeathBall = 0;
    public static int BlockProjectile = 1;
    public static int FragBall = 2;
    public static int SuperFragBall = 3;
    public static int TunnelBore = 4;

    /// <summary>
    /// Whenever a new projectile is added, this number should be increased to the highest one
    /// </summary>
    public static int Max = 4;
}