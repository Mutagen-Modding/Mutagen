namespace Mutagen.Bethesda.Fallout76;

partial class Impact
{
    [Flags]
    public enum MajorFlag
    {
    }

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
