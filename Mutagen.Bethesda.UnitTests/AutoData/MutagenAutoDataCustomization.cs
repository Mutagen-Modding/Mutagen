using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataCustomization : ICustomization
    {
        private readonly GameRelease _release;
        private readonly bool _strict;
        private readonly bool _configureMembers;

        public MutagenAutoDataCustomization(
            bool strict,
            bool configureMembers, 
            GameRelease release)
        {
            _release = release;
            _strict = strict;
            _configureMembers = configureMembers;
        }
        
        public void Customize(IFixture fixture)
        {
            var autoMock = new AutoNSubstituteCustomization()
            {
                ConfigureMembers = _configureMembers,
                GenerateDelegates = true
            };
            fixture.Customize(autoMock);
            fixture.Customizations.Add(new BaseEnvironmentBuilder(_release));
            fixture.Customizations.Add(new ModKeyBuilder());
            fixture.Customizations.Add(new ModListingBuilder());
            fixture.Customizations.Add(new OrderBuilder());
            fixture.Customize(new DefaultCustomization());
        }
    }
}