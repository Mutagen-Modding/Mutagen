using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Generator;

public interface IGenerationConstructor
{
    LoquiGenerator Construct();
}
