namespace Mutagen.Bethesda.Fallout76;

partial class VisualEffect
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        RotateToFaceTarget = 0x01,
        AttachToCamera = 0x02,
        InheritRotation = 0x04,
    }
}