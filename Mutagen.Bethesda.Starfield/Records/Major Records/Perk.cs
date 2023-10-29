namespace Mutagen.Bethesda.Starfield;

public partial class Perk
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x4
    }

    public enum PerkCategory
    {
        None,
        Combat,
        Science,
        Tech,
        Physical,
        Social,
        All,
        AllIncludingNone,
    }

    public enum PerkSkillGroup
    {
        None,
        Basic,
        Advanced,
        Expert,
        Master
    }

    public enum PerkCrewAssignment
    {
        None,
        CrewShip,
        CrewOutpost,
    }

    [Flags]
    public enum Flag
    {
        PcTrait = 0x001,
        PcPlayable = 0x002,
        ShowInCrewUi = 0x008,
        PcBackground = 0x010,
    }

    public enum EffectType
    {
        Quest,
        Ability,
        EntryPoint,
    }
}