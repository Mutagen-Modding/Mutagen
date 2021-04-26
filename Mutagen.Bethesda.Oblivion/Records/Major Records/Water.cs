using System;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
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
        public partial class WaterDataBinaryCreateTranslation
        {
            static partial void FillBinaryOddExtraBytesCustom(MutagenFrame frame, IWaterData item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            static partial void FillBinaryBloodCustomLogicCustom(MutagenFrame frame, IWaterData item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            static partial void FillBinaryOilCustomLogicCustom(MutagenFrame frame, IWaterData item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }
        }

        public partial class WaterBinaryCreateTranslation
        {
            public static WaterData CreateCustom(MutagenFrame frame)
            {
                var subHeader = frame.GetSubrecord();

                if (subHeader.ContentLength == 2)
                {
                    return new WaterData()
                    {
                        Versioning = WaterData.VersioningBreaks.Break0
                    };
                }
                else
                {
                    return WaterData.CreateFromBinary(frame);
                }
            }

            static partial void FillBinaryDataCustom(MutagenFrame frame, IWaterInternal item)
            {
                item.Data = CreateCustom(frame);
            }
        }

        partial class WaterBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IWaterGetter item)
            {
                if (!item.Data.TryGet(out var data)) return;
                data.WriteToBinary(writer);
            }
        }

        public partial class WaterBinaryOverlay
        {
            private WaterData? _waterData;

            partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                this._waterData = WaterBinaryCreateTranslation.CreateCustom(new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)));
            }

            WaterData? GetDataCustom() => _waterData;
        }
    }
}
