using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim;

public partial class AStoryManagerNode
{
    [Flags]
    public enum Flag
    {
        Random = 0x01,
        WarnIfNoChildQuestStarted = 0x02,
    }
}

partial class AStoryManagerNodeBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayCountedList(stream, _package);
    }
}