namespace Mutagen.Bethesda.Plugins.IO;

[Flags]
public enum AssociatedModFileCategory
{
    Plugin = 0x01,
    RawStrings = 0x02,
    Archives = 0x04,
}