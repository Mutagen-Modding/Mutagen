namespace Mutagen.Bethesda.Fallout76;

public partial class Model
{
    [Flags]
    public enum Flag
    {
        HasFaceBonesModel = 0x01,
        HasFirstPersonModel = 0x02
    }
}