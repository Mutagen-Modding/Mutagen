using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataAttribute : AutoDataAttribute
    {
        public MutagenAutoDataAttribute(bool Strict = true, bool ConfigureMembers = false)
            : base(() =>
            {
                var customization = new AutoFakeItEasyCustomization()
                {
                    ConfigureMembers = ConfigureMembers,
                    GenerateDelegates = true
                };
                if (Strict)
                {
                    customization.Relay = new FakeItEasyStrictRelay();
                }
                return new Fixture()
                    .Customize(customization)
                    .Customize(new MutagenAutoDataCustomization())
                    .Customize(new FileSystemCustomization());
            })
        {
        }
    }
}