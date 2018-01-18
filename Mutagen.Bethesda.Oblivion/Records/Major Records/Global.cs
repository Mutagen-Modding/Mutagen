using Loqui;
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
        
        public static (Global Object, Global_ErrorMask ErrorMask) Create_Binary(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter,
            bool doMasks)
        {
            // Skip to FNAM
            var initialPos = frame.Position;
            frame.Reader.CheckUpcomingRead(new ContentLength(26));
            frame.Position += 24;
            var edidLength = frame.Reader.ReadInt16();
            frame.Position += edidLength;

            // Confirm FNAM
            var type = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!type.Equals(FNAM))
            {
                var ex = new ArgumentException($"Could not find FNAM in its expected location: {frame.Position}");
                if (!doMasks) throw ex;
                return (null, new Global_ErrorMask()
                {
                    Overall = ex
                });
            }
            if (len.Value != 1)
            {
                var ex = new ArgumentException($"FNAM had non 1 length: {len}");
                if (!doMasks) throw ex;
                return (null, new Global_ErrorMask()
                {
                    Overall = ex
                });
            }

            // Create proper Global subclass
            var triggerChar = (char)frame.Reader.ReadByte();
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
                    var ex = new ArgumentException($"Unknown trigger char: {triggerChar}");
                    if (!doMasks) throw ex;
                    return (null, new Global_ErrorMask()
                    {
                        Overall = ex
                    });
            }

            // Fill with major record fields
            frame.Position = initialPos + 8;
            MajorRecord.Fill_Binary(
                frame,
                g,
                doMasks,
                out var majorErrMask);

            // Skip to and read data
            frame.Reader.Position += 13;
            var floatParse = Mutagen.Bethesda.Binary.FloatBinaryTranslation.Instance.Parse(
                frame,
                doMasks,
                out var floatMask);
            if (floatParse.Succeeded)
            {
                g.RawFloat = floatParse.Value;
            }
            Global_ErrorMask errMask;
            if (floatMask != null)
            {
                errMask = new Global_ErrorMask()
                {
                    RawFloat = floatMask
                };
            }
            else
            {
                errMask = null;
            }
            return (g, errMask);
        }

        static partial void WriteBinary_TypeChar_Custom(
            MutagenWriter writer, 
            Global item, 
            int fieldIndex,
            Func<Global_ErrorMask> errorMask)
        {
            Mutagen.Bethesda.Binary.CharBinaryTranslation.Instance.Write(
                writer,
                item.TypeChar,
                header: Global_Registration.FNAM_HEADER,
                fieldIndex: fieldIndex,
                nullable: false,
                errorMask: errorMask);
        }
    }
}
