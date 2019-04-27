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

        public abstract float RawFloat { get; set; }
        
        public static Global Create_Binary(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            // Skip to FNAM
            var initialPos = frame.Position;
            if (HeaderTranslation.ReadNextRecordType(frame.Reader, out var recLen) != Global_Registration.GLOB_HEADER)
            {
                throw new ArgumentException();
            }
            frame.CheckUpcomingRead(18);
            frame.Reader.Position += 16;
            var edidLength = frame.Reader.ReadInt16();
            frame.Reader.Position += edidLength;

            // Confirm FNAM
            var type = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!type.Equals(FNAM))
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Could not find FNAM in its expected location: {frame.Position}"));
                return null;
            }
            if (len != 1)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"FNAM had non 1 length: {len}"));
            }

            // Create proper Global subclass
            var triggerChar = (char)frame.Reader.ReadUInt8();
            Global g;
            switch (triggerChar)
            {
                case GlobalInt.TRIGGER_CHAR:
                    g = GlobalInt.Factory();
                    break;
                case GlobalShort.TRIGGER_CHAR:
                    g = GlobalShort.Factory();
                    break;
                case GlobalFloat.TRIGGER_CHAR:
                    g = GlobalFloat.Factory();
                    break;
                default:
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException($"Unknown trigger char: {triggerChar}"));
                    return null;
            }

            // Fill with major record fields
            frame.Reader.Position = initialPos + 8;
            OblivionMajorRecord.Fill_Binary(
                frame,
                g,
                masterReferences,
                errorMask);

            // Skip to and read data
            frame.Reader.Position += 13;
            if (Mutagen.Bethesda.Binary.FloatBinaryTranslation.Instance.Parse(
                frame,
                out var rawFloat,
                errorMask))
            {
                g.RawFloat = rawFloat;
            }
            return g;
        }

        static partial void WriteBinary_TypeChar_Custom(
            MutagenWriter writer, 
            Global item,
            MasterReferences masterReferences,
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
