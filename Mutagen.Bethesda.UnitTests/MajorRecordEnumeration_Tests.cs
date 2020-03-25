using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MajorRecordEnumeration_Tests
    {
        [Fact]
        public void Empty()
        {
            var mod = new OblivionMod(Utility.ModKey);
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords());
            Assert.Empty(((IOblivionMod)mod).EnumerateMajorRecords());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<IMajorRecordCommon>());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<IMajorRecordCommonGetter>());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<INpc>());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<INpcGetter>());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<Npc>());
        }

        [Fact]
        public void EnumerateAll()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            mod.Ammo.AddNew();
            Assert.Equal(2, ((IOblivionModGetter)mod).EnumerateMajorRecords().Count());
            Assert.Equal(2, ((IOblivionMod)mod).EnumerateMajorRecords().Count());
        }

        [Fact]
        public void EnumerateAllViaGeneric()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            mod.Ammo.AddNew();
            Assert.Equal(2, ((IOblivionModGetter)mod).EnumerateMajorRecords<IMajorRecordCommon>().Count());
            Assert.Equal(2, ((IOblivionModGetter)mod).EnumerateMajorRecords<IMajorRecordCommonGetter>().Count());
        }

        [Fact]
        public void EnumerateSpecificType_Matched()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            mod.Ammo.AddNew();
            Assert.Single(((IOblivionModGetter)mod).EnumerateMajorRecords<INpc>());
            Assert.Single(((IOblivionModGetter)mod).EnumerateMajorRecords<INpcGetter>());
            Assert.Single(((IOblivionModGetter)mod).EnumerateMajorRecords<Npc>());
        }

        [Fact]
        public void EnumerateSpecificType_Unmatched()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<IAmmo>());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<IAmmoGetter>());
            Assert.Empty(((IOblivionModGetter)mod).EnumerateMajorRecords<Ammo>());
        }
    }
}
