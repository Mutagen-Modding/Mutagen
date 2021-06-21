using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Environments
{
    internal class DataDirectoryInjector : IDataDirectoryProvider
    {
        private readonly DirectoryPath _dataDirectory;

        public DataDirectoryInjector(DirectoryPath dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }
        
        public IEnumerable<DirectoryPath> GetAll(GameRelease release)
        {
            yield return _dataDirectory;
        }

        public bool TryGet(GameRelease release, out DirectoryPath path)
        {
            path = _dataDirectory;
            return true;
        }

        public DirectoryPath Get(GameRelease release)
        {
            return _dataDirectory;
        }
    }
}