using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Road
    {
        public static readonly RecordType PGRP = new RecordType("PGRP");
        public static readonly RecordType PGRR = new RecordType("PGRR");
        public const int POINT_LEN = 16;

        static partial void FillBinary_Points_Custom(MutagenFrame frame, Road item, ErrorMaskBuilder errorMask)
        {
            var nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!nextRec.Equals(PGRP))
            {
                frame.Reader.Position -= Constants.RECORD_LENGTH;
                return;
            }
            var pointBytes = frame.Reader.ReadBytes(len);

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
            switch (nextRec.Type)
            {
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
                                        new P3Float(
                                            x: connectionReader.ReadFloat(),
                                            y: connectionReader.ReadFloat(),
                                            z: connectionReader.ReadFloat()),
                                        cmds: null);
                                }
                                item.Points.Add(pt);
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
                    using (var ptByteReader = new BinaryMemoryReadStream(pointBytes))
                    {
                        while (!ptByteReader.Complete)
                        {
                            item.Points.Add(
                                ReadPathGridPoint(ptByteReader, out var numConn));
                        }
                    }
                    break;
            }
        }

        private static RoadPoint ReadPathGridPoint(IBinaryReadStream reader, out byte numConn)
        {
            var pt = new RoadPoint();
            pt.Point = new Noggog.P3Float(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
            numConn = reader.ReadUInt8();
            pt.NumConnectionsFluffBytes = reader.ReadBytes(3);
            return pt;
        }

        static partial void WriteBinary_Points_Custom(MutagenWriter writer, Road item, ErrorMaskBuilder errorMask)
        {
            bool anyConnections = false;
            using (HeaderExport.ExportSubRecordHeader(writer, PGRP))
            {
                foreach (var pt in item.Points)
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

            if (!anyConnections) return;
            using (HeaderExport.ExportSubRecordHeader(writer, PGRR))
            {
                foreach (var pt in item.Points)
                {
                    foreach (var conn in pt.Connections)
                    {
                        writer.Write(conn.X);
                        writer.Write(conn.Y);
                        writer.Write(conn.Z);
                    }
                }
            }
        }
    }
}
