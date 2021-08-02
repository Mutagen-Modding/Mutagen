using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class MutagenDefaultCustomization : ICustomization
    {
        private readonly bool _configureMembers;
        private readonly GameRelease _release;
        private readonly bool _useMockFileSystem;

        public MutagenDefaultCustomization(
            bool configureMembers, 
            GameRelease release,
            bool useMockFileSystem)
        {
            _configureMembers = configureMembers;
            _release = release;
            _useMockFileSystem = useMockFileSystem;
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
            fixture.Customize(new DefaultCustomization(_useMockFileSystem));
        }
    }
}