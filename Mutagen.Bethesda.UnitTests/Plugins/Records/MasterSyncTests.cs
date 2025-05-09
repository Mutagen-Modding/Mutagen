using System.IO.Abstractions;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MasterSyncTests
{
    #region MasterFlagSync

    [Theory, MutagenAutoData]
    public void MasterFlagSync_MasterThrow(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var masterMod = new OblivionMod(new ModKey("Test", ModType.Master),
            OblivionRelease.Oblivion);
        var masterPath = Path.Combine(existingFolder, "Test.esp");
        Assert.Throws<ArgumentException>(() =>
        {
            masterMod.BeginWrite
                .ToPath(masterPath)
                .WithNoLoadOrder()
                .WithModKeySync(ModKeyOption.ThrowIfMisaligned)
                .NoMastersListContentCheck()
                .WithFileSystem(fileSystem)
                .Write();
        });
    }

    [Theory, MutagenAutoData]
    public void MasterFlagSync_ChildThrow(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var masterMod = new OblivionMod(new ModKey("Test", ModType.Plugin),
            OblivionRelease.Oblivion);
        var masterPath = Path.Combine(existingFolder, "Test.esm");
        Assert.Throws<ArgumentException>(() =>
        {
            masterMod.BeginWrite
                .ToPath(masterPath)
                .WithNoLoadOrder()
                .WithModKeySync(ModKeyOption.ThrowIfMisaligned)
                .NoMastersListContentCheck()
                .WithFileSystem(fileSystem)
                .Write();
        });
    }
    #endregion

    #region MasterListSync
    [Theory, MutagenAutoData]
    public void MasterListSync_AddMissingToEmpty(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var knights = ModKey.FromNameAndExtension("Knights.esm");
        var other = ModKey.FromNameAndExtension("Other.esp");
        var mod = new OblivionMod(obliv,
            OblivionRelease.Oblivion);
        var otherNpc = new Npc(new FormKey(other, 0x123456),
            OblivionRelease.Oblivion);
        mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456),
            OblivionRelease.Oblivion));
        mod.Npcs.RecordCache.Set(otherNpc);
        otherNpc.Race.FormKey = new FormKey(knights, 0x123456);
        var modPath = Path.Combine(existingFolder, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = OblivionMod.Create.FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        Assert.Equal(2, reimport.MasterReferences.Count);
        Assert.Contains(knights, reimport.MasterReferences.Select(m => m.Master));
        Assert.Contains(other, reimport.MasterReferences.Select(m => m.Master));
    }

    [Theory, MutagenAutoData]
    public void MasterListSync_RemoveUnnecessary(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var knights = ModKey.FromNameAndExtension("Knights.esm");
        var mod = new OblivionMod(obliv,
            OblivionRelease.Oblivion);
        mod.Potions.RecordCache.Set(new Potion(new FormKey(obliv, 0x123456),
            OblivionRelease.Oblivion));
        mod.Npcs.RecordCache.Set(new Npc(new FormKey(knights, 0x123456),
            OblivionRelease.Oblivion));
        mod.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = ModKey.FromNameAndExtension("Other.esp")
        });
        var modPath = Path.Combine(existingFolder, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = OblivionMod.Create
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        Assert.Equal(
            reimport.ModHeader.MasterReferences.Select(m => m.Master),
            new ModKey[]
            {
                knights,
            });
    }

    [Theory, MutagenAutoData]
    public void MasterListSync_SkipNulls(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var mod = new OblivionMod(obliv,
            OblivionRelease.Oblivion);
        var npc = mod.Npcs.AddNew();
        npc.Race.Clear();
        var modPath = Path.Combine(existingFolder, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = OblivionMod.Create
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        Assert.Empty(reimport.ModHeader.MasterReferences);
    }
    #endregion

    #region Master Order Sync
    [Theory, MutagenAutoData]
    public void MasterOrderSync_Typical(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var knights = ModKey.FromNameAndExtension("Knights.esm");
        var other = ModKey.FromNameAndExtension("Other.esp");
        var mod = new OblivionMod(obliv,
            OblivionRelease.Oblivion);
        var knightsNpc = new Npc(new FormKey(knights, 0x123456),
            OblivionRelease.Oblivion);
        mod.Npcs.RecordCache.Set(knightsNpc);
        var otherNpc = new Npc(new FormKey(other, 0x123456),
            OblivionRelease.Oblivion);
        mod.Npcs.RecordCache.Set(otherNpc);
        var modPath = Path.Combine(existingFolder, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = OblivionMod.Create
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        Assert.Equal(
            new ModKey[]
            {
                knights,
                other,
            },
            reimport.ModHeader.MasterReferences.Select(m => m.Master));
    }

    [Theory, MutagenAutoData]
    public void MasterOrderSync_EsmFirst(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var first = ModKey.FromNameAndExtension("First.esp");
        var second = ModKey.FromNameAndExtension("Second.esp");
        var mod = new OblivionMod(obliv,
            OblivionRelease.Oblivion);
        var secondNpc = new Npc(new FormKey(second, 0x123456),
            OblivionRelease.Oblivion);
        mod.Npcs.RecordCache.Set(secondNpc);
        var firstNpc = new Npc(new FormKey(first, 0x123456),
            OblivionRelease.Oblivion);
        mod.Npcs.RecordCache.Set(firstNpc);
        var modPath = Path.Combine(existingFolder, obliv.ToString());
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithMastersListOrdering(MastersListOrderingOption.MastersFirst)
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = OblivionMod.Create
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        Assert.Equal(
            new ModKey[]
            {
                first,
                second,
            },
            reimport.ModHeader.MasterReferences.Select(m => m.Master));
    }

    [Theory, MutagenAutoData]
    public void MasterOrderSync_ByLoadOrder(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var obliv = ModKey.FromNameAndExtension("Oblivion.esm");
        var esm = ModKey.FromNameAndExtension("First.esm");
        var esp = ModKey.FromNameAndExtension("Second.esp");
        var mod = new OblivionMod(obliv,
            OblivionRelease.Oblivion);
        var espNpc = new Npc(new FormKey(esp, 0x123456),
            OblivionRelease.Oblivion);
        mod.Npcs.RecordCache.Set(espNpc);
        var esmNpc = new Npc(new FormKey(esm, 0x123456),
            OblivionRelease.Oblivion);
        mod.Npcs.RecordCache.Set(esmNpc);
        var modPath = Path.Combine(existingFolder, obliv.ToString());
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
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = OblivionMod.Create
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        Assert.Equal(
            loadOrder,
            reimport.ModHeader.MasterReferences.Select(m => m.Master));
    }
    #endregion
}