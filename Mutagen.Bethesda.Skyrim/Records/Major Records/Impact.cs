namespace Mutagen.Bethesda.Skyrim;

public partial class Impact
{
    public enum ResultType
    {
        Default,
        Destroy,
        Bounce,
        Impale,
        Stick
    }

    public enum OrientationType
    {
        SurfaceNormal,
        ProjectileVector,
        ProjectileReflection,
    }
}