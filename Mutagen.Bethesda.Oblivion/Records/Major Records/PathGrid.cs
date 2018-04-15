using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

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
            using (var subFrame = frame.Spawn(len))
            {
                ptCount = frame.Reader.ReadUInt16();
            }

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
            if (!nextRec.Equals(PGRP))
            {
                frame.Reader.Position -= Constants.RECORD_LENGTH;
                return;
            }
            var pointBytes = frame.Reader.ReadBytes(len);
            var bytePointsNum = pointBytes.Length / POINT_LEN;
            if (bytePointsNum != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pointBytes.Length} bytes: {bytePointsNum} != {ptCount} points.");
            }

            for (int recAttempt = 0; recAttempt < 2; recAttempt++)
            {
                nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
                switch (nextRec.Type)
                {
                    case "PGAG":
                        using (var subFrame = frame.Spawn(len))
                        {
                            var UnknowntryGet = Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                               subFrame,
                               fieldIndex: (int)PathGrid_FieldIndex.Unknown,
                               errorMask: errorMask);
                            item._Unknown.SetIfSucceeded(UnknowntryGet);
                        }
                        break;
                    case "PGRR":
                        var connectionBytes = frame.Reader.ReadBytes(len);
                        using (var ptByteReader = new MutagenReader(pointBytes))
                        {
                            using (var connectionReader = new MutagenReader(connectionBytes))
                            {
                                for (int i = 0; i < pointBytes.Length; i = i + POINT_LEN)
                                {
                                    var pt = new PathGridPoint();
                                    pt.Point = new Noggog.P3Float(
                                        ptByteReader.ReadFloat(),
                                        ptByteReader.ReadFloat(),
                                        ptByteReader.ReadFloat());
                                    var numConn = ptByteReader.ReadByte();
                                    ptByteReader.Position += 3;
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
                        break;
                    default:
                        frame.Reader.Position -= Constants.SUBRECORD_LENGTH;
                        return;
                }
            }
        }

        static partial void WriteBinary_PointToPointConnections_Custom(MutagenWriter writer, PathGrid item, int fieldIndex, Func<PathGrid_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.DATA_HEADER))
            {
                writer.Write(item.PointToPointConnections.Count);
            }

            using (HeaderExport.ExportSubRecordHeader(writer, PGRP))
            {
                foreach (var pt in item.PointToPointConnections)
                {
                    writer.Write(pt.Point.X);
                    writer.Write(pt.Point.Y);
                    writer.Write(pt.Point.Z);
                    writer.Write((byte)(pt.Connections.Count));
                }
            }

            if (item.Unknown_Property.HasBeenSet)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.PGAG_HEADER))
                {
                    writer.Write(item.Unknown);
                }
            }

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
