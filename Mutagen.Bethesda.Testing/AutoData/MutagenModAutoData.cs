using AutoFixture;
using AutoFixture.Xunit2;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenModAutoDataAttribute : AutoDataAttribute
{
    public MutagenModAutoDataAttribute(
        GameRelease Release = GameRelease.SkyrimSE,
        TargetFileSystem FileSystem = TargetFileSystem.Fake,
        bool ConfigureMembers = false)
        : base(() =>
        {
            var ret = new Fixture();
            ret.Customize(new MutagenDefaultCustomization(
                targetFileSystem: FileSystem,
                configureMembers: ConfigureMembers,
                release: Release));
            ret.Customize(new MutagenConcreteModsCustomization(release: Release, configureMembers: ConfigureMembers));
            return ret;
        })
    {
    }
}