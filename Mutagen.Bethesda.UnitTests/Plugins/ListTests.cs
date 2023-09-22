using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class ListTests
{
    [Theory, MutagenModAutoData]
    public void LListCounter(
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
            writer.MetaData.MasterReferences = masters;
            leveledItem.WriteToBinary(writer);
        }

        using (var stream = new MutagenBinaryReadStream(path, GameRelease.SkyrimSE, fileSystem: fileSystem))
        {
            var rec = stream.ReadMajorRecord();
            var llct = rec.FindSubrecord(RecordTypes.LLCT);
            llct.AsInt8().Should().Be(55);
        }
    }
    
    [Theory, MutagenModAutoData]
    public void LListOverflowPrintsZeroCounter(
        LeveledItem leveledItem,
        MasterReferenceCollection masters,
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
            writer.MetaData.MasterReferences = masters;
            leveledItem.WriteToBinary(writer);
        }

        using (var stream = new MutagenBinaryReadStream(path, GameRelease.SkyrimSE, fileSystem: fileSystem))
        {
            var rec = stream.ReadMajorRecord();
            var llct = rec.FindSubrecord(RecordTypes.LLCT);
            llct.AsInt8().Should().Be(0);
        }
    }
}