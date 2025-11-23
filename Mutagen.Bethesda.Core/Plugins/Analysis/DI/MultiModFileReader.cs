using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

/// <summary>
/// Reads multiple split mod files and merges them into a single mod object.
/// Used to read mods that were split due to exceeding master limits.
/// </summary>
public interface IMultiModFileReader
{
    /// <summary>
    /// Reads split mod files from a directory and returns a multi-file overlay that presents them as a single unified mod.
    /// The returned mod is read-only (implements IModGetter only).
    /// </summary>
    /// <typeparam name="TModGetter">Getter mod type</typeparam>
    /// <param name="folder">Directory containing the split mod files</param>
    /// <param name="modKey">The base ModKey to look for (without _1, _2 suffixes)</param>
    /// <param name="gameRelease">The game release for the mods</param>
    /// <param name="loadOrder">Load order to use for master ordering</param>
    /// <param name="readParams">Binary read parameters</param>
    /// <returns">Merged mod overlay with the original ModKey (read-only)</returns>
    TModGetter Read<TModGetter>(
        DirectoryPath folder,
        ModKey modKey,
        GameRelease gameRelease,
        IEnumerable<IModMasterStyledGetter> loadOrder,
        BinaryReadParameters readParams)
        where TModGetter : class, IModDisposeGetter;
}

/// <summary>
/// Implementation of multi-mod file reader for dependency injection scenarios.
/// For static access, use <see cref="MultiModFileAnalysis"/>.
/// </summary>
public class MultiModFileReader : IMultiModFileReader
{
    /// <inheritdoc />
    public TModGetter Read<TModGetter>(
        DirectoryPath folder,
        ModKey modKey,
        GameRelease gameRelease,
        IEnumerable<IModMasterStyledGetter> loadOrder,
        BinaryReadParameters readParams)
        where TModGetter : class, IModDisposeGetter
    {
        var fileSystem = readParams.FileSystem.GetOrDefault();

        // Get and validate split files
        var splitFiles = MultiModFileAnalysis.GetSplitModFiles(folder, modKey, fileSystem);

        if (splitFiles.Count == 0)
        {
            throw new SplitModException($"No split files found for {modKey} in {folder}");
        }

        // Create a ModImporter to handle the multi-file import
        var gameReleaseContext = new GameReleaseInjection(gameRelease);
        var modImporter = new Records.DI.ModImporter(fileSystem, gameReleaseContext);

        // Use ModImporter to read and merge the split files
        return modImporter.ImportMultiFile<TModGetter>(modKey, splitFiles.Select(f => (ModPath)f.Path), loadOrder, readParams);
    }
}
