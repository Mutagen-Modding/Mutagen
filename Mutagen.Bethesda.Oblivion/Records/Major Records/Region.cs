using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class RegionBinaryCreateTranslation
    {
        public static readonly RecordType RDOT = new RecordType("RDOT");
        public static readonly RecordType RDWT = new RecordType("RDWT");
        public static readonly RecordType RDMP = new RecordType("RDMP");
        public static readonly RecordType ICON = new RecordType("ICON");
        public static readonly RecordType RDGS = new RecordType("RDGS");
        public static readonly RecordType RDSD = new RecordType("RDSD");
        public static readonly RecordType RDMD = new RecordType("RDMD");

        static partial void FillBinaryRegionAreaLogicCustom(MutagenFrame frame, Region item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var rdat = HeaderTranslation.GetNextSubRecordType(frame.Reader, out var rdatType);
            while (rdat.Equals(Region_Registration.RDAT_HEADER))
            {
                ParseRegionData(frame, item, masterReferences, errorMask);
                if (frame.Complete) break;
                rdat = HeaderTranslation.GetNextSubRecordType(frame.Reader, out rdatType);
            }
        }

        static bool IsExpected(RegionData.RegionDataType dataType, RecordType recordType)
        {
            switch (dataType)
            {
                case RegionData.RegionDataType.Objects:
                    if (!recordType.Equals(RDOT)) return false;
                    break;
                case RegionData.RegionDataType.Weather:
                    if (!recordType.Equals(RDWT)) return false;
                    break;
                case RegionData.RegionDataType.MapName:
                    if (!recordType.Equals(RDMP)) return false;
                    break;
                case RegionData.RegionDataType.Icon:
                    if (!recordType.Equals(ICON)) return false;
                    break;
                case RegionData.RegionDataType.Grasses:
                    if (!recordType.Equals(RDGS)) return false;
                    break;
                case RegionData.RegionDataType.Sounds:
                    if (!recordType.Equals(RDSD) && !recordType.Equals(RDMD)) return false;
                    break;
                default:
                    break;
            }
            return true;
        }

        static void ParseRegionData(MutagenFrame frame, Region item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var rdatFrame = frame.MetaData.GetSubRecordFrame(frame);
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.DataSpan);
            var subMeta = frame.MetaData.GetSubRecord(frame, offset: rdatFrame.Header.TotalLength);
            int len = rdatFrame.Header.TotalLength;
            if (IsExpected(dataType, subMeta.RecordType))
            {
                len += subMeta.TotalLength;
            }
            switch (dataType)
            {
                case RegionData.RegionDataType.Objects:
                    item.Objects = RegionDataObjects.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false), masterReferences);
                    break;
                case RegionData.RegionDataType.MapName:
                    item.MapName = RegionDataMapName.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false), masterReferences);
                    break;
                case RegionData.RegionDataType.Grasses:
                    item.Grasses = RegionDataGrasses.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false), masterReferences);
                    break;
                case RegionData.RegionDataType.Sounds:
                    var nextRec = frame.MetaData.GetSubRecord(frame, offset: len);
                    if (nextRec.RecordType.Equals(RDSD) || nextRec.RecordType.Equals(RDMD))
                    {
                        len += nextRec.TotalLength;
                    }
                    item.Sounds = RegionDataSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false), masterReferences);
                    break;
                case RegionData.RegionDataType.Weather:
                    item.Weather = RegionDataWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false), masterReferences);
                    break;
                case RegionData.RegionDataType.Icon:
                    frame.Position += frame.MetaData.SubConstants.HeaderLength + rdatFrame.Header.TotalLength;
                    len = len - frame.MetaData.SubConstants.HeaderLength - rdatFrame.Header.TotalLength;
                    if (StringBinaryTranslation.Instance.Parse(
                        frame.SpawnWithLength(len, checkFraming: false),
                        out var iconVal))
                    {
                        item.Icon = iconVal;
                    }
                    else
                    {
                        item.Icon_Unset();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public partial class RegionBinaryWriteTranslation
    {
        static partial void WriteBinaryRegionAreaLogicCustom(MutagenWriter writer, IRegionInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (item.Objects_IsSet)
            {
                item.Objects.WriteToBinary(writer, masterReferences);
            }
            if (item.Weather_IsSet)
            {
                item.Weather.WriteToBinary(writer, masterReferences);
            }
            if (item.MapName_IsSet)
            {
                item.MapName.WriteToBinary(writer, masterReferences);
            }
            if (item.Grasses_IsSet)
            {
                item.Grasses.WriteToBinary(writer, masterReferences);
            }
            if (item.Sounds_IsSet)
            {
                item.Sounds.WriteToBinary(writer, masterReferences);
            }
        }
    }
}
