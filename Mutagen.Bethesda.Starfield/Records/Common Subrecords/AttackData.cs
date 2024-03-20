namespace Mutagen.Bethesda.Starfield;

public partial class AttackData
{
    // ToDo
    // Copy Paste Risk
    [Flags]
    public enum Flag : uint
    {
        IgnoreWeapon = 0x0000_0001,
        BashAttack = 0x0000_0002,
        PowerAttack = 0x0000_0004,
        ChargeAttack = 0x0000_0008,
        RotatingAttack = 0x0000_0010,
        ContinuousAttack = 0x0000_0020,
        OverrideData = 0x8000_0000,
    }
}