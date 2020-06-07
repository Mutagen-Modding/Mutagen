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
                new FormKey(modKey: new ModKey("Oblivion", true), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_String0x()
        {
            Assert.True(
                FormKey.TryFactory("00C51A:Oblivion.esm", out FormKey id));
            Assert.Equal(
                new FormKey(modKey: new ModKey("Oblivion", true), id: 0x00C51A),
                id);
        }

        [Fact]
        public void Import_LoopbackString()
        {
            var formKey = new FormKey(modKey: new ModKey("Oblivion", true), id: 0x00C51A);
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
        public ModKey TargetModKey() => new ModKey("Master2", true);

        public MasterReferenceReader TypicalMasters() => 
            new MasterReferenceReader(
                Utility.ModKey,
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
                ModKey.Factory("Oblivion.esm"),
                ModKey.Factory("Knights.esm"),
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
                ModKey.Factory("Oblivion.esm"),
                ModKey.Factory("Knights.esm"),
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
                ModKey.Factory("Oblivion.esm"),
                ModKey.Factory("Knights.esm"),
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
                ModKey.Factory("Oblivion.esm"),
                ModKey.Factory("Knights.esm"),
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
                ModKey.Factory("Oblivion.esm"),
                ModKey.Factory("Knights.esm"),
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
                new OblivionMod(ModKey.Factory("Oblivion.esm")),
                new OblivionMod(ModKey.Factory("Knights.esm")),
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
                new OblivionMod(ModKey.Factory("Oblivion.esm")),
                new OblivionMod(ModKey.Factory("Knights.esm")),
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
                new OblivionMod(ModKey.Factory("Oblivion.esm")),
                new OblivionMod(ModKey.Factory("Knights.esm")),
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
                new OblivionMod(ModKey.Factory("Oblivion.esm")),
                new OblivionMod(ModKey.Factory("Knights.esm")),
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
                new OblivionMod(ModKey.Factory("Oblivion.esm")),
                new OblivionMod(ModKey.Factory("Knights.esm")),
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
