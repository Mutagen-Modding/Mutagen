using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            if (t == typeof(ModKey))
            {
                return Utility.PluginModKey;
            }
            else if (t == typeof(ModPath))
            {
                var dataDir = context.Create<IDataDirectoryProvider>();
                var modKey = context.Create<ModKey>();
                return new ModPath(modKey, Path.Combine(dataDir.Path, modKey.FileName));
            }
            else if (t == typeof(IDataDirectoryProvider))
            {
                var dir = context.Create<DirectoryPath>();
                return new DataDirectoryInjection(Path.Combine(dir.Path, "DataDirectory"));
            }
            else if (t == typeof(IFileSystem))
            {
                return context.Create<MockFileSystem>();
            }
            else if(t == typeof(MockFileSystem))
            {
                var ret = new MockFileSystem(new Dictionary<string, MockFileData>())
                {
                    FileSystemWatcher = context.Create<IFileSystemWatcherFactory>()
                };
                ret.Directory.CreateDirectory("C:\\ExistingDirectory");
                ret.Directory.CreateDirectory("C:\\ExistingDirectory\\DataDirectory");
                ret.File.Create($"C:\\ExistingDirectory\\DataDirectory\\{Utility.PluginModKey}");
                return ret;
            }
            return new NoSpecimen();
        }
    }
}