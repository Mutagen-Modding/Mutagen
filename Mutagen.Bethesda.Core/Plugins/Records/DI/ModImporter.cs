using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;

namespace Mutagen.Bethesda.Plugins.Records.DI;

public interface IModImporter
{
    TMod Import<TMod>(ModPath modPath, BinaryReadParameters? param = null)
        where TMod : IModGetter;

    IModGetter Import(ModPath modPath, BinaryReadParameters? param = null);

    /// <summary>
    /// Imports multiple split mod files and returns a multi-file overlay that presents them as a single unified mod.
    /// </summary>
    /// <typeparam name="TMod">Type of mod to return</typeparam>
    /// <param name="targetModKey">The ModKey for the unified overlay (typically the base name without _1, _2 suffixes)</param>
    /// <param name="splitFiles">Paths to the split mod files to merge</param>
    /// <param name="loadOrder">Load order to use for master ordering</param>
    /// <param name="param">Binary read parameters</param>
    /// <returns>Multi-file overlay presenting all split files as a single mod</returns>
    TMod ImportMultiFile<TMod>(
        ModKey targetModKey,
        IEnumerable<ModPath> splitFiles,
        IEnumerable<IModMasterStyledGetter> loadOrder,
        BinaryReadParameters? param = null)
        where TMod : IModDisposeGetter;
}
    
public interface IModImporter<TMod>
    where TMod : IModKeyed
{
    TMod Import(ModPath modPath, BinaryReadParameters? param = null);
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

    public TMod Import<TMod>(ModPath modPath, BinaryReadParameters? param = null)
        where TMod : IModGetter
    {
        param = (param ?? BinaryReadParameters.Default) with
        {
            FileSystem = _fileSystem
        };
        return ModFactory<TMod>.Importer(modPath, _gameRelease.Release, param);
    }

    public IModGetter Import(ModPath modPath, BinaryReadParameters? param = null)
    {
        param = (param ?? BinaryReadParameters.Default) with
        {
            FileSystem = _fileSystem
        };
        return ModFactory.ImportGetter(modPath, _gameRelease.Release, param);
    }

    public TMod ImportMultiFile<TMod>(
        ModKey targetModKey,
        IEnumerable<ModPath> splitFiles,
        IEnumerable<IModMasterStyledGetter> loadOrder,
        BinaryReadParameters? param = null)
        where TMod : IModDisposeGetter
    {
        param = (param ?? BinaryReadParameters.Default) with
        {
            FileSystem = _fileSystem
        };

        return ModFactory<TMod>.ImportMultiFileGetter(targetModKey, splitFiles, loadOrder, _gameRelease.Release, param);
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

    public TMod Import(ModPath modPath, BinaryReadParameters? param = null)
    {
        param = (param ?? BinaryReadParameters.Default) with
        {
            FileSystem = _fileSystem
        };
        return ModFactory<TMod>.Importer(modPath, _gameRelease.Release, param);
    }
}

public sealed class ModImporterWrapper<TMod> : IModImporter<TMod>
    where TMod : IModKeyed
{
    private readonly Func<ModPath, BinaryReadParameters?, TMod> _factory;

    public ModImporterWrapper(Func<ModPath, TMod> factory)
    {
        _factory = (p, _) => factory(p);
    }

    public ModImporterWrapper(Func<ModPath, BinaryReadParameters?, TMod> factory)
    {
        _factory = factory;
    }

    public TMod Import(ModPath modPath, BinaryReadParameters? param = null)
    {
        return _factory(modPath, param);
    }
}