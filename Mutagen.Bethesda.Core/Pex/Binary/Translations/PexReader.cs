using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Pex.Binary.Translations
{
    public class PexReader : BinaryReadStream
    {
        public long ExpectedPos;
        public Dictionary<ushort, string> Strings { get; } = new();

        public PexReader(Stream stream, bool isLittleEndian)
            : base(stream, isLittleEndian: isLittleEndian)
        {
        }

        public string ReadString()
        {
            var index = ReadUInt16();
            return Strings[index];
        }

        public DateTime ReadDateTime()
        {
            return ReadUInt64().ToDateTime();
        }

        public void EnsureVariableType(VariableType type)
        {
            var actual = (VariableType)this.ReadUInt8();
            if (type != actual)
            {
                throw new ArgumentException($"Did not encounter expected variable type {type}.  Instead found: {actual}");
            }
        }

        public void EnsureVariableType(VariableType type, VariableType type2)
        {
            var actual = (VariableType)this.ReadUInt8();
            if (type != actual && type2 != actual)
            {
                throw new ArgumentException($"Did not encounter expected variable type {type} or {type2}.  Instead found: {actual}");
            }
        }
    }
}
