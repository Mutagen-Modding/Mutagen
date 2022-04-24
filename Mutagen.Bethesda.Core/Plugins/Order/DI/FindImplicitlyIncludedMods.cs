using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IFindImplicitlyIncludedMods
{
    /// <summary>
    /// Given a list of mods to consider and their enabled state, locate all mods that are implicitly
    /// required but not active.
    /// </summary>
    /// <param name="loadOrderListing">List of mods to consider</param>
    /// <param name="skipMissingMods">Whether to skip any mod that does not exist in the data directory</param>
    /// <returns>ModKeys that were referenced but not enabled</returns>
    /// <exception cref="MissingModException">If a mod was missing and <see cref="skipMissingMods"/> was false</exception>
    IEnumerable<ModKey> Find(
        IEnumerable<IModListingGetter> loadOrderListing,
        bool skipMissingMods = false);
}

public class FindImplicitlyIncludedMods : IFindImplicitlyIncludedMods
{
    public IDataDirectoryProvider DirectoryProvider { get; }
    private readonly IFileSystem _fileSystem;
    public IMasterReferenceReaderFactory ReaderFactory { get; }

    public FindImplicitlyIncludedMods(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IMasterReferenceReaderFactory readerFactory)
    {
        _fileSystem = fileSystem;
        DirectoryProvider = dataDirectoryProvider;
        ReaderFactory = readerFactory;
    }

    public IEnumerable<ModKey> Find(
        IEnumerable<IModListingGetter> loadOrderListing,
        bool skipMissingMods = false)
    {
        var listingToIndices = loadOrderListing
            .ToDictionary(x => x.ModKey);
        HashSet<ModKey> referencedMasters = new();
        Queue<ModKey> toCheck = new(listingToIndices
            .Where(x => x.Value.Enabled)
            .Select(x => x.Value.ModKey));
        while (toCheck.Count > 0)
        {
            var key = toCheck.Dequeue();
            if (!referencedMasters.Add(key)) continue;
            if (!listingToIndices.TryGetValue(key, out var listing)) continue;
            if (!listing.Enabled)
            {
                yield return listing.ModKey;
            }
                
            var path = Path.Combine(DirectoryProvider.Path, listing.ModKey.FileName);
            if (!_fileSystem.File.Exists(path))
            {
                if (skipMissingMods) continue;
                throw new MissingModException(new ModPath(listing.ModKey, path));
            }

            try
            {
                var reader = ReaderFactory.FromPath(path);
                foreach (var master in reader.Masters)
                {
                    if (!referencedMasters.Contains(master.Master))
                    {
                        toCheck.Enqueue(master.Master);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ModHeaderMalformedException(new ModPath(key, path), innerException: e);
            }
        }
    }
}