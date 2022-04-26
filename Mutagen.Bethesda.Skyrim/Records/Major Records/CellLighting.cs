namespace Mutagen.Bethesda.Skyrim;

public partial class CellLighting
{
    [Flags]
    public enum Inherit
    {
        AmbientColor = 0x0001,
        DirectionalColor = 0x0002,
        FogColor = 0x0004,
        FogNear = 0x0008,
        FogFar = 0x0010,
        DirectionalRotation = 0x0020,
        DirectionalFade = 0x0040,
        ClipDistance = 0x0080,
        FogPower = 0x0100,
        FogMax = 0x0200,
        LightFadeDistances = 0x0400,
    }
}
