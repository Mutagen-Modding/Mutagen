using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class OverrideIteration_Tests
    {
        [Fact]
        public void WinningOverrides_Typical()
        {
            var master = new OblivionMod(new ModKey("Base", ModType.Master));
            var baseNpc = master.Npcs.AddNew();
            var otherMasterNpc = master.Npcs.AddNew();
            var plugin = new OblivionMod(new ModKey("Plugin", ModType.Plugin));
            var pluginNpc = plugin.Npcs.AddNew();
            var overrideNpc = (Npc)baseNpc.DeepCopy();
            plugin.Npcs.RecordCache.Set(overrideNpc);
            overrideNpc.Name = "Override";

            var winningOverrides = plugin.AsEnumerable().And(master)
                .WinningOverrides<Npc>()
                .ToDictionary(n => n.FormKey);

            Assert.Equal(3, winningOverrides.Count);
            Assert.Same(otherMasterNpc, winningOverrides[otherMasterNpc.FormKey]);
            Assert.Same(pluginNpc, winningOverrides[pluginNpc.FormKey]);
            Assert.Equal(overrideNpc.FormKey, baseNpc.FormKey);
            Assert.Same(overrideNpc, winningOverrides[baseNpc.FormKey]);
        }
    }
}
