using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;
using static Mutagen.Bethesda.UnitTests.Plugins.Records.RawRecordBytes;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class DialogResponseTroiTests
{
    private static readonly GameConstants Constants = GameConstants.Starfield;
    private const uint LinkedInfoId = 0x802;
    private const uint NoEmotion = uint.MaxValue;

    private static (string, byte[])[] Response(string text, byte[] hash, uint? troi)
    {
        var trda = U32(NoEmotion).Concat(U32(0)).Concat(BitConverter.GetBytes(0f)).ToArray();
        var subrecords = new List<(string, byte[])> { ("TRDA", trda) };
        if (troi.HasValue)
        {
            subrecords.Add(("TROI", U32(troi.Value)));
        }
        subrecords.Add(("NAM1", Str(text)));
        subrecords.Add(("NAM2", Str(string.Empty)));
        subrecords.Add(("NAM3", Str(string.Empty)));
        subrecords.Add(("NAM4", Str(string.Empty)));
        subrecords.Add(("NAM9", hash));
        return subrecords.ToArray();
    }

    private static byte[] InfoWithTroiOnSecondResponse(string firstText, byte[] firstHash, string secondText, byte[] secondHash)
    {
        return Record(Constants, "INFO",
        [
            ("ENAM", U32(0)),
            .. Response(firstText, firstHash, troi: null),
            .. Response(secondText, secondHash, troi: LinkedInfoId),
            ("NAM0", Str(string.Empty)),
            ("INAM", U32(0)),
        ]);
    }

    private static ParsingMeta Meta(ModKey modKey) => new(
        Constants,
        modKey,
        SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(modKey)));

    private static IEnumerable<IDialogResponsesGetter> ReadBoth(ModKey modKey, byte[] bytes)
    {
        var meta = Meta(modKey);
        yield return DialogResponses.CreateFromBinary(new MutagenFrame(new MutagenMemoryReadStream(bytes, meta)));
        yield return DialogResponsesBinaryOverlay.DialogResponsesFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));
    }

    private static byte[] Write(ModKey modKey, IDialogResponsesGetter info)
    {
        var masters = new MasterReferenceCollection(modKey);
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(
                   stream,
                   new WritingBundle(Constants)
                   {
                       MasterReferences = masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(masters),
                   },
                   dispose: false))
        {
            info.WriteToBinary(writer);
        }
        return stream.ToArray();
    }

    [Theory, MutagenAutoData(GameRelease.Starfield)]
    public void TroiAfterTrda_KeepsResponseTextAndFollowingSubrecords(
        ModKey modKey,
        string firstText,
        byte[] firstHash,
        string secondText,
        byte[] secondHash)
    {
        var bytes = InfoWithTroiOnSecondResponse(firstText, firstHash, secondText, secondHash);

        foreach (var info in ReadBoth(modKey, bytes))
        {
            info.Responses.Count.ShouldBe(2);

            var first = info.Responses[0];
            first.TROI.IsNull.ShouldBeTrue();
            first.ResponseText.String.ShouldBe(firstText);
            first.TextHash!.Value.ToArray().ShouldBe(firstHash);

            var second = info.Responses[1];
            second.TROI.FormKeyNullable.ShouldBe(new FormKey(modKey, LinkedInfoId));
            second.ResponseText.String.ShouldBe(secondText);
            second.TextHash!.Value.ToArray().ShouldBe(secondHash);
        }
    }

    [Theory, MutagenAutoData(GameRelease.Starfield)]
    public void TroiAfterTrda_RoundTrips(
        ModKey modKey,
        string firstText,
        byte[] firstHash,
        string secondText,
        byte[] secondHash)
    {
        var bytes = InfoWithTroiOnSecondResponse(firstText, firstHash, secondText, secondHash);

        foreach (var info in ReadBoth(modKey, bytes))
        {
            Write(modKey, info).ShouldBe(bytes);
        }
    }
}
