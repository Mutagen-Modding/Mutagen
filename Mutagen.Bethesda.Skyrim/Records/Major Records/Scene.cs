using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Scene
    {
        [Flags]
        public enum Flag
        {
            BeginOnQuestStart = 0x001,
            StopOnQuestEnd = 0x002,
            RepeatConditionsWhileTrue = 0x008,
            Interruptable = 0x010,
        }
    }

    namespace Internals
    {
        public partial class SceneBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }
        }
    }
}
