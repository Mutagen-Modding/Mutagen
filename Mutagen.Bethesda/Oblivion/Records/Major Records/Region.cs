using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
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

        static partial void FillBinaryRegionAreaLogicCustom(MutagenFrame frame, IRegionInternal item)
        {
            var rdat = HeaderTranslation.GetNextSubRecordType(frame.Reader, out var rdatType);
            while (rdat.Equals(Region_Registration.RDAT_HEADER))
            {
                ParseRegionData(frame, item);
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

        static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
        {
            var rdatFrame = frame.GetSubRecordFrame();
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
            var subMeta = frame.GetSubRecord(offset: rdatFrame.Header.TotalLength);
            int len = rdatFrame.Header.TotalLength;
            if (IsExpected(dataType, subMeta.RecordType))
            {
                len += subMeta.TotalLength;
            }
            switch (dataType)
            {
                case RegionData.RegionDataType.Objects:
                    item.Objects = RegionDataObjects.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.MapName:
                    item.MapName = RegionDataMapName.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Grasses:
                    item.Grasses = RegionDataGrasses.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Sounds:
                    var nextRec = frame.GetSubRecord(offset: len);
                    if (nextRec.RecordType.Equals(RDSD) || nextRec.RecordType.Equals(RDMD))
                    {
                        len += nextRec.TotalLength;
                    }
                    item.Sounds = RegionDataSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Weather:
                    item.Weather = RegionDataWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
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
                        item.Icon = null;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public partial class RegionBinaryWriteTranslation
    {
        static partial void WriteBinaryRegionAreaLogicCustom(MutagenWriter writer, IRegionGetter item)
        {
            item.Objects?.WriteToBinary(writer);
            item.Weather?.WriteToBinary(writer);
            item.MapName?.WriteToBinary(writer);
            item.Grasses?.WriteToBinary(writer);
            item.Sounds?.WriteToBinary(writer);
        }
    }

    public partial class RegionBinaryOverlay : IRegionGetter
    {
        #region Icon
        private int? _IconLocation;
        private int? _SecondaryIconLocation;
        string? GetIconCustom()
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
        public IRegionDataObjectsGetter? Objects => _ObjectsSpan.HasValue ? RegionDataObjectsBinaryOverlay.RegionDataObjectsFactory(new BinaryMemoryReadStream(_ObjectsSpan.Value), _package) : default;

        private ReadOnlyMemorySlice<byte>? _WeatherSpan;
        public IRegionDataWeatherGetter? Weather => _WeatherSpan.HasValue ? RegionDataWeatherBinaryOverlay.RegionDataWeatherFactory(new BinaryMemoryReadStream(_WeatherSpan.Value), _package) : default;
        
        private ReadOnlyMemorySlice<byte>? _MapNameSpan;
        public IRegionDataMapNameGetter? MapName => _MapNameSpan.HasValue ? RegionDataMapNameBinaryOverlay.RegionDataMapNameFactory(new BinaryMemoryReadStream(_MapNameSpan.Value), _package) : default;
        
        private ReadOnlyMemorySlice<byte>? _GrassesSpan;
        public IRegionDataGrassesGetter? Grasses => _GrassesSpan.HasValue ? RegionDataGrassesBinaryOverlay.RegionDataGrassesFactory(new BinaryMemoryReadStream(_GrassesSpan.Value), _package) : default;

        private ReadOnlyMemorySlice<byte>? _SoundsSpan;
        public IRegionDataSoundsGetter? Sounds => _SoundsSpan.HasValue ? RegionDataSoundsBinaryOverlay.RegionDataSoundsFactory(new BinaryMemoryReadStream(_SoundsSpan.Value), _package) : default;

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
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
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
