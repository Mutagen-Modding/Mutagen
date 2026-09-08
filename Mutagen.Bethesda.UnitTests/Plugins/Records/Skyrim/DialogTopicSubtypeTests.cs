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

/// <summary>
/// DialogTopic takes SNAM as the authoritative statement of a topic's subtype.
/// Bethesda inserted the six FlyingMount subtypes at index 20 with the Dragonborn-era Creation Kit,
/// so a DIAL authored before that stores a DATA subtype six lower than SubtypeEnum's numbering, and
/// nothing on the record says which numbering it used. The three legacy cases here are real records
/// out of Skyrim.esm.
/// </summary>
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

    private static byte[] MakeTopicBytes(byte[]? data, string? snam, ushort formVersion = 40)
    {
        var content = new MemoryStream();
        WriteSubrecord(content, "EDID", Encoding.ASCII.GetBytes("TestTopic\0"));
        WriteSubrecord(content, "PNAM", BitConverter.GetBytes(50f));
        if (data is not null)
        {
            WriteSubrecord(content, "DATA", data);
        }
        if (snam is not null)
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

    /// <summary>The content of the first subrecord of the given type in a written record.</summary>
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

    // DIAL 0002707A, 000904AC and 00000E3C from Skyrim.esm, FormVersion 40. Each stores a DATA
    // subtype six lower than the modern numbering; SNAM says what the topic really is.
    [Theory]
    [InlineData(new byte[] { 0x00, 0x07, 0x49, 0x00 }, "HELO", DialogTopic.SubtypeEnum.Hello, DialogTopic.CategoryEnum.Misc)]
    [InlineData(new byte[] { 0x00, 0x07, 0x48, 0x00 }, "GBYE", DialogTopic.SubtypeEnum.Goodbye, DialogTopic.CategoryEnum.Misc)]
    [InlineData(new byte[] { 0x00, 0x03, 0x17, 0x00 }, "HIT_", DialogTopic.SubtypeEnum.Hit, DialogTopic.CategoryEnum.Combat)]
    public void LegacyNumberedRecord_TakesSubtypeFromSnam(
        byte[] data,
        string snam,
        DialogTopic.SubtypeEnum expectedSubtype,
        DialogTopic.CategoryEnum expectedCategory)
    {
        var bytes = MakeTopicBytes(data, snam);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.SubtypeName.ShouldBe(new RecordType(snam));
            topic.Subtype.ShouldBe(expectedSubtype);
            topic.Category.ShouldBe(expectedCategory);
        }
    }

    // A modern-numbered record: DATA already agrees with SNAM, so nothing changes.
    [Fact]
    public void ModernNumberedRecord_Agrees()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x4F, 0x00 }, "HELO", formVersion: 44);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.Hello);
            topic.Category.ShouldBe(DialogTopic.CategoryEnum.Misc);
        }
    }

    // No SNAM: the raw DATA values are all there is, so they stand.
    [Fact]
    public void MissingSnam_FallsBackToRawData()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x07, 0x49, 0x00 }, snam: null);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.SubtypeName.ShouldBe(RecordType.Null);
            topic.Subtype.ShouldBe(DialogTopic.SubtypeEnum.RechargeExit);
            topic.Category.ShouldBe(DialogTopic.CategoryEnum.Misc);
        }
    }

    // FVDL is a real marker at raw index 3 that SubtypeEnum has no member for, and a blank SNAM is
    // not a marker at all. Neither names a subtype we model, so the raw DATA values stand.
    [Theory]
    [InlineData("FVDL")]
    [InlineData("\0\0\0\0")]
    public void UnknownSnam_FallsBackToRawData(string snam)
    {
        var bytes = MakeTopicBytes(new byte[] { 0x00, 0x03, 0x17, 0x00 }, snam);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.Subtype.ShouldBe((DialogTopic.SubtypeEnum)23);
            topic.Category.ShouldBe(DialogTopic.CategoryEnum.Combat);
        }
    }

    // A DATA shorter than four bytes must read as absent, not into the following subrecord.
    [Fact]
    public void ShortData_DoesNotOverRead()
    {
        var bytes = MakeTopicBytes(new byte[] { 0x01, 0x03 }, snam: null);

        foreach (var topic in new[] { ReadDirect(bytes), ReadOverlay(bytes) })
        {
            topic.TopicFlags.ShouldBe(DialogTopic.TopicFlag.DoAllBeforeRepeating);
            topic.Category.ShouldBe(DialogTopic.CategoryEnum.Combat);
            topic.Subtype.ShouldBe(default(DialogTopic.SubtypeEnum));
        }
    }

    // Setting Subtype alone is enough: the write derives DATA's category and subtype and the SNAM marker.
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

    // Reading a legacy record and writing it back renumbers DATA to match SNAM, which is what the
    // Creation Kit and xEdit both do.
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
    public void MarkerLookupsRoundTrip()
    {
        foreach (var subtype in Enum.GetValues<DialogTopic.SubtypeEnum>())
        {
            var marker = DialogTopic.MarkerFromSubtype(subtype);
            marker.ShouldNotBeNull();
            DialogTopic.SubtypeFromMarker(marker!.Value).ShouldBe(subtype);
            DialogTopic.CategoryFromSubtype(subtype).ShouldNotBeNull();
        }

        DialogTopic.SubtypeFromMarker(new RecordType("FVDL")).ShouldBeNull();
        DialogTopic.SubtypeFromMarker(RecordType.Null).ShouldBeNull();
    }
}
