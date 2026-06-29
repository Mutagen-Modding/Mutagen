using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class Ingestible
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }

    [Flags]
    public enum IngestibleFlag
    {
        NoAutoCalc = 0x0000_0001,
        FoodItem = 0x0000_0002,
        Medicine = 0x0000_0004,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}
