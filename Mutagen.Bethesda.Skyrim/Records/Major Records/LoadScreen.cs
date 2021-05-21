using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class LoadScreen
    {
        [Flags]
        public enum MajorFlag
        {
            DisplaysInMainMenu = 0x0000_0400
        }
    }

    namespace Internals
    {
        public partial class LoadScreenBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }
        }
    }
}
