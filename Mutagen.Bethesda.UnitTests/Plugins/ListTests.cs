﻿using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class ListTests
{
    [Theory, MutagenModAutoData]
    public void LListCounter(
        SkyrimMod mod,
        LeveledItem leveledItem,
        MasterReferenceCollection masters,
        ModPath path,
        IFileSystem fileSystem)
    {
        leveledItem.Entries ??= new();
        for (int i = 0; i < 55; i++)
        {
            leveledItem.Entries.Add(new LeveledItemEntry());
        }

        using (var writer = new MutagenWriter(fileSystem.File.OpenWrite(path), GameConstants.SkyrimSE, dispose: true))
        {
            writer.MetaData.MasterReferences = new MasterReferenceCollection(mod.ModKey);
            writer.MetaData.SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(writer.MetaData.MasterReferences);
            leveledItem.WriteToBinary(writer);
        }

        using (var stream = new MutagenBinaryReadStream(path, GameRelease.SkyrimSE, masterFlagLookup: null, fileSystem: fileSystem))
        {
            var rec = stream.ReadMajorRecord();
            var llct = rec.FindSubrecord(RecordTypes.LLCT);
            llct.AsInt8().ShouldEqual(55);
        }
    }
    
    [Theory, MutagenModAutoData]
    public void LListOverflowPrintsZeroCounter(
        SkyrimMod mod,
        LeveledItem leveledItem,
        ModPath path,
        IFileSystem fileSystem)
    {
        leveledItem.Entries ??= new();
        for (int i = 0; i < 1000; i++)
        {
            leveledItem.Entries.Add(new LeveledItemEntry());
        }

        using (var writer = new MutagenWriter(fileSystem.File.OpenWrite(path), GameConstants.SkyrimSE, dispose: true))
        {
            writer.MetaData.MasterReferences = new MasterReferenceCollection(mod.ModKey);
            writer.MetaData.SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(writer.MetaData.MasterReferences);
            leveledItem.WriteToBinary(writer);
        }

        using (var stream = new MutagenBinaryReadStream(path, GameRelease.SkyrimSE, masterFlagLookup: null, fileSystem: fileSystem))
        {
            var rec = stream.ReadMajorRecord();
            var llct = rec.FindSubrecord(RecordTypes.LLCT);
            llct.AsInt8().ShouldEqual(0);
        }
    }
}