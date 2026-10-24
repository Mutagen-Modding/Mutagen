using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class WeatherFogDistanceDefaultTests
{
    private const int FloatsBeforeFirstBreak = 8;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildWthrBytes(int fnamFloats)
    {
        var content = Subrecord("FNAM", new byte[fnamFloats * 4]);

        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("WTHR").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), 0x00000001);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), 131);

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
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static Weather ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return Weather.CreateFromBinary(new MutagenFrame(reader));
    }

    private static IWeatherGetter ParseOverlay(byte[] bytes)
    {
        return WeatherBinaryOverlay.WeatherFactory(
            bytes,
            new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(IWeatherGetter item)
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
            WeatherBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static void ShouldHoldTheDefaultsAfterTheBreak(IWeatherGetter item)
    {
        item.FogDistanceDayNearHeightRange.ShouldBe(Weather.FogDistanceDayNearHeightRangeDefault);
        item.FogDistanceNightNearHeightRange.ShouldBe(Weather.FogDistanceNightNearHeightRangeDefault);
        item.FogDistanceDayHighDensityScale.ShouldBe(Weather.FogDistanceDayHighDensityScaleDefault);
        item.FogDistanceNightHighDensityScale.ShouldBe(Weather.FogDistanceNightHighDensityScaleDefault);
        item.FogDistanceDayFarHeightRange.ShouldBe(Weather.FogDistanceDayFarHeightRangeDefault);
        item.FogDistanceNightFarHeightRange.ShouldBe(Weather.FogDistanceNightFarHeightRangeDefault);
    }

    [Fact]
    public void New_HoldsTheDefaults()
    {
        ShouldHoldTheDefaultsAfterTheBreak(new Weather(FormKey.Factory("123456:Fallout4.esm"), Fallout4Release.Fallout4));
    }

    [Fact]
    public void DeepParse_FnamEndingAtTheFirstBreak_HoldsTheDefaults()
    {
        ShouldHoldTheDefaultsAfterTheBreak(ParseDeep(BuildWthrBytes(FloatsBeforeFirstBreak)));
    }

    [Fact]
    public void Overlay_FnamEndingAtTheFirstBreak_HoldsTheDefaults()
    {
        ShouldHoldTheDefaultsAfterTheBreak(ParseOverlay(BuildWthrBytes(FloatsBeforeFirstBreak)));
    }

    [Fact]
    public void DeepParse_FnamEndingAtTheFirstBreak_WritesItBackAtTheBreak()
    {
        var written = Write(ParseDeep(BuildWthrBytes(FloatsBeforeFirstBreak)));
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(written).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                GameConstants.Fallout4,
                RecordTypes.FNAM)
            .Single().ContentLength.ShouldBe(FloatsBeforeFirstBreak * 4);
        ShouldHoldTheDefaultsAfterTheBreak(ParseOverlay(written));
    }
}
