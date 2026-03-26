using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Fallout76;

partial class CameraPath
{
    [Flags]
    public enum MajorFlag
    {
    }

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

partial class CameraPathBinaryOverlay
{
}
