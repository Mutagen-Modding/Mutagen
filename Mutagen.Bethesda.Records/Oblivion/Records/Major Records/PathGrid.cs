using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class PathGridBinaryCreateTranslation
    {
        public static readonly RecordType PGRP = new RecordType("PGRP");
        public static readonly RecordType PGRR = new RecordType("PGRR");
        public const int POINT_LEN = 16;

        static partial void FillBinaryPointToPointConnectionsCustom(MutagenFrame frame, PathGrid item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var subMeta = frame.MetaData.ReadSubRecord(frame);
            if (subMeta.RecordType != PathGrid_Registration.DATA_HEADER)
            {
                frame.Position -= subMeta.HeaderLength;
                return;
            }

            uint ptCount = frame.Reader.ReadUInt16();

            subMeta = frame.MetaData.ReadSubRecord(frame);
            if (subMeta.RecordType != PGRP)
            {
                frame.Position -= subMeta.HeaderLength;
                return;
            }
            var pointDataSpan = frame.Reader.ReadSpan(subMeta.RecordLength);
            var bytePointsNum = pointDataSpan.Length / POINT_LEN;
            if (bytePointsNum != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pointDataSpan.Length} bytes: {bytePointsNum} != {ptCount} points.");
            }

            bool readPGRR = false;
            for (int recAttempt = 0; recAttempt < 2; recAttempt++)
            {
                if (frame.Reader.Complete) break;
                subMeta = frame.MetaData.GetSubRecord(frame);
                switch (subMeta.RecordType.TypeInt)
                {
                    case 0x47414750: //"PGAG":
                        frame.Reader.Position += subMeta.HeaderLength;
                        if (Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                           frame.SpawnWithLength(subMeta.RecordLength, checkFraming: false),
                           item: out var unknownBytes))
                        {
                            item.Unknown = unknownBytes;
                        }
                        else
                        {
                            item.Unknown_Unset();
                        }
                        break;
                    case 0x52524750: // "PGRR":
                        frame.Reader.Position += subMeta.HeaderLength;
                        var connectionInts = frame.Reader.ReadSpan(subMeta.RecordLength).AsInt16Span();
                        int numPts = pointDataSpan.Length / POINT_LEN;
                        PathGridPoint[] pathGridPoints = new PathGridPoint[numPts];
                        for (int i = 0; i < numPts; i++)
                        {
                            var pt = ReadPathGridPoint(pointDataSpan, out var numConn);
                            pt.Connections.AddRange(connectionInts.Slice(0, numConn).ToArray());
                            pathGridPoints[i] = pt;
                            pointDataSpan = pointDataSpan.Slice(16);
                            connectionInts = connectionInts.Slice(numConn);
                        }
                        item.PointToPointConnections.AddRange(pathGridPoints);
                        readPGRR = true;
                        break;
                    default:
                        break;
                }
            }

            if (!readPGRR)
            {
                List<PathGridPoint> list = new List<PathGridPoint>();
                while (pointDataSpan.Length > 0)
                {
                    list.Add(
                        ReadPathGridPoint(pointDataSpan, out var numConn));
                    pointDataSpan = pointDataSpan.Slice(16);
                }
                item.PointToPointConnections.AddRange(list);
            }
        }

        public static PathGridPoint ReadPathGridPoint(ReadOnlySpan<byte> reader, out byte numConn)
        {
            var pt = new PathGridPoint();
            pt.Point = new Noggog.P3Float(
                reader.GetFloat(),
                reader.Slice(4).GetFloat(),
                reader.Slice(8).GetFloat());
            numConn = reader[12];
            pt.FluffBytes = reader.Slice(13, 3).ToArray();
            return pt;
        }
    }

    public partial class PathGridBinaryWriteTranslation
    {
        static partial void WriteBinaryPointToPointConnectionsCustom(MutagenWriter writer, IPathGridGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.DATA_HEADER))
            {
                writer.Write((ushort)item.PointToPointConnections.Count);
            }

            bool anyConnections = false;
            using (HeaderExport.ExportSubRecordHeader(writer, PathGridBinaryCreateTranslation.PGRP))
            {
                foreach (var pt in item.PointToPointConnections)
                {
                    writer.Write(pt.Point.X);
                    writer.Write(pt.Point.Y);
                    writer.Write(pt.Point.Z);
                    writer.Write((byte)(pt.Connections.Count));
                    writer.Write(pt.FluffBytes);
                    if (pt.Connections.Count > 0)
                    {
                        anyConnections = true;
                    }
                }
            }

            if (item.Unknown_IsSet)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.PGAG_HEADER))
                {
                    writer.Write(item.Unknown);
                }
            }

            if (!anyConnections) return;
            using (HeaderExport.ExportSubRecordHeader(writer, PathGridBinaryCreateTranslation.PGRR))
            {
                foreach (var pt in item.PointToPointConnections)
                {
                    foreach (var conn in pt.Connections)
                    {
                        writer.Write(conn);
                    }
                }
            }
        }
    }

    public partial class PathGridBinaryWrapper
    {
        public IReadOnlySetList<IPathGridPointGetter> PointToPointConnections { get; private set; } = EmptySetList<IPathGridPointGetter>.Instance;

        private int? _UnknownLocation;
        public bool Unknown_IsSet => this._UnknownLocation.HasValue;
        public ReadOnlySpan<byte> Unknown => HeaderTranslation.ExtractSubrecordSpan(_data, _UnknownLocation.Value, _package.Meta);

        partial void PointToPointConnectionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            var dataFrame = _package.Meta.ReadSubRecordFrame(stream);
            uint ptCount = BinaryPrimitives.ReadUInt16LittleEndian(dataFrame.ContentSpan);

            var pgrpMeta = _package.Meta.GetSubRecord(stream);
            if (pgrpMeta.RecordType != PathGridBinaryCreateTranslation.PGRP) return;
            stream.Position += pgrpMeta.HeaderLength;
            var pointData = stream.ReadMemory(pgrpMeta.RecordLength);
            var bytePointsNum = pgrpMeta.RecordLength / PathGridBinaryCreateTranslation.POINT_LEN;
            if (bytePointsNum != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pgrpMeta.RecordLength} bytes: {bytePointsNum} != {ptCount} points.");
            }

            bool readPGRR = false;
            for (int recAttempt = 0; recAttempt < 2; recAttempt++)
            {
                if (stream.Complete) break;
                var subMeta = _package.Meta.GetSubRecord(stream);
                switch (subMeta.RecordType.TypeInt)
                {
                    case 0x47414750: //"PGAG":
                        this._UnknownLocation = stream.Position - offset;
                        stream.Position += subMeta.TotalLength;
                        break;
                    case 0x52524750: // "PGRR":
                        stream.Position += subMeta.HeaderLength;
                        var connectionPtData = stream.ReadMemory(subMeta.RecordLength);
                        this.PointToPointConnections = BinaryWrapperSetList<IPathGridPointGetter>.FactoryByLazyParse(
                            pointData,
                            _package,
                            getter: (s, p) =>
                            {
                                var connectionInts = connectionPtData.Span.AsInt16Span();
                                IPathGridPointGetter[] pathGridPoints = new IPathGridPointGetter[bytePointsNum];
                                for (int i = 0; i < bytePointsNum; i++)
                                {
                                    var pt = PathGridPointBinaryWrapper.Factory(s, p);
                                    pt.Connections = connectionInts.Slice(0, pt.NumConnections).ToArray().ToList();
                                    pathGridPoints[i] = pt;
                                    s = s.Slice(16);
                                    connectionInts = connectionInts.Slice(pt.NumConnections);
                                }
                                return pathGridPoints;
                            });
                        readPGRR = true;
                        break;
                    default:
                        break;
                }
            }

            if (!readPGRR)
            {
                this.PointToPointConnections = BinaryWrapperSetList<IPathGridPointGetter>.FactoryByStartIndex(
                    pointData,
                    this._package,
                    itemLength: 16,
                    getter: (s, p) =>
                    {
                        return PathGridBinaryCreateTranslation.ReadPathGridPoint(s, out var numConn);
                    });
            }
        }
    }
}
