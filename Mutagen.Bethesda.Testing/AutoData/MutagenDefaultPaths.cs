using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenDefaultPaths : DefaultFileSystemEnvironmentInstructions
{
    public override IEnumerable<FilePath> FilePaths(ISpecimenContext context)
    {
        foreach (var path in base.FilePaths(context))
        {
            yield return path;
        }

        yield return Path.Combine(PathBuilder.ExistingDirectory, "Plugins.txt");
        var gameDir = context.Create<IGameDirectoryProvider>();
        var categoryContext = context.Create<IGameCategoryContext>();
        var gameDirPath = gameDir.Path;
        if (gameDirPath != null)
        {
            yield return Path.Combine(gameDirPath, $"{categoryContext.Category}.ccc");
        }
    }

    public override IEnumerable<DirectoryPath> DirectoryPaths(ISpecimenContext context)
    {
        foreach (var path in base.DirectoryPaths(context))
        {
            yield return path;
        }

        var gameDir = context.Create<IGameDirectoryProvider>();
        var gameDirPath = gameDir.Path;
        if (gameDirPath != null)
        {
            yield return Path.Combine(gameDirPath, "DataDirectory");
        }
    }
}