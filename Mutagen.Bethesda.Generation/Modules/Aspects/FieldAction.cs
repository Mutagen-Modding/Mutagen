using Loqui.Generation;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public record FieldAction(LoquiInterfaceType Type, string Name,
    Action<ObjectGeneration, TypeGeneration, StructuredStringBuilder> Actions);