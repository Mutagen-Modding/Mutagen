using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffect
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        [Flags]
        public enum Flag
        {
            Hostile = 0x0000_0001,
            Recover = 0x0000_0002,
            Detrimental = 0x0000_0004,
            SnapToNavmesh = 0x0000_0008,
            NoHitEvent = 0x0000_0010,
            DispelWithKeywords = 0x0000_0100,
            NoDuration = 0x0000_0200,
            NoMagnitude = 0x0000_0400,
            NoArea = 0x0000_0800,
            FXPersist = 0x0000_1000,
            GoryVisuals = 0x0000_4000,
            HideInUI = 0x0000_8000,
            NoRecast = 0x0002_0000,
            PowerAffectsMagnitude = 0x0020_0000,
            PowerAffectsDuration = 0x0040_0000,
            Painless = 0x0400_0000,
            NoHitEffect = 0x0800_0000,
            NoDeathDispel = 0x1000_0000,
        }

        public enum SoundType
        {
            SheathDraw = 0,
            Charge = 1,
            Ready = 2,
            Release = 3,
            ConcentrationCastLoop = 4,
            OnHit = 5,
        }
    }

    namespace Internals
    {
        public partial class MagicEffectBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IMagicEffectInternal item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }
        }

        public partial class MagicEffectBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IMagicEffectGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }
        }

        public partial class MagicEffectBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }
        }
    }
}
