namespace Mutagen.Bethesda.Starfield;

partial class Impact
{
    public enum OrientationType
    {
        SurfaceNormal,
        ProjectileVector,
        ProjectileReflection,
    }

    public enum ResultType
    {
        Default,
        Destroy,
        Bounce,
        Impale,
        Stick
    }
}