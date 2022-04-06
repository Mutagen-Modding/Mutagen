using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class Destructible
{
    [Flags]
    public enum DestructibleFlag
    {
        VATSTargetable,
        LargeActorDestroys
    }

    [Flags]
    public enum DestructionStageDataFlag
    {
        CapDamage,
        Disable,
        Destroy,
        IgnoreExternalDamage,
        BecomesDynamic
    }
}