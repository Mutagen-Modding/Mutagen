namespace Mutagen.Bethesda.Fallout3;

public partial class DestructionStageData
{
    [Flags]
    public enum Flag
    {
        CapDamage = 0x01,
        Disable = 0x02,
        Destroy = 0x04,
    }
}
