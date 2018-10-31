using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;

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

        static partial void FillBinary_SecondaryFlags_Custom(MutagenFrame frame, CombatStyle item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            int flags = frame.ReadInt32();
            var otherFlag = (CombatStyle.Flag)(flags << 8);
            item.Flags = item.Flags | otherFlag;
        }

        static partial void WriteBinary_SecondaryFlags_Custom(MutagenWriter writer, CombatStyle item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            int flags = (int)item.Flags;
            flags = flags >> 8;
            writer.Write(flags);
        }
    }
}
