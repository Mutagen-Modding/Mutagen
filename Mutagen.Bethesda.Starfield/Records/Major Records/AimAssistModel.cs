namespace Mutagen.Bethesda.Starfield;

public partial class AimAssistModel
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x4,
        GroundPiece = 0x10,
        HiddenFromLocalMap = 0x200,
        UsedAsPlatform = 0x800,
        HasCurrents = 0x8_0000,
        NavmeshFilter = 0x400_0000,
        NavmeshBoundingBox = 0x800_0000,
        NavmeshOnlyCut = 0x1000_0000,
        NavmeshIgnoreErosion = 0x2000_0000,
        NavmeshGround = 0x4000_0000,
    }
}
