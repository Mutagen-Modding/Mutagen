using System.Text;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class DialogTopicSubtypeTests
{
    private static readonly ModKey TestModKey = new("Test", ModType.Plugin);

    private static ParsingMeta Meta()
    {
        var masters = SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(TestModKey));
        return new ParsingMeta(GameConstants.SkyrimSE, TestModKey, masters);
    }

    private static void WriteSubrecord(MemoryStream stream, string type, byte[] content)
    {
        stream.Write(Encoding.ASCII.GetBytes(type));
        stream.Write(BitConverter.GetBytes(checked((ushort)content.Length)));
        stream.Write(content);
    }

    private static byte[] MakeTopicBytes(
        byte[]? data,
        string? snam,
        ushort formVersion = 40,
        bool snamFirst = false,
        bool countBetween = false)
    {
        var content = new MemoryStream();
        WriteSubrecord(content, "EDID", Encoding.ASCII.GetBytes("TestTopic\0"));
        WriteSubrecord(content, "PNAM", BitConverter.GetBytes(50f));
        if (snamFirst && snam is not null)
        {
            WriteSubrecord(content, "SNAM", Encoding.ASCII.GetBytes(snam));
        }
        if (countBetween)
        {
            WriteSubrecord(content, "TIFC", new byte[4]);
        }
        if (data is not null)
        {
            WriteSubrecord(content, "DATA", data);
        }
        if (!snamFirst && snam is not null)
        {
            WriteSubrecord(content, "SNAM", Encoding.ASCII.GetBytes(snam));
        }
        var body = content.ToArray();

        var record = new MemoryStream();
        record.Write(Encoding.ASCII.GetBytes("DIAL"));
        record.Write(BitConverter.GetBytes(body.Length));
        record.Write(BitConverter.GetBytes(0));
        record.Write(BitConverter.GetBytes(0x800));
        record.Write(BitConverter.GetBytes(0));
        record.Write(BitConverter.GetBytes(formVersion));
        record.Write(BitConverter.GetBytes((ushort)0));
        record.Write(body);
        return record.ToArray();
    }

    private static IDialogTopicGetter ReadDirect(byte[] bytes)
    {
        return DialogTopic.CreateFromBinary(
            new MutagenFrame(new MutagenMemoryReadStream(bytes, Meta())));
    }

    private static IDialogTopicGetter ReadOverlay(byte[] bytes)
    {
        var meta = Meta();
        return DialogTopicBinaryOverlay.DialogTopicFactory(
            new OverlayStream(bytes, meta),
            new BinaryOverlayFactoryPackage(meta));
    }

    private static byte[] Write(IDialogTopicGetter topic)
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
            topic.WriteToBinary(writer);
        }
        return memStream.ToArray();
    }

    private static byte[] Subrecord(byte[] bytes, string type)
    {
        var target = Encoding.ASCII.GetBytes(type);
        for (int i = 24; i + 6 <= bytes.Length; )
        {
            var len = BitConverter.ToUInt16(bytes, i + 4);
            if (bytes.AsSpan(i, 4).SequenceEqual(target))
            {
                return bytes.AsSpan(i + 6, len).ToArray();
            }
            i += 6 + len;
        }
        throw new InvalidOperationException($"No {type} subrecord found.");
    }


    // DIAL 0002707A, 000904AC and 00000E3C from Skyrim.esm.
    [Theory]
    [InlineData(new byte[] { 0x00, 0x07, 0x49, 0x00 }, "HELO", DialogTopic.SubtypeEnum.Hello)]
    [InlineData(new byte[] { 0x00, 0x07, 0x48, 0x00 }, "GBYE", DialogTopic.SubtypeEnum.Goodbye)]
    [InlineData(new byte[] { 0x00, 0x03, 0x17, 0x00 }, "HIT_", DialogTopic.SubtypeEnum.Hit)]
    public void LegacyNumberedRecord_TakesSubtypeFromSnam(
        byte[] data,
        string snam,
        DialogTopic.SubtypeEnum expectedSubtype)
    {
        var bytes = MakeTopicBytes(data, snam);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(expectedSubtype);
        }
    }

    [Fact]
    public void ModernNumberedRecord_Agrees()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x4F, 0x00 }, "HELO", formVersion: 44);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
        }
    }

    [Fact]
    public void SnamBeforeData_TakesSubtypeFromSnam()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x01, 0x07, 0x49, 0x00 }, "HELO", snamFirst: true);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
            topic.TopicFlags.ShouldBe(DialogTopic.TopicFlag.DoAllBeforeRepeating);
        }
    }

    [Fact]
    public void SnamAndDataApart_TakesSubtypeFromSnam()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x49, 0x00 }, "HELO", snamFirst: true, countBetween: true);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
        }
    }

    [Fact]
    public void MissingData_TakesSubtypeFromSnam()
    {
        var bytes = MakeTopicBytes(data: null, snam: "HELO");

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
            topic.TopicFlags.ShouldBe(default(DialogTopic.TopicFlag));
        }
    }

    [Fact]
    public void EmptyData_TakesSubtypeFromSnam()
    {
        var bytes = MakeTopicBytes(Array.Empty<byte>(), "HELO");

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
            topic.TopicFlags.ShouldBe(default(DialogTopic.TopicFlag));
        }
    }

    [Fact]
    public void ShortSnam_LeavesTheSubtypeDefault()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x49, 0x00 }, "AB");

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Custom);
        }
    }

    [Fact]
    public void MissingSnam_LeavesTheSubtypeDefault()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x49, 0x00 }, snam: null);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Custom);
        }
    }

    [Theory]
    [InlineData("ZZZZ")]
    [InlineData("\0\0\0\0")]
    public void UnknownSnam_LeavesTheSubtypeDefault(string snam)
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x03, 0x17, 0x00 }, snam);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Custom);
        }
    }

    [Fact]
    public void ShortData_DoesNotOverRead()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x01, 0x03 }, snam: null);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.TopicFlags.ShouldBe(DialogTopic.TopicFlag.DoAllBeforeRepeating);
            topic.Subtype.ShouldBe(default(DialogTopic.SubtypeEnum));
        }
    }

    [Fact]
    public void SettingSubtypeAlone_WritesModernDataAndMatchingSnam()
    {
        var topic = new DialogTopic(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE)
        {
            EditorID = "TestTopic",
            Subtype = DialogTopic.SubtypeEnum.Hello,
        };

        var bytes = Write(topic);

        Subrecord(bytes, "DATA").ShouldBe(new byte[] { 0x00, 0x07, 0x4F, 0x00 });
        Subrecord(bytes, "SNAM").ShouldBe(Encoding.ASCII.GetBytes("HELO"));

        ReadDirect(bytes).Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
        ReadOverlay(bytes).Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
    }

    [Fact]
    public void DefaultTopic_WritesTheDefaultSubtype()
    {
        var topic = new DialogTopic(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE);

        var bytes = Write(topic);

        Subrecord(bytes, "DATA").ShouldBe(new byte[] { 0x00, 0x00, 0x00, 0x00 });
        Subrecord(bytes, "SNAM").ShouldBe(Encoding.ASCII.GetBytes("CUST"));
    }

    [Fact]
    public void LegacyRecord_WritesBackRenumbered()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x49, 0x00 }, "HELO");

        var written = Write(ReadDirect(bytes));

        Subrecord(written, "DATA").ShouldBe(new byte[] { 0x00, 0x07, 0x4F, 0x00 });
        Subrecord(written, "SNAM").ShouldBe(Encoding.ASCII.GetBytes("HELO"));
        Write(ReadOverlay(bytes)).ShouldBe(written);
    }

    [Fact]
    public void FvdlRecord_RoundTripsAsCustomFvdl()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x03, 0x17, 0x00 }, "FVDL");

        var written = Write(ReadDirect(bytes));

        ReadDirect(bytes).Subtype.ShouldBe(DialogTopic.SubtypeEnum.CustomFVDL);
        ReadOverlay(bytes).Subtype.ShouldBe(DialogTopic.SubtypeEnum.CustomFVDL);
        Subrecord(written, "DATA").ShouldBe(new byte[] { 0x00, 0x07, 0x67, 0x00 });
        Subrecord(written, "SNAM").ShouldBe(Encoding.ASCII.GetBytes("FVDL"));
        ReadDirect(written).Subtype.ShouldBe(DialogTopic.SubtypeEnum.CustomFVDL);
        Write(ReadOverlay(bytes)).ShouldBe(written);
    }

    [Fact]
    public void MissingSnam_WritesBackAsTheDefaultSubtype()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x49, 0x00 }, snam: null);

        var written = Write(ReadDirect(bytes));

        Subrecord(written, "SNAM").ShouldBe(Encoding.ASCII.GetBytes("CUST"));
        Subrecord(written, "DATA").ShouldBe(new byte[] { 0x00, 0x00, 0x00, 0x00 });
        Write(ReadOverlay(bytes)).ShouldBe(written);
    }

    [Fact]
    public void SubtypeOutsideTheEnum_WritesAZeroedMarker()
    {
        var topic = new DialogTopic(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE)
        {
            EditorID = "TestTopic",
            Subtype = (DialogTopic.SubtypeEnum)9999,
        };

        var bytes = Write(topic);

        Subrecord(bytes, "DATA").ShouldBe(new byte[] { 0x00, 0x00, 0x0F, 0x27 });
        Subrecord(bytes, "SNAM").ShouldBe(new byte[] { 0x00, 0x00, 0x00, 0x00 });
    }

    [Fact]
    public void CopyInFromBinary_OverExistingSubtype_TakesTheNewSnam()
    {
        var topic = new DialogTopic(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE)
        {
            Subtype = DialogTopic.SubtypeEnum.Hello,
        };
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x48, 0x00 }, "GBYE");

        topic.CopyInFromBinary(new MutagenFrame(new MutagenMemoryReadStream(bytes, Meta())));

        topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Goodbye);
    }

    [Fact]
    public void MarkerLookupsRoundTrip()
    {
        foreach (var subtype in Enum.GetValues<DialogTopic.SubtypeEnum>())
        {
            var marker = DialogTopic.MarkerFromSubtype(subtype);
            marker.ShouldNotBeNull();
            DialogTopic.SubtypeFromMarker(marker!.Value).ShouldBe(subtype);
        }

        DialogTopic.SubtypeFromMarker(RecordType.Null).ShouldBeNull();
    }
}
