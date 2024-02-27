public static class ProjectileID ///Team members that contributed to this script: Ian Bunnell
{
    /// Why am I using a static class instead of ENUMS?
    /// Besides being a preference thing, there are some mathemetical advantages to this (such as being able to use IDs in integer functions)
    /// It also means I don't need to cast back in forth between int and enum when different circumstances call for different things.
    public const int BouncyDeathBall = 0;
    public const int BlockProjectile = 1;
    public const int FragBall = 2;
    public const int SuperFragBall = 3;
    public const int TunnelBore = 4;
    public const int FragFragBall = 5;
    public const int Slime = 6;
    public const int KillerFly = 7;

    /// <summary>
    /// Whenever a new projectile is added, this number should be increased to the highest one
    /// </summary>
    public const int Max = 7;
}