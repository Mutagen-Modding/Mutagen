using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class RawFormID_Tests
    {
        [Fact]
        public void Import_Zero()
        {
            byte[] bytes = new byte[4];
            RawFormID id = RawFormID.Factory(bytes);
            Assert.Equal(0, id.ModID.ID);
            Assert.Equal(uint.MinValue, id.ID);
        }

        [Fact]
        public void Import_Typical()
        {
            byte[] bytes = new byte[4]
            {
                216,
                203,
                0,
                5,
            };
            RawFormID id = RawFormID.Factory(bytes);
            Assert.Equal(5, id.ModID.ID);
            bool i = 52184 == id.ID;
            Assert.True(i);
        }

        [Fact]
        public void Import_String()
        {
            Assert.True(
                RawFormID.TryFactory("0100C51A", out RawFormID id));
            Assert.Equal(
                new RawFormID(modID: new ModID(1), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_String0x()
        {
            Assert.True(
                RawFormID.TryFactory("0x0100C51A", out RawFormID id));
            Assert.Equal(
                new RawFormID(modID: new ModID(1), id: 0x00C51A),
                id);
        }
    }
}
