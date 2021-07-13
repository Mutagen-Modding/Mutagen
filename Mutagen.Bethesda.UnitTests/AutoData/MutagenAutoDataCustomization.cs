using AutoFixture;
using AutoFixture.AutoFakeItEasy;
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
            var autoFakeItEasy = new AutoFakeItEasyCustomization()
            {
                ConfigureMembers = _configureMembers,
                GenerateDelegates = true
            };
            if (_strict)
            {
                autoFakeItEasy.Relay = new FakeItEasyStrictRelay();
            }
            fixture.Customize(autoFakeItEasy);
            fixture.Customizations.Add(new BaseEnvironmentBuilder(_release));
            fixture.Customizations.Add(new OrderBuilder());
            fixture.Customize(new DefaultCustomization());
        }
    }
}