namespace Mutagen.Bethesda.Generation.Generator;

public interface IGenerateCoreSettings
{
    bool ShouldGenerate { get; }
}

public class ShouldNotGenerateCoreSettings : IGenerateCoreSettings
{
    public bool ShouldGenerate => false;
}
