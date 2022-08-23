using AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenReleaseCustomization : ICustomization
{
    private readonly GameRelease _release;

    public MutagenReleaseCustomization(GameRelease release)
    {
        _release = release;
    }
        
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new BaseEnvironmentBuilder(_release));
        var modBuilder = new ModMocksBuilder(_release);
        fixture.Customizations.Add(modBuilder);
        fixture.Customizations.Add(new GameReleaseBuilder(_release));
    }
}