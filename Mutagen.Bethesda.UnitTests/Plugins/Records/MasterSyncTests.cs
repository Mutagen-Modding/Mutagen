using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MasterSyncTests
{
    #region MasterFlagSync

    [Fact]
    public void MasterFlagSync_MasterThrow()
    {
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
        var masterMod = new OblivionMod(new ModKey("Test", ModType.Master));
        var masterPath = Path.Combine(folder.Dir.Path, "Test.esp");
        Assert.Throws<ArgumentException>(() =>
        {
            masterMod.BeginWrite
                .ToPath(masterPath)
                .WithNoLoadOrder()
                .WithModKeySync(ModKeyOption.ThrowIfMisaligned)
                .NoMastersListContentCheck()
                .Write();
        });
    }

    [Fact]
    public void MasterFlagSync_ChildThrow()
    {
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
        var masterMod = new OblivionMod(new ModKey("Test", ModType.Plugin));
        var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
        Assert.Throws<ArgumentException>(() =>
        {
            masterMod.BeginWrite
                .ToPath(masterPath)
                .WithNoLoadOrder()
                .WithModKeySync(ModKeyOption.ThrowIfMisaligned)
                .NoMastersListContentCheck()
                .Write();
        });
    }
    #endregion

    #region MasterListSync
    [Fact]
    public void MasterListSync_AddMissingToEmpty()
    {
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var knights = ModKey.FromNameAndExtension("Knights.esm");
        var other = ModKey.FromNameAndExtension("Other.esp");
        var mod = new OblivionMod(obliv);
        var otherNpc = new Npc(new FormKey(other, 0x123456));
        mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456)));
        mod.Npcs.RecordCache.Set(otherNpc);
        otherNpc.Race.FormKey = new FormKey(knights, 0x123456);
        var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .Write();
        using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
        Assert.Equal(2, reimport.MasterReferences.Count);
        Assert.Contains(knights, reimport.MasterReferences.Select(m => m.Master));
        Assert.Contains(other, reimport.MasterReferences.Select(m => m.Master));
    }

    [Fact]
    public void MasterListSync_RemoveUnnecessary()
    {
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
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
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .Write();
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
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var mod = new OblivionMod(obliv);
        var npc = mod.Npcs.AddNew();
        npc.Race.Clear();
        var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .Write();
        using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
        Assert.Empty(reimport.ModHeader.MasterReferences);
    }
    #endregion

    #region Master Order Sync
    [Fact]
    public void MasterOrderSync_Typical()
    {
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var knights = ModKey.FromNameAndExtension("Knights.esm");
        var other = ModKey.FromNameAndExtension("Other.esp");
        var mod = new OblivionMod(obliv);
        var knightsNpc = new Npc(new FormKey(knights, 0x123456));
        mod.Npcs.RecordCache.Set(knightsNpc);
        var otherNpc = new Npc(new FormKey(other, 0x123456));
        mod.Npcs.RecordCache.Set(otherNpc);
        var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .Write();
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
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var first = ModKey.FromNameAndExtension("First.esp");
        var second = ModKey.FromNameAndExtension("Second.esp");
        var mod = new OblivionMod(obliv);
        var secondNpc = new Npc(new FormKey(second, 0x123456));
        mod.Npcs.RecordCache.Set(secondNpc);
        var firstNpc = new Npc(new FormKey(first, 0x123456));
        mod.Npcs.RecordCache.Set(firstNpc);
        var modPath = Path.Combine(folder.Dir.Path, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithMastersListOrdering(MastersListOrderingOption.MastersFirst)
            .Write();
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
        Warmup.Init();
        using var folder = TestPathing.GetTempFolder(nameof(MasterSyncTests));
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
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithMastersListOrdering(loadOrder)
            .Write();
        using var reimport = OblivionMod.CreateFromBinaryOverlay(modPath);
        Assert.Equal(
            loadOrder,
            reimport.ModHeader.MasterReferences.Select(m => m.Master));
    }
    #endregion
}