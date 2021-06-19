using Mutagen.Bethesda.Pex;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Text;
using Xunit;
using MemoryStream = System.IO.MemoryStream;
using BinaryWriter = System.IO.BinaryWriter;

namespace Mutagen.Bethesda.UnitTests.Pex
{
    public class BinaryExtensionsTests
    {
        [Theory]
        [InlineData("Hello World")]
        public void TestWStringBE(string expected)
        {
            var bytes = Encoding.UTF8.GetBytes(expected);
            DoTest(bytes.Length + sizeof(uint), expected,
                bw => bw.WriteWStringBE(expected),
                br => br.ReadPrependedString(2));
        }

        [Theory]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        public void TestUInt16(ushort expected)
        {
            DoTest(sizeof(ushort), expected, 
                bw => bw.WriteUInt16BE(expected), 
                br => br.ReadUInt16());
        }

        [Theory]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void TestUInt32(uint expected)
        {
            DoTest(sizeof(uint), expected, 
                bw => bw.WriteUInt32BE(expected), 
                br => br.ReadUInt32());
        }
        
        [Theory]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        public void TestUInt64(ulong expected)
        {
            DoTest(sizeof(ulong), expected, 
                bw => bw.WriteUInt64BE(expected), 
                br => br.ReadUInt64());
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void TestInt32(int expected)
        {
            DoTest(sizeof(int), expected, 
                bw => bw.WriteInt32BE(expected), 
                br => br.ReadInt32());
        }
        
        [Theory]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        public void TestSingle(float expected)
        {
            DoTest(sizeof(float), expected, 
                bw => bw.WriteSingleBE(expected), 
                br => br.ReadFloat());
        }
        
        private static void DoTest<T>(int streamCapacity, T expected, Action<BinaryWriter> write,
            Func<IBinaryReadStream, T> read)
        {
            using var ms = new MemoryStream(streamCapacity);
            using var bw = new BinaryWriter(ms);
            using var br = new BinaryReadStream(ms, isLittleEndian: false);

            write(bw);
            ms.Position = 0;

            var actual = read(br);
            Assert.Equal(expected, actual);
            Assert.Equal(ms.Length, ms.Position);
        }
    }
}
