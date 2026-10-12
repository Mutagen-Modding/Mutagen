using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Xunit;
using SkyrimConstants = Mutagen.Bethesda.Skyrim.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class PerkFunctionParametersTests
{
    private const byte AbilityEffectType = 1;
    private const uint AbilitySpell = 0x00000800;
    private const string ButtonLabel = "Label";

    private static readonly MasterReferenceCollection Masters = new(SkyrimConstants.Skyrim);

    private static byte[] AbilityPerkWithAButtonLabel()
    {
        byte[][] subrecords =
        [
            Subrecord("DATA", new byte[5]),
            Subrecord("PRKE", [AbilityEffectType, 0, 0]),
            Subrecord("DATA", BitConverter.GetBytes(AbilitySpell)),
            Subrecord("EPF2", Encoding.ASCII.GetBytes(ButtonLabel + "\0")),
            Subrecord("PRKF", []),
        ];
        var content = subrecords.SelectMany(x => x).ToArray();
        var header = new byte[GameConstants.SkyrimSE.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("PERK").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        return header.Concat(content).ToArray();
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static ParsingMeta Meta() => new(
        GameConstants.SkyrimSE,
        SkyrimConstants.Skyrim,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static IPerkGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return Perk.CreateFromBinary(new MutagenFrame(reader));
    }

    private static IPerkGetter ParseOverlay(byte[] bytes)
    {
        return PerkBinaryOverlay.PerkFactory(bytes, new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(IPerkGetter item)
    {
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(
                   stream,
                   new WritingBundle(GameConstants.SkyrimSE)
                   {
                       MasterReferences = Masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(Masters),
                   }))
        {
            PerkBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static void ShouldHoldTheLabel(IPerkGetter perk)
    {
        perk.Effects.Single().ShouldBeAssignableTo<IPerkAbilityEffectGetter>().ShouldNotBeNull()
            .ButtonLabel.ShouldNotBeNull().String.ShouldBe(ButtonLabel);
    }

    [Fact]
    public void DeepParse_AnAbilityWithAButtonLabel_WritesIt_AndTheOverlayRereadsIt()
    {
        ShouldHoldTheLabel(ParseOverlay(Write(ParseDeep(AbilityPerkWithAButtonLabel()))));
    }

    [Fact]
    public void Overlay_AnAbilityWithAButtonLabel_WritesIt_AndTheDeepParseRereadsIt()
    {
        ShouldHoldTheLabel(ParseDeep(Write(ParseOverlay(AbilityPerkWithAButtonLabel()))));
    }
}
