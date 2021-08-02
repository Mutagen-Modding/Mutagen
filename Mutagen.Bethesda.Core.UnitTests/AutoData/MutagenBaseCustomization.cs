using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class MutagenBaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new ModKeyBuilder());
            fixture.Customizations.Add(new ModListingBuilder());
            fixture.Customizations.Add(new OrderBuilder());
        }
    }
}