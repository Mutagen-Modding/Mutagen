namespace Mutagen.Bethesda.Fallout4;

partial class CameraPath
{
    [Flags]
    public enum Flags
    {
        Disable = 0x0001,
        ShotList = 0x0002,
        DynamicCameraTimes = 0x0004,
        RandomizePaths = 0x0040,
        NotMustHaveCameraShots = 0x0080,
    }
}
