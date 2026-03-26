namespace Mutagen.Bethesda.Fallout76;

partial class ActorValueInformation
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag : uint
    {
        Skill = 0x02,
        UsesEnum = 0x04,
        DoNotAllowScriptEdits = 0x8,
        IsFullAvCached = 0x10,
        IsPermanentAvCached = 0x020,
        DefaultToZero = 0x400,
        DefaultToOne = 0x800,
        DefaultToOneHundred = 0x1000,
        ContainsList = 0x8000,
        ValueLessThanOne = 0x8_0000,
        MinimumOne = 0x10_0000,
        MaximumTen = 0x20_0000,
        MaximumOneHundred = 0x40_0000,
        MultiplyByOneHundred = 0x80_0000,
        Percentage = 0x100_0000,
        DamageIsPositive = 0x400_0000,
        GodModeImmune = 0x800_0000,
        Hardcoded = 0x8000_0000,
    }

    public enum ActorValueType
    {
        DerivedAttribute = 0,
        Special = 1,
        Skill = 2,
        AiAttribute = 3,
        Resistance = 4,
        Condition = 5,
        Charge = 6,
        IntValue = 7,
        Variable = 8,
        Resource = 9,
        Aggregate = 10,
        Reputation = 11,
        Unknown = 12,
    }
}
