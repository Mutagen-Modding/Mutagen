﻿using System.Buffers.Binary;
using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class SeparatedMastersTesting
{
    MajorRecordFrame ReadFrame(MutagenBinaryReadStream stream, RecordLocatorResults locs, FormKey formKey)
    {
        var loc = locs.GetRecord(formKey);
        stream.Position = loc.Location.Min;
        return stream.ReadMajorRecord();
    }

    [Theory, MutagenModAutoData]
    public void NonSeparated(
        IFileSystem fileSystem,
        DirectoryPath existingDir,
        ModKey originatingKey,
        ModKey modAKey,
        ModKey modBKey)
    {
        var originating = new SkyrimMod(originatingKey, SkyrimRelease.SkyrimSE);
        var modA = new SkyrimMod(modAKey, SkyrimRelease.SkyrimSE);
        var modB = new SkyrimMod(modBKey, SkyrimRelease.SkyrimSE);
        
        var modANpc = modA.Npcs.AddNew();
        var modBWeapon = modB.Weapons.AddNew();

        originating.Npcs.GetOrAddAsOverride(modANpc);
        originating.Weapons.GetOrAddAsOverride(modBWeapon);
        
        var originatingNpc = originating.Npcs.AddNew();
        originatingNpc.Template.SetTo(modANpc);
        var originatingWeapon = originating.Weapons.AddNew();
        originatingWeapon.Template.SetTo(modBWeapon);

        var lo = new LoadOrder<IModFlagsGetter>(new[]
        {
            MastersTestUtil.GetFlags(modAKey, MasterStyle.Full),
            MastersTestUtil.GetFlags(modBKey, MasterStyle.Full),
        });

        var modPath = Path.Combine(existingDir, originatingKey.FileName);
        originating.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(lo)
            .WithFileSystem(fileSystem)
            .Write();

        var locs = RecordLocator.GetLocations(modPath, GameRelease.SkyrimSE,
            loadOrder: lo,
            fileSystem: fileSystem);
        locs.ListedRecords.Select(x => x.Value.FormKey).ShouldEqual(
            modBWeapon.FormKey, 
            originatingWeapon.FormKey,
            modANpc.FormKey,
            originatingNpc.FormKey);
        {
            var meta = ParsingMeta.Factory(new BinaryReadParameters()
            {
                FileSystem = fileSystem
            }, GameRelease.SkyrimSE, modPath);
            using var stream = new MutagenBinaryReadStream(modPath, meta);
            var modANpcFrame = ReadFrame(stream, locs, modANpc.FormKey);
            modANpcFrame.FormID.Raw.ShouldBe(FormID.Factory(MasterStyle.Full, 0, modANpc.FormKey.ID).Raw);
            var modBWeaponFrame = ReadFrame(stream, locs, modBWeapon.FormKey);
            modBWeaponFrame.FormID.Raw.ShouldBe(FormID.Factory(MasterStyle.Full, 1, modBWeapon.FormKey.ID).Raw);
            var origNpcFrame = ReadFrame(stream, locs, originatingNpc.FormKey);
            origNpcFrame.FormID.Raw.ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingNpc.FormKey.ID).Raw);
            var npcTplt = origNpcFrame.FindSubrecord(RecordTypes.TPLT);
            npcTplt.AsFormID().ShouldBe(FormID.Factory(MasterStyle.Full, 0, modANpc.FormKey.ID));
            var origWeaponFrame = ReadFrame(stream, locs, originatingWeapon.FormKey);
            origWeaponFrame.FormID.Raw.ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingWeapon.FormKey.ID).Raw);
            var weaponCnam = origWeaponFrame.FindSubrecord(RecordTypes.CNAM);
            weaponCnam.AsFormID().ShouldBe(FormID.Factory(MasterStyle.Full, 1, modBWeapon.FormKey.ID));
        }

        using var reimport = SkyrimMod.CreateFromBinaryOverlay(modPath, SkyrimRelease.SkyrimSE, new BinaryReadParameters()
        {
            FileSystem = fileSystem,
            MasterFlagsLookup = lo
        });
        reimport.Npcs.Select(x => x.FormKey).ShouldEqual(modANpc.FormKey, originatingNpc.FormKey);
        var reimportNpc = reimport.Npcs[originatingNpc.FormKey];
        reimportNpc.Template.FormKey.ShouldBe(modANpc.FormKey);
        reimport.Weapons.Select(x => x.FormKey).ShouldEqual(modBWeapon.FormKey, originatingWeapon.FormKey);
        var reimportWeapon = reimport.Weapons[originatingWeapon.FormKey];
        reimportWeapon.Template.FormKey.ShouldBe(modBWeapon.FormKey);
    }

    private RecordType waim = new RecordType("WAIM");
    
    [Theory, MutagenAutoData]
    public void Separated(
        IFileSystem fileSystem,
        DirectoryPath existingDir,
        ModKey originatingKey,
        ModKey modAKey,
        ModKey modBKey,
        ModKey lightAKey,
        ModKey lightBKey,
        ModKey mediumAKey,
        ModKey mediumBKey)
    {
        var originating = new StarfieldMod(originatingKey, StarfieldRelease.Starfield);
        var modA = new StarfieldMod(modAKey, StarfieldRelease.Starfield);
        var modB = new StarfieldMod(modBKey, StarfieldRelease.Starfield);
        var lightModA = new StarfieldMod(lightAKey, StarfieldRelease.Starfield);
        var lightModB = new StarfieldMod(lightBKey, StarfieldRelease.Starfield);
        var mediumModA = new StarfieldMod(mediumAKey, StarfieldRelease.Starfield);
        var mediumModB = new StarfieldMod(mediumBKey, StarfieldRelease.Starfield);
        
        var modANpc = modA.Npcs.AddNew();
        var modBAim = modB.AimModels.AddNew();
        var lightModANpc = lightModA.Npcs.AddNew();
        var lightModBAim = lightModB.AimModels.AddNew();
        var mediumModANpc = mediumModA.Npcs.AddNew();
        var mediumModBAim = mediumModB.AimModels.AddNew();

        originating.Npcs.GetOrAddAsOverride(modANpc);
        originating.AimModels.GetOrAddAsOverride(modBAim);
        originating.Npcs.GetOrAddAsOverride(lightModANpc);
        originating.AimModels.GetOrAddAsOverride(lightModBAim);
        originating.Npcs.GetOrAddAsOverride(mediumModANpc);
        originating.AimModels.GetOrAddAsOverride(mediumModBAim);
        
        var originatingNpc = originating.Npcs.AddNew();
        originatingNpc.InheritsSoundsFrom.SetTo(modANpc);
        var originatingWeapon = originating.Weapons.AddNew();
        originatingWeapon.AimModel.SetTo(modBAim);
        var originatingLightNpc = originating.Npcs.AddNew();
        originatingLightNpc.InheritsSoundsFrom.SetTo(lightModANpc);
        var originatingLightWeapon = originating.Weapons.AddNew();
        originatingLightWeapon.AimModel.SetTo(lightModBAim);
        var originatingMediumNpc = originating.Npcs.AddNew();
        originatingMediumNpc.InheritsSoundsFrom.SetTo(mediumModANpc);
        var originatingMediumWeapon = originating.Weapons.AddNew();
        originatingMediumWeapon.AimModel.SetTo(mediumModBAim);

        var lo = new LoadOrder<IModFlagsGetter>(new[]
        {
            MastersTestUtil.GetFlags(modAKey, MasterStyle.Full),
            MastersTestUtil.GetFlags(lightAKey, MasterStyle.Small),
            MastersTestUtil.GetFlags(mediumAKey, MasterStyle.Medium),
            MastersTestUtil.GetFlags(modBKey, MasterStyle.Full),
            MastersTestUtil.GetFlags(lightBKey, MasterStyle.Small),
            MastersTestUtil.GetFlags(mediumBKey, MasterStyle.Medium),
        });

        var modPath = Path.Combine(existingDir, originatingKey.FileName);
        originating.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(lo)
            .WithFileSystem(fileSystem)
            .Write();

        var locs = RecordLocator.GetLocations(modPath, GameRelease.Starfield,
            loadOrder: lo,
            fileSystem: fileSystem);
        locs.ListedRecords.Select(x => x.Value.FormKey).ShouldEqual(
            originatingWeapon.FormKey,
            originatingLightWeapon.FormKey,
            originatingMediumWeapon.FormKey,
            modANpc.FormKey,
            lightModANpc.FormKey,
            mediumModANpc.FormKey,
            originatingNpc.FormKey,
            originatingLightNpc.FormKey,
            originatingMediumNpc.FormKey,
            modBAim.FormKey, 
            lightModBAim.FormKey, 
            mediumModBAim.FormKey);
        {
            var meta = ParsingMeta.FactoryNoMasters(new BinaryReadParameters()
            {
                FileSystem = fileSystem
            }, GameRelease.Starfield, modPath);
            using var stream = new MutagenBinaryReadStream(modPath, meta);
            var modANpcFrame = ReadFrame(stream, locs, modANpc.FormKey);
            modANpcFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 0, modANpc.FormKey.ID).Raw);
            var modBAimFrame = ReadFrame(stream, locs, modBAim.FormKey);
            modBAimFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 1, modBAim.FormKey.ID).Raw);
            var lightModANpcFrame = ReadFrame(stream, locs, lightModANpc.FormKey);
            lightModANpcFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Small, 0, lightModANpc.FormKey.ID).Raw);
            var lightModBAimFrame = ReadFrame(stream, locs, lightModBAim.FormKey);
            lightModBAimFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Small, 1, lightModBAim.FormKey.ID).Raw);
            var mediumModANpcFrame = ReadFrame(stream, locs, mediumModANpc.FormKey);
            mediumModANpcFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Medium, 0, mediumModANpc.FormKey.ID).Raw);
            var mediumModBAimFrame = ReadFrame(stream, locs, mediumModBAim.FormKey);
            mediumModBAimFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Medium, 1, mediumModBAim.FormKey.ID).Raw);
            
            
            var origNpcFrame = ReadFrame(stream, locs, originatingNpc.FormKey);
            origNpcFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingNpc.FormKey.ID).Raw);
            var npcCscr = origNpcFrame.FindSubrecord(RecordTypes.CSCR);
            npcCscr.AsFormID()
                .ShouldBe(FormID.Factory(MasterStyle.Full, 0, modANpc.FormKey.ID));
            var origWeaponFrame = ReadFrame(stream, locs, originatingWeapon.FormKey);
            origWeaponFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingWeapon.FormKey.ID).Raw);
            var weaponWaim = origWeaponFrame.FindSubrecord(waim);
            new FormID(BinaryPrimitives.ReadUInt32LittleEndian(weaponWaim.Content.Slice(8)))
                .ShouldBe(FormID.Factory(MasterStyle.Full, 1, modBAim.FormKey.ID));
            
            var origLightNpcFrame = ReadFrame(stream, locs, originatingLightNpc.FormKey);
            origLightNpcFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingLightNpc.FormKey.ID).Raw);
            var lightNpcCscr = origLightNpcFrame.FindSubrecord(RecordTypes.CSCR);
            lightNpcCscr.AsFormID()
                .ShouldBe(FormID.Factory(MasterStyle.Small, 0, lightModANpc.FormKey.ID));
            var origLightWeaponFrame = ReadFrame(stream, locs, originatingLightWeapon.FormKey);
            origLightWeaponFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingLightWeapon.FormKey.ID).Raw);
            var lightWeaponWaim = origLightWeaponFrame.FindSubrecord(waim);
            new FormID(BinaryPrimitives.ReadUInt32LittleEndian(lightWeaponWaim.Content.Slice(8)))
                .ShouldBe(FormID.Factory(MasterStyle.Small, 1, lightModBAim.FormKey.ID));
            
            var origMediumNpcFrame = ReadFrame(stream, locs, originatingMediumNpc.FormKey);
            origMediumNpcFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingMediumNpc.FormKey.ID).Raw);
            var mediumNpcCscr = origMediumNpcFrame.FindSubrecord(RecordTypes.CSCR);
            mediumNpcCscr.AsFormID()
                .ShouldBe(FormID.Factory(MasterStyle.Medium, 0, mediumModANpc.FormKey.ID));
            var origMediumWeaponFrame = ReadFrame(stream, locs, originatingMediumWeapon.FormKey);
            origMediumWeaponFrame.FormID.Raw
                .ShouldBe(FormID.Factory(MasterStyle.Full, 2, originatingMediumWeapon.FormKey.ID).Raw);
            var mediumWeaponWaim = origMediumWeaponFrame.FindSubrecord(waim);
            new FormID(BinaryPrimitives.ReadUInt32LittleEndian(mediumWeaponWaim.Content.Slice(8)))
                .ShouldBe(FormID.Factory(MasterStyle.Medium, 1, mediumModBAim.FormKey.ID));
        }
        
        using var reimport = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield, new BinaryReadParameters()
        {
            FileSystem = fileSystem,
            MasterFlagsLookup = lo
        });
        reimport.Npcs.Select(x => x.FormKey).ShouldEqual(
            modANpc.FormKey,
            lightModANpc.FormKey,
            mediumModANpc.FormKey,
            originatingNpc.FormKey,
            originatingLightNpc.FormKey,
            originatingMediumNpc.FormKey);
        var reimportNpc = reimport.Npcs[originatingNpc.FormKey];
        reimportNpc.InheritsSoundsFrom.FormKey.ShouldBe(modANpc.FormKey);
        var reimportLightNpc = reimport.Npcs[originatingLightNpc.FormKey];
        reimportLightNpc.InheritsSoundsFrom.FormKey.ShouldBe(lightModANpc.FormKey);
        var reimportMediumNpc = reimport.Npcs[originatingMediumNpc.FormKey];
        reimportMediumNpc.InheritsSoundsFrom.FormKey.ShouldBe(mediumModANpc.FormKey);
        reimport.Weapons.Select(x => x.FormKey).ShouldEqual(
            originatingWeapon.FormKey,
            originatingLightWeapon.FormKey,
            originatingMediumWeapon.FormKey);
        var reimportWeapon = reimport.Weapons[originatingWeapon.FormKey];
        reimportWeapon.AimModel.FormKey.ShouldBe(modBAim.FormKey);
        var reimportLightWeapon = reimport.Weapons[originatingLightWeapon.FormKey];
        reimportLightWeapon.AimModel.FormKey.ShouldBe(lightModBAim.FormKey);
        var reimportMediumWeapon = reimport.Weapons[originatingMediumWeapon.FormKey];
        reimportMediumWeapon.AimModel.FormKey.ShouldBe(mediumModBAim.FormKey);
        reimport.AimModels.Select(x => x.FormKey).ShouldEqual(
            modBAim.FormKey,
            lightModBAim.FormKey,
            mediumModBAim.FormKey);
    }
}