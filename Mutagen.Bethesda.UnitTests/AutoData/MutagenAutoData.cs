using AutoFixture;
using AutoFixture.Xunit2;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataAttribute : AutoDataAttribute
    {
        public MutagenAutoDataAttribute(
            bool ConfigureMembers = false, 
            GameRelease Release = GameRelease.SkyrimSE,
            bool UseMockFileSystem = true)
            : base(() =>
            {
                return new Fixture()
                    .Customize(new MutagenAutoDataCustomization(
                        useMockFilesystem: UseMockFileSystem,
                        configureMembers: ConfigureMembers,
                        release: Release));
            })
        {
        }
    }
}