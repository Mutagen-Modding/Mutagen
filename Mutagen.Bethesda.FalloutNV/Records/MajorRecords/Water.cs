namespace Mutagen.Bethesda.FalloutNV;

public partial class Water
{
    [Flags]
    public enum Flag
    {
        CausesDamage = 0x01,
        Reflective = 0x02,
    }
}
