using Noggog;
using System;
using System.IO;
using Mutagen.Bethesda.Archives.Exceptions;

namespace Mutagen.Bethesda.Archives.Bsa
{
    class BsaFileNameBlock
    {
        public readonly Lazy<ReadOnlyMemorySlice<byte>[]> Names;

        public BsaFileNameBlock(BsaReader bsa, long position)
        {
            Names = new Lazy<ReadOnlyMemorySlice<byte>[]>(
                mode: System.Threading.LazyThreadSafetyMode.ExecutionAndPublication,
                valueFactory: () =>
                {
                    try
                    {
                        using var stream = bsa.GetStream();
                        stream.BaseStream.Position = position;
                        ReadOnlyMemorySlice<byte> data = stream.ReadBytes(checked((int)bsa.TotalFileNameLength));
                        ReadOnlyMemorySlice<byte>[] names = new ReadOnlyMemorySlice<byte>[bsa.FileCount];
                        for (int i = 0; i < bsa.FileCount; i++)
                        {
                            var index = data.Span.IndexOf(default(byte));
                            if (index == -1)
                            {
                                throw new InvalidDataException("Did not end all of its strings in null bytes");
                            }
                            names[i] = data.Slice(0, index + 1);
                            data = data.Slice(index + 1);
                        }
                        // Data doesn't seem to need to be fully consumed.
                        // Official BSAs have overflow of zeros
                        return names;
                    }
                    catch (Exception e)
                    when (bsa.FilePath != null)
                    {
                        throw ArchiveException.EnrichWithArchivePath("Strings section was not able to be read", e,
                            bsa.FilePath.Value);
                    }
                });
        }
    }
}
