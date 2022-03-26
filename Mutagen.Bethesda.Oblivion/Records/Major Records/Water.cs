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
            public static partial void FillBinaryOddExtraBytesCustom(MutagenFrame frame, IWaterData item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            public static partial void FillBinaryBloodCustomLogicCustom(MutagenFrame frame, IWaterData item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }

            public static partial void FillBinaryOilCustomLogicCustom(MutagenFrame frame, IWaterData item)
            {
                if (frame.Remaining == 2)
                {
                    frame.Position += 2;
                }
            }
        }

        public partial class WaterDataBinaryWriteTranslation
        {
            public static partial void WriteBinaryBloodCustomLogicCustom(MutagenWriter writer, IWaterDataGetter item)
            {
            }

            public static partial void WriteBinaryOddExtraBytesCustom(MutagenWriter writer, IWaterDataGetter item)
            {
            }

            public static partial void WriteBinaryOilCustomLogicCustom(MutagenWriter writer, IWaterDataGetter item)
            {
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

            public static partial void FillBinaryDataCustom(MutagenFrame frame, IWaterInternal item)
            {
                item.Data = CreateCustom(frame);
            }
        }

        partial class WaterBinaryWriteTranslation
        {
            public static partial void WriteBinaryDataCustom(MutagenWriter writer, IWaterGetter item)
            {
                if (item.Data is not { } data) return;
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

            public partial IWaterDataGetter? GetDataCustom() => _waterData;
        }
    }
}
