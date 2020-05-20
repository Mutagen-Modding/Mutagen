using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Region
    {
        [Flags]
        public enum MajorFlag
        {
            BorderRegion = 0x40
        }
    }

    namespace Internals
    {
        public partial class RegionBinaryCreateTranslation
        {
            public static readonly RecordType RDOT = new RecordType("RDOT");
            public static readonly RecordType RDWT = new RecordType("RDWT");
            public static readonly RecordType RDMP = new RecordType("RDMP");
            public static readonly RecordType RDGS = new RecordType("RDGS");
            public static readonly RecordType RDSA = new RecordType("RDSA");
            public static readonly RecordType RDMO = new RecordType("RDMO");

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
                    case RegionData.RegionDataType.Grass:
                        if (!recordType.Equals(RDGS)) return false;
                        break;
                    case RegionData.RegionDataType.Sound:
                        if (!recordType.Equals(RDSA) && !recordType.Equals(RDMO)) return false;
                        break;
                    case RegionData.RegionDataType.Land:
                    default:
                        return false;
                }
                return true;
            }

            static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
            {
                var rdatFrame = frame.Reader.GetSubrecordFrame();
                int len = rdatFrame.Header.TotalLength;
                var subMeta = frame.Reader.GetSubrecord(offset: len);
                var recType = subMeta.RecordType;
                if (recType == RegionData_Registration.ICON_HEADER)
                {
                    len += subMeta.TotalLength;
                    // Skip icon subrecord for now
                    subMeta = frame.Reader.GetSubrecord(offset: rdatFrame.Header.TotalLength + subMeta.TotalLength);
                }
                RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
                if (IsExpected(dataType, recType))
                {
                    len += subMeta.TotalLength;
                }
                switch (dataType)
                {
                    case RegionData.RegionDataType.Object:
                        item.Objects = RegionObjects.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                        break;
                    case RegionData.RegionDataType.Map:
                        item.Map = RegionMap.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                        break;
                    case RegionData.RegionDataType.Grass:
                        item.Grasses = RegionGrasses.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                        break;
                    case RegionData.RegionDataType.Sound:
                        if (frame.Reader.TryGetSubrecord(out var nextRec, offset: len)
                            && (nextRec.RecordType.Equals(RDSA) || nextRec.RecordType.Equals(RDMO)))
                        {
                            len += nextRec.TotalLength;
                        }
                        item.Sounds = RegionSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                        break;
                    case RegionData.RegionDataType.Weather:
                        item.Weather = RegionWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                        break;
                    case RegionData.RegionDataType.Land:
                        item.Land = RegionLand.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
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
                item.Map?.WriteToBinary(writer);
                item.Land?.WriteToBinary(writer);
                item.Grasses?.WriteToBinary(writer);
                item.Sounds?.WriteToBinary(writer);
            }
        }

        public partial class RegionBinaryOverlay : IRegionGetter
        {
            private ReadOnlyMemorySlice<byte>? _ObjectsSpan;
            public IRegionObjectsGetter? Objects => _ObjectsSpan.HasValue ? RegionObjectsBinaryOverlay.RegionObjectsFactory(new BinaryMemoryReadStream(_ObjectsSpan.Value), _package) : default;

            private ReadOnlyMemorySlice<byte>? _WeatherSpan;
            public IRegionWeatherGetter? Weather => _WeatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new BinaryMemoryReadStream(_WeatherSpan.Value), _package) : default;

            private ReadOnlyMemorySlice<byte>? _MapSpan;
            public IRegionMapGetter? Map => _MapSpan.HasValue ? RegionMapBinaryOverlay.RegionMapFactory(new BinaryMemoryReadStream(_MapSpan.Value), _package) : default;

            private ReadOnlyMemorySlice<byte>? _GrassesSpan;
            public IRegionGrassesGetter? Grasses => _GrassesSpan.HasValue ? RegionGrassesBinaryOverlay.RegionGrassesFactory(new BinaryMemoryReadStream(_GrassesSpan.Value), _package) : default;

            private ReadOnlyMemorySlice<byte>? _SoundsSpan;
            public IRegionSoundsGetter? Sounds => _SoundsSpan.HasValue ? RegionSoundsBinaryOverlay.RegionSoundsFactory(new BinaryMemoryReadStream(_SoundsSpan.Value), _package) : default;

            private ReadOnlyMemorySlice<byte>? _LandSpan;
            public IRegionLandGetter? Land => _LandSpan.HasValue ? RegionLandBinaryOverlay.RegionLandFactory(new BinaryMemoryReadStream(_LandSpan.Value), _package) : default;

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

            private void ParseRegionData(BinaryMemoryReadStream stream, int offset)
            {
                int loc = stream.Position - offset;
                var rdatFrame = this._package.Meta.ReadSubrecordFrame(stream);
                RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
                var len = rdatFrame.Header.TotalLength;
                if (!stream.Complete)
                {
                    var contentMeta = this._package.Meta.GetSubrecord(stream);
                    var recType = contentMeta.RecordType;
                    if (recType == Region_Registration.ICON_HEADER)
                    {
                        var totalLen = contentMeta.TotalLength;
                        len += totalLen;
                        // Skip icon subrecord for now
                        contentMeta = this._package.Meta.GetSubrecord(stream, offset: rdatFrame.Header.TotalLength + totalLen);
                    }
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
                    case RegionData.RegionDataType.Land:
                        _LandSpan = this._data.Slice(loc, len);
                        break;
                    case RegionData.RegionDataType.Sound:
                        if (this._package.Meta.TryGetSubrecord(stream, out var nextRec)
                            && (nextRec.RecordType.Equals(RegionBinaryCreateTranslation.RDSA)
                                || nextRec.RecordType.Equals(RegionBinaryCreateTranslation.RDMO)))
                        {
                            len += nextRec.TotalLength;
                        }
                        _SoundsSpan = this._data.Slice(loc, len);
                        break;
                    case RegionData.RegionDataType.Weather:
                        _WeatherSpan = this._data.Slice(loc, len);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
