namespace Mutagen.Bethesda.FalloutNV;

public partial class Book
{
    [Flags]
    public enum BookFlag : byte
    {
        Scroll = 0x01,
        CantBeTaken = 0x02,
    }
}
