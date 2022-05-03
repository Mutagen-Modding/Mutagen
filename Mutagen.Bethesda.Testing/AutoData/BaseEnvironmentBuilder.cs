using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.FileSystem;
using Noggog.Testing.IO;
using NSubstitute;

namespace Mutagen.Bethesda.Testing.AutoData;

public class BaseEnvironmentBuilder : ISpecimenBuilder
{
    private readonly GameRelease _release;
    private MockFileSystem? _mockFileSystem;
    public static readonly DirectoryPath GameDirectory = $"{PathingUtil.DrivePrefix}GameDirectory";

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
            return new GameDirectoryInjection(GameDirectory);
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
            if (_mockFileSystem == null)
            {
                _mockFileSystem = new NoggogMockFileSystem(
                    new Dictionary<string, MockFileData>(),
                    fileSystemWatcher: context.Create<IFileSystemWatcherFactory>());
                _mockFileSystem.Directory.CreateDirectory(PathBuilder.ExistingDirectory);
                _mockFileSystem.File.Create(PathBuilder.ExistingFile);
                _mockFileSystem.Directory.CreateDirectory(Path.Combine(GameDirectory, "DataDirectory"));
                _mockFileSystem.File.Create(Path.Combine(PathBuilder.ExistingDirectory, "Plugins.txt"));
                _mockFileSystem.File.Create(Path.Combine(GameDirectory, $"{_release.ToCategory()}.ccc"));
            }
            return _mockFileSystem;
        }

        return new NoSpecimen();
    }
}