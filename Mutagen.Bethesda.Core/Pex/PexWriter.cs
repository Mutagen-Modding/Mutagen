using Mutagen.Bethesda.Binary;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Pex;

internal class PexWriter : BinaryWriter
{
    private readonly bool _isBigEndian;
        
    public PexWriter(Stream output, bool isBigEndian) : base(output)
    {
        _isBigEndian = isBigEndian;
    }

    public PexWriter(Stream output, Encoding encoding, bool isBigEndian) : base(output, encoding)
    {
        _isBigEndian = isBigEndian;
    }

    public PexWriter(Stream output, Encoding encoding, bool leaveOpen, bool isBigEndian) : base(output, encoding,
        leaveOpen)
    {
        _isBigEndian = isBigEndian;
    }

    public override void Write(ushort value)
    {
        if (_isBigEndian)
            this.WriteUInt16BE(value);
        else
            base.Write(value);
    }

    public override void Write(uint value)
    {
        if (_isBigEndian)
            this.WriteUInt32BE(value);
        else
            base.Write(value);
    }
        
    public override void Write(ulong value)
    {
        if (_isBigEndian)
            this.WriteUInt64BE(value);
        else
            base.Write(value);
    }

    public override void Write(int value)
    {
        if (_isBigEndian)
            this.WriteInt32BE(value);
        else
            base.Write(value);
    }

    public override void Write(float value)
    {
        if (_isBigEndian)
            this.WriteSingleBE(value);
        else
            base.Write(value);
    }

    public override void Write(string value)
    {
        if (_isBigEndian)
            this.WriteWStringBE(value);
        else
            this.WriteWStringLE(value);
    }
}