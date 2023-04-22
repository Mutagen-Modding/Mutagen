using AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenConcreteModsCustomization : ICustomization
{
    private readonly GameRelease _release;
    private readonly bool _configureMembers;

    public MutagenConcreteModsCustomization(GameRelease release, bool configureMembers)
    {
        _release = release;
        _configureMembers = configureMembers;
    }
        
    public void Customize(IFixture fixture)
    {
        var modBuilder = new ModConcreteBuilder(_release);
        fixture.Customizations.Add(modBuilder);
        fixture.Customizations.Add(new MajorRecordBuilder(_release, modBuilder, _configureMembers));
    }
}