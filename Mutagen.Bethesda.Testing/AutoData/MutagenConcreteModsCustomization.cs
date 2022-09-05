using AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenConcreteModsCustomization : ICustomization
{
    private readonly GameRelease _release;

    public MutagenConcreteModsCustomization(GameRelease release)
    {
        _release = release;
    }
        
    public void Customize(IFixture fixture)
    {
        var modBuilder = new ModConcreteBuilder(_release);
        fixture.Customizations.Add(modBuilder);
        fixture.Customizations.Add(new MajorRecordBuilder(_release, modBuilder));
    }
}