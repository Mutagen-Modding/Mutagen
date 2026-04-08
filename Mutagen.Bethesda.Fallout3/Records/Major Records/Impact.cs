namespace Mutagen.Bethesda.Fallout3;

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
