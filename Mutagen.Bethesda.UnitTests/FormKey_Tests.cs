using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
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
                FormKey.TryFactory("00C51A:Oblivion.esm", out FormKey id));
            Assert.Equal(
                new FormKey(modKey: new ModKey("Oblivion", ModType.Master), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_String0x()
        {
            Assert.True(
                FormKey.TryFactory("00C51A:Oblivion.esm", out FormKey id));
            Assert.Equal(
                new FormKey(modKey: new ModKey("Oblivion", ModType.Master), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_LoopbackString()
        {
            var formKey = new FormKey(modKey: new ModKey("Oblivion", ModType.Master), id: 0x00C51A);
            Assert.True(
                FormKey.TryFactory(formKey.ToString(), out FormKey id));
            Assert.Equal(
                formKey,
                id);
        }

        [Fact]
        public void Import_Null()
        {
            Assert.True(
                FormKey.TryFactory(FormKey.NullStr, out FormKey id));
            Assert.Equal(
                FormKey.Null,
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

        #region BinaryTranslation
        public ModKey TargetModKey() => new ModKey("Master2", ModType.Master);

        public MasterReferenceReader TypicalMasters() => 
            new MasterReferenceReader(
                Utility.PluginModKey,
                new List<IMasterReferenceGetter>()
                {
                    new MasterReference()
                    {
                        Master = new ModKey("Master1", ModType.Master)
                    },
                    new MasterReference()
                    {
                        Master = TargetModKey()
                    },
                    new MasterReference()
                    {
                        Master = new ModKey("Master3", type: ModType.Master)
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
            Assert.False(formKey.IsNull);
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
            Assert.False(formKey.IsNull);
        }

        [Fact]
        public void BinaryTranslation_Zeros()
        {
            byte[] b = new byte[]
            {
                0,0,0,0
            };
            var formKey = FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters());
            Assert.Equal(ModKey.Null, formKey.ModKey);
            Assert.Equal((uint)0, formKey.ID);
            Assert.True(formKey.IsNull);
        }
        #endregion

        #region Null
        [Fact]
        public void Null_TypicalNotNull()
        {
            Utility.Skyrim.IsNull.Should().BeFalse();
        }

        [Fact]
        public void Null_NullIsNull()
        {
            FormKey.Null.IsNull.Should().BeTrue();
        }

        [Fact]
        public void Null_ExistingModKeyIsNull()
        {
            new FormKey(Utility.Skyrim, 0).IsNull.Should().BeTrue();
        }

        [Fact]
        public void Null_ExistingIdIsNull()
        {
            new FormKey(ModKey.Null, 123456).IsNull.Should().BeTrue();
        }
        #endregion

        #region Comparers
        #region Alphabetical
        [Fact]
        public void Comparer_Alphabetical_ByMaster()
        {
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Oblivion.esp");
            var compare = FormKey.AlphabeticalComparer();
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByMasterGreater()
        {
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Oblivion.esp");
            var compare = FormKey.AlphabeticalComparer();
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByName()
        {
            FormKey k1 = FormKey.Factory("00C51A:Knights.esm");
            FormKey k2 = FormKey.Factory("00C51A:Oblivion.esm");
            var compare = FormKey.AlphabeticalComparer();
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByNameGreater()
        {
            FormKey k1 = FormKey.Factory("00C51A:Knights.esm");
            FormKey k2 = FormKey.Factory("00C51A:Oblivion.esm");
            var compare = FormKey.AlphabeticalComparer();
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByID()
        {
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.AlphabeticalComparer();
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByIDGreater()
        {
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.AlphabeticalComparer();
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_Alphabetical_Equal()
        {
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Oblivion.esm");
            var compare = FormKey.AlphabeticalComparer();
            Assert.Equal(0, compare.Compare(k2, k1));
        }
        #endregion
        #region ModKey List
        [Fact]
        public void Comparer_ModKeyList_Typical()
        {
            List<ModKey> modKeys = new List<ModKey>()
            {
                ModKey.FromNameAndExtension("Oblivion.esm"),
                ModKey.FromNameAndExtension("Knights.esm"),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
            var compare = FormKey.LoadOrderComparer(modKeys);
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_ModKeyList_TypicalGreater()
        {
            List<ModKey> modKeys = new List<ModKey>()
            {
                ModKey.FromNameAndExtension("Oblivion.esm"),
                ModKey.FromNameAndExtension("Knights.esm"),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
            var compare = FormKey.LoadOrderComparer(modKeys);
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_ModKeyList_Fallback()
        {
            List<ModKey> modKeys = new List<ModKey>()
            {
                ModKey.FromNameAndExtension("Oblivion.esm"),
                ModKey.FromNameAndExtension("Knights.esm"),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.LoadOrderComparer(modKeys);
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_ModKeyList_FallbackGreater()
        {
            List<ModKey> modKeys = new List<ModKey>()
            {
                ModKey.FromNameAndExtension("Oblivion.esm"),
                ModKey.FromNameAndExtension("Knights.esm"),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.LoadOrderComparer(modKeys);
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_ModKeyList_Unknown()
        {
            List<ModKey> modKeys = new List<ModKey>()
            {
                ModKey.FromNameAndExtension("Oblivion.esm"),
                ModKey.FromNameAndExtension("Knights.esm"),
            };
            FormKey k1 = FormKey.Factory("00C51A:MyMod.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.LoadOrderComparer(modKeys);
            Assert.Throws<ArgumentOutOfRangeException>(() => compare.Compare(k1, k2));
        }
        #endregion
        #region LoadOrder List
        [Fact]
        public void Comparer_LoadOrder_Typical()
        {
            var loadOrder = new LoadOrder<OblivionMod>()
            {
                new OblivionMod(ModKey.FromNameAndExtension("Oblivion.esm")),
                new OblivionMod(ModKey.FromNameAndExtension("Knights.esm")),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
            var compare = FormKey.LoadOrderComparer(loadOrder);
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_LoadOrder_TypicalGreater()
        {
            var loadOrder = new LoadOrder<OblivionMod>()
            {
                new OblivionMod(ModKey.FromNameAndExtension("Oblivion.esm")),
                new OblivionMod(ModKey.FromNameAndExtension("Knights.esm")),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
            var compare = FormKey.LoadOrderComparer(loadOrder);
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_LoadOrder_Fallback()
        {
            var loadOrder = new LoadOrder<OblivionMod>()
            {
                new OblivionMod(ModKey.FromNameAndExtension("Oblivion.esm")),
                new OblivionMod(ModKey.FromNameAndExtension("Knights.esm")),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.LoadOrderComparer(loadOrder);
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_LoadOrder_FallbackGreater()
        {
            var loadOrder = new LoadOrder<OblivionMod>()
            {
                new OblivionMod(ModKey.FromNameAndExtension("Oblivion.esm")),
                new OblivionMod(ModKey.FromNameAndExtension("Knights.esm")),
            };
            FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.LoadOrderComparer(loadOrder);
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_LoadOrder_Unknown()
        {
            var loadOrder = new LoadOrder<OblivionMod>()
            {
                new OblivionMod(ModKey.FromNameAndExtension("Oblivion.esm")),
                new OblivionMod(ModKey.FromNameAndExtension("Knights.esm")),
            };
            FormKey k1 = FormKey.Factory("00C51A:MyMod.esm");
            FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
            var compare = FormKey.LoadOrderComparer(loadOrder);
            Assert.Throws<ArgumentOutOfRangeException>(() => compare.Compare(k1, k2));
        }
        #endregion
        #endregion
    }
}
