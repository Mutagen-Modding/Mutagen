using AutoFixture;
using AutoFixture.Xunit2;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataAttribute : AutoDataAttribute
    {
        public MutagenAutoDataAttribute(
            bool Strict = true,
            bool ConfigureMembers = false, 
            GameRelease Release = GameRelease.SkyrimSE)
            : base(() =>
            {
                return new Fixture()
                    .Customize(new MutagenAutoDataCustomization(
                        strict: Strict,
                        configureMembers: ConfigureMembers,
                        release: Release));
            })
        {
        }
    }
}