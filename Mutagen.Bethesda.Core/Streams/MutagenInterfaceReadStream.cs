using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda
{
    public class MutagenInterfaceReadStream : IMutagenReadStream
    {
        private readonly IBinaryReadStream _readStream;

        /// <inheritdoc />
        public long OffsetReference { get; }

        /// <inheritdoc />
        public GameConstants MetaData { get; }

        /// <inheritdoc />
        public MasterReferenceReader? MasterReferences { get; set; }

        /// <inheritdoc/>
        public RecordInfoCache? RecordInfoCache { get; set; }

        public MutagenInterfaceReadStream(
            IBinaryReadStream stream,
            GameConstants metaData,
            MasterReferenceReader? masterReferences = null,
            long offsetReference = 0)
        {
            _readStream = stream;
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
            this.OffsetReference = offsetReference;
        }

        /// <inheritdoc />
        public long Position 
        {
            get => _readStream.Position;
            set => _readStream.Position = value;
        }

        /// <inheritdoc />
        public long Length => _readStream.Length;

        /// <inheritdoc />
        public long Remaining => _readStream.Remaining;

        /// <inheritdoc />
        public bool Complete => _readStream.Complete;

        /// <inheritdoc />
        public ReadOnlySpan<byte> RemainingSpan => _readStream.RemainingSpan;

        /// <inheritdoc />
        public ReadOnlyMemorySlice<byte> RemainingMemory => _readStream.RemainingMemory;

        /// <inheritdoc />
        public void Dispose() => _readStream.Dispose();

        /// <inheritdoc />
        public int Get(byte[] buffer, int targetOffset, int amount) => _readStream.Get(buffer, targetOffset, amount);

        /// <inheritdoc />
        public int Get(byte[] buffer, int targetOffset) => _readStream.Get(buffer, targetOffset);

        /// <inheritdoc />
        public bool GetBool() => _readStream.GetBool();

        /// <inheritdoc />
        public bool GetBool(int offset) => _readStream.GetBool(offset);

        /// <inheritdoc />
        public byte[] GetBytes(int amount) => _readStream.GetBytes(amount);

        /// <inheritdoc />
        public double GetDouble() => _readStream.GetDouble();

        /// <inheritdoc />
        public double GetDouble(int offset) => _readStream.GetDouble(offset);

        /// <inheritdoc />
        public float GetFloat() => _readStream.GetFloat();

        /// <inheritdoc />
        public float GetFloat(int offset) => _readStream.GetFloat(offset);

        /// <inheritdoc />
        public short GetInt16() => _readStream.GetInt16();

        /// <inheritdoc />
        public short GetInt16(int offset) => _readStream.GetInt16(offset);

        /// <inheritdoc />
        public int GetInt32() => _readStream.GetInt32();

        /// <inheritdoc />
        public int GetInt32(int offset) => _readStream.GetInt32(offset);

        /// <inheritdoc />
        public long GetInt64() => _readStream.GetInt64();

        /// <inheritdoc />
        public long GetInt64(int offset) => _readStream.GetInt64(offset);

        /// <inheritdoc />
        public sbyte GetInt8() => _readStream.GetInt8();

        /// <inheritdoc />
        public sbyte GetInt8(int offset) => _readStream.GetInt8(offset);

        /// <inheritdoc />
        public ReadOnlyMemorySlice<byte> GetMemory(int amount, bool readSafe = true) => _readStream.GetMemory(amount, readSafe);

        /// <inheritdoc />
        public ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset, bool readSafe = true) => _readStream.GetMemory(amount, offset, readSafe);

        /// <inheritdoc />
        public ReadOnlySpan<byte> GetSpan(int amount, bool readSafe = true) => _readStream.GetSpan(amount, readSafe);

        /// <inheritdoc />
        public ReadOnlySpan<byte> GetSpan(int amount, int offset, bool readSafe = true) => _readStream.GetSpan(amount, offset, readSafe);

        /// <inheritdoc />
        public string GetStringUTF8(int amount) => _readStream.GetStringUTF8(amount);

        /// <inheritdoc />
        public string GetStringUTF8(int amount, int offset) => _readStream.GetStringUTF8(amount, offset);

        /// <inheritdoc />
        public ushort GetUInt16() => _readStream.GetUInt16();

        /// <inheritdoc />
        public ushort GetUInt16(int offset) => _readStream.GetUInt16(offset);

        /// <inheritdoc />
        public uint GetUInt32() => _readStream.GetUInt32();

        /// <inheritdoc />
        public uint GetUInt32(int offset) => _readStream.GetUInt32(offset);

        /// <inheritdoc />
        public ulong GetUInt64() => _readStream.GetUInt64();

        /// <inheritdoc />
        public ulong GetUInt64(int offset) => _readStream.GetUInt64(offset);

        /// <inheritdoc />
        public byte GetUInt8() => _readStream.GetUInt8();

        /// <inheritdoc />
        public byte GetUInt8(int offset) => _readStream.GetUInt8(offset);

        /// <inheritdoc />
        public int Read(byte[] buffer, int offset, int amount) => _readStream.Read(buffer, offset, amount);

        /// <inheritdoc />
        public int Read(byte[] buffer) => _readStream.Read(buffer);

        /// <inheritdoc />
        public IMutagenReadStream ReadAndReframe(int length)
        {
            var offset = this.OffsetReference + this.Position;
            return new MutagenMemoryReadStream(
                this.ReadMemory(length, readSafe: true), 
                this.MetaData,
                this.MasterReferences, 
                offsetReference: offset,
                infoCache: this.RecordInfoCache);
        }

        /// <inheritdoc />
        public bool ReadBool() => _readStream.ReadBool();

        /// <inheritdoc />
        public byte[] ReadBytes(int amount) => _readStream.ReadBytes(amount);

        /// <inheritdoc />
        public double ReadDouble() => _readStream.ReadDouble();

        /// <inheritdoc />
        public float ReadFloat() => _readStream.ReadFloat();

        /// <inheritdoc />
        public short ReadInt16() => _readStream.ReadInt16();

        /// <inheritdoc />
        public int ReadInt32() => _readStream.ReadInt32();

        /// <inheritdoc />
        public long ReadInt64() => _readStream.ReadInt64();

        /// <inheritdoc />
        public sbyte ReadInt8() => _readStream.ReadInt8();

        /// <inheritdoc />
        public ReadOnlyMemorySlice<byte> ReadMemory(int amount, bool readSafe = true) => _readStream.ReadMemory(amount, readSafe);

        /// <inheritdoc />
        public ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset, bool readSafe = true) => _readStream.ReadMemory(amount, offset, readSafe);

        /// <inheritdoc />
        public ReadOnlySpan<byte> ReadSpan(int amount, bool readSafe = true) => _readStream.ReadSpan(amount, readSafe);

        /// <inheritdoc />
        public ReadOnlySpan<byte> ReadSpan(int amount, int offset, bool readSafe = true) => _readStream.ReadSpan(amount, offset, readSafe);

        /// <inheritdoc />
        public string ReadStringUTF8(int amount) => _readStream.ReadStringUTF8(amount);

        /// <inheritdoc />
        public ushort ReadUInt16() => _readStream.ReadUInt16();

        /// <inheritdoc />
        public uint ReadUInt32() => _readStream.ReadUInt32();

        /// <inheritdoc />
        public ulong ReadUInt64() => _readStream.ReadUInt64();

        /// <inheritdoc />
        public byte ReadUInt8() => _readStream.ReadUInt8();

        /// <inheritdoc />
        public void WriteTo(Stream stream, int amount) => _readStream.WriteTo(stream, amount);
    }
}
