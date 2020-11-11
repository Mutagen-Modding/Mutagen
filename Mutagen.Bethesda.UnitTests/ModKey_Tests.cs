using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class ModKey_Tests
    {
        #region TryFactory
        [Fact]
        public void TryFactory_TypicalMaster()
        {
            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esm", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esm", modKey.FileName);
            Assert.Equal(ModType.Master, modKey.Type);
        }

        [Fact]
        public void TryFactory_TypicalLightMaster()
        {
            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esl", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esl", modKey.FileName);
            Assert.Equal(ModType.LightMaster, modKey.Type);
        }

        [Fact]
        public void TryFactory_TypicalMod()
        {
            Assert.True(ModKey.TryFromNameAndExtension("Knights.esp", out var modKey));
            Assert.Equal("Knights", modKey.Name);
            Assert.Equal("Knights.esp", modKey.FileName);
            Assert.Equal(ModType.Plugin, modKey.Type);
        }

        [Fact]
        public void TryFactory_ExtraPeriod()
        {
            Assert.True(ModKey.TryFromNameAndExtension("Obliv.ion.esm", out var modKey));
            Assert.Equal("Obliv.ion", modKey.Name);
            Assert.Equal("Obliv.ion.esm", modKey.FileName);
            Assert.Equal(ModType.Master, modKey.Type);
        }

        [Fact]
        public void TryFactory_ImproperlyLengthedSuffix()
        {
            Assert.False(ModKey.TryFromNameAndExtension("Obliv.ion.esmz", out var modKey));
        }

        [Fact]
        public void TryFactory_ImproperSuffix()
        {
            Assert.False(ModKey.TryFromNameAndExtension("Obliv.ion.esn", out var modKey));
        }

        [Fact]
        public void TryFactory_TypicalDuplicate()
        {
            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esm", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esm", modKey.FileName);
            Assert.Equal(ModType.Master, modKey.Type);
            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esm", out var modKey2));
            Assert.Equal("Oblivion", modKey2.Name);
            Assert.Equal("Oblivion.esm", modKey2.FileName);
            Assert.Equal(ModType.Master, modKey.Type);
            Assert.Same(modKey.Name, modKey2.Name);
        }

        [Fact]
        public void TryFactory_TypicalBothDuplicates()
        {
            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esm", out var masterKey));
            Assert.Equal("Oblivion", masterKey.Name);
            Assert.Equal("Oblivion.esm", masterKey.FileName);
            Assert.Equal(ModType.Master, masterKey.Type);

            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esp", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esp", modKey.FileName);
            Assert.Equal(ModType.Plugin, modKey.Type);

            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esm", out var masterKey2));
            Assert.Equal("Oblivion", masterKey2.Name);
            Assert.Equal("Oblivion.esm", masterKey2.FileName);
            Assert.Equal(ModType.Master, masterKey2.Type);

            Assert.True(ModKey.TryFromNameAndExtension("Oblivion.esp", out var modKey2));
            Assert.Equal("Oblivion", modKey2.Name);
            Assert.Equal("Oblivion.esp", modKey2.FileName);
            Assert.Equal(ModType.Plugin, modKey2.Type);

            Assert.Same(masterKey.Name, masterKey2.Name);
            Assert.Same(modKey.Name, modKey2.Name);
        }

        [Fact]
        public void CaseEquality()
        {
            ModKey modKey = new ModKey("Oblivion", ModType.Master);
            ModKey modKey2 = new ModKey("OblivioN", ModType.Master);
            Assert.Equal(modKey, modKey2);
            Assert.Equal(modKey.GetHashCode(), modKey2.GetHashCode());
        }

        [Fact]
        public void LookupCorrectness()
        {
            ModKey modKey = new ModKey("Oblivion", ModType.Master);
            ModKey modKey2 = new ModKey("OblivioN", ModType.Master);
            var set = new HashSet<ModKey>()
            {
                modKey
            };
            Assert.Contains(modKey2, set);
        }
        #endregion

        [Fact]
        public void NullHashDesync()
        {
            var modKey = new ModKey(null!, type: ModType.Master);
            var modKey2 = new ModKey(string.Empty, type: ModType.Master);
            Assert.Equal(ModKey.Null, modKey);
            Assert.Equal(ModKey.Null.GetHashCode(), modKey.GetHashCode());
            Assert.Equal(ModKey.Null, modKey2);
            Assert.Equal(ModKey.Null.GetHashCode(), modKey2.GetHashCode());
            Assert.Equal(default(ModKey), ModKey.Null);
            Assert.Equal(ModKey.Null.GetHashCode(), default(ModKey).GetHashCode());
        }

        #region Comparers
        #region Alphabetical
        [Fact]
        public void Comparer_Alphabetical_ByMaster()
        {
            ModKey k1 = ModKey.FromNameAndExtension("Oblivion.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Oblivion.esp");
            var compare = ModKey.AlphabeticalAndMastersFirst;
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByMasterGreater()
        {
            ModKey k1 = ModKey.FromNameAndExtension("Oblivion.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Oblivion.esp");
            var compare = ModKey.AlphabeticalAndMastersFirst;
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByName()
        {
            ModKey k1 = ModKey.FromNameAndExtension("Knights.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Oblivion.esm");
            var compare = ModKey.AlphabeticalAndMastersFirst;
            Assert.True(compare.Compare(k1, k2) < 0);
        }

        [Fact]
        public void Comparer_Alphabetical_ByNameGreater()
        {
            ModKey k1 = ModKey.FromNameAndExtension("Knights.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Oblivion.esm");
            var compare = ModKey.AlphabeticalAndMastersFirst;
            Assert.True(compare.Compare(k2, k1) > 0);
        }

        [Fact]
        public void Comparer_Alphabetical_Equal()
        {
            ModKey k1 = ModKey.FromNameAndExtension("Oblivion.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Oblivion.esm");
            var compare = ModKey.AlphabeticalAndMastersFirst;
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
            ModKey k1 = ModKey.FromNameAndExtension("Oblivion.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Knights.esm");
            var compare = ModKey.LoadOrderComparer(modKeys);
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
            ModKey k1 = ModKey.FromNameAndExtension("Oblivion.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Knights.esm");
            var compare = ModKey.LoadOrderComparer(modKeys);
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
            ModKey k1 = ModKey.FromNameAndExtension("MyMod.esm");
            ModKey k2 = ModKey.FromNameAndExtension("Oblivion.esm");
            var compare = ModKey.LoadOrderComparer(modKeys);
            Assert.Throws<ArgumentOutOfRangeException>(() => compare.Compare(k1, k2));
        }
        #endregion
        #endregion

        [Fact]
        public void PathCharsThrow()
        {
            Assert.Throws<ArgumentException>(() => new ModKey("/hello", ModType.Master));
            Assert.Throws<ArgumentException>(() => new ModKey("\\hello", ModType.Master));
        }

        #region ToString
        [Fact]
        public void ToString_Null()
        {
            Assert.Equal("Null", default(ModKey).ToString());
        }

        [Fact]
        public void ToString_Esp()
        {
            Assert.Equal("Oblivion.esp", new ModKey("Oblivion", type: ModType.Plugin).ToString());
        }

        [Fact]
        public void ToString_Esl()
        {
            Assert.Equal("Oblivion.esl", new ModKey("Oblivion", type: ModType.LightMaster).ToString());
        }

        [Fact]
        public void ToString_Esm()
        {
            Assert.Equal("Oblivion.esm", new ModKey("Oblivion", type: ModType.Master).ToString());
        }
        #endregion
    }
}
