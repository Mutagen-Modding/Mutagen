using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
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
            fixture.Customize(new MutagenBaseCustomization());
            fixture.Customize(new MutagenReleaseCustomization(_release));
            fixture.Customize(new DefaultCustomization(_useMockFilesystem));
        }
    }
}