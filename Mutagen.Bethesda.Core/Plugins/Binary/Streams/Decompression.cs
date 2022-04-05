using Ionic.Zlib;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

public static class Decompression
{
    public static byte[] Decompress(byte[] bytes, uint resultLen)
    {
        // ToDo
        // Swap to span version if Zlib updates
        return ZlibStream.UncompressBuffer(bytes);
    }
}