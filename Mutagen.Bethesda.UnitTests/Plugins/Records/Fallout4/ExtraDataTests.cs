using Shouldly;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class ExtraDataTests
{
    [Theory, MutagenAutoData]
    public async Task NpcOwner_FromAnotherMod_ReadsCorrectly(
        IFileSystem fileSystem,
        DirectoryPath existingTempDir)
    {
        // Create master mod (Mod B) with an NPC
        var masterModKey = new ModKey("TestMaster", ModType.Master);
        var masterMod = new Fallout4Mod(masterModKey, Fallout4Release.Fallout4);
        var npc = masterMod.Npcs.AddNew("TestNPC");

        // Create plugin mod (Mod A) that references the NPC from master
        var pluginModKey = new ModKey("TestPlugin", ModType.Plugin);
        var pluginMod = new Fallout4Mod(pluginModKey, Fallout4Release.Fallout4);

        // Add a container with an entry that has ExtraData with NpcOwner
        var container = pluginMod.Containers.AddNew("TestContainer");
        container.Items = new Noggog.ExtendedList<ContainerEntry>();
        var entry = new ContainerEntry
        {
            Item = new ContainerItem
            {
                Item = new FormLink<IItemGetter>(npc.FormKey), // Just reference something for the item
                Count = 1
            },
            Data = new ExtraData
            {
                Owner = new NpcOwner
                {
                    Npc = new FormLink<INpcGetter>(npc.FormKey),
                    Global = new FormLink<IGlobalGetter>()
                }
            }
        };
        container.Items.Add(entry);

        // Write both mods to temp files
        var masterPath = Path.Combine(existingTempDir, masterModKey.FileName);
        var pluginPath = Path.Combine(existingTempDir, pluginModKey.FileName);

        await masterMod.BeginWrite
            .ToPath(masterPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .WriteAsync();

        await pluginMod.BeginWrite
            .ToPath(pluginPath)
            .WithLoadOrder(masterMod)
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .WriteAsync();

        // Read the plugin mod with the master in load order
        using var readMod = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(pluginPath)
            .WithLoadOrder(masterModKey, pluginModKey)
            .WithDataFolder(existingTempDir)
            .WithFileSystem(fileSystem)
            .Construct();

        // Get the container and check the owner type
        var readContainer = readMod.Containers.First();
        var readEntry = readContainer.Items.First();
        var owner = readEntry.Data?.Owner;

        // With the load order, this should now correctly resolve to NpcOwner
        owner.ShouldNotBeNull();
        owner.ShouldBeOfType<NpcOwner>($"Expected NpcOwner but got {owner.GetType().Name}");

        var npcOwner = owner as NpcOwner;
        npcOwner.ShouldNotBeNull();
        npcOwner.Npc.FormKey.ShouldBe(npc.FormKey);
    }

    [Theory, MutagenAutoData]
    public async Task FactionOwner_FromAnotherMod_ReadsCorrectly(
        IFileSystem fileSystem,
        DirectoryPath existingTempDir)
    {
        // Create master mod (Mod B) with a Faction
        var masterModKey = new ModKey("TestMaster", ModType.Master);
        var masterMod = new Fallout4Mod(masterModKey, Fallout4Release.Fallout4);
        var faction = masterMod.Factions.AddNew("TestFaction");

        // Create plugin mod (Mod A) that references the Faction from master
        var pluginModKey = new ModKey("TestPlugin", ModType.Plugin);
        var pluginMod = new Fallout4Mod(pluginModKey, Fallout4Release.Fallout4);

        // Add a container with an entry that has ExtraData with FactionOwner
        var container = pluginMod.Containers.AddNew("TestContainer");
        container.Items = new Noggog.ExtendedList<ContainerEntry>();
        var entry = new ContainerEntry
        {
            Item = new ContainerItem
            {
                Item = new FormLink<IItemGetter>(faction.FormKey), // Just reference something for the item
                Count = 1
            },
            Data = new ExtraData
            {
                Owner = new FactionOwner
                {
                    Faction = new FormLink<IFactionGetter>(faction.FormKey),
                    RequiredRank = 5
                }
            }
        };
        container.Items.Add(entry);

        // Write both mods to temp files
        var masterPath = Path.Combine(existingTempDir, masterModKey.FileName);
        var pluginPath = Path.Combine(existingTempDir, pluginModKey.FileName);

        await masterMod.BeginWrite
            .ToPath(masterPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .WriteAsync();

        await pluginMod.BeginWrite
            .ToPath(pluginPath)
            .WithLoadOrder(masterMod)
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .WriteAsync();

        // Read the plugin mod with the master in load order
        using var readMod = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(pluginPath)
            .WithLoadOrder(masterModKey, pluginModKey)
            .WithDataFolder(existingTempDir)
            .WithFileSystem(fileSystem)
            .Construct();

        // Get the container and check the owner type
        var readContainer = readMod.Containers.First();
        var readEntry = readContainer.Items.First();
        var owner = readEntry.Data?.Owner;

        // With the load order, this should now correctly resolve to FactionOwner
        owner.ShouldNotBeNull();
        owner.ShouldBeOfType<FactionOwner>($"Expected FactionOwner but got {owner.GetType().Name}");

        var factionOwner = owner as FactionOwner;
        factionOwner.ShouldNotBeNull();
        factionOwner.Faction.FormKey.ShouldBe(faction.FormKey);
        factionOwner.RequiredRank.ShouldBe(5);
    }
}
