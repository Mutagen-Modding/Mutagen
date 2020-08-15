using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public abstract class AMajorRecordEnumeration_Tests
    {
        protected abstract IOblivionModGetter ConvertMod(OblivionMod mod);

        public abstract bool Getter { get; }

        [Fact]
        public void Empty()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var conv = ConvertMod(mod);
            Assert.Empty(conv.EnumerateMajorRecords());
            Assert.Empty(conv.EnumerateMajorRecords<IMajorRecordCommon>());
            Assert.Empty(conv.EnumerateMajorRecords<IMajorRecordCommonGetter>());
            Assert.Empty(conv.EnumerateMajorRecords<INpc>());
            Assert.Empty(conv.EnumerateMajorRecords<INpcGetter>());
            Assert.Empty(conv.EnumerateMajorRecords<Npc>());
        }

        [Fact]
        public void EnumerateAll()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            mod.Ammunitions.AddNew();
            var conv = ConvertMod(mod);
            Assert.Equal(2, conv.EnumerateMajorRecords().Count());
        }

        [Fact]
        public void EnumerateAllViaGeneric()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            mod.Ammunitions.AddNew();
            var conv = ConvertMod(mod);
            Assert.Equal(Getter ? 0 : 2, conv.EnumerateMajorRecords<IMajorRecordCommon>().Count());
            Assert.Equal(2, conv.EnumerateMajorRecords<IMajorRecordCommonGetter>().Count());
        }

        [Fact]
        public void EnumerateSpecificType_Matched()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            mod.Ammunitions.AddNew();
            var conv = ConvertMod(mod);
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<INpc>().Count());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<Npc>().Count());
            Assert.Single(conv.EnumerateMajorRecords<INpcGetter>());
        }

        [Fact]
        public void EnumerateSpecificType_Unmatched()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            var conv = ConvertMod(mod);
            Assert.Empty(conv.EnumerateMajorRecords<IAmmunition>());
            Assert.Empty(conv.EnumerateMajorRecords<Ammunition>());
            Assert.Empty(conv.EnumerateMajorRecords<IAmmunitionGetter>());
        }

        [Fact]
        public void EnumerateLinkInterface()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Factions.AddNew();
            var conv = ConvertMod(mod);
            Assert.NotEmpty(conv.EnumerateMajorRecords<IFactionGetter>());
            Assert.NotEmpty(conv.EnumerateMajorRecords<IOwnerGetter>());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<IOwner>().Count());
        }

        [Fact]
        public void EnumerateDeepLinkInterface()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Cells.Records.Add(new CellBlock()
            {
                BlockNumber = 0,
                GroupType = GroupTypeEnum.InteriorCellBlock,
                LastModified = 4,
                SubBlocks =
                {
                    new CellSubBlock()
                    {
                        BlockNumber = 0,
                        GroupType = GroupTypeEnum.InteriorCellSubBlock,
                        LastModified = 4,
                        Cells =
                        {
                            new Cell(mod.GetNextFormKey())
                            {
                                Persistent =
                                {
                                    new PlacedNpc(mod.GetNextFormKey())
                                }
                            }
                        }
                    }
                }
            });
            var conv = ConvertMod(mod);
            Assert.NotEmpty(conv.EnumerateMajorRecords<ICellGetter>());
            Assert.NotEmpty(conv.EnumerateMajorRecords<IPlacedGetter>());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<IPlaced>().Count());
        }
    }

    public class MajorRecordEnumeration_Tests_Direct : AMajorRecordEnumeration_Tests
    {
        public override bool Getter => false;

        protected override IOblivionModGetter ConvertMod(OblivionMod mod)
        {
            return mod;
        }
    }

    public class MajorRecordEnumeration_Tests_Overlay : AMajorRecordEnumeration_Tests
    {
        public override bool Getter => true;

        protected override IOblivionModGetter ConvertMod(OblivionMod mod)
        {
            var stream = new MemoryTributary();
            mod.WriteToBinary(stream);
            stream.Position = 0;
            return OblivionMod.CreateFromBinaryOverlay(stream, mod.ModKey);
        }
    }
}
