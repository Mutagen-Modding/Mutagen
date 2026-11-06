using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class ConstructibleObject
{
    [Flags]
    public enum MajorFlag
    {
        ListContainsVariants = 0x0008_0000
    }
    
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

    [Obsolete("Use ConstructibleComponents")]
    public ExtendedList<ConstructibleObjectComponent>? ConstructableComponents
    {
        get => ConstructibleComponents;
        set => ConstructibleComponents = value;
    }
}

public partial interface IConstructibleObject
{
    [Obsolete("Use ConstructibleComponents")]
    new ExtendedList<ConstructibleObjectComponent>? ConstructableComponents
    {
        get => ConstructibleComponents;
        set => ConstructibleComponents = value;
    }
}

public partial interface IConstructibleObjectGetter
{
    [Obsolete("Use ConstructibleComponents")]
    IReadOnlyList<IConstructibleObjectComponentGetter>? ConstructableComponents => ConstructibleComponents;
}