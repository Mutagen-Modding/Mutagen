using System;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.Utility;

namespace Mutagen.Bethesda.Core.UnitTests.Plugins.Allocators
{
    public class SQLiteFormKeyAllocator_Tests : ISharedFormKeyAllocator_Tests<SQLiteFormKeyAllocator>, IDisposable
    {
        protected override IFileSystem GetFileSystem() => IFileSystemExt.DefaultFilesystem;

        protected Lazy<TempFolder> tempFolder;

        protected Lazy<TempFile> tempFile;

        private bool disposedValue;

        public SQLiteFormKeyAllocator_Tests()
        {
            tempFolder = new(() => TempFolder.Factory());
            tempFile = new(() => new TempFile(extraDirectoryPaths: Utility.TempFolderPath));
        }
        
        protected override string ConstructTypicalPath(IFileSystem fileSystem)
        {
            return tempFile.Value.File.Path;
        }

        protected override SQLiteFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod, string path)
        {
            if (fileSystem != IFileSystemExt.DefaultFilesystem)
            {
                throw new NotImplementedException();
            }
            return new(mod, path);
        }

        protected override SQLiteFormKeyAllocator CreateNamedAllocator(IFileSystem fileSystem, IMod mod, string path, string patcherName)
        {
            return new(mod, path, patcherName);
        }

        public void Dispose()
        {
            if (tempFile.IsValueCreated)
                tempFile.Value.Dispose();
            if (tempFolder.IsValueCreated)
                tempFile.Value.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}