namespace Mutagen.Bethesda.Skyrim;

public partial class DestructionStageData
{
    [Flags]
    public enum Flag
    {
        CapDamage = 0x01,
        Disable = 0x02,
        Destroy= 0x04,
        IgnoreExternalDamage = 0x08
    }
}