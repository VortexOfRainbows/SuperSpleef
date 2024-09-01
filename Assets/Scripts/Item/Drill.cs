
public class Drill : Item ///Team members that contributed to this script: Ian Bunnell
{
    public override bool OnSecondaryUse(Player player)
    {
        return false;
    }
    public override void SetDefaults()
    {
        MaxCount = 1;
        Firerate = 7;
    }
    public override bool IsConsumedOnUse(Player player)
    {
        return false;
    }
}
