using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class PathGrid
    {
        public static readonly RecordType PGRP = new RecordType("PGRP");
        public static readonly RecordType PGRR = new RecordType("PGRR");
        public const int POINT_LEN = 16;

        static partial void FillBinary_PointToPointConnections_Custom(MutagenFrame frame, PathGrid item, int fieldIndex, Func<PathGrid_ErrorMask> errorMask)
        {
            var nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!nextRec.Equals(PathGrid_Registration.DATA_HEADER))
            {
                frame.Reader.Position -= Constants.RECORD_LENGTH;
                return;
            }
            uint ptCount;
            using (var subFrame = frame.SpawnWithLength(len))
            {
                ptCount = frame.Reader.ReadUInt16();
            }

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var pointsLen);
            if (!nextRec.Equals(PGRP))
            {
                frame.Reader.Position -= Constants.RECORD_LENGTH;
                return;
            }
            var pointBytes = frame.Reader.ReadBytes(pointsLen);
            var bytePointsNum = pointBytes.Length / POINT_LEN;
            if (bytePointsNum != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pointBytes.Length} bytes: {bytePointsNum} != {ptCount} points.");
            }

            bool readPGRR = false;
            for (int recAttempt = 0; recAttempt < 2; recAttempt++)
            {
                if (frame.Reader.Complete) break;
                nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
                switch (nextRec.Type)
                {
                    case "PGAG":
                        using (var subFrame = frame.SpawnWithLength(len))
                        {
                            var UnknowntryGet = Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                               subFrame,
                               fieldIndex: (int)PathGrid_FieldIndex.Unknown,
                               errorMask: errorMask);
                            if (UnknowntryGet.Succeeded)
                            {
                                item.SetUnknown(UnknowntryGet.Value);
                            }
                            else
                            {
                                item.UnsetUnknown();
                            }
                        }
                        break;
                    case "PGRR":
                        var connectionBytes = frame.Reader.ReadBytes(len);
                        using (var ptByteReader = new BinaryMemoryReadStream(pointBytes))
                        {
                            using (var connectionReader = new BinaryMemoryReadStream(connectionBytes))
                            {
                                for (int i = 0; i < pointBytes.Length; i = i + POINT_LEN)
                                {
                                    var pt = ReadPathGridPoint(ptByteReader, out var numConn);
                                    for (int j = 0; j < numConn; j++)
                                    {
                                        pt.Connections.Add(
                                            connectionReader.ReadInt16(),
                                            cmds: null);
                                    }
                                    item.PointToPointConnections.Add(pt);
                                }
                                if (!connectionReader.Complete)
                                {
                                    throw new ArgumentException("Connection reader did not complete as expected.");
                                }
                            }
                        }
                        readPGRR = true;
                        break;
                    default:
                        frame.Reader.Position -= Constants.SUBRECORD_LENGTH;
                        break;
                }
            }

            if (!readPGRR)
            {
                using (var ptByteReader = new BinaryMemoryReadStream(pointBytes))
                {
                    while (!ptByteReader.Complete)
                    {
                        item.PointToPointConnections.Add(
                            ReadPathGridPoint(ptByteReader, out var numConn));
                    }
                }
            }
        }

        private static PathGridPoint ReadPathGridPoint(IBinaryReadStream reader, out byte numConn)
        {
            var pt = new PathGridPoint();
            pt.Point = new Noggog.P3Float(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
            numConn = reader.ReadUInt8();
            pt.NumConnectionsFluffBytes = reader.ReadBytes(3);
            return pt;
        }

        static partial void WriteBinary_PointToPointConnections_Custom(MutagenWriter writer, PathGrid item, int fieldIndex, Func<PathGrid_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.DATA_HEADER))
            {
                writer.Write((ushort)item.PointToPointConnections.Count);
            }

            bool anyConnections = false;
            using (HeaderExport.ExportSubRecordHeader(writer, PGRP))
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

            if (item.Unknown_Property.HasBeenSet)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.PGAG_HEADER))
                {
                    writer.Write(item.Unknown);
                }
            }

            if (!anyConnections) return;
            using (HeaderExport.ExportSubRecordHeader(writer, PGRR))
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
