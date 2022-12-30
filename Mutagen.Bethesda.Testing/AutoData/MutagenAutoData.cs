using AutoFixture;
using AutoFixture.Xunit2;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenAutoDataAttribute : AutoDataAttribute
{
    public MutagenAutoDataAttribute(
        GameRelease Release = GameRelease.SkyrimSE,
        bool ConfigureMembers = false, 
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