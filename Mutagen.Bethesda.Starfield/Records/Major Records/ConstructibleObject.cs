namespace Mutagen.Bethesda.Starfield;

public partial class ConstructibleObject
{
    public enum LearnMethodEnum
    {
        PickupOrScript,
        Scrapping,
        Ingesting,
        DefaultOrConditions,
        Plan,
    }

    [Flags]
    public enum Flag
    {
        FilterNotRequiredToLearn = 0x00000001,
        ExcludeFromWorkshopLod = 0x00000004,
    }
}