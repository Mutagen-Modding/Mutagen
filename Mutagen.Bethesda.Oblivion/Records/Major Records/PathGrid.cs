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

        private static PathGridPoint ReadPathGridPoint(ReadOnlySpan<byte> reader, out byte numConn)
        {
            var pt = new PathGridPoint();
            pt.Point = new Noggog.P3Float(
                reader.GetFloat(),
                reader.Slice(4).GetFloat(),
                reader.Slice(8).GetFloat());
            numConn = reader[12];
            pt.NumConnectionsFluffBytes = reader.Slice(13, 3).ToArray();
            return pt;
        }
    }

    public partial class PathGridBinaryWriteTranslation
    {
        static partial void WriteBinaryPointToPointConnectionsCustom(MutagenWriter writer, IPathGridInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
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
                    writer.Write(pt.NumConnectionsFluffBytes);
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
}
