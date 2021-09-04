using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class EffectBinaryOverlay
    {
        public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

        partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousSubrecordParse lastParsed)
        {
            Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
        }
    }
}
