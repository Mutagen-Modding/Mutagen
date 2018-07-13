using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Condition
    {
        [Flags]
        public enum Flag
        {
            OR = 0x01,
            RunOnTarget = 0x02,
            UseGlobal = 0x04
        }
        
        static Condition CustomRecordTypeTrigger(
            MutagenFrame frame,
            RecordType recordType, 
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            byte[] bytes = frame.ReadBytes(0x1A);
            bytes[4] = 0x18;
            byte[] newBytes = new byte[bytes.Length + 4];
            bytes[3] = (byte)'A';
            Array.Copy(bytes, newBytes, bytes.Length);
            LoquiBinaryTranslation<Condition>.Instance.Parse(
                frame: new MutagenFrame(new BinaryMemoryReadStream(newBytes)),
                item: out var item,
                errorMask: errorMask,
                recordTypeConverter: recordTypeConverter);
            return item;
        }
    }
}
