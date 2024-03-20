using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
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
            ModKey = ModKeyOption.NoCheck
        }, fileSystem: fileSystem);
        using var reimport = OblivionMod.CreateFromBinaryOverlay(existingModPath, fileSystem: fileSystem);
        reimport.ModHeader.Stats.NextFormID.Should().Be(nextId + 1);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void DifferentModKeyExport(
        OblivionMod mod,  
        Npc npc,
        Race race,
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        var weap = mod.Weapons.AddNew(FormKey.Factory("123456:Skyrim.esm"));
        mod.ModKey.Should().NotBe(existingModPath.ModKey);
        npc.Race.SetTo(race);
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.CorrectToPath
        }, fileSystem: fileSystem);
        
        // Check FormKeys
        using var reimport = OblivionMod.CreateFromBinaryOverlay(existingModPath, fileSystem: fileSystem);
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
        using var stream = new MutagenBinaryReadStream(existingModPath, mod.GameRelease, fileSystem: fileSystem);
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
}