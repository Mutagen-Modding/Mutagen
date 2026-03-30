namespace Mutagen.Bethesda.Fallout3;

public partial class Water
{
    [Flags]
    public enum Flag
    {
        CausesDamage = 0x01,
        Reflective = 0x02,
    }
}
