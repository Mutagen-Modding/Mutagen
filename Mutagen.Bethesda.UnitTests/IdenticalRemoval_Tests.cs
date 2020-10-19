using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class IdenticalRemoval_Tests
    {
        [Fact]
        public void NoRemovals()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc = mod.Npcs.AddNew();
            var mod2 = new OblivionMod(Utility.ModKey2);
            mod.RemoveIdentical(mod2.ToImmutableLinkCache());
            Assert.Single(mod.Npcs);
        }

        [Fact]
        public void SelfRemoval()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc = mod.Npcs.AddNew();
            mod.RemoveIdentical(mod.ToImmutableLinkCache());
            Assert.Empty(mod.Npcs);
        }
    }
}
