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
        protected abstract byte TriggerByte { get; }
        
        public static (Global Object, Global_ErrorMask ErrorMask) Create_Binary(
            BinaryReader reader,
            bool doMasks)
        {
            var triggerByte = reader.ReadByte();
            Global g;
            switch (triggerByte)
            {
                case GlobalInt.TRIGGER_BYTE:
                    g = new GlobalInt();
                    break;
                case GlobalShort.TRIGGER_BYTE:
                    g = new GlobalShort();
                    break;
                case GlobalFloat.TRIGGER_BYTE:
                    g = new GlobalFloat();
                    break;
                default:
                    var ex = new ArgumentException($"Unknown trigger byte: {triggerByte}");
                    if (!doMasks) throw ex;
                    return (null, new Global_ErrorMask()
                    {
                        Overall = ex
                    });
            }

            MajorRecord.Fill_Binary(
                reader,
                g,
                doMasks,
                out var majorErrMask);


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

                };
            }
            else
            {

            }
            return (g, null);
        }
    }
}
