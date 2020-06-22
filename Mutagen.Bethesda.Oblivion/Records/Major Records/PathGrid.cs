using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class PathGridBinaryCreateTranslation
    {
        public static readonly RecordType PGRP = new RecordType("PGRP");
        public static readonly RecordType PGRR = new RecordType("PGRR");
        public const int POINT_LEN = 16;

        static partial void FillBinaryPointToPointConnectionsCustom(MutagenFrame frame, IPathGridInternal item)
        {
            FillBinaryPointToPointConnections(frame, item);
        }

        public static void FillBinaryPointToPointConnections(MutagenFrame frame, IPathGridInternal item)
        {
            if (!frame.TryReadSubrecord(RecordTypes.DATA, out var subMeta)) return;

            uint ptCount = frame.Reader.ReadUInt16();

            if (!frame.Reader.TryReadSubrecord(PGRP, out subMeta)) return;
            var pointDataSpan = frame.Reader.ReadSpan(subMeta.ContentLength);
            var bytePointsNum = pointDataSpan.Length / POINT_LEN;
            if (bytePointsNum != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pointDataSpan.Length} bytes: {bytePointsNum} != {ptCount} points.");
            }

            bool readPGRR = false;
            for (int recAttempt = 0; recAttempt < 2; recAttempt++)
            {
                if (frame.Reader.Complete) break;
                subMeta = frame.GetSubrecord();
                switch (subMeta.RecordType.TypeInt)
                {
                    case 0x47414750: //"PGAG":
                        frame.Reader.Position += subMeta.HeaderLength;
                        if (Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                           frame.SpawnWithLength(subMeta.ContentLength, checkFraming: false),
                           item: out var unknownBytes))
                        {
                            item.PGAG = unknownBytes;
                        }
                        else
                        {
                            item.PGAG = default;
                        }
                        break;
                    case 0x52524750: // "PGRR":
                        frame.Reader.Position += subMeta.HeaderLength;
                        var connectionInts = frame.Reader.ReadSpan(subMeta.ContentLength).AsInt16Span();
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
                        item.PointToPointConnections = pathGridPoints.ToExtendedList();
                        readPGRR = true;
                        break;
                    default:
                        break;
                }
            }

            if (!readPGRR)
            {
                ExtendedList<PathGridPoint> list = new ExtendedList<PathGridPoint>();
                while (pointDataSpan.Length > 0)
                {
                    list.Add(
                        ReadPathGridPoint(pointDataSpan, out var numConn));
                    pointDataSpan = pointDataSpan.Slice(16);
                }
                item.PointToPointConnections = list;
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
            pt.Unused = reader.Slice(13, 3).ToArray();
            return pt;
        }
    }

    public partial class PathGridBinaryWriteTranslation
    {
        static partial void WriteBinaryPointToPointConnectionsCustom(MutagenWriter writer, IPathGridGetter item)
        {
            if (!item.PointToPointConnections.TryGet(out var ptToPt)) return;

            using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
            {
                writer.Write((ushort)ptToPt.Count);
            }

            bool anyConnections = false;
            using (HeaderExport.Subrecord(writer, PathGridBinaryCreateTranslation.PGRP))
            {
                foreach (var pt in ptToPt)
                {
                    writer.Write(pt.Point.X);
                    writer.Write(pt.Point.Y);
                    writer.Write(pt.Point.Z);
                    writer.Write((byte)(pt.Connections.Count));
                    writer.Write(pt.Unused);
                    if (pt.Connections.Count > 0)
                    {
                        anyConnections = true;
                    }
                }
            }

            if (item.PGAG.TryGet(out var pgag))
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.PGAG))
                {
                    writer.Write(pgag);
                }
            }

            if (!anyConnections) return;
            using (HeaderExport.Subrecord(writer, PathGridBinaryCreateTranslation.PGRR))
            {
                foreach (var pt in ptToPt)
                {
                    foreach (var conn in pt.Connections)
                    {
                        writer.Write(conn);
                    }
                }
            }
        }
    }

    public partial class PathGridBinaryOverlay
    {
        public IReadOnlyList<IPathGridPointGetter>? PointToPointConnections { get; private set; }

        private int? _PGAGLocation;
        public bool PGAG_IsSet => _PGAGLocation.HasValue;
        public ReadOnlyMemorySlice<byte>? PGAG => _PGAGLocation.HasValue ? HeaderTranslation.ExtractSubrecordMemory(_data, _PGAGLocation.Value, _package.MetaData.Constants) : default(ReadOnlyMemorySlice<byte>?);

        partial void PointToPointConnectionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            var dataFrame = stream.ReadSubrecordFrame();
            uint ptCount = BinaryPrimitives.ReadUInt16LittleEndian(dataFrame.Content);

            var pgrpMeta = stream.GetSubrecord();
            if (pgrpMeta.RecordType != PathGridBinaryCreateTranslation.PGRP) return;
            stream.Position += pgrpMeta.HeaderLength;
            var pointData = stream.ReadMemory(pgrpMeta.ContentLength);
            var bytePointsNum = pgrpMeta.ContentLength / PathGridBinaryCreateTranslation.POINT_LEN;
            if (bytePointsNum != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pgrpMeta.ContentLength} bytes: {bytePointsNum} != {ptCount} points.");
            }

            bool readPGRR = false;
            for (int recAttempt = 0; recAttempt < 2; recAttempt++)
            {
                if (stream.Complete) break;
                var subMeta = stream.GetSubrecord();
                switch (subMeta.RecordType.TypeInt)
                {
                    case 0x47414750: //"PGAG":
                        this._PGAGLocation = stream.Position - offset;
                        stream.Position += subMeta.TotalLength;
                        break;
                    case 0x52524750: // "PGRR":
                        stream.Position += subMeta.HeaderLength;
                        var connectionPtData = stream.ReadMemory(subMeta.ContentLength);
                        this.PointToPointConnections = BinaryOverlayList<IPathGridPointGetter>.FactoryByLazyParse(
                            pointData,
                            _package,
                            getter: (s, p) =>
                            {
                                var connectionInts = connectionPtData.Span.AsInt16Span();
                                IPathGridPointGetter[] pathGridPoints = new IPathGridPointGetter[bytePointsNum];
                                for (int i = 0; i < bytePointsNum; i++)
                                {
                                    var pt = PathGridPointBinaryOverlay.Factory(s, p);
                                    pt.Connections.AddRange(connectionInts.Slice(0, pt.NumConnections).ToArray());
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
                this.PointToPointConnections = BinaryOverlayList<IPathGridPointGetter>.FactoryByStartIndex(
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
