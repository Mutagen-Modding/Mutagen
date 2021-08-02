using AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class MutagenBaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new ModKeyBuilder());
            fixture.Customizations.Add(new ModListingBuilder());
            fixture.Customizations.Add(new ModPathBuilder());
            fixture.Customizations.Add(new OrderBuilder());
        }
    }
}