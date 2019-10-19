using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;

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

        static partial void FillBinaryRegionAreaLogicCustom(MutagenFrame frame, IRegionInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var rdat = HeaderTranslation.GetNextSubRecordType(frame.Reader, out var rdatType);
            while (rdat.Equals(Region_Registration.RDAT_HEADER))
            {
                ParseRegionData(frame, item, masterReferences, errorMask);
                if (frame.Complete) break;
                rdat = HeaderTranslation.GetNextSubRecordType(frame.Reader, out rdatType);
            }
        }

        public static bool IsExpected(RegionData.RegionDataType dataType, RecordType recordType)
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

        static void ParseRegionData(MutagenFrame frame, IRegionInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var rdatFrame = frame.MetaData.GetSubRecordFrame(frame);
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.ContentSpan);
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
        static partial void WriteBinaryRegionAreaLogicCustom(MutagenWriter writer, IRegionGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
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

    public partial class RegionBinaryWrapper : IRegionGetter
    {
        #region Icon
        private int? _IconLocation;
        private int? _SecondaryIconLocation;
        bool GetIconIsSetCustom() => _IconLocation.HasValue || _SecondaryIconLocation.HasValue;
        string GetIconCustom()
        {
            if (_IconLocation.HasValue)
            {
                return BinaryStringUtility.ProcessWholeToZString(HeaderTranslation.ExtractSubrecordSpan(_data, _IconLocation.Value, _package.Meta));
            }
            if (_SecondaryIconLocation.HasValue)
            {
                return BinaryStringUtility.ProcessWholeToZString(HeaderTranslation.ExtractSubrecordSpan(_data, _SecondaryIconLocation.Value, _package.Meta));
            }
            return default;
        }
        #endregion

        private ReadOnlyMemorySlice<byte>? _ObjectsSpan;
        public bool Objects_IsSet => _ObjectsSpan.HasValue;
        public IRegionDataObjectsGetter Objects => RegionDataObjectsBinaryWrapper.RegionDataObjectsFactory(new BinaryMemoryReadStream(_ObjectsSpan.Value), _package);

        private ReadOnlyMemorySlice<byte>? _WeatherSpan;
        public bool Weather_IsSet => _WeatherSpan.HasValue;
        public IRegionDataWeatherGetter Weather => RegionDataWeatherBinaryWrapper.RegionDataWeatherFactory(new BinaryMemoryReadStream(_WeatherSpan.Value), _package);
        
        private ReadOnlyMemorySlice<byte>? _MapNameSpan;
        public bool MapName_IsSet => _MapNameSpan.HasValue;
        public IRegionDataMapNameGetter MapName => RegionDataMapNameBinaryWrapper.RegionDataMapNameFactory(new BinaryMemoryReadStream(_MapNameSpan.Value), _package);
        
        private ReadOnlyMemorySlice<byte>? _GrassesSpan;
        public bool Grasses_IsSet => _GrassesSpan.HasValue;
        public IRegionDataGrassesGetter Grasses => RegionDataGrassesBinaryWrapper.RegionDataGrassesFactory(new BinaryMemoryReadStream(_GrassesSpan.Value), _package);

        private ReadOnlyMemorySlice<byte>? _SoundsSpan;
        public bool Sounds_IsSet => _SoundsSpan.HasValue;
        public IRegionDataSoundsGetter Sounds => RegionDataSoundsBinaryWrapper.RegionDataSoundsFactory(new BinaryMemoryReadStream(_SoundsSpan.Value), _package);

        partial void RegionAreaLogicCustomParse(
            BinaryMemoryReadStream stream,
            int offset)
        {
            var rdat = this._package.Meta.GetSubRecord(stream);
            while (rdat.RecordType.Equals(Region_Registration.RDAT_HEADER))
            {
                ParseRegionData(stream, offset);
                if (stream.Complete) break;
                rdat = this._package.Meta.GetSubRecord(stream);
            }
        }

        partial void IconCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
        {
            _IconLocation = (ushort)(stream.Position - offset);
        }

        private void ParseRegionData(BinaryMemoryReadStream stream, int offset)
        {
            int loc = stream.Position - offset;
            var rdatFrame = this._package.Meta.ReadSubRecordFrame(stream);
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.ContentSpan);
            var len = rdatFrame.Header.TotalLength;
            if (!stream.Complete)
            {
                var contentMeta = this._package.Meta.GetSubRecord(stream);
                if (RegionBinaryCreateTranslation.IsExpected(dataType, contentMeta.RecordType))
                {
                    len += contentMeta.TotalLength;
                    stream.Position += contentMeta.TotalLength;
                }
            }
            switch (dataType)
            {
                case RegionData.RegionDataType.Objects:
                    _ObjectsSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.MapName:
                    _MapNameSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Grasses:
                    _GrassesSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Sounds:
                    var nextRec = this._package.Meta.GetSubRecord(stream);
                    if (nextRec.RecordType.Equals(RegionBinaryCreateTranslation.RDSD) || nextRec.RecordType.Equals(RegionBinaryCreateTranslation.RDMD))
                    {
                        len += nextRec.TotalLength;
                    }
                    _SoundsSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Weather:
                    _WeatherSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Icon:
                    _SecondaryIconLocation = loc + rdatFrame.Header.TotalLength;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
