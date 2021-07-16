using System;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Xunit;
using RecordTypes = Mutagen.Bethesda.Plugins.Records.Internals.RecordTypes;
using MemoryStream = System.IO.MemoryStream;

namespace Mutagen.Bethesda.Core.UnitTests.Plugins.Binary.Headers
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
