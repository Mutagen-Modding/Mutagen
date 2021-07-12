using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.Kernel;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Noggog.Reactive;
using Noggog.WPF;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenSpecimenBuilder : ISpecimenBuilder
    {
        private readonly GameRelease _release;
        private bool _queriedForTestScheduler = false;

        public MutagenSpecimenBuilder(GameRelease release)
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
                return new GameReleaseInjection(_release);
            }
            else if (t == typeof(IGameDirectoryProvider))
            {
                var dir = context.Create<DirectoryPath>();
                return new GameDirectoryInjection(Path.Combine(dir.Path, "GameDirectory"));
            }
            else if (t == typeof(IDataDirectoryProvider))
            {
                var dir = context.Create<IGameDirectoryProvider>();
                return new DataDirectoryInjection(Path.Combine(dir.Path, "DataDirectory"));
            }
            else if (t == typeof(IPluginListingsPathProvider))
            {
                return new PluginListingsPathInjection($"C:\\ExistingDirectory\\Plugins.txt");
            }
            else if (t == typeof(ICreationClubListingsPathProvider))
            {
                var dir = context.Create<IGameDirectoryProvider>();
                return new CreationClubListingsPathInjection(Path.Combine(dir.Path, $"{_release.ToCategory()}.ccc"));
            }
            else if (t == typeof(IFileSystem))
            {
                return context.Create<MockFileSystem>();
            }
            else if (t == typeof(MockFileSystem))
            {
                var ret = new MockFileSystem(new Dictionary<string, MockFileData>())
                {
                    FileSystemWatcher = context.Create<IFileSystemWatcherFactory>()
                };
                ret.Directory.CreateDirectory("C:\\ExistingDirectory");
                ret.Directory.CreateDirectory("C:\\ExistingDirectory\\GameDirectory\\DataDirectory");
                ret.File.Create($"C:\\ExistingDirectory\\Plugins.txt");
                ret.File.Create($"C:\\ExistingDirectory\\GameDirectory\\{_release.ToCategory()}.ccc");
                ret.File.Create($"C:\\ExistingDirectory\\GameDirectory\\DataDirectory\\{Utility.PluginModKey}");
                return ret;
            }
            else if (t == typeof(ISchedulerProvider))
            {
                var scheduler = A.Fake<ISchedulerProvider>();
                A.CallTo(() => scheduler.TaskPool).Returns(Scheduler.CurrentThread);
                A.CallTo(() => scheduler.MainThread).Returns(Scheduler.CurrentThread);
                return new SchedulerProvider();
            }
            else if (t == typeof(IScheduler))
            {
                if (_queriedForTestScheduler) return context.Create<TestScheduler>();
                return Scheduler.CurrentThread;
            }
            else if (t == typeof(TestScheduler))
            {
                _queriedForTestScheduler = true;
                return new TestScheduler();
            }
            return new NoSpecimen();
        }
    }
}