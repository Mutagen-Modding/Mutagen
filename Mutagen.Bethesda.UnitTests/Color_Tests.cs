using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Color_Tests
    {
        [Fact]
        public void ColorPassthrough_Half()
        {
            var bytes = new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0x3F,
                0, 0, 0x80, 0x3F
            };
            var color = IBinaryStreamExt.ReadColor(bytes, ColorBinaryType.NoAlphaFloat);
            var outBytes = new byte[12];
            using (var writer = new MutagenWriter(new MemoryStream(outBytes), default(WritingBundle)!))
            {
                writer.Write(color, ColorBinaryType.NoAlphaFloat);
            }
            Assert.Equal(bytes, outBytes);
        }
    }
}
