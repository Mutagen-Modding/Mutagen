using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    public class HeaderParsing
    {
        static byte[] bytes = new byte[] { 0x54, 0x45, 0x53, 0x34, 0xE8, 0x02, 0x00, 0x00 };
        static MutagenFrame frame = new MutagenFrame(new MutagenMemoryReadStream(bytes));
        static RecordType type = new RecordType("TES4");

        [Benchmark]
        public (bool, long) ReadNextRecordType()
        {
            return (HeaderTranslation.TryGetRecordType(frame, 4, out long len, type), len);
        }
    }
}
