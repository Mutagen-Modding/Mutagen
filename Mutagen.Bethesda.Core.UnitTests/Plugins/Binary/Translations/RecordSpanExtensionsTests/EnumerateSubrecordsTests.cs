﻿using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class EnumerateSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void EnumerateSubrecordsEmpty()
    {
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.EnumerateSubrecords(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion)
            .ShouldBeEmpty();
    }

    [Fact]
    public void EnumerateSubrecordsTypical()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(GetTypical(), GameConstants.Oblivion).ToArray();
        ret.Length.ShouldBe(2);
        ret[0].RecordType.ShouldBe(FirstType);
        ret[0].Location.ShouldBe(FirstLocation);
        ret[0].ContentLength.ShouldBe(FirstLength);
        ret[1].RecordType.ShouldBe(SecondType);
        ret[1].Location.ShouldBe(SecondLocation);
        ret[1].ContentLength.ShouldBe(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsOffset()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(Offset(), GameConstants.Oblivion, offset: OffsetAmount).ToArray();
        ret.Length.ShouldBe(2);
        ret[0].RecordType.ShouldBe(FirstType);
        ret[0].Location.ShouldBe(FirstLocation + OffsetAmount);
        ret[0].ContentLength.ShouldBe(FirstLength);
        ret[1].RecordType.ShouldBe(SecondType);
        ret[1].Location.ShouldBe(SecondLocation + OffsetAmount);
        ret[1].ContentLength.ShouldBe(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsWithOverflow()
    {
        var recs = RecordSpanExtensions.EnumerateSubrecords(Overflow(), GameConstants.Oblivion).ToArray();
        recs.Length.ShouldBe(3);
        recs.Select(x => x.RecordType).ShouldEqualEnumerable(
            RecordTypes.MAST,
            RecordTypes.DATA,
            RecordTypes.EDID);
        recs.Select(x => x.ContentLength).ShouldEqualEnumerable(4, 2, 4);
        recs[0].AsInt32().ShouldBe(0x04030201);
        recs[1].AsInt16().ShouldEqual(0x0809);
        recs[2].AsInt32().ShouldBe(0x44332211);
    }

    [Fact]
    public void EnumerateSubrecordsDuplicate()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(GetDuplicate(), GameConstants.Oblivion).ToArray();
        ret.Length.ShouldBe(3);
        ret[0].RecordType.ShouldBe(FirstType);
        ret[0].Location.ShouldBe(FirstLocation);
        ret[0].ContentLength.ShouldBe(FirstLength);
        ret[1].RecordType.ShouldBe(SecondType);
        ret[1].Location.ShouldBe(SecondLocation);
        ret[1].ContentLength.ShouldBe(SecondLength);
        ret[2].RecordType.ShouldBe(DuplicateType);
        ret[2].Location.ShouldBe(DuplicateLocation);
        ret[2].ContentLength.ShouldBe(DuplicateLength);
    }
    
    [Fact]
    public void EnumerateSubrecordsActionEmpty()
    {
        byte[] b = Array.Empty<byte>();
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion, ret.Add);
        ret.ShouldBeEmpty();
    }

    [Fact]
    public void EnumerateSubrecordsActionTypical()
    {
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(GetTypical(), GameConstants.Oblivion, ret.Add);
        ret.Count.ShouldBe(2);
        ret[0].RecordType.ShouldBe(FirstType);
        ret[0].Location.ShouldBe(FirstLocation);
        ret[0].ContentLength.ShouldBe(FirstLength);
        ret[1].RecordType.ShouldBe(SecondType);
        ret[1].Location.ShouldBe(SecondLocation);
        ret[1].ContentLength.ShouldBe(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsActionOffset()
    {
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(Offset(), GameConstants.Oblivion, ret.Add, offset: OffsetAmount);
        ret.Count.ShouldBe(2);
        ret[0].RecordType.ShouldBe(FirstType);
        ret[0].Location.ShouldBe(FirstLocation + OffsetAmount);
        ret[0].ContentLength.ShouldBe(FirstLength);
        ret[1].RecordType.ShouldBe(SecondType);
        ret[1].Location.ShouldBe(SecondLocation + OffsetAmount);
        ret[1].ContentLength.ShouldBe(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsActionWithOverflow()
    {
        List<SubrecordPinFrame> recs = new();
        RecordSpanExtensions.EnumerateSubrecords(Overflow(), GameConstants.Oblivion, recs.Add);
        recs.Count.ShouldBe(3);
        recs.Select(x => x.RecordType).ShouldEqualEnumerable(
            RecordTypes.MAST,
            RecordTypes.DATA,
            RecordTypes.EDID);
        recs.Select(x => x.ContentLength).ShouldEqualEnumerable(4, 2, 4);
        recs[0].AsInt32().ShouldBe(0x04030201);
        recs[1].AsInt16().ShouldEqual(0x0809);
        recs[2].AsInt32().ShouldBe(0x44332211);
    }

    [Fact]
    public void EnumerateSubrecordsActionDuplicate()
    {
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(GetDuplicate(), GameConstants.Oblivion, ret.Add);
        ret.Count.ShouldBe(3);
        ret[0].RecordType.ShouldBe(FirstType);
        ret[0].Location.ShouldBe(FirstLocation);
        ret[0].ContentLength.ShouldBe(FirstLength);
        ret[1].RecordType.ShouldBe(SecondType);
        ret[1].Location.ShouldBe(SecondLocation);
        ret[1].ContentLength.ShouldBe(SecondLength);
        ret[2].RecordType.ShouldBe(DuplicateType);
        ret[2].Location.ShouldBe(DuplicateLocation);
        ret[2].ContentLength.ShouldBe(DuplicateLength);
    }
}