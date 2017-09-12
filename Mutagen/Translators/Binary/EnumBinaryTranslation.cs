using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class EnumXmlTranslation<E> : PrimitiveBinaryTranslation<E>
        where E : struct, IComparable, IConvertible
    {
        public readonly static EnumXmlTranslation<E> Instance = new EnumXmlTranslation<E>();
        public override byte ExpectedLength => throw new NotImplementedException();

        protected override E ParseValue(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void WriteValue(BinaryWriter writer, E item)
        {
            throw new NotImplementedException();
        }
    }
}
