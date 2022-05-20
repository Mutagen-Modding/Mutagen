using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class QuestReferenceAliasBinaryOverlay
{
    public uint ID => throw new NotImplementedException();
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = Array.Empty<IConditionGetter>();
}

