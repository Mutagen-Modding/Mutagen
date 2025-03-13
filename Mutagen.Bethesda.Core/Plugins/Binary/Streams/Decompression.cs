using System.Buffers.Binary;
using System.IO.Compression;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

public static class Decompression
{
    public static byte[] Decompress(ReadOnlyMemorySlice<byte> bytes, uint uncompressedLength)
    {
        byte[] buf = new byte[checked((int)uncompressedLength)];
        
        using (var stream = new ZLibStream(new ByteMemorySliceStream(bytes),
                   CompressionMode.Decompress))
        {
            stream.ReadExactly(buf, 0, checked((int)uncompressedLength));
        }

        return buf;
    }

    internal static ReadOnlyMemorySlice<byte> DecompressMajorRecordSpan(ReadOnlyMemorySlice<byte> slice, GameConstants meta)
    {
        var majorMeta = meta.MajorRecordHeader(slice);
        if (majorMeta.IsCompressed)
        {
            uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(slice.Slice(majorMeta.HeaderLength));
            byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
            // Copy major meta bytes over
            slice.Span.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
            // Set length bytes
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength),
                uncompressedLength);
            // Copy uncompressed data over
            using (var stream = new ZLibStream(new ByteMemorySliceStream(slice.Slice(majorMeta.HeaderLength + 4)),
                       CompressionMode.Decompress))
            {
                stream.ReadExactly(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
            }

            slice = new MemorySlice<byte>(buf);
        }

        return slice;
    }

    internal static OverlayStream DecompressStream(OverlayStream stream)
    {
        var majorMeta = stream.GetMajorRecordHeader();
        if (majorMeta.IsCompressed)
        {
            uint uncompressedLength =
                BinaryPrimitives.ReadUInt32LittleEndian(stream.RemainingSpan.Slice(majorMeta.HeaderLength));
            byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
            // Copy major meta bytes over
            stream.RemainingSpan.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
            // Set length bytes
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength),
                uncompressedLength);
            // Copy uncompressed data over
            using (var compessionStream =
                   new ZLibStream(new ByteMemorySliceStream(stream.RemainingMemory.Slice(majorMeta.HeaderLength + 4)),
                       CompressionMode.Decompress))
            {
                compessionStream.ReadExactly(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
            }

            stream.Position += checked((int)majorMeta.TotalLength);
            stream = new OverlayStream(buf, stream.MetaData);
        }

        return stream;
    }
}