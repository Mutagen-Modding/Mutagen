using System.IO.Abstractions;
using FluentAssertions;
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
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class WriteOptionsTests
{
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void NextFormID(
        OblivionMod mod, 
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        var nextId = mod.ModHeader.Stats.NextFormID;
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.Should().Be(nextId);
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            FileSystem = fileSystem
        });
        using var reimport = OblivionMod.CreateFromBinaryOverlay(existingModPath,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.ModHeader.Stats.NextFormID.Should().Be(nextId + 1);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void DifferentModKeyExport(
        OblivionMod mod,
        Mutagen.Bethesda.Oblivion.Npc npc,
        Mutagen.Bethesda.Oblivion.Race race,
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        var weap = mod.Weapons.AddNew(FormKey.Factory("123456:Skyrim.esm"));
        mod.ModKey.Should().NotBe(existingModPath.ModKey);
        npc.Race.SetTo(race);
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.CorrectToPath,
            FileSystem = fileSystem
        });
        
        // Check FormKeys
        using var reimport = OblivionMod.CreateFromBinaryOverlay(existingModPath, 
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        var reimportWeapon = reimport.Weapons.First();
        reimportWeapon.FormKey.Should().Be(weap.FormKey);
        var reimportNpc = reimport.Npcs.First();
        reimportNpc.FormKey.ModKey.Should().Be(existingModPath.ModKey);
        reimportNpc.FormKey.ID.Should().Be(npc.FormKey.ID);
        var reimportRace = reimport.Races.First();
        reimportRace.FormKey.ModKey.Should().Be(existingModPath.ModKey);
        reimportRace.FormKey.ID.Should().Be(race.FormKey.ID);
        reimportNpc.Race.FormKey.Should().Be(reimportRace.FormKey);
        
        // Check OnDisk FormIDs
        using var stream = new MutagenBinaryReadStream(existingModPath, mod.GameRelease, loadOrder: null, fileSystem: fileSystem);
        stream.ReadModHeaderFrame();
        while (stream.TryReadGroup(out var group))
        {
            if (group.ContainedRecordType == RecordTypes.NPC_)
            {
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.Should().Be(1);
                recs[0].FormID.ID.Should().Be(reimportNpc.FormKey.ID);
                recs[0].FormID.ModIndex.ID.Should().Be(1);
            }
            else if (group.ContainedRecordType == RecordTypes.WEAP)
            {
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.Should().Be(1);
                recs[0].FormID.ID.Should().Be(reimportWeapon.FormKey.ID);
                recs[0].FormID.ModIndex.ID.Should().Be(0);
            }
            else if (group.ContainedRecordType == RecordTypes.RACE)
            {
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.Should().Be(1);
                recs[0].FormID.ID.Should().Be(reimportRace.FormKey.ID);
                recs[0].FormID.ModIndex.ID.Should().Be(1);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void DisallowedLowerRangeFormIDThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE, forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.Should().Be(1);
        Assert.Throws<LowerFormKeyRangeDisallowedException>(() =>
        {
            mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
            {
                ModKey = ModKeyOption.NoCheck,
                LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.Throw,
                FileSystem = fileSystem
            });
        });
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void DisallowedLowerRangeFormIDPlaceholderModKey(
        IFileSystem fileSystem,
        ModPath existingModPath,
        ModKey modKey)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE, forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.Should().Be(1);
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(modKey),
            FileSystem = fileSystem
        });
        using var reimport = SkyrimMod.CreateFromBinaryOverlay(existingModPath, SkyrimRelease.SkyrimSE, 
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should().Equal(modKey);
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void DisallowedLowerRangeFormIDPlaceholderLoadOrderEmptyThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE, forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.Should().Be(1);

        var lo = new LoadOrder<ModListing>();

        Assert.Throws<LowerFormKeyRangeDisallowedException>(() =>
        {
            mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
            {
                ModKey = ModKeyOption.NoCheck,
                LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo),
                FileSystem = fileSystem
            });
        });
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void DisallowedLowerRangeFormIDPlaceholderLoadOrder(
        IFileSystem fileSystem,
        ModPath existingModPath,
        ModKey modKey)
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE, forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.Should().Be(1);

        var lo = new LoadOrder<ModListing>()
        {
            new ModListing()
            {
                ModKey = modKey
            }
        };

        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo),
            FileSystem = fileSystem
        });
        using var reimport = SkyrimMod.CreateFromBinaryOverlay(existingModPath, SkyrimRelease.SkyrimSE,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should().Equal(modKey);
    }

    [Theory, MutagenAutoData]
    public void LightMasterFormIDCompactionThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield, forceUseLowerFormIDRanges: true);
        mod.IsLightMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFF));
        
        Assert.Throws<FormIDCompactionOutOfBoundsException>(() =>
        {
            mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
            {
                ModKey = ModKeyOption.NoCheck,
                FormIDCompaction = FormIDCompactionOption.Iterate,
                FileSystem = fileSystem
            });
        });
    }

    [Theory, MutagenAutoData]
    public void LightMasterFormIDCompactionNoCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield, forceUseLowerFormIDRanges: true);
        mod.IsLightMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFF));
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            FormIDCompaction = FormIDCompactionOption.NoCheck,
            FileSystem = fileSystem
        });
    }

    [Theory, MutagenAutoData]
    public void MediumMasterFormIDCompactionThrows(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield, forceUseLowerFormIDRanges: true);
        mod.IsLightMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFFF));
        
        Assert.Throws<FormIDCompactionOutOfBoundsException>(() =>
        {
            mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
            {
                ModKey = ModKeyOption.NoCheck,
                FormIDCompaction = FormIDCompactionOption.Iterate,
                FileSystem = fileSystem
            });
        });
    }

    [Theory, MutagenAutoData]
    public void MediumMasterFormIDCompactionNoCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield, forceUseLowerFormIDRanges: true);
        mod.IsLightMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFFF));
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            FormIDCompaction = FormIDCompactionOption.NoCheck,
            FileSystem = fileSystem
        });
    }

    [Theory, MutagenAutoData]
    public void MediumMasterFormIDCompactionCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield, forceUseLowerFormIDRanges: true);
        mod.IsMediumMaster = true;
        mod.Npcs.AddNew(new FormKey(mod.ModKey, 0x1FFF));
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            FormIDCompaction = FormIDCompactionOption.Iterate,
            FileSystem = fileSystem
        });
    }
}