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

        static partial void FillBinary_Points_Custom(MutagenFrame frame, PathGrid item, int fieldIndex, Func<PathGrid_ErrorMask> errorMask)
        {
            frame.Position = frame.FinalLocation;
            var nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!nextRec.Equals(PathGrid_Registration.DATA_HEADER))
            {
                throw new ArgumentException();
            }
            uint ptCount;
            using (var subFrame = frame.Spawn(len))
            {
                ptCount = frame.Reader.ReadUInt32();
            }

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
            if (!nextRec.Equals(PGRP))
            {
                throw new ArgumentException();
            }
            var pointBytes = frame.Reader.ReadBytes(len);
            if (pointBytes.Length / 13 != ptCount)
            {
                throw new ArgumentException($"Unexpected point byte length, when compared to expected point count. {pointBytes.Length} bytes.  {ptCount} points.");
            }

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
            if (!nextRec.Equals(PathGrid_Registration.PGAG_HEADER))
            {
                throw new ArgumentException();
            }
            using (var subFrame = frame.Spawn(len))
            {
                var UnknowntryGet = Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                   subFrame,
                   fieldIndex: (int)PathGrid_FieldIndex.Unknown,
                   errorMask: errorMask);
                item._Unknown.SetIfSucceeded(UnknowntryGet);
            }

            nextRec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out len);
            if (!nextRec.Equals(PGRR))
            {
                throw new ArgumentException();
            }
            var connectionBytes = frame.Reader.ReadBytes(len);
            if (len != ptCount * 2)
            {
                throw new ArgumentException($"Defined connection data did not match the expected length: {len} != {ptCount * 2}");
            }
            using (var ptByteReader = new MutagenReader(pointBytes))
            {
                using (var connectionReader = new MutagenReader(connectionBytes))
                {
                    for (int i = 0; i < pointBytes.Length; i = i + 13)
                    {
                        var pt = new PathGridPoint();
                        pt.Point = new Noggog.P3Float(
                            ptByteReader.ReadFloat(),
                            ptByteReader.ReadFloat(),
                            ptByteReader.ReadFloat());
                        var numConn = ptByteReader.ReadByte();
                        for (int j = 0; j < numConn; j++)
                        {
                            pt.Connections.Add(
                                connectionReader.ReadInt16(),
                                cmds: null);
                        }
                    }
                }
            }
        }

        static partial void WriteBinary_Points_Custom(MutagenWriter writer, PathGrid item, int fieldIndex, Func<PathGrid_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.DATA_HEADER))
            {
                writer.Write(item.Points.Count);
            }

            using (HeaderExport.ExportSubRecordHeader(writer, PGRP))
            {
                foreach (var pt in item.Points)
                {
                    writer.Write(pt.Point.X);
                    writer.Write(pt.Point.Y);
                    writer.Write(pt.Point.Z);
                    writer.Write((byte)(pt.Connections.Count));
                }
            }

            using (HeaderExport.ExportSubRecordHeader(writer, PathGrid_Registration.PGAG_HEADER))
            {
                writer.Write(item.Unknown);
            }

            using (HeaderExport.ExportSubRecordHeader(writer, PGRR))
            {
                foreach (var pt in item.Points)
                {
                    foreach(var conn in pt.Connections)
                    {
                        writer.Write(conn);
                    }
                }
            }
        }
    }
}
