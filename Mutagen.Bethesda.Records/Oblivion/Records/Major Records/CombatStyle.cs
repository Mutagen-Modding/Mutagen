using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CombatStyle
    {
        [Flags]
        public enum Flag
        {
            Advanced = 0x0001,
            ChooseAttackUsingPercentChance = 0x0002,
            IgnoreAlliesInArea = 0x0004,
            WillYield = 0x0008,
            RejectsYields = 0x0010,
            FleeingDisabled = 0x0020,
            PrefersRanged = 0x0040,
            MeleeAlertOK = 0x0080,
            DoNotAcquire = 0x0100,
        }
    }

    namespace Internals
    {
        public partial class CombatStyleBinaryCreateTranslation
        {
            static partial void FillBinarySecondaryFlagsCustom(MutagenFrame frame, ICombatStyleInternal item, MasterReferences masterReferences)
            {
                int flags = frame.ReadInt32();
                var otherFlag = (CombatStyle.Flag)(flags << 8);
                item.Flags |= otherFlag;
            }
        }

        public partial class CombatStyleBinaryWriteTranslation
        {
            static partial void WriteBinarySecondaryFlagsCustom(MutagenWriter writer, ICombatStyleGetter item, MasterReferences masterReferences)
            {
                int flags = (int)item.Flags;
                flags >>= 8;
                writer.Write(flags);
            }
        }

        public partial class CombatStyleBinaryOverlay
        {
            private bool GetFlagsIsSetCustom() => _CSTDLocation.HasValue;
            private CombatStyle.Flag GetFlagsCustom()
            {
                if (!_Flags_IsSet) return default;
                var ret = (CombatStyle.Flag)_data.Span.Slice(_FlagsLocation, 1)[0];
                if (!this.CSTDDataTypeState.HasFlag(CombatStyle.CSTDDataType.Break4))
                {
                    int flags = BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(_CSTDLocation!.Value + 0x78));
                    var otherFlag = (CombatStyle.Flag)(flags << 8);
                    ret |= otherFlag;
                }
                return ret;
            }
        }
    }
}
