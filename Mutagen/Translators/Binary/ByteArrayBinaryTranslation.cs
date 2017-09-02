using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class ByteArrayBinaryTranslation : TypicalBinaryTranslation<byte[]>
    {
        public readonly static ByteArrayBinaryTranslation Instance = new ByteArrayBinaryTranslation();

        protected override string GetItemStr(byte[] item)
        {
            return item.ToHexString();
        }

        protected override void WriteValue(BinaryWriter writer, byte[] item)
        {
            if (item == null) return;
            base.WriteValue(writer, item);
        }

        protected override Byte[] ParseValue(BinaryReader reader, long length)
        {
            return reader.ReadBytes((int)length);
        }

        protected override byte[] ParseBytes(byte[] bytes)
        {
            return bytes;
        }
    }
}
