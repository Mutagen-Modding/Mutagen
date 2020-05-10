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
            Assert.True(ModKey.TryFactory("Oblivion.esm", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esm", modKey.FileName);
            Assert.True(modKey.Master);
        }

        [Fact]
        public void TryFactory_TypicalMod()
        {
            Assert.True(ModKey.TryFactory("Knights.esp", out var modKey));
            Assert.Equal("Knights", modKey.Name);
            Assert.Equal("Knights.esp", modKey.FileName);
            Assert.False(modKey.Master);
        }

        [Fact]
        public void TryFactory_ExtraPeriod()
        {
            Assert.True(ModKey.TryFactory("Obliv.ion.esm", out var modKey));
            Assert.Equal("Obliv.ion", modKey.Name);
            Assert.Equal("Obliv.ion.esm", modKey.FileName);
            Assert.True(modKey.Master);
        }

        [Fact]
        public void TryFactory_ImproperlyLengthedSuffix()
        {
            Assert.False(ModKey.TryFactory("Obliv.ion.esmz", out var modKey));
        }

        [Fact]
        public void TryFactory_ImproperSuffix()
        {
            Assert.False(ModKey.TryFactory("Obliv.ion.esn", out var modKey));
        }

        [Fact]
        public void TryFactory_TypicalDuplicate()
        {
            Assert.True(ModKey.TryFactory("Oblivion.esm", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esm", modKey.FileName);
            Assert.True(modKey.Master);
            Assert.True(ModKey.TryFactory("Oblivion.esm", out var modKey2));
            Assert.Equal("Oblivion", modKey2.Name);
            Assert.Equal("Oblivion.esm", modKey2.FileName);
            Assert.True(modKey2.Master);
            Assert.Same(modKey.Name, modKey2.Name);
        }

        [Fact]
        public void TryFactory_TypicalBothDuplicates()
        {
            Assert.True(ModKey.TryFactory("Oblivion.esm", out var masterKey));
            Assert.Equal("Oblivion", masterKey.Name);
            Assert.Equal("Oblivion.esm", masterKey.FileName);
            Assert.True(masterKey.Master);

            Assert.True(ModKey.TryFactory("Oblivion.esp", out var modKey));
            Assert.Equal("Oblivion", modKey.Name);
            Assert.Equal("Oblivion.esp", modKey.FileName);
            Assert.False(modKey.Master);

            Assert.True(ModKey.TryFactory("Oblivion.esm", out var masterKey2));
            Assert.Equal("Oblivion", masterKey2.Name);
            Assert.Equal("Oblivion.esm", masterKey2.FileName);
            Assert.True(masterKey2.Master);

            Assert.True(ModKey.TryFactory("Oblivion.esp", out var modKey2));
            Assert.Equal("Oblivion", modKey2.Name);
            Assert.Equal("Oblivion.esp", modKey2.FileName);
            Assert.False(modKey2.Master);

            Assert.Same(masterKey.Name, masterKey2.Name);
            Assert.Same(modKey.Name, modKey2.Name);
        }

        [Fact]
        public void CaseEquality()
        {
            ModKey modKey = new ModKey("Oblivion", true);
            ModKey modKey2 = new ModKey("OblivioN", true);
            Assert.Equal(modKey, modKey2);
            Assert.Equal(modKey.GetHashCode(), modKey2.GetHashCode());
        }

        [Fact]
        public void LookupCorrectness()
        {
            ModKey modKey = new ModKey("Oblivion", true);
            ModKey modKey2 = new ModKey("OblivioN", true);
            var set = new HashSet<ModKey>()
            {
                modKey
            };
            Assert.Contains(modKey2, set);
        }
        #endregion
    }
}
