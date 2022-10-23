using System.Data;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
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
        var reader = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        foreach (var t in types)
        {
            reader.ReadSubrecord(t);
        }

        if (!reader.Complete)
        {
            throw new DataException();
        }
    }
    
    [Fact]
    public void GrabsAllRelated()
    {
        var bytes = GetBytes(
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        AssertBytes(
            AlignmentRepeatedRule.Basic(
                TestRecordTypes.TX00,
                TestRecordTypes.TX01).GetBytes(read),
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Fact]
    public void TriggersOffLaterItems()
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        AssertBytes(
            AlignmentRepeatedRule.Basic(
                TestRecordTypes.TX00,
                TestRecordTypes.TX01).GetBytes(read),
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Fact]
    public void BasicDoesNotEnforceOrder()
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        AssertBytes(
            AlignmentRepeatedRule.Basic(
                TestRecordTypes.TX00,
                TestRecordTypes.TX01).GetBytes(read),
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Fact]
    public void SortingWithRepeatedButSingleEntries()
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        AssertBytes(
            AlignmentRepeatedRule.Sorted(
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX00, true),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX01, true))
                .GetBytes(read),
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01);
    }
    
    [Fact]
    public void SortingConfusedWithMultiple()
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        AssertBytes(
            AlignmentRepeatedRule.Sorted(
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX00, false),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX01, true))
                .GetBytes(read),
            TestRecordTypes.TX00,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX01);
    }
    
    [Fact]
    public void SortingWithStabilizingEntries()
    {
        var bytes = GetBytes(
            TestRecordTypes.TX01,
            TestRecordTypes.TX00,
            TestRecordTypes.TX02,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX02,
            TestRecordTypes.TX03);
        var read = new MutagenMemoryReadStream(bytes, new ParsingBundle(GameConstants.SkyrimSE, null!));
        AssertBytes(
            AlignmentRepeatedRule.Sorted(
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX00, true),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX01, true),
                    new AlignmentRepeatedSubrule(TestRecordTypes.TX02, true))
                .GetBytes(read),
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX02,
            TestRecordTypes.TX00,
            TestRecordTypes.TX01,
            TestRecordTypes.TX02);
    }
}