namespace Mutagen.Bethesda.FalloutNV;

public partial class Impact
{
    [Flags]
    public enum Flag
    {
        NoDecalData = 0x01,
    }

    public enum OrientationType
    {
        SurfaceNormal,
        ProjectileVector,
        ProjectileReflection,
    }
}
