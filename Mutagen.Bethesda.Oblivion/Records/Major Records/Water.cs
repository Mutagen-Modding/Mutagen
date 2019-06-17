using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Water
    {
        public enum Flag
        {
            CausesDamage = 0x01,
            Reflective = 0x02
        }
    }

    namespace Internals
    {
        public partial class WaterBinaryTranslation
        {
            static partial void FillBinaryOddExtraBytesCustom(MutagenFrame frame, Water item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            static partial void FillBinaryBloodCustomLogicCustom(MutagenFrame frame, Water item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            static partial void FillBinaryNothingCustomLogicCustom(MutagenFrame frame, Water item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
                item.DATADataTypeState |= Water.DATADataType.Has;
            }

            static partial void FillBinaryOilCustomLogicCustom(MutagenFrame frame, Water item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }
        }
    }
}
