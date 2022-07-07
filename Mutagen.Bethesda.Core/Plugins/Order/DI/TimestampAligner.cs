using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ITimestampAligner
{
    /// <summary>
    /// Returns whether given game needs timestamp alignment for its load order
    /// </summary>
    /// <param name="game">Game to check</param>
    /// <returns>True if file located</returns>
    bool NeedsTimestampAlignment(GameCategory game);

    /// <summary>
    /// Constructs a load order from a list of mods and a data folder.
    /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
    /// </summary>
    /// <param name="incomingLoadOrder">Mods to include</param>
    /// <param name="dataPath">Path to data folder</param>
    /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
    /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
    /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
    IEnumerable<ILoadOrderListingGetter> AlignToTimestamps(
        IEnumerable<ILoadOrderListingGetter> incomingLoadOrder,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true);

    /// <summary>
    /// Constructs a load order from a list of mods and a data folder.
    /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
    /// </summary>
    /// <param name="incomingLoadOrder">Mods and their write timestamps</param>
    /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
    /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
    IEnumerable<ModKey> AlignToTimestamps(IEnumerable<(ModKey ModKey, DateTime Write)> incomingLoadOrder);

    /// <summary>
    /// Modifies time stamps of files to match the given ordering
    /// <param name="loadOrder">Order to conform files to</param>
    /// <param name="dataPath">Path to data folder</param>
    /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
    /// <param name="startDate">Date to give the first file</param>
    /// <param name="interval">Time interval to space between each file's date</param>
    /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
    /// </summary>
    void AlignTimestamps(
        IEnumerable<ModKey> loadOrder,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true,
        DateTime? startDate = null,
        TimeSpan? interval = null);
}

public sealed class TimestampAligner : ITimestampAligner
{
    private readonly IFileSystem _FileSystem;

    public TimestampAligner(IFileSystem fileSystem)
    {
        _FileSystem = fileSystem;
    }
        
    /// <inheritdoc />
    public bool NeedsTimestampAlignment(GameCategory game)
    {
        switch (game)
        {
            case GameCategory.Oblivion:
                return true;
            case GameCategory.Skyrim:
                return false;
            case GameCategory.Fallout4:
                return false;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public IEnumerable<ILoadOrderListingGetter> AlignToTimestamps(
        IEnumerable<ILoadOrderListingGetter> incomingLoadOrder,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true)
    {
        var list = new List<(bool Enabled, ModKey ModKey, DateTime Write)>();
        foreach (var key in incomingLoadOrder)
        {
            ModPath modPath = new ModPath(key.ModKey, Path.Combine(dataPath.Path, key.ModKey.FileName));
            if (!_FileSystem.File.Exists(modPath.Path))
            {
                if (throwOnMissingMods) throw new MissingModException(modPath);
                continue;
            }
            list.Add((key.Enabled, key.ModKey, _FileSystem.File.GetLastWriteTime(modPath.Path.Path)));
        }
        var comp = new LoadOrderTimestampComparer(incomingLoadOrder.Select(i => i.ModKey).ToList());
        return list
            .OrderBy(i => (i.ModKey, i.Write), comp)
            .Select(i => new LoadOrderListing(i.ModKey, i.Enabled));
    }

    /// <inheritdoc />
    public IEnumerable<ModKey> AlignToTimestamps(IEnumerable<(ModKey ModKey, DateTime Write)> incomingLoadOrder)
    {
        return incomingLoadOrder
            .OrderBy(i => i, new LoadOrderTimestampComparer(incomingLoadOrder.Select(i => i.ModKey).ToList()))
            .Select(i => i.ModKey);
    }

    /// <inheritdoc />
    public void AlignTimestamps(
        IEnumerable<ModKey> loadOrder,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true,
        DateTime? startDate = null,
        TimeSpan? interval = null)
    {
        startDate ??= DateTime.Today.AddDays(-1);
        interval ??= TimeSpan.FromMinutes(1);
        foreach (var mod in loadOrder)
        {
            ModPath modPath = new ModPath(mod, Path.Combine(dataPath.Path, mod.FileName));
            if (!modPath.Path.Exists)
            {
                if (throwOnMissingMods) throw new MissingModException(modPath);
                continue;
            }
            _FileSystem.File.SetLastWriteTime(modPath.Path.Path, startDate.Value);
            startDate = startDate.Value.Add(interval.Value);
        }
    }
}