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
        static MutagenFrame frame = new MutagenFrame(new MutagenMemoryReadStream(bytes, GameConstants.Oblivion));
        static RecordType type = new RecordType("TES4");

        //[Benchmark]
        //public (bool, long) ReadNextRecordType()
        //{
        //    if (frame.Remaining < Constants.HEADER_LENGTH + 4) return (false, -1);
        //    var meta = new RecordMeta(frame.MetaData, frame.RemainingSpan.Slice(0, 8));
        //    if (meta.RecordType != type) return (false, -1);
        //    return (true, meta.RecordLength);
        //}
    }
}
