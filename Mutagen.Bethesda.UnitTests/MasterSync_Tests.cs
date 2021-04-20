using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Records.Binary;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MasterSync_Tests
    {
        #region MasterFlagSync
        [Fact]
        public void MasterFlagSync_Correct()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var masterMod = new OblivionMod(new ModKey("Test", ModType.Master));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.ThrowIfMisaligned,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.NoCheck,
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(masterPath);
            Assert.True(reimport.ModHeader.Flags.HasFlag(OblivionModHeader.HeaderFlag.Master));
            var childMod = new OblivionMod(new ModKey("Test", ModType.Plugin));
            var childPath = Path.Combine(folder.Dir.Path, "Test.esp");
            childMod.WriteToBinary(childPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.ThrowIfMisaligned,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.NoCheck,
                });
            using var reimport2 = OblivionMod.CreateFromBinaryOverlay(childPath);
            Assert.False(reimport2.ModHeader.Flags.HasFlag(OblivionModHeader.HeaderFlag.Master));
        }

        [Fact]
        public void MasterFlagSync_MasterThrow()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var masterMod = new OblivionMod(new ModKey("Test", ModType.Master));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esp");
            Assert.Throws<ArgumentException>(() =>
            {
                masterMod.WriteToBinary(masterPath,
                    new BinaryWriteParameters()
                    {
                        ModKey = BinaryWriteParameters.ModKeyOption.ThrowIfMisaligned,
                        MastersListContent = BinaryWriteParameters.MastersListContentOption.NoCheck,
                    });
            });
        }

        [Fact]
        public void MasterFlagSync_ChildThrow()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var masterMod = new OblivionMod(new ModKey("Test", ModType.Plugin));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            Assert.Throws<ArgumentException>(() =>
            {
                masterMod.WriteToBinary(masterPath,
                    new BinaryWriteParameters()
                    {
                        ModKey = BinaryWriteParameters.ModKeyOption.ThrowIfMisaligned,
                        MastersListContent = BinaryWriteParameters.MastersListContentOption.NoCheck,
                    });
            });
        }
        #endregion

        #region MasterListSync
        [Fact]
        public void MasterListSync_AddMissingToEmpty()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
            var knights = ModKey.FromNameAndExtension("Knights.esm");
            var other = ModKey.FromNameAndExtension("Other.esp");
            var mod = new OblivionMod(obliv);
            var otherNpc = new Npc(new FormKey(other, 0x123456));
            mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456)));
            mod.Npcs.RecordCache.Set(otherNpc);
            otherNpc.Race.FormKey = new FormKey(knights, 0x123456);
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Equal(2, reimport.MasterReferences.Count);
            Assert.Contains(knights, reimport.MasterReferences.Select(m => m.Master));
            Assert.Contains(other, reimport.MasterReferences.Select(m => m.Master));
        }

        [Fact]
        public void MasterListSync_RemoveUnnecessary()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
            var knights = ModKey.FromNameAndExtension("Knights.esm");
            var mod = new OblivionMod(obliv);
            mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456)));
            mod.Npcs.RecordCache.Set(new Npc(new FormKey(knights, 0x123456)));
            mod.ModHeader.MasterReferences.Add(new MasterReference()
            {
                Master = ModKey.FromNameAndExtension("Other.esp")
            });
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Equal(
                reimport.ModHeader.MasterReferences.Select(m => m.Master),
                new ModKey[]
                {
                    knights,
                });
        }

        [Fact]
        public void MasterListSync_SkipNulls()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
            var mod = new OblivionMod(obliv);
            var npc = mod.Npcs.AddNew();
            npc.Race.Clear();
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Empty(reimport.ModHeader.MasterReferences);
        }
        #endregion

        #region Master Order Sync
        [Fact]
        public void MasterOrderSync_Typical()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
            var knights = ModKey.FromNameAndExtension("Knights.esm");
            var other = ModKey.FromNameAndExtension("Other.esp");
            var mod = new OblivionMod(obliv);
            var knightsNpc = new Npc(new FormKey(knights, 0x123456));
            mod.Npcs.RecordCache.Set(knightsNpc);
            var otherNpc = new Npc(new FormKey(other, 0x123456));
            mod.Npcs.RecordCache.Set(otherNpc);
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Equal(
                new ModKey[]
                {
                    knights,
                    other,
                },
                reimport.ModHeader.MasterReferences.Select(m => m.Master));
        }

        [Fact]
        public void MasterOrderSync_EsmFirst()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
            var first = ModKey.FromNameAndExtension("First.esp");
            var second = ModKey.FromNameAndExtension("Second.esp");
            var mod = new OblivionMod(obliv);
            var secondNpc = new Npc(new FormKey(second, 0x123456));
            mod.Npcs.RecordCache.Set(secondNpc);
            var firstNpc = new Npc(new FormKey(first, 0x123456));
            mod.Npcs.RecordCache.Set(firstNpc);
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                    MastersListOrdering = BinaryWriteParameters.MastersListOrderingOption.MastersFirst,
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Equal(
                new ModKey[]
                {
                    first,
                    second,
                },
                reimport.ModHeader.MasterReferences.Select(m => m.Master));
        }

        [Fact]
        public void MasterOrderSync_ByLoadOrder()
        {
            WarmupOblivion.Init();
            using var folder = Utility.GetTempFolder(nameof(MasterSync_Tests));
            var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
            var esm = ModKey.FromNameAndExtension("First.esm");
            var esp = ModKey.FromNameAndExtension("Second.esp");
            var mod = new OblivionMod(obliv);
            var espNpc = new Npc(new FormKey(esp, 0x123456));
            mod.Npcs.RecordCache.Set(espNpc);
            var esmNpc = new Npc(new FormKey(esm, 0x123456));
            mod.Npcs.RecordCache.Set(esmNpc);
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            var loadOrder = new ModKey[]
            {
                esm,
                esp,
            };
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                    MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                    MastersListOrdering = new BinaryWriteParameters.MastersListOrderingByLoadOrder(loadOrder)
                });
            using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Equal(
                loadOrder,
                reimport.ModHeader.MasterReferences.Select(m => m.Master));
        }
        #endregion
    }
}
