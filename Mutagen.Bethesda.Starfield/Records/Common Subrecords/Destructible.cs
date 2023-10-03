namespace Mutagen.Bethesda.Starfield;

public partial class Destructible
{
    [Flags]
    public enum DestructibleFlag
    {
        VATSTargetable = 0x01,
        LargeActorDestroys = 0x02,
    }

    [Flags]
    public enum DestructionStageDataFlag
    {
        CapDamage = 0x01,
        Disable = 0x02,
        Destroy = 0x04,
        IgnoreExternalDamage = 0x08,
        BecomesDynamic = 0x10,
    }
}