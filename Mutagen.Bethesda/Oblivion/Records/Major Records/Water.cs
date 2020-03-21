using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
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
        public partial class WaterBinaryCreateTranslation
        {
            static partial void FillBinaryOddExtraBytesCustom(MutagenFrame frame, IWaterInternal item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            static partial void FillBinaryBloodCustomLogicCustom(MutagenFrame frame, IWaterInternal item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            static partial void FillBinaryNothingCustomLogicCustom(MutagenFrame frame, IWaterInternal item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
                item.DATADataTypeState |= Water.DATADataType.Has;
            }

            static partial void FillBinaryOilCustomLogicCustom(MutagenFrame frame, IWaterInternal item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }
        }

        public partial class WaterBinaryOverlay
        {
            partial void DATACustomParse(BinaryMemoryReadStream stream, int offset)
            {
                this._DATALocation = (ushort)(stream.Position - offset) + _package.Meta.SubConstants.TypeAndLengthLength;
                this.DATADataTypeState = Water.DATADataType.Has;
                var subLen = _package.Meta.Subrecord(_data.Slice((stream.Position - offset))).ContentLength;
                if (subLen <= 0x2)
                {
                    this.DATADataTypeState |= Water.DATADataType.Break0;
                }
                if (subLen <= 0x2A)
                {
                    this.DATADataTypeState |= Water.DATADataType.Break1;
                }
                if (subLen <= 0x3E)
                {
                    this.DATADataTypeState |= Water.DATADataType.Break2;
                }
                if (subLen <= 0x56)
                {
                    this.DATADataTypeState |= Water.DATADataType.Break3;
                }
            }
        }
    }
}
