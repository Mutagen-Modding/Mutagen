using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Archives.DI;

public interface IArchiveNameFromModKeyProvider
{
    FileName Get(ModKey modKey);
}

public class ArchiveNameFromModKeyProvider : IArchiveNameFromModKeyProvider
{
    private readonly IGameReleaseContext _gameRelease;
    
    public ArchiveNameFromModKeyProvider(IGameReleaseContext gameRelease)
    {
        _gameRelease = gameRelease;
    }

    public FileName Get(ModKey modKey)
    {
        return $"{modKey.FileName.NameWithoutExtension}{Archive.GetExtension(_gameRelease.Release)}";
    }
}