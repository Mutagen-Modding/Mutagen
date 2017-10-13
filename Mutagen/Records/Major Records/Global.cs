using Mutagen.Binary;
using Mutagen.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public partial class Global
    {
        protected abstract char TriggerChar { get; }
        protected static readonly RecordType FNAM = new RecordType("FNAM");
        
        public static (Global Object, Global_ErrorMask ErrorMask) Create_Binary(
            BinaryReader reader,
            bool doMasks)
        {
            // Skip to FNAM
            var initialPos = reader.BaseStream.Position;
            reader.BaseStream.Position += 24;
            var edidLength = reader.ReadInt16();
            reader.BaseStream.Position += edidLength;

            // Confirm FNAM
            var type = HeaderTranslation.ReadNextSubRecordType(reader, out var len);
            if (!type.Equals(FNAM))
            {
                var ex = new ArgumentException($"Could not find FNAM in its expected location: {reader.BaseStream.Position}");
                if (!doMasks) throw ex;
                return (null, new Global_ErrorMask()
                {
                    Overall = ex
                });
            }
            if (len != 1)
            {
                var ex = new ArgumentException($"FNAM had non 1 length: {len}");
                if (!doMasks) throw ex;
                return (null, new Global_ErrorMask()
                {
                    Overall = ex
                });
            }

            // Create proper Global subclass
            var triggerChar = (char)reader.ReadByte();
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
            reader.BaseStream.Position = initialPos + 8;
            MajorRecord.Fill_Binary(
                reader,
                g,
                doMasks,
                out var majorErrMask);

            // Skip to and read data
            reader.BaseStream.Position += 13;
            var floatParse = Mutagen.Binary.FloatBinaryTranslation.Instance.Parse(
                reader,
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
    }
}
