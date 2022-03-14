namespace Mutagen.Bethesda.Generation.Generator;

public class GenerationRunner
{
    private readonly IGenerationConstructor[] _generationConstructors;

    public GenerationRunner(IGenerationConstructor[] generationConstructors)
    {
        _generationConstructors = generationConstructors;
    }

    public async Task Generate()
    {
        foreach (var item in _generationConstructors)
        {
            await item.Construct().Generate();
        }
    }
}
