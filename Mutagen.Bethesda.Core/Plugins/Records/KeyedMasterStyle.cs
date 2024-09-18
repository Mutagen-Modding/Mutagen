using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Headers;

namespace Mutagen.Bethesda.Plugins.Records;

public record KeyedMasterStyle(ModKey ModKey, MasterStyle MasterStyle) : IModMasterStyledGetter
{
    public static KeyedMasterStyle FromMod(IModFlagsGetter masterFlagsGetter)
    {
        return new KeyedMasterStyle(masterFlagsGetter.ModKey, masterFlagsGetter.GetMasterStyle());
    }

    public static KeyedMasterStyle FromPath(
        ModPath path,
        GameRelease release,
        IFileSystem? fileSystem = null)
    {
        var header = ModHeaderFrame.FromPath(path, release, fileSystem: fileSystem);
        return new KeyedMasterStyle(path.ModKey, header.MasterStyle);
    }
}