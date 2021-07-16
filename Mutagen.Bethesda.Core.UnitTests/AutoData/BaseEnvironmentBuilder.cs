using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Noggog.Reactive;
using Noggog.WPF;
using NSubstitute;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
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
            if (request is SeededRequest seed)
            {
                request = seed.Request;
            }
            
            if (request is not Type t) return new NoSpecimen();
            if (t == typeof(IGameReleaseContext))
            {
                var ret = Substitute.For<IGameReleaseContext>();
                ret.Release.Returns(_release);
                return ret;
            }
            else if (t == typeof(IGameCategoryContext))
            {
                var ret = Substitute.For<IGameCategoryContext>();
                ret.Category.Returns(_release.ToCategory());
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
                var ret = Substitute.For<IDataDirectoryProvider>();
                ret.Path.Returns(x =>
                {
                    return new DirectoryPath(Path.Combine(dir.Path, "DataDirectory"));
                });
                return ret;
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
                var scheduler = Substitute.For<ISchedulerProvider>();
                scheduler.TaskPool.Returns(Scheduler.CurrentThread);
                scheduler.MainThread.Returns(Scheduler.CurrentThread);
                return new SchedulerProvider();
            }

            return new NoSpecimen();
        }
    }
}