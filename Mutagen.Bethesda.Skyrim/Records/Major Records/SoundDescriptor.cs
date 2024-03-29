namespace Mutagen.Bethesda.Skyrim;

public partial class SoundDescriptor
{
    public enum DescriptorType : uint
    {
        Standard = 0x1EEF540A
    }

    public enum LoopType
    {
        None = 0,
        Loop = 0x08,
        EnvelopeFast = 0x10,
        EnvelopeSlow = 0x20,
    }
}
