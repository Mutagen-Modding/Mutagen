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
            var rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out var rdatType);
            while (rdat.Equals(Region_Registration.RDAT_HEADER))
            {
                ParseRegionData(frame, item);
                if (frame.Complete) break;
                rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out rdatType);
            }
        }

        public static bool IsExpected(RegionData.RegionDataType dataType, RecordType recordType)
        {
            switch (dataType)
            {
                case RegionData.RegionDataType.Object:
                    if (!recordType.Equals(RDOT)) return false;
                    break;
                case RegionData.RegionDataType.Weather:
                    if (!recordType.Equals(RDWT)) return false;
                    break;
                case RegionData.RegionDataType.Map:
                    if (!recordType.Equals(RDMP)) return false;
                    break;
                case RegionData.RegionDataType.Icon:
                    if (!recordType.Equals(ICON)) return false;
                    break;
                case RegionData.RegionDataType.Grass:
                    if (!recordType.Equals(RDGS)) return false;
                    break;
                case RegionData.RegionDataType.Sound:
                    if (!recordType.Equals(RDSD) && !recordType.Equals(RDMD)) return false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
        {
            var rdatFrame = frame.GetSubrecordFrame();
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
            var subMeta = frame.GetSubrecord(offset: rdatFrame.Header.TotalLength);
            int len = rdatFrame.Header.TotalLength;
            if (IsExpected(dataType, subMeta.RecordType))
            {
                len += subMeta.TotalLength;
            }
            switch (dataType)
            {
                case RegionData.RegionDataType.Object:
                    item.Objects = RegionObjects.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Map:
                    item.MapName = RegionMap.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Grass:
                    item.Grasses = RegionGrasses.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Sound:
                    if (frame.Reader.TryGetSubrecord(out var nextRec, offset: len)
                        && (nextRec.RecordType.Equals(RDSD) || nextRec.RecordType.Equals(RDMD)))
                    {
                        len += nextRec.TotalLength;
                    }
                    item.Sounds = RegionSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                    break;
                case RegionData.RegionDataType.Weather:
                    item.Weather = RegionWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
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
        public IRegionObjectsGetter? Objects => _ObjectsSpan.HasValue ? RegionObjectsBinaryOverlay.RegionObjectsFactory(new BinaryMemoryReadStream(_ObjectsSpan.Value), _package) : default;

        private ReadOnlyMemorySlice<byte>? _WeatherSpan;
        public IRegionWeatherGetter? Weather => _WeatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new BinaryMemoryReadStream(_WeatherSpan.Value), _package) : default;
        
        private ReadOnlyMemorySlice<byte>? _MapSpan;
        public IRegionMapGetter? MapName => _MapSpan.HasValue ? RegionMapBinaryOverlay.RegionMapFactory(new BinaryMemoryReadStream(_MapSpan.Value), _package) : default;
        
        private ReadOnlyMemorySlice<byte>? _GrassesSpan;
        public IRegionGrassesGetter? Grasses => _GrassesSpan.HasValue ? RegionGrassesBinaryOverlay.RegionGrassesFactory(new BinaryMemoryReadStream(_GrassesSpan.Value), _package) : default;

        private ReadOnlyMemorySlice<byte>? _SoundsSpan;
        public IRegionSoundsGetter? Sounds => _SoundsSpan.HasValue ? RegionSoundsBinaryOverlay.RegionSoundsFactory(new BinaryMemoryReadStream(_SoundsSpan.Value), _package) : default;

        partial void RegionAreaLogicCustomParse(
            BinaryMemoryReadStream stream,
            int offset)
        {
            var rdat = this._package.Meta.GetSubrecord(stream);
            while (rdat.RecordType.Equals(Region_Registration.RDAT_HEADER))
            {
                ParseRegionData(stream, offset);
                if (stream.Complete) break;
                rdat = this._package.Meta.GetSubrecord(stream);
            }
        }

        partial void IconCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
        {
            _IconLocation = (ushort)(stream.Position - offset);
        }

        private void ParseRegionData(BinaryMemoryReadStream stream, int offset)
        {
            int loc = stream.Position - offset;
            var rdatFrame = this._package.Meta.ReadSubrecordFrame(stream);
            RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
            var len = rdatFrame.Header.TotalLength;
            if (!stream.Complete)
            {
                var contentMeta = this._package.Meta.GetSubrecord(stream);
                if (RegionBinaryCreateTranslation.IsExpected(dataType, contentMeta.RecordType))
                {
                    len += contentMeta.TotalLength;
                    stream.Position += contentMeta.TotalLength;
                }
            }
            switch (dataType)
            {
                case RegionData.RegionDataType.Object:
                    _ObjectsSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Map:
                    _MapSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Grass:
                    _GrassesSpan = this._data.Slice(loc, len);
                    break;
                case RegionData.RegionDataType.Sound:
                    var nextRec = this._package.Meta.GetSubrecord(stream);
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
