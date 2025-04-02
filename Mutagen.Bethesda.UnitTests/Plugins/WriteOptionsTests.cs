using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class WriteOptionsTests
{
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public async Task NextFormID(
        OblivionMod mod,
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        var nextId = mod.ModHeader.Stats.NextFormID;
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.ShouldBe(nextId);
        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .WriteAsync();
        using var reimport = OblivionMod.Create
            .FromPath(existingModPath)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.ModHeader.Stats.NextFormID.ShouldBe(nextId + 1);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public async Task DifferentModKeyExport(
        OblivionMod mod,
        Mutagen.Bethesda.Oblivion.Npc npc,
        Mutagen.Bethesda.Oblivion.Race race,
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        var weap = mod.Weapons.AddNew(FormKey.Factory("123456:Skyrim.esm"));
        mod.ModKey.ShouldNotBe(existingModPath.ModKey);
        npc.Race.SetTo(race);
        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .WithModKeySync(ModKeyOption.CorrectToPath)
            .WithFileSystem(fileSystem)
            .WriteAsync();

        // Check FormKeys
        using var reimport = OblivionMod.Create
            .FromPath(existingModPath)
            .WithFileSystem(fileSystem)
            .Construct();
        var reimportWeapon = reimport.Weapons.First();
        reimportWeapon.FormKey.ShouldBe(weap.FormKey);
        var reimportNpc = reimport.Npcs.First();
        reimportNpc.FormKey.ModKey.ShouldBe(existingModPath.ModKey);
        reimportNpc.FormKey.ID.ShouldBe(npc.FormKey.ID);
        var reimportRace = reimport.Races.First();
        reimportRace.FormKey.ModKey.ShouldBe(existingModPath.ModKey);
        reimportRace.FormKey.ID.ShouldBe(race.FormKey.ID);
        reimportNpc.Race.FormKey.ShouldBe(reimportRace.FormKey);

        // Check OnDisk FormIDs
        using var stream =
            new MutagenBinaryReadStream(existingModPath, mod.GameRelease, masterFlagLookup: null, fileSystem: fileSystem);
        stream.ReadModHeaderFrame();
        while (stream.TryReadGroup(out var group))
        {
            if (group.ContainedRecordType == RecordTypes.NPC_)
            {
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.ShouldBe(1);
                recs[0].FormID.FullId.ShouldBe(reimportNpc.FormKey.ID);
                recs[0].FormID.FullMasterIndex.ShouldEqual(1);
            }
            else if (group.ContainedRecordType == RecordTypes.WEAP)
            {
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.ShouldBe(1);
                recs[0].FormID.FullId.ShouldBe(reimportWeapon.FormKey.ID);
                recs[0].FormID.FullMasterIndex.ShouldEqual(0);
            }
            else if (group.ContainedRecordType == RecordTypes.RACE)
            {
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.ShouldBe(1);
                recs[0].FormID.FullId.ShouldBe(reimportRace.FormKey.ID);
                recs[0].FormID.FullMasterIndex.ShouldEqual(1);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public async Task DisallowedLowerRangeFormIDThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE,
            forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.ShouldEqual(0);
        await Assert.ThrowsAsync<LowerFormKeyRangeDisallowedException>(async () =>
        {
            await mod.BeginWrite
                .ToPath(existingModPath)
                .WithNoLoadOrder()
                .NoModKeySync()
                .ThrowIfLowerRangeDisallowed()
                .WithFileSystem(fileSystem)
                .WriteAsync();
        });
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public async Task DisallowedLowerRangeFormIDPlaceholderModKey(
        IFileSystem fileSystem,
        ModPath existingModPath,
        ModKey modKey)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE,
            forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.ShouldEqual(0);
        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithPlaceholderMasterIfLowerRangeDisallowed(modKey)
            .WithFileSystem(fileSystem)
            .WriteAsync();
        using var reimport = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(existingModPath)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).ShouldEqual(modKey);
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public async Task DisallowedLowerRangeFormIDPlaceholderLoadOrderEmptyThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE,
            forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.ShouldEqual(0);

        var lo = new LoadOrder<ModListing>();

        await Assert.ThrowsAsync<LowerFormKeyRangeDisallowedException>(async () =>
        {
            await mod.BeginWrite
                .ToPath(existingModPath)
                .WithNoLoadOrder()
                .NoModKeySync()
                .WithPlaceholderMasterIfLowerRangeDisallowed(lo)
                .WithFileSystem(fileSystem)
                .WriteAsync();
        });
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public async Task DisallowedLowerRangeFormIDPlaceholderLoadOrder(
        IFileSystem fileSystem,
        ModPath existingModPath,
        ModKey modKey)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE,
            forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.ShouldEqual(0);

        var lo = new LoadOrder<ModListing>()
        {
            new ModListing()
            {
                ModKey = modKey
            }
        };

        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithPlaceholderMasterIfLowerRangeDisallowed(lo)
            .WithFileSystem(fileSystem)
            .WriteAsync();
        using var reimport = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(existingModPath)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).ShouldEqual(modKey);
    }

    [Theory, MutagenAutoData]
    public async Task LightMasterFormIDCompactionThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield,
            forceUseLowerFormIDRanges: true);
        mod.IsSmallMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFF));

        await Assert.ThrowsAsync<FormIDCompactionOutOfBoundsException>(async () =>
        {
            await mod.BeginWrite
                .ToPath(existingModPath)
                .WithNoLoadOrder()
                .NoModKeySync()
                .WithFormIDCompactnessCheck(FormIDCompactionOption.Iterate)
                .WithFileSystem(fileSystem)
                .WriteAsync();
        });
    }

    [Theory, MutagenAutoData]
    public async Task LightMasterFormIDCompactionNoCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield,
            forceUseLowerFormIDRanges: true);
        mod.IsSmallMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFF));
        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .NoFormIDCompactnessCheck()
            .WithFileSystem(fileSystem)
            .WriteAsync();
    }

    [Theory, MutagenAutoData]
    public async Task MediumMasterFormIDCompactionThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield,
            forceUseLowerFormIDRanges: true);
        mod.IsSmallMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFFF));

        await Assert.ThrowsAsync<FormIDCompactionOutOfBoundsException>(async () =>
        {
            await mod.BeginWrite
                .ToPath(existingModPath)
                .WithNoLoadOrder()
                .NoModKeySync()
                .WithFormIDCompactnessCheck(FormIDCompactionOption.Iterate)
                .WithFileSystem(fileSystem)
                .WriteAsync();
        });
    }

    [Theory, MutagenAutoData]
    public async Task MediumMasterFormIDCompactionNoCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield,
            forceUseLowerFormIDRanges: true);
        mod.IsSmallMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFFF));
        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .NoFormIDCompactnessCheck()
            .WithFileSystem(fileSystem)
            .WriteAsync();
    }

    [Theory, MutagenAutoData]
    public async Task MediumMasterFormIDCompactionCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield,
            forceUseLowerFormIDRanges: true);
        mod.IsMediumMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFF));
        await mod.BeginWrite
            .ToPath(existingModPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFormIDCompactnessCheck(FormIDCompactionOption.Iterate)
            .WithFileSystem(fileSystem)
            .WriteAsync();
    }
    
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public async Task NoSyncOverridesRespected(
        OblivionMod mod,
        FormKey fk,
        FormKey fk2,
        FormKey fk3,
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        mod.Npcs.AddNew();
        mod.Npcs.AddNew(fk);
        mod.Npcs.AddNew(fk2);
        mod.Npcs.AddNew(fk3);
        await mod.BeginWrite
            .ToPath(Path.Combine(existingDir, mod.ModKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .WithMastersListOrdering(MastersListOrderingOption.NoCheck)
            .WithFileSystem(fileSystem)
            .WriteAsync();
    }

}