using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Xunit;
using SkyrimConstants = Mutagen.Bethesda.Skyrim.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class WeatherVolumetricLightingTests
{
    private const ushort FormVersionBelowFormerGate = 40;
    private const ushort SpecialEditionFormVersion = 44;

    private static readonly MasterReferenceCollection Masters = new(SkyrimConstants.Skyrim);
    private static readonly uint[] VolumetricLightingIds = [0x00012301, 0x00012302, 0x00012303, 0x00012304];

    private static byte[] WeatherWithVolumetricLighting(ushort formVersion)
    {
        var content = Subrecord("HNAM", VolumetricLightingIds.SelectMany(BitConverter.GetBytes).ToArray());
        var header = new byte[GameConstants.SkyrimSE.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("WTHR").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), formVersion);
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

    private static IWeatherGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return Weather.CreateFromBinary(new MutagenFrame(reader));
    }

    private static IWeatherGetter ParseOverlay(byte[] bytes)
    {
        return WeatherBinaryOverlay.WeatherFactory(bytes, new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(IWeatherGetter item)
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
            WeatherBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static void ShouldHoldTheVolumetricLighting(IWeatherGetter weather)
    {
        var lighting = weather.VolumetricLighting.ShouldNotBeNull();
        new[] { lighting.Sunrise, lighting.Day, lighting.Sunset, lighting.Night }
            .Select(x => x.FormKey)
            .ShouldBe(VolumetricLightingIds.Select(id => new FormKey(SkyrimConstants.Skyrim, id)));
    }

    [Theory]
    [InlineData(FormVersionBelowFormerGate)]
    [InlineData(SpecialEditionFormVersion)]
    public void DeepParse_ReadsTheVolumetricLightingOfAnyFormVersion(ushort formVersion)
    {
        ShouldHoldTheVolumetricLighting(ParseDeep(WeatherWithVolumetricLighting(formVersion)));
    }

    [Theory]
    [InlineData(FormVersionBelowFormerGate)]
    [InlineData(SpecialEditionFormVersion)]
    public void DeepParse_WritesTheVolumetricLightingItRead(ushort formVersion)
    {
        ShouldHoldTheVolumetricLighting(ParseOverlay(Write(ParseDeep(WeatherWithVolumetricLighting(formVersion)))));
    }

    [Theory]
    [InlineData(FormVersionBelowFormerGate)]
    [InlineData(SpecialEditionFormVersion)]
    public void Overlay_WritesTheVolumetricLightingItRead(ushort formVersion)
    {
        ShouldHoldTheVolumetricLighting(ParseDeep(Write(ParseOverlay(WeatherWithVolumetricLighting(formVersion)))));
    }
}
