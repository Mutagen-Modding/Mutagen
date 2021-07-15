using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class MutagenAutoDataCustomization : ICustomization
    {
        private readonly GameRelease _release;
        private readonly bool _useMockFilesystem;
        private readonly bool _configureMembers;

        public MutagenAutoDataCustomization(
            bool configureMembers, 
            GameRelease release,
            bool useMockFilesystem)
        {
            _release = release;
            _useMockFilesystem = useMockFilesystem;
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
            fixture.Customizations.Add(new FileSystemBuilder(_useMockFilesystem));
            fixture.Customizations.Add(new SchedulerBuilder());
            fixture.Customizations.Add(new PathBuilder());
        }
    }
}