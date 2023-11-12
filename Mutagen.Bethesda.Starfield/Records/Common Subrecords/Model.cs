namespace Mutagen.Bethesda.Starfield;

public partial class Model
{
    [Flags]
    public enum Flag
    {
        HasFaceBonesModel = 0x01,
        HasFirstPersonModel = 0x02
    }
}