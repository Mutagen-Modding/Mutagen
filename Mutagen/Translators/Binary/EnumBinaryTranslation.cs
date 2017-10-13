using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class EnumBinaryTranslation<E> : PrimitiveBinaryTranslation<E>
        where E : struct, IComparable, IConvertible
    {
        public readonly static EnumBinaryTranslation<E> Instance = new EnumBinaryTranslation<E>();
        public override byte ExpectedLength => 4;

        protected override E ParseValue(BinaryReader reader)
        {
            var i = reader.ReadInt32();
            return (E)Enum.ToObject(typeof(E), i);
        }

        protected override void WriteValue(BinaryWriter writer, E item)
        {
            writer.Write(item.ToInt32(null));
        }
    }
}
