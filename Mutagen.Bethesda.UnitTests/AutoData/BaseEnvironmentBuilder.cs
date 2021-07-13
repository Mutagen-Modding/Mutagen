using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.Kernel;
using FakeItEasy;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using Noggog.Reactive;
using Noggog.WPF;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class BaseEnvironmentBuilder : ISpecimenBuilder
    {
        private readonly GameRelease _release;

        public BaseEnvironmentBuilder(GameRelease release)
        {
            _release = release;
        }

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
            else if (t == typeof(IGameReleaseContext))
            {
                var ret = A.Fake<IGameReleaseContext>();
                A.CallTo(() => ret.Release).Returns(_release);
                return ret;
            }
            else if (t == typeof(IGameCategoryContext))
            {
                var ret = A.Fake<IGameCategoryContext>();
                A.CallTo(() => ret.Category).Returns(_release.ToCategory());
                return ret;
            }
            else if (t == typeof(IGameDirectoryProvider))
            {
                var dir = context.Create<DirectoryPath>();
                return new GameDirectoryInjection(Path.Combine(dir.Path, "GameDirectory"));
            }
            else if (t == typeof(IDataDirectoryProvider))
            {
                var dir = context.Create<IGameDirectoryProvider>();
                var ret = A.Fake<IDataDirectoryProvider>();
                A.CallTo(() => ret.Path).ReturnsLazily(() =>
                {
                    return Path.Combine(dir.Path, "DataDirectory");
                });
                return ret;
            }
            else if (t == typeof(IEnumerable<ModPath>))
            {
                var modKeys = context.CreateMany<ModKey>();
                var dataFolder = context.Create<IDataDirectoryProvider>();
                return modKeys.Select(x => Path.Combine(dataFolder.Path, x.FileName));
            }
            else if (t == typeof(IEnumerable<ModListing>))
            {
                var modKeys = context.CreateMany<ModKey>();
                return modKeys.Select(x => new ModListing(x, true));
            }
            else if (t == typeof(MockFileSystem))
            {
                var ret = new MockFileSystem(new Dictionary<string, MockFileData>())
                {
                    FileSystemWatcher = context.Create<IFileSystemWatcherFactory>()
                };
                var dir = context.Create<DirectoryPath>();
                ret.Directory.CreateDirectory(dir.Path);
                ret.Directory.CreateDirectory(Path.Combine(dir.Path, "GameDirectory", "DataDirectory"));
                ret.File.Create(Path.Combine(dir.Path, "Plugins.txt"));
                ret.File.Create(Path.Combine(dir.Path, "GameDirectory", $"{_release.ToCategory()}.ccc"));
                ret.File.Create(Path.Combine(dir.Path, "GameDirectory", "DataDirectory", Utility.PluginModKey.FileName));
                return ret;
            }
            else if (t == typeof(ISchedulerProvider))
            {
                var scheduler = A.Fake<ISchedulerProvider>();
                A.CallTo(() => scheduler.TaskPool).Returns(Scheduler.CurrentThread);
                A.CallTo(() => scheduler.MainThread).Returns(Scheduler.CurrentThread);
                return new SchedulerProvider();
            }

            return new NoSpecimen();
        }
    }
}