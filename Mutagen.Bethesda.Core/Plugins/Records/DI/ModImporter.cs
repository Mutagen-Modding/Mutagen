using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Records.DI;

public interface IModImporter
{
    TMod Import<TMod>(ModPath modPath, StringsReadParameters? stringsParam = null)
        where TMod : IModGetter;

    IModGetter Import(ModPath modPath, StringsReadParameters? stringsParam = null);
}
    
public interface IModImporter<TMod>
    where TMod : IModGetter
{
    TMod Import(ModPath modPath, StringsReadParameters? stringsParam = null);
}

public sealed class ModImporter : IModImporter, IModImporter<IModGetter>
{
    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameRelease;

    public ModImporter(
        IFileSystem fileSystem,
        IGameReleaseContext gameRelease)
    {
        _fileSystem = fileSystem;
        _gameRelease = gameRelease;
    }

    public TMod Import<TMod>(ModPath modPath, StringsReadParameters? stringsParam = null)
        where TMod : IModGetter
    {
        return ModInstantiator<TMod>.Importer(modPath, _gameRelease.Release, _fileSystem, stringsParam);
    }

    public IModGetter Import(ModPath modPath,StringsReadParameters? stringsParam = null)
    {
        return ModInstantiator.Importer(modPath, _gameRelease.Release, _fileSystem, stringsParam);
    }
}

public sealed class ModImporter<TMod> : IModImporter<TMod>
    where TMod : IModGetter
{
    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameRelease;

    public ModImporter(
        IFileSystem fileSystem,
        IGameReleaseContext gameRelease)
    {
        _fileSystem = fileSystem;
        _gameRelease = gameRelease;
    }

    public TMod Import(ModPath modPath, StringsReadParameters? stringsParam = null)
    {
        return ModInstantiator<TMod>.Importer(modPath, _gameRelease.Release, _fileSystem, stringsParam);
    }
}

public sealed class ModImporterWrapper<TMod> : IModImporter<TMod>
    where TMod : IModGetter
{
    private readonly Func<ModPath, StringsReadParameters?, TMod> _factory;

    public ModImporterWrapper(Func<ModPath, TMod> factory)
    {
        _factory = (p, _) => factory(p);
    }

    public ModImporterWrapper(Func<ModPath, StringsReadParameters?, TMod> factory)
    {
        _factory = factory;
    }

    public TMod Import(ModPath modPath, StringsReadParameters? stringsParam = null)
    {
        return _factory(modPath, stringsParam);
    }
}