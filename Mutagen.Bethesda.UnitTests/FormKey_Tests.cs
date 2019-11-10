using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class FormKey_Tests
    {
        [Fact]
        public void Import_String()
        {
            Assert.True(
                FormKey.TryFactory("00C51AOblivion.esm", out FormKey id));
            Assert.Equal(
                new FormKey(modKey: new ModKey("Oblivion", true), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_String0x()
        {
            Assert.True(
                FormKey.TryFactory("00C51AOblivion.esm", out FormKey id));
            Assert.Equal(
                new FormKey(modKey: new ModKey("Oblivion", true), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_Null()
        {
            Assert.True(
                FormKey.TryFactory(FormKey.NULL_STR, out FormKey id));
            Assert.Equal(
                FormKey.NULL,
                id);
        }

        [Fact]
        public void Import_Whitespace()
        {
            Assert.False(
                FormKey.TryFactory(" ", out FormKey id));
        }

        [Fact]
        public void Import_Malformed()
        {
            Assert.False(
                FormKey.TryFactory("00C51AOblivionesm", out FormKey id));
        }

        [Fact]
        public void Null_Equality()
        {
            Assert.Equal(FormKey.NULL, new FormKey());
        }

        [Fact]
        public void Null_Hash()
        {
            Assert.Equal(0, FormKey.NULL.GetHashCode());
        }
    }
}
