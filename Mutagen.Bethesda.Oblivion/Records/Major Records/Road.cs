using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class RoadBinaryCreateTranslation
    {
        public static readonly RecordType PGRP = new RecordType("PGRP");
        public static readonly RecordType PGRR = new RecordType("PGRR");
        public const int POINT_LEN = 16;

        //ToDo
        //Upgrade to spans
        static partial void FillBinary_Points_Custom(MutagenFrame frame, Road item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!nextRec.Equals(PGRP))
            {
                frame.Reader.Position -= Mutagen.Bethesda.Constants.RECORD_LENGTH;
                return;
            }
            var pointBytes = frame.Reader.ReadSpan(len);

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
            switch (nextRec.TypeInt)
            {
                case 0x52524750: // "PGRR":
                    var connBytes = frame.Reader.ReadSpan(len);
                    var connFloats = connBytes.AsFloatSpan();
                    int numPts = pointBytes.Length / POINT_LEN;
                    RoadPoint[] points = new RoadPoint[numPts];
                    for (int i = 0; i < numPts; i++)
                    {
                        var pt = ReadPathGridPoint(pointBytes, out var numConn);
                        pointBytes = pointBytes.Slice(16);
                        P3Float[] conns = new P3Float[numConn];
                        for (int j = 0; j < numConn; j++)
                        {
                            conns[j] = new P3Float(
                                    x: connFloats[0],
                                    y: connFloats[1],
                                    z: connFloats[2]);
                            connFloats = connFloats.Slice(3);
                        }
                        pt.Connections.AddRange(conns);
                        points[i] = pt;
                    }
                    item.Points.AddRange(points);
                    if (connFloats.Length > 0)
                    {
                        throw new ArgumentException("Connection reader did not complete as expected.");
                    }
                    break;
                default:
                    frame.Reader.Position -= Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;
                    while (pointBytes.Length > 0)
                    {
                        item.Points.Add(
                            ReadPathGridPoint(pointBytes, out var numConn));
                        pointBytes = pointBytes.Slice(16);
                    }
                    break;
            }
        }

        private static RoadPoint ReadPathGridPoint(ReadOnlySpan<byte> reader, out byte numConn)
        {
            var pt = new RoadPoint();
            pt.Point = new Noggog.P3Float(
                reader.GetFloat(),
                reader.Slice(4).GetFloat(),
                reader.Slice(8).GetFloat());
            numConn = reader[12];
            pt.NumConnectionsFluffBytes = reader.Slice(13, 3).ToArray();
            return pt;
        }
    }

    public partial class RoadBinaryWriteTranslation
    {
        static partial void WriteBinary_Points_Custom(MutagenWriter writer, IRoadInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            bool anyConnections = false;
            using (HeaderExport.ExportSubRecordHeader(writer, RoadBinaryCreateTranslation.PGRP))
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
            using (HeaderExport.ExportSubRecordHeader(writer, RoadBinaryCreateTranslation.PGRR))
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
