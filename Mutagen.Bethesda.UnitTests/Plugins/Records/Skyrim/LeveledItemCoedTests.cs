using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

/// <summary>
/// Regression tests for a LeveledItem (LVLI) whose entries carry COED extra data.
/// A COED is trailing extra data belonging to the preceding LVLO entry, so a list of
/// several LVLO entries followed by a single COED on the last entry must round-trip
/// with exactly one COED. A prior ordering regression in the generated TriggerSpecs
/// (COED sorted before LVLO) caused the overlay parser to spawn a phantom entry for the
/// trailing COED, which then got re-emitted as a duplicate COED on export.
/// </summary>
public class LeveledItemCoedTests
{
    private static readonly ModKey TestModKey = new("Test", ModType.Plugin);

    private static ParsingMeta Meta(byte[] bytes)
    {
        var masters = SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(TestModKey));
        var meta = new ParsingMeta(GameConstants.SkyrimSE, TestModKey, masters);
        // COED owner parsing needs a non-null cache to disambiguate owner type; our owners
        // use null links so the lookup short-circuits without ever touching the stream.
        meta.RecordInfoCache = new RecordTypeInfoCacheReader(
            () => new MutagenMemoryReadStream(bytes, meta),
            TestModKey);
        return meta;
    }

    private static LeveledItem MakeLeveledItem(int entryCount)
    {
        var lvli = new LeveledItem(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE)
        {
            EditorID = "TestLeveledItem",
            Entries = new ExtendedList<LeveledItemEntry>(),
        };
        for (int i = 0; i < entryCount; i++)
        {
            var entry = new LeveledItemEntry
            {
                Data = new LeveledItemEntryData
                {
                    Level = 1,
                    Count = 1,
                    Reference = new FormLink<IItemGetter>(),
                },
            };
            // COED rides on the final entry only, mirroring the reported record shape.
            if (i == entryCount - 1)
            {
                entry.ExtraData = new ExtraData
                {
                    Owner = new NpcOwner
                    {
                        Npc = new FormLink<INpcGetter>(),
                        Global = new FormLink<IGlobalGetter>(),
                    },
                    ItemCondition = 1f,
                };
            }
            lvli.Entries.Add(entry);
        }
        return lvli;
    }

    private static byte[] Write(ILeveledItemGetter lvli)
    {
        var masters = new MasterReferenceCollection(TestModKey);
        var bundle = new WritingBundle(GameConstants.SkyrimSE)
        {
            MasterReferences = masters,
            SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(masters),
        };
        var memStream = new MemoryStream();
        using (var writer = new MutagenWriter(memStream, bundle, dispose: false))
        {
            lvli.WriteToBinary(writer);
        }
        return memStream.ToArray();
    }

    private static ILeveledItemGetter ReadDirect(byte[] bytes)
    {
        return LeveledItem.CreateFromBinary(
            new MutagenFrame(new MutagenMemoryReadStream(bytes, Meta(bytes))));
    }

    private static ILeveledItemGetter ReadOverlay(byte[] bytes)
    {
        var meta = Meta(bytes);
        return LeveledItemBinaryOverlay.LeveledItemFactory(
            new OverlayStream(bytes, meta),
            new BinaryOverlayFactoryPackage(meta));
    }

    private static int CoedCount(ILeveledItemGetter lvli)
        => lvli.Entries!.Count(e => e.ExtraData is not null);

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    public void TrailingCoed_RoundTripsDirect(int entryCount)
    {
        var lvli = MakeLeveledItem(entryCount);
        var bytes = Write(lvli);

        var readBack = ReadDirect(bytes);
        readBack.Entries.ShouldNotBeNull();
        readBack.Entries!.Count.ShouldBe(entryCount);
        CoedCount(readBack).ShouldBe(1);
        Write(readBack).ShouldBe(bytes);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    public void TrailingCoed_RoundTripsOverlay(int entryCount)
    {
        var lvli = MakeLeveledItem(entryCount);
        var bytes = Write(lvli);

        var readBack = ReadOverlay(bytes);
        readBack.Entries.ShouldNotBeNull();
        readBack.Entries!.Count.ShouldBe(entryCount);
        CoedCount(readBack).ShouldBe(1);
        Write(readBack).ShouldBe(bytes);
    }

    [Fact]
    public void TrailingCoed_DirectAndOverlayAgree()
    {
        var bytes = Write(MakeLeveledItem(4));

        var direct = ReadDirect(bytes);
        var overlay = ReadOverlay(bytes);

        overlay.Entries!.Count.ShouldBe(direct.Entries!.Count);
        CoedCount(overlay).ShouldBe(CoedCount(direct));
        Write(overlay).ShouldBe(Write(direct));
    }
}
