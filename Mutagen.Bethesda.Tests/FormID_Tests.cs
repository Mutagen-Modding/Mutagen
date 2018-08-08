using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class FormID_Tests
    {
        [Fact]
        public void Import_Zero()
        {
            byte[] bytes = new byte[4];
            FormID id = FormID.Factory(bytes);
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
            FormID id = FormID.Factory(bytes);
            Assert.Equal(5, id.ModID.ID);
            Assert.Equal((uint)0xCBD8, id.ID);
        }

        [Fact]
        public void Import_String()
        {
            Assert.True(
                FormID.TryFactory("0100C51A", out FormID id));
            Assert.Equal(
                new FormID(modID: new ModID(1), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_String0x()
        {
            Assert.True(
                FormID.TryFactory("0x0100C51A", out FormID id));
            Assert.Equal(
                new FormID(modID: new ModID(1), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Ctor_Typical()
        {
            FormID id = new FormID(0x12345678);
            Assert.Equal((uint)(0x345678), id.ID);
            Assert.Equal((byte)(0x12), id.ModID.ID);
            Assert.Equal((uint)0x12345678, id.Raw);
        }

        [Fact]
        public void Ctor_WithModID()
        {
            FormID id = new FormID(new ModID(0x12), 0x99345678);
            Assert.Equal((uint)(0x345678), id.ID);
            Assert.Equal((byte)(0x12), id.ModID.ID);
            Assert.Equal((uint)0x12345678, id.Raw);
        }
    }
}
