using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: true));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.ThrowIfMisaligned,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(masterPath);
            Assert.True(reimport.ModHeader.Flags.HasFlag(ModHeader.HeaderFlag.Master));
            var childMod = new OblivionMod(new ModKey("Test", master: false));
            var childPath = Path.Combine(folder.Dir.Path, "Test.esp");
            childMod.WriteToBinary(childPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.ThrowIfMisaligned,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                });
            reimport = OblivionMod.CreateFromBinaryOverlay(childPath);
            Assert.False(reimport.ModHeader.Flags.HasFlag(ModHeader.HeaderFlag.Master));
        }

        [Fact]
        public void MasterFlagSync_MasterThrow()
        {
            WarmupOblivion.Init();
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: true));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esp");
            Assert.Throws<ArgumentException>(() =>
            {
                masterMod.WriteToBinary(masterPath,
                    new BinaryWriteParameters()
                    {
                        ModKeySync = BinaryWriteParameters.ModKeySyncOption.ThrowIfMisaligned,
                        MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                    });
            });
        }

        [Fact]
        public void MasterFlagSync_ChildThrow()
        {
            WarmupOblivion.Init();
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: false));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            Assert.Throws<ArgumentException>(() =>
            {
                masterMod.WriteToBinary(masterPath,
                    new BinaryWriteParameters()
                    {
                        ModKeySync = BinaryWriteParameters.ModKeySyncOption.ThrowIfMisaligned,
                        MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                    });
            });
        }

        [Fact]
        public void MasterFlagSync_MasterCorrect()
        {
            WarmupOblivion.Init();
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: true));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esp");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.CorrectToPath,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(masterPath);
            Assert.False(reimport.ModHeader.Flags.HasFlag(ModHeader.HeaderFlag.Master));
        }

        [Fact]
        public void MasterFlagSync_ChildCorrect()
        {
            WarmupOblivion.Init();
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: false));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.CorrectToPath,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(masterPath);
            Assert.True(reimport.ModHeader.Flags.HasFlag(ModHeader.HeaderFlag.Master));
        }
        #endregion

        #region MasterListSync
        [Fact]
        public void MasterListSync_AddMissingToEmpty()
        {
            WarmupOblivion.Init();
            using var folder = new TempFolder();
            var obliv = ModKey.Factory("Oblivion.esm");
            var knights = ModKey.Factory("Knights.esm");
            var other = ModKey.Factory("Other.esp");
            var mod = new OblivionMod(obliv);
            var otherNpc = new Npc(new FormKey(other, 0x123456));
            mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456)));
            mod.Npcs.RecordCache.Set(otherNpc);
            otherNpc.Race = new FormKey(knights, 0x123456);
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.Iterate,
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Equal(
                reimport.ModHeader.MasterReferences.Select(m => m.Master),
                new ModKey[]
                {
                    knights,
                    other,
                });
        }

        [Fact]
        public void MasterListSync_RemoveUnnecessary()
        {
            WarmupOblivion.Init();
            using var folder = new TempFolder();
            var obliv = ModKey.Factory("Oblivion.esm");
            var knights = ModKey.Factory("Knights.esm");
            var mod = new OblivionMod(obliv);
            mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456)));
            mod.Npcs.RecordCache.Set(new Npc(new FormKey(knights, 0x123456)));
            mod.ModHeader.MasterReferences.Add(new MasterReference()
            {
                Master = ModKey.Factory("Other.esp")
            });
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.Iterate,
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
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
            using var folder = new TempFolder();
            var obliv = ModKey.Factory("Oblivion.esm");
            var mod = new OblivionMod(obliv);
            var npc = mod.Npcs.AddNew();
            npc.Race = FormKey.Null;
            var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
            mod.WriteToBinary(modPath,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.Iterate,
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
            Assert.Empty(reimport.ModHeader.MasterReferences);
        }
        #endregion
    }
}
