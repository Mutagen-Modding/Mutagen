using System;
using System.IO;
using System.Text;
using Mutagen.Bethesda.Pex.Extensions;
using Xunit;

namespace Mutagen.Bethesda.Pex.Tests
{
    public class BinaryExtensionsTests
    {
        [Theory]
        [InlineData("Hello World")]
        public void TestWString(string expected)
        {
            var bytes = Encoding.UTF8.GetBytes(expected);
            DoTest(bytes.Length + sizeof(uint), expected,
                bw => bw.WriteWString(expected),
                br => br.ReadWString());
        }

        [Theory]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        public void TestUInt16(ushort expected)
        {
            DoTest(sizeof(ushort), expected, 
                bw => bw.WriteUInt16BE(expected), 
                br => br.ReadUInt16BE());
        }

        [Theory]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void TestUInt32(uint expected)
        {
            DoTest(sizeof(uint), expected, 
                bw => bw.WriteUInt32BE(expected), 
                br => br.ReadUInt32BE());
        }
        
        [Theory]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        public void TestUInt64(ulong expected)
        {
            DoTest(sizeof(ulong), expected, 
                bw => bw.WriteUInt64BE(expected), 
                br => br.ReadUInt64BE());
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void TestInt32(int expected)
        {
            DoTest(sizeof(int), expected, 
                bw => bw.WriteInt32BE(expected), 
                br => br.ReadInt32BE());
        }
        
        [Theory]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        public void TestSingle(float expected)
        {
            DoTest(sizeof(float), expected, 
                bw => bw.WriteSingleBE(expected), 
                br => br.ReadSingleBE());
        }
        
        private static void DoTest<T>(int streamCapacity, T expected, Action<BinaryWriter> write,
            Func<BinaryReader, T> read)
        {
            using var ms = new MemoryStream(streamCapacity);
            using var bw = new BinaryWriter(ms);
            using var br = new BinaryReader(ms);

            write(bw);
            ms.Position = 0;

            var actual = read(br);
            Assert.Equal(expected, actual);
            Assert.Equal(ms.Length, ms.Position);
        }
    }
}
