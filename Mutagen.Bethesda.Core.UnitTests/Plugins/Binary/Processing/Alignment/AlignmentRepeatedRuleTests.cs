using System.Data;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Processing.Alignment;

public class AlignmentRepeatedRuleTests
{
    public static byte[] GetBytes(params RecordType[] types)
    {
        var bytes = new MemoryStream();
        using (var writer = new MutagenWriter(bytes, GameConstants.SkyrimSE, dispose: false))
        {
            foreach (var type in types)
            {
                using (HeaderExport.Subrecord(writer, type))
                {
                    writer.Write(3);
                }
            }
        }
        bytes.Position = 0;
        return bytes.ToArray();
    }

    private void AssertBytes(ReadOnlyMemorySlice<byte> bytes, params RecordType[] types)
    {
        var reader = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, ModKey.Null, null!));
        foreach (var t in types)
        {
            reader.ReadSubrecord(t);
        }

        if (!reader.Complete)
        {
            throw new DataException();
        }
    }
    
    [Theory, MutagenAutoData]
    public void GrabsAllRelated(ModKey modKey)
    {
        var bytes = GetBytes(
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, modKey, null!));
        AssertBytes(
            AlignmentRepeatedRule.Basic(
                TestRecordTypes.TX00,
                TestRecordTypes.TX01).ReadBytes(read, null),
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Theory, MutagenAutoData]
    public void TriggersOffLaterItems(ModKey modKey)
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, modKey, null!));
        AssertBytes(
            AlignmentRepeatedRule.Basic(
                TestRecordTypes.TX00,
                TestRecordTypes.TX01).ReadBytes(read, null),
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Theory, MutagenAutoData]
    public void BasicDoesNotEnforceOrder(ModKey modKey)
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, modKey, null!));
        AssertBytes(
            AlignmentRepeatedRule.Basic(
                TestRecordTypes.TX00,
                TestRecordTypes.TX01).ReadBytes(read, null),
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Theory, MutagenAutoData]
    public void SortingWithRepeatedButSingleEntries(ModKey modKey)
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, modKey, null!));
        AssertBytes(
            AlignmentRepeatedRule.Sorted(
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX00, true),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX01, true))
                .ReadBytes(read, null),
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Theory, MutagenAutoData]
    public void SortingConfusedWithMultiple(ModKey modKey)
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, modKey, null!));
        AssertBytes(
            AlignmentRepeatedRule.Sorted(
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX00, false),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX01, true))
                .ReadBytes(read, null),
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX01);
    }
    
    [Theory, MutagenAutoData]
    public void SortingWithStabilizingEntries(ModKey modKey)
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX02,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX02,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, modKey, null!));
        AssertBytes(
            AlignmentRepeatedRule.Sorted(
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX00, true),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX01, true),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX02, true))
                .ReadBytes(read, null),
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX02,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX02);
    }
}