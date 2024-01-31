public static class BlockID
{
    /// Why am I using a static class instead of ENUMS?
    /// Besides being a preference thing, there are some mathemetical advantages to this
    /// Primarily, I can use the block IDS as integers, allowing me to cover multiple blocks with comparisons
    /// It also means I don't need to cast back in forth between int and enum when different circumstances call for different things.
    public static int Air = 0;
    public static int Dirt = 1;
    public static int Grass = 2;
    public static int Glass = 3;
    public static int Stone = 4;
    public static int Wood = 5;
    public static int Leaves = 6;

    /// <summary>
    /// Whenever a new block is added, this number should be increased to the highest one
    /// </summary>
    public static int Max = 6;
}