using AutoFixture;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new MutagenSpecimenBuilder());
        }
    }
}