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
            var mod = new OblivionMod(Utility.Dummy);
            var rec = mod.Npcs.AddNew();
            Assert.Equal(1, mod.Npcs.Count);
            Assert.Same(mod.Npcs.Records.First(), rec);
        }

        [Fact]
        public void AddNew_DifferentForSecond()
        {
            var mod = new OblivionMod(Utility.Dummy);
            var rec = mod.Npcs.AddNew();
            var rec2 = mod.Npcs.AddNew();
            Assert.Equal(2, mod.Npcs.Count);
            Assert.NotSame(rec, rec2);
        }
    }
}
