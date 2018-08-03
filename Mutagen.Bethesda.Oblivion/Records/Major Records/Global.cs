using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Global
    {
        protected static readonly RecordType FNAM = new RecordType("FNAM");
        
        public static Global Create_Binary(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            // Skip to FNAM
            var initialPos = frame.Position;
            if (HeaderTranslation.ReadNextRecordType(frame.Reader, out var recLen) != Global_Registration.GLOB_HEADER)
            {
                throw new ArgumentException();
            }
            using (var subFrame = frame.SpawnWithLength(recLen + Constants.RECORD_META_SKIP))
            {
                subFrame.CheckUpcomingRead(18);
                subFrame.Reader.Position += 16;
                var edidLength = subFrame.Reader.ReadInt16();
                subFrame.Reader.Position += edidLength;

                // Confirm FNAM
                var type = HeaderTranslation.ReadNextSubRecordType(subFrame.Reader, out var len);
                if (!type.Equals(FNAM))
                {
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException($"Could not find FNAM in its expected location: {subFrame.Position}"));
                    return null;
                }
                if (len != 1)
                {
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException($"FNAM had non 1 length: {len}"));
                }

                // Create proper Global subclass
                var triggerChar = (char)subFrame.Reader.ReadUInt8();
                Global g;
                switch (triggerChar)
                {
                    case GlobalInt.TRIGGER_CHAR:
                        g = new GlobalInt();
                        break;
                    case GlobalShort.TRIGGER_CHAR:
                        g = new GlobalShort();
                        break;
                    case GlobalFloat.TRIGGER_CHAR:
                        g = new GlobalFloat();
                        break;
                    default:
                        errorMask.ReportExceptionOrThrow(
                            new ArgumentException($"Unknown trigger char: {triggerChar}"));
                        return null;
                }

                // Fill with major record fields
                subFrame.Reader.Position = initialPos + 8;
                MajorRecord.Fill_Binary(
                    subFrame,
                    g,
                    errorMask);

                // Skip to and read data
                subFrame.Reader.Position += 13;
                if (Mutagen.Bethesda.Binary.FloatBinaryTranslation.Instance.Parse(
                    subFrame,
                    out var rawFloat,
                    errorMask))
                {
                    g.RawFloat = rawFloat;
                }
                return g;
            }
        }

        static partial void WriteBinary_TypeChar_Custom(
            MutagenWriter writer, 
            Global item, 
            ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.CharBinaryTranslation.Instance.Write(
                writer,
                item.TypeChar,
                header: Global_Registration.FNAM_HEADER,
                nullable: false,
                errorMask: errorMask);
        }
    }
}
