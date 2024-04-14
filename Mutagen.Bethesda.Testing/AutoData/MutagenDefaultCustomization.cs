using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenDefaultCustomization : ICustomization
{
    private readonly bool _configureMembers;
    private readonly GameRelease _release;
    private readonly TargetFileSystem _targetFileSystem;

    public MutagenDefaultCustomization(
        bool configureMembers, 
        GameRelease release,
        TargetFileSystem targetFileSystem)
    {
        _configureMembers = configureMembers;
        _release = release;
        _targetFileSystem = targetFileSystem;
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
        fixture.Customize(new DefaultCustomization(_targetFileSystem));
    }
}