using Noggog;
using System;

namespace Mutagen.Binary
{
    public class EnumXmlTranslation<E> : PrimitiveBinaryTranslation<E>
        where E : struct, IComparable, IConvertible
    {
        public readonly static EnumXmlTranslation<E> Instance = new EnumXmlTranslation<E>();
        public override byte ExpectedLength => throw new NotImplementedException();

        protected override E ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
