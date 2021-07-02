using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Environments;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ICreationClubRawListingsReader
    {
        IEnumerable<IModListingGetter> Read(Stream stream);
    }

    public class CreationClubRawListingsReader : ICreationClubRawListingsReader
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDataDirectoryContext _dataDirectoryContext;

        public CreationClubRawListingsReader(
            IFileSystem fileSystem,
            IDataDirectoryContext dataDirectoryContext)
        {
            _fileSystem = fileSystem;
            _dataDirectoryContext = dataDirectoryContext;
        }
        
        public IEnumerable<IModListingGetter> Read(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            while (!streamReader.EndOfStream)
            {
                var str = streamReader.ReadLine().AsSpan();
                var modKey = ModKey.FromNameAndExtension(str);
                yield return new ModListing(modKey, enabled: true);
            }
        }
    }
}