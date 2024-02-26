public static class BlockID ///Team members that contributed to this script: Ian Bunnell
{
    /// Why am I using a static class instead of ENUMS?
    /// Besides being a preference thing, there are some mathemetical advantages to this
    /// Primarily, I can use the block IDS as integers, allowing me to cover multiple blocks with comparisons
    /// It also means I don't need to cast back in forth between int and enum when different circumstances call for different things.
    public const int Air = 0;
    public const int Dirt = 1;
    public const int Grass = 2;
    public const int Glass = 3;
    public const int Stone = 4;
    public const int Wood = 5;
    public const int Leaves = 6;
    public const int YellowBricks = 7;
    public const int BlueBricks = 8;
    /// <summary>
    /// Whenever a new block is added, this number should be increased to the highest one
    /// </summary>
    public const int Max = 8;
}