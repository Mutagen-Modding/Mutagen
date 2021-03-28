using System.IO;
using System.Text;
using Mutagen.Bethesda.Core.Pex.Extensions;

namespace Mutagen.Bethesda.Core.Pex
{
    internal class PexReader : BinaryReader
    {
        private readonly bool _isBigEndian;

        public PexReader(Stream input, bool isBigEndian) : base(input)
        {
            _isBigEndian = isBigEndian;
        }

        public PexReader(Stream input, Encoding encoding, bool isBigEndian) : base(input, encoding)
        {
            _isBigEndian = isBigEndian;
        }

        public PexReader(Stream input, Encoding encoding, bool leaveOpen, bool isBigEndian) : base(input, encoding,
            leaveOpen)
        {
            _isBigEndian = isBigEndian;
        }

        public override ushort ReadUInt16()
        {
            return _isBigEndian ? this.ReadUInt16BE() : base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            return _isBigEndian ? this.ReadUInt32BE() : base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            return _isBigEndian ? this.ReadUInt64BE() : base.ReadUInt64();
        }

        public override int ReadInt32()
        {
            return _isBigEndian ? this.ReadInt32BE() : base.ReadInt32();
        }

        public override float ReadSingle()
        {
            return _isBigEndian ? this.ReadSingleBE() : base.ReadSingle();
        }

        public override string ReadString()
        {
            return _isBigEndian ? this.ReadWStringBE() : this.ReadWStringLE();
        }
    }
}