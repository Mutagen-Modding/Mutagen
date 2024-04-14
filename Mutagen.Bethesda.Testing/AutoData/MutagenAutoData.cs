using AutoFixture;
using AutoFixture.Xunit2;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenAutoDataAttribute : AutoDataAttribute
{
    public MutagenAutoDataAttribute(
        GameRelease Release = GameRelease.SkyrimSE,
        bool ConfigureMembers = false, 
        TargetFileSystem FileSystem = TargetFileSystem.Fake)
        : base(() =>
        {
            return new Fixture()
                .Customize(new MutagenDefaultCustomization(
                    targetFileSystem: FileSystem,
                    configureMembers: ConfigureMembers,
                    release: Release));
        })
    {
    }
}