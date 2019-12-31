using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class GroupCommon_Tests
    {
        [Fact]
        public void AddNew()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var rec = mod.NPCs.AddNew();
            Assert.Equal(1, mod.NPCs.Count);
            Assert.Same(mod.NPCs.Records.First(), rec);
        }

        [Fact]
        public void AddNew_DifferentForSecond()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var rec = mod.NPCs.AddNew();
            var rec2 = mod.NPCs.AddNew();
            Assert.Equal(2, mod.NPCs.Count);
            Assert.NotSame(rec, rec2);
        }
    }
}
