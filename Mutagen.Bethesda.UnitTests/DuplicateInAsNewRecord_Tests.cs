using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class DuplicateInAsNewRecord_Tests
    {
        [Fact]
        public void Typical()
        {
            const string Name = "TEST";

            var npc = new Npc(Utility.Form1, SkyrimRelease.SkyrimSE)
            {
                Name = Name
            };
            var mod = new SkyrimMod(Utility.PluginModKey4, SkyrimRelease.SkyrimSE);
            var npc2 = mod.Npcs.DuplicateInAsNewRecord(npc);
            npc2.FormKey.ModKey.Should().Be(mod.ModKey);
            npc2.Name!.String.Should().Be(Name);
        }

        [Fact]
        public void Abstract()
        {
            const int Data = 15;

            Global glob = new GlobalInt(Utility.Form1, SkyrimRelease.SkyrimSE)
            {
                Data = Data
            };
            var mod = new SkyrimMod(Utility.PluginModKey4, SkyrimRelease.SkyrimSE);
            var glob2 = (GlobalInt)mod.Globals.DuplicateInAsNewRecord(glob);
            glob2.FormKey.ModKey.Should().Be(mod.ModKey);
            glob2.Data.Should().Be(Data);
        }

        [Fact]
        public void AbstractPassDecendant()
        {
            const int Data = 15;

            var glob = new GlobalInt(Utility.Form1, SkyrimRelease.SkyrimSE)
            {
                Data = Data
            };
            var mod = new SkyrimMod(Utility.PluginModKey4, SkyrimRelease.SkyrimSE);
            var glob2 = (GlobalInt)mod.Globals.DuplicateInAsNewRecord<Global, IGlobalIntGetter, IGlobalGetter>(glob);
            glob2.FormKey.ModKey.Should().Be(mod.ModKey);
            glob2.Data.Should().Be(Data);
        }
    }
}
