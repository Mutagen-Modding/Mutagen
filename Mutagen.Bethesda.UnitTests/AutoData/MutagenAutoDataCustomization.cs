using AutoFixture;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataCustomization : ICustomization
    {
        private readonly GameRelease _release;

        public MutagenAutoDataCustomization(GameRelease release)
        {
            _release = release;
        }
        
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new MutagenSpecimenBuilder(_release));
        }
    }
}