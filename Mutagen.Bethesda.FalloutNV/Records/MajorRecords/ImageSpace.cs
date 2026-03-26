namespace Mutagen.Bethesda.FalloutNV;

public partial class ImageSpace
{
    [Flags]
    public enum Flag
    {
        Saturation = 0x01,
        Contrast = 0x02,
        Tint = 0x04,
        Brightness = 0x08,
    }
}
