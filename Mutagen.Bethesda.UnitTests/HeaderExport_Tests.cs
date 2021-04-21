using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Records;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Mutagen.Bethesda.Records.Internals;
using System;
using System.IO;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class HeaderExport_Tests
    {
        [Fact]
        public void Overflow()
        {
            Assert.Throws<OverflowException>(() =>
            {
                byte[] b = new byte[ushort.MaxValue + 100];
                byte[] b2 = new byte[ushort.MaxValue + 1];
                using var writer = new MutagenWriter(new MemoryStream(b), GameConstants.Oblivion);
                using var header = HeaderExport.Header(writer, RecordTypes.EDID, ObjectType.Subrecord);
                writer.Write(b2);
            });
        }
    }
}
