using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
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

        #region BinaryTranslation
        public ModKey TargetModKey() => new ModKey("Master2", true);

        public MasterReferences TypicalMasters() => 
            new MasterReferences(
                ModKey.Dummy,
                new List<IMasterReferenceGetter>()
                {
                    new MasterReference()
                    {
                        Master = new ModKey("Master1", true)
                    },
                    new MasterReference()
                    {
                        Master = TargetModKey()
                    },
                    new MasterReference()
                    {
                        Master = new ModKey("Master3", false)
                    },
                });

        [Fact]
        public void BinaryTranslation_Typical()
        {
            byte[] b = new byte[]
            {
                0x56,
                0x34,
                0x12,
                1
            };
            var formKey = FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters());
            Assert.Equal(TargetModKey(), formKey.ModKey);
            Assert.Equal((uint)0x123456, formKey.ID);
        }

        [Fact]
        public void BinaryTranslation_TooShort()
        {
            byte[] b = new byte[]
            {
                0x56,
                0x34,
                0x12,
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters()));
        }

        [Fact]
        public void BinaryTranslation_TooLong()
        {
            byte[] b = new byte[]
            {
                0x56,
                0x34,
                0x12,
                1,
                0x99
            };
            var formKey = FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters());
            Assert.Equal(TargetModKey(), formKey.ModKey);
            Assert.Equal((uint)0x123456, formKey.ID);
        }
        #endregion
    }
}
