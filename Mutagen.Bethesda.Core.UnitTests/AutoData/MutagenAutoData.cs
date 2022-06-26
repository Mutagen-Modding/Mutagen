using AutoFixture;
using AutoFixture.Xunit2;
using Mutagen.Bethesda.Testing.AutoData;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class MutagenAutoDataAttribute : AutoDataAttribute
{
    public MutagenAutoDataAttribute(
        bool ConfigureMembers = false, 
        GameRelease Release = GameRelease.SkyrimSE,
        bool UseMockFileSystem = true)
        : base(() =>
        {
            return new Fixture()
                .Customize(new MutagenDefaultCustomization(
                    useMockFileSystem: UseMockFileSystem,
                    configureMembers: ConfigureMembers,
                    release: Release));
        })
    {
    }
}