namespace Mutagen.Bethesda.FalloutNV;

public partial class Condition
{
    [Flags]
    public enum Flag
    {
        OR = 0x01,
        UseAliases = 0x02,
        UseGlobal = 0x04,
        UsePackData = 0x08,
        SwapSubjectAndTarget = 0x10
    }

    public enum RunOn
    {
        Subject = 0,
        Target = 1,
        Reference = 2,
        CombatTarget = 3,
        LinkedReference = 4,
    }
}
