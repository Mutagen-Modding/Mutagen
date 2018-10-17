using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
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
        #endregion
    }
}
