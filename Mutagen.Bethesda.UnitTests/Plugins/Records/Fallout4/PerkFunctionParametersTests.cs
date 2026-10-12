using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class PerkFunctionParametersTests
{
    private const byte AbilityEffectType = 1;
    private const byte EntryPointEffectType = 2;
    private const uint AbilitySpell = 0x00000800;
    private const ushort PerkEntryId = 10;
    private const byte ActivateEntryPoint = 0x0E;
    private const byte AddActivateChoice = 9;
    private const byte SpellWithStrings = 4;
    private const string ButtonLabel = "Label";
    private const ushort RunImmediately = 0x0001;
    private const ushort FormVersion = 131;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] PerkRecord(params byte[][] effectSubrecords)
    {
        var content = Subrecord("DATA", new byte[5]).Concat(effectSubrecords.SelectMany(x => x)).ToArray();
        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("PERK").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), FormVersion);
        return header.Concat(content).ToArray();
    }

    private static byte[] ActivateChoicePerk(ushort[] epf3) => PerkRecord(
        [
            Subrecord("PRKE", [EntryPointEffectType, 0, 0]),
            Subrecord("DATA", [ActivateEntryPoint, AddActivateChoice, 0]),
            Subrecord("EPFT", [SpellWithStrings]),
            Subrecord("EPF2", Encoding.ASCII.GetBytes(ButtonLabel + "\0")),
            .. epf3.Select(flags => Subrecord("EPF3", BitConverter.GetBytes(flags))),
            Subrecord("PRKF", []),
        ]);

    private static byte[] AbilityPerkCarryingParameters() => PerkRecord(
        Subrecord("PRKE", [AbilityEffectType, 0, 0]),
        Subrecord("DATA", BitConverter.GetBytes(AbilitySpell)),
        Subrecord("EPFB", BitConverter.GetBytes(PerkEntryId)),
        Subrecord("EPF2", Encoding.ASCII.GetBytes(ButtonLabel + "\0")),
        Subrecord("EPF3", BitConverter.GetBytes(RunImmediately)),
        Subrecord("PRKF", []));

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static ParsingMeta Meta() => new(
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
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
                   new WritingBundle(GameConstants.Fallout4)
                   {
                       MasterReferences = Masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(Masters),
                   }))
        {
            PerkBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static IReadOnlyList<ushort> WrittenFlags(byte[] record) =>
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                GameConstants.Fallout4,
                RecordTypes.EPF3)
            .Select(x => BinaryPrimitives.ReadUInt16LittleEndian(x.Content))
            .ToList();

    private static void ShouldHoldTheChoice(IPerkGetter perk, ushort? flags)
    {
        var choice = perk.Effects.Single().ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull();
        choice.ButtonLabel.ShouldNotBeNull().String.ShouldBe(ButtonLabel);
        ((ushort?)choice.Flags).ShouldBe(flags);
    }

    private static void ShouldHoldTheAbilityParameters(IPerkGetter perk)
    {
        var ability = perk.Effects.Single().ShouldBeAssignableTo<IPerkAbilityEffectGetter>().ShouldNotBeNull();
        ability.PerkEntryID.ShouldBe(PerkEntryId);
        ability.ButtonLabel.ShouldNotBeNull().String.ShouldBe(ButtonLabel);
        ((ushort?)ability.Flags).ShouldBe(RunImmediately);
    }

    public static TheoryData<ushort?, ushort[]> FlagsAndTheirEpf3 => new()
    {
        { null, [] },
        { RunImmediately, [RunImmediately] },
    };

    [Theory]
    [MemberData(nameof(FlagsAndTheirEpf3))]
    public void DeepParse_AChoiceWithOrWithoutEpf3_ReadsItsFlags(ushort? flags, ushort[] epf3)
    {
        ShouldHoldTheChoice(ParseDeep(ActivateChoicePerk(epf3)), flags);
    }

    [Theory]
    [MemberData(nameof(FlagsAndTheirEpf3))]
    public void Overlay_AChoiceWithOrWithoutEpf3_ReadsItsFlags(ushort? flags, ushort[] epf3)
    {
        ShouldHoldTheChoice(ParseOverlay(ActivateChoicePerk(epf3)), flags);
    }

    [Theory]
    [MemberData(nameof(FlagsAndTheirEpf3))]
    public void DeepParse_WritesEpf3OnlyForFlagsItRead_AndTheOverlayRereadsThem(ushort? flags, ushort[] epf3)
    {
        var written = Write(ParseDeep(ActivateChoicePerk(epf3)));
        WrittenFlags(written).ShouldBe(epf3);
        ShouldHoldTheChoice(ParseOverlay(written), flags);
    }

    [Theory]
    [MemberData(nameof(FlagsAndTheirEpf3))]
    public void Overlay_WritesEpf3OnlyForFlagsItRead_AndTheDeepParseRereadsThem(ushort? flags, ushort[] epf3)
    {
        var written = Write(ParseOverlay(ActivateChoicePerk(epf3)));
        WrittenFlags(written).ShouldBe(epf3);
        ShouldHoldTheChoice(ParseDeep(written), flags);
    }

    [Fact]
    public void DeepParse_AnAbilityCarryingFunctionParameters_WritesThem_AndTheOverlayRereadsThem()
    {
        ShouldHoldTheAbilityParameters(ParseOverlay(Write(ParseDeep(AbilityPerkCarryingParameters()))));
    }

    [Fact]
    public void Overlay_AnAbilityCarryingFunctionParameters_WritesThem_AndTheDeepParseRereadsThem()
    {
        ShouldHoldTheAbilityParameters(ParseDeep(Write(ParseOverlay(AbilityPerkCarryingParameters()))));
    }
}
