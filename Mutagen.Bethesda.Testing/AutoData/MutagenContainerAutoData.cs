using Autofac;
using AutoFixture;
using AutoFixture.Xunit2;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenContainerAutoData : AutoDataAttribute
{
    public MutagenContainerAutoData(
        GameRelease Release = GameRelease.SkyrimSE,
        TargetFileSystem FileSystem = TargetFileSystem.Fake)
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize(new MutagenBaseCustomization());
            fixture.Customize(new MutagenReleaseCustomization(Release));
            fixture.Customize(new DefaultCustomization(FileSystem));
            fixture.Customize(new MutagenConcreteModsCustomization(release: Release, configureMembers: false));
            fixture.Customize(new MutagenContainerCustomization(Release));
            return fixture;
        })
    {
    }
}

public class MutagenContainerAutoData<TModule> : AutoDataAttribute
    where TModule : Module, new()
{
    public MutagenContainerAutoData(
        GameRelease Release = GameRelease.SkyrimSE,
        TargetFileSystem FileSystem = TargetFileSystem.Fake)
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize(new MutagenBaseCustomization());
            fixture.Customize(new MutagenReleaseCustomization(Release));
            fixture.Customize(new DefaultCustomization(FileSystem));
            fixture.Customize(new MutagenConcreteModsCustomization(release: Release, configureMembers: false));
            fixture.Customize(new MutagenContainerCustomization(Release, new TModule()));
            return fixture;
        })
    {
    }
}