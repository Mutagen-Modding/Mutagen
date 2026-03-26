namespace Mutagen.Bethesda.FalloutNV;

public partial class Model
{
    [Flags]
    public enum FaceGenFlag
    {
        Head = 0x01,
        Torso = 0x02,
        RightHand = 0x04,
        LeftHand = 0x08,
    }
}