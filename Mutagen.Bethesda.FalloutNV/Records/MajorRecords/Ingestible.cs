namespace Mutagen.Bethesda.FalloutNV;

public partial class Ingestible
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }

    [Flags]
    public enum Flag
    {
        NoAutoCalc = 0x01,
        FoodItem = 0x02,
        Medicine = 0x04,
    }

    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => _Effects;
}
