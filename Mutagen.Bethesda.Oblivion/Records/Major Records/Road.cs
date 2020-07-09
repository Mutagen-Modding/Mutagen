using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class RoadBinaryCreateTranslation
    {
        public static readonly RecordType PGRP = new RecordType("PGRP");
        public static readonly RecordType PGRR = new RecordType("PGRR");
        public const int POINT_LEN = 16;
        
        static partial void FillBinaryPointsCustom(MutagenFrame frame, IRoadInternal item)
        {
            if (!frame.Reader.TryReadSubrecord(PGRP, out var subMeta)) return;
            var pointBytes = frame.Reader.ReadSpan(subMeta.ContentLength);

            subMeta = frame.ReadSubrecord();
            switch (subMeta.RecordType.TypeInt)
            {
                case 0x52524750: // "PGRR":
                    var connBytes = frame.Reader.ReadSpan(subMeta.ContentLength);
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
                    item.Points = points.ToExtendedList();
                    if (connFloats.Length > 0)
                    {
                        throw new ArgumentException("Connection reader did not complete as expected.");
                    }
                    break;
                default:
                    frame.Reader.Position -= subMeta.HeaderLength;
                    item.Points = new ExtendedList<RoadPoint>();
                    while (pointBytes.Length > 0)
                    {
                        item.Points.Add(
                            ReadPathGridPoint(pointBytes, out var numConn));
                        pointBytes = pointBytes.Slice(16);
                    }
                    break;
            }
        }

        public static RoadPoint ReadPathGridPoint(ReadOnlySpan<byte> reader, out byte numConn)
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
        static partial void WriteBinaryPointsCustom(MutagenWriter writer, IRoadGetter item)
        {
            bool anyConnections = false;
            using (HeaderExport.Subrecord(writer, RoadBinaryCreateTranslation.PGRP))
            {
                foreach (var pt in item.Points.TryIterate())
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
            using (HeaderExport.Subrecord(writer, RoadBinaryCreateTranslation.PGRR))
            {
                foreach (var pt in item.Points.TryIterate())
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

    public partial class RoadBinaryOverlay
    {
        public IReadOnlyList<IRoadPointGetter>? Points { get; private set; }

        partial void PointsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            if (stream.Complete) return;
            var subMeta = stream.GetSubrecord();
            if (subMeta.RecordType != RecordTypes.PGRP) return;
            stream.Position += subMeta.HeaderLength;
            var pointBytes = stream.ReadMemory(subMeta.ContentLength);
            subMeta = stream.GetSubrecord();
            switch (subMeta.RecordTypeInt)
            {
                case 0x52524750: // "PGRR":
                    stream.Position += subMeta.HeaderLength;
                    var connBytes = stream.ReadMemory(subMeta.ContentLength);
                    this.Points = BinaryOverlayList.FactoryByLazyParse<IRoadPointGetter>(
                        pointBytes,
                        _package,
                        getter: (s, p) =>
                        {
                            int numPts = pointBytes.Length / RoadBinaryCreateTranslation.POINT_LEN;
                            RoadPoint[] points = new RoadPoint[numPts];
                            var connFloats = connBytes.Span.AsFloatSpan();
                            for (int i = 0; i < numPts; i++)
                            {
                                var pt = RoadBinaryCreateTranslation.ReadPathGridPoint(s, out var numConn);
                                s = s.Slice(RoadBinaryCreateTranslation.POINT_LEN);
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
                            return points;
                        });
                    break;
                default:
                    this.Points = BinaryOverlayList.FactoryByStartIndex<IRoadPointGetter>(
                        pointBytes,
                        _package,
                        itemLength: RoadBinaryCreateTranslation.POINT_LEN,
                        getter: (s, p) => RoadBinaryCreateTranslation.ReadPathGridPoint(s, out var numConn));
                    break;
            }
        }
    }
}
