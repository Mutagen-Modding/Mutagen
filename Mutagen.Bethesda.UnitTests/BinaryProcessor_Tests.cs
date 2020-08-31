using Mutagen.Bethesda.Processing;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class BinaryProcessor_Tests
    {
        public const int BUFFER_SIZE = 16;

        public byte[] GetBuffer(int len)
        {
            byte[] ret = new byte[len];
            for (int i = 0; i < len; i++)
            {
                ret[i] = (byte)i;
            }
            return ret;
        }

        public byte[] GetBytes(BinaryFileProcessor proc, int expectedLen)
        {
            byte[] ret = new byte[expectedLen];
            using (var stream = new MemoryStream(ret))
            {
                proc.CopyTo(stream);
            }
            return ret;
        }

        public byte[] GetBytes(BinaryFileProcessor.Config config, byte[] source)
        {
            return GetBytes(
                new BinaryFileProcessor(
                    new MemoryStream(source),
                    config,
                    bufferLen: BUFFER_SIZE),
                source.Length);
        }

        public byte[] DoMove(byte[] buf, RangeInt64 section, long loc)
        {
            byte[] move = new byte[section.Width];
            Array.Copy(buf, section.Min, move, 0, section.Width);
            List<byte> ret = new List<byte>(buf);
            ret.InsertRange((int)(loc), move);
            ret.RemoveRange((int)section.Min, checked((int)section.Width));
            return ret.ToArray();
        }

        [Fact]
        public void MultiBufferRead()
        {
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 3.7d));
            var outputBuf = GetBytes(
                new BinaryFileProcessor.Config(),
                sourceBuf);
            Assert.Equal(sourceBuf, outputBuf);
        }

        #region Single Substitutions
        [Fact]
        public void Substitutions_FirstPass()
        {
            var loc = 6;
            byte val = 75;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE));
            var expectedBuf = GetBuffer((int)(BUFFER_SIZE));
            expectedBuf[loc] = val;
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetSubstitution(loc, val);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Substitutions_SecondPass()
        {
            var loc = BUFFER_SIZE + 6;
            byte val = 75;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            expectedBuf[loc] = val;
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetSubstitution(loc, val);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Substitutions_FirstByte()
        {
            var loc = BUFFER_SIZE;
            byte val = 75;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            expectedBuf[loc] = val;
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetSubstitution(loc, val);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Substitutions_LastByte()
        {
            var loc = BUFFER_SIZE - 1;
            byte val = 75;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            expectedBuf[loc] = val;
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetSubstitution(loc, val);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Substitutions_OnExtraRead()
        {
            //ToDo
        }
        #endregion

        #region Section Substitutions
        [Fact]
        public void Substitutions_Section_Typical()
        {
            var loc = 6;
            byte[] val = new byte[] { 75, 76, 77 };
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE));
            var expectedBuf = GetBuffer((int)(BUFFER_SIZE));
            for (int i = 0; i < val.Length; i++)
            {
                expectedBuf[loc + i] = val[i];
            }
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetSubstitution(loc, val);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Substitutions_Section_Overlap()
        {
            var loc = 15;
            byte[] val = new byte[] { 75, 76, 77, 78 };
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            for (int i = 0; i < val.Length; i++)
            {
                expectedBuf[loc + i] = val[i];
            }
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetSubstitution(loc, val);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }
        #endregion

        #region Moves
        [Fact]
        public void Move_Inside_FirstPass()
        {
            var section = new RangeInt64(5, 6);
            long loc = 11;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 2)),
                section,
                loc);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_Inside_SecondPass()
        {
            var section = new RangeInt64(BUFFER_SIZE + 4, BUFFER_SIZE + 7);
            long loc = BUFFER_SIZE + 9;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 3));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 3)),
                section,
                loc);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_Inside_Two()
        {
            var section = new RangeInt64(BUFFER_SIZE + 2, BUFFER_SIZE + 4);
            long loc = BUFFER_SIZE + 6;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 3));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 3)),
                section,
                loc);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_StartingEdge()
        {
            var section = new RangeInt64(BUFFER_SIZE * 2, BUFFER_SIZE * 2 + 4);
            long loc = BUFFER_SIZE * 2 + 7;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 3));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 3)),
                section,
                loc);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_EndingEdge()
        {
            var section = new RangeInt64(BUFFER_SIZE * 2 - 4, BUFFER_SIZE * 2 - 1);
            long loc = BUFFER_SIZE * 2 + 5;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 3));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 3)),
                section,
                loc);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_TwoBeforePaste()
        {
            //ToDo
            //Implement

            //var section = new RangeInt64(1, 3);
            //var section2 = new RangeInt64(5, 6);
            //long loc = 9;
            //long loc2 = 11;
            //var sourceBuf = GetBuffer((int)(BUFFER_SIZE));
            //var expectedBuf = new byte[]
            //{
            //    0,
            //    4,
            //    7,
            //    8,
            //    1,
            //    2,
            //    3,
            //    9,
            //    10,
            //    5,
            //    6,
            //    11,
            //    12,
            //    13,
            //    14,
            //    15,
            //};
            //BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            //config.SetMove(section, loc);
            //var outputBuf = GetBytes(
            //    config,
            //    sourceBuf);
            //Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_Overlap()
        {
            var section = new RangeInt64(BUFFER_SIZE - 3, BUFFER_SIZE + 3);
            long loc = BUFFER_SIZE + 7;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 2)),
                section,
                loc);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_Overlap_And_Another()
        {
            var section = new RangeInt64(BUFFER_SIZE - 2, BUFFER_SIZE + 2);
            long loc = BUFFER_SIZE + 4;
            var section2 = new RangeInt64(BUFFER_SIZE + 7, BUFFER_SIZE + 9);
            long loc2 = BUFFER_SIZE + 12;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE * 2));
            var expectedBuf = DoMove(
                GetBuffer((int)(BUFFER_SIZE * 2)),
                section,
                loc);
            expectedBuf = DoMove(
                expectedBuf,
                section2,
                loc2);
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            config.SetMove(section2, loc2);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }
        [Fact]
        public void Move_SameLocation()
        {
            var section = new RangeInt64(2, 3);
            long loc = 12;
            var section2 = new RangeInt64(7, 9);
            long loc2 = 12;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE));
            var expectedBuf = new byte[]
            {
                0,
                1,
                4,
                5,
                6,
                10,
                11,
                7,
                8,
                9,
                2,
                3,
                12,
                13,
                14,
                15
            };
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            config.SetMove(section2, loc2);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_SameLocation_Flip()
        {
            var section = new RangeInt64(2, 3);
            long loc = 12;
            var section2 = new RangeInt64(7, 9);
            long loc2 = 12;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE));
            var expectedBuf = new byte[]
            {
                0,
                1,
                4,
                5,
                6,
                10,
                11,
                2,
                3,
                7,
                8,
                9,
                12,
                13,
                14,
                15
            };
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section2, loc2);
            config.SetMove(section, loc);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }

        [Fact]
        public void Move_Cross()
        {
            var section = new RangeInt64(2, 3);
            long loc = 12;
            var section2 = new RangeInt64(6, 8);
            long loc2 = 10;
            var sourceBuf = GetBuffer((int)(BUFFER_SIZE));
            var expectedBuf = new byte[]
            {
                0,
                1,
                4,
                5,
                9,
                6,
                7,
                8,
                10,
                11,
                2,
                3,
                12,
                13,
                14,
                15
            };
            BinaryFileProcessor.Config config = new BinaryFileProcessor.Config();
            config.SetMove(section, loc);
            config.SetMove(section2, loc2);
            var outputBuf = GetBytes(
                config,
                sourceBuf);
            Assert.Equal(expectedBuf, outputBuf);
        }
        #endregion
    }
}
