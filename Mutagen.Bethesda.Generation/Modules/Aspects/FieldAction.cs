using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public record FieldAction(LoquiInterfaceType Type, string Name,
    Action<ObjectGeneration, TypeGeneration, FileGeneration> Actions);