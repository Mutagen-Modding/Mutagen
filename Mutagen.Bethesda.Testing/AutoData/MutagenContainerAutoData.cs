using System.IO.Abstractions;
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
            fixture.Customize(new ContainerAutoDataCustomization(
                new MutagenTestModule(
                    Release,
                    fixture.Create<IFileSystem>(),
                    [])));
            return fixture;
        })
    {
    }
    
    public MutagenContainerAutoData(
        Module module,
        GameRelease Release = GameRelease.SkyrimSE,
        TargetFileSystem FileSystem = TargetFileSystem.Fake)
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize(new MutagenBaseCustomization());
            fixture.Customize(new MutagenReleaseCustomization(Release));
            fixture.Customize(new DefaultCustomization(FileSystem));
            fixture.Customize(new ContainerAutoDataCustomization(
                new MutagenTestModule(
                    Release,
                    fixture.Create<IFileSystem>(),
                    [module])));
            return fixture;
        })
    {
    }
}