using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4;

public partial class Weather
{
    [Flags]
    public enum Flag
    {
        Pleasant = 0x001,
        Cloudy = 0x002,
        Rainy = 0x004,
        Snow = 0x008,
        SkyStaticsAlwaysVisible = 0x010,
        SkyStaticsFollowsSunPosition = 0x020,
        RainOcclusion = 0x040,
        HudRainEffects = 0x080,
    }
}

partial class WeatherBinaryCreateTranslation
{
    public const int TextureIntBase = 0x58543030;
    public static uint OldVersion = 109;
    public static int NumLayersOld => 16;
    public static int NumLayersNew => 32;
    public static int NumLayers(uint formVersion) => formVersion <= OldVersion ? NumLayersOld : NumLayersNew;

    public static float ConvertToSpeed(byte b)
    {
        return (b - 127) / 127f / 10f;
    }

    public static void FillCloudTexture(IMutagenReadStream stream, RecordType nextRecordType, string?[] textures)
    {
        int layer = nextRecordType.TypeInt - TextureIntBase;
        if (layer > 29 || layer < 0)
        {
            throw new ArgumentException();
        }
        var subRec = stream.ReadSubrecord();
        textures[layer] = BinaryStringUtility.ProcessWholeToZString(subRec.Content, stream.MetaData.Encodings.NonTranslated);
    }

    public static partial ParseResult FillBinaryCloudAlphasCustom(MutagenFrame frame, IWeatherInternal item, PreviousParse lastParsed)
    {
        FillBinaryCloudAlphas(frame, item.Clouds, item.FormVersion);
        return null;
    }

    public static void FillBinaryCloudAlphas(MutagenFrame frame, CloudLayer[] clouds, uint formVersion)
    {
        frame.ReadSubrecordHeader();
        for (int i = 0; i < NumLayers(formVersion); i++)
        {
            if (frame.Complete) break;
            clouds[i].Alphas = new WeatherAlpha()
            {
                Sunrise = frame.ReadFloat(),
                Day = frame.ReadFloat(),
                Sunset = frame.ReadFloat(),
                Night = frame.ReadFloat(),
                EarlySunrise = frame.ReadFloat(),
                LateSunrise = frame.ReadFloat(),
                EarlySunset = frame.ReadFloat(),
                LateSunset = frame.ReadFloat(),
            };
        }
    }

    public static partial ParseResult FillBinaryCloudColorsCustom(MutagenFrame frame, IWeatherInternal item, PreviousParse lastParsed)
    {
        FillBinaryCloudColors(frame, item.Clouds, item.FormVersion);
        return null;
    }

    public static void FillBinaryCloudColors(MutagenFrame frame, CloudLayer[] clouds, uint formVersion)
    {
        var rec = frame.ReadSubrecordHeader();
        frame = frame.SpawnWithLength(rec.ContentLength);
        for (int i = 0; i < NumLayers(formVersion); i++)
        {
            if (frame.Complete) return;
            clouds[i].Colors = new WeatherColor()
            {
                Sunrise = frame.ReadColor(ColorBinaryType.Alpha),
                Day = frame.ReadColor(ColorBinaryType.Alpha),
                Sunset = frame.ReadColor(ColorBinaryType.Alpha),
                Night = frame.ReadColor(ColorBinaryType.Alpha),
                EarlySunrise = frame.ReadColor(ColorBinaryType.Alpha),
                LateSunrise = frame.ReadColor(ColorBinaryType.Alpha),
                EarlySunset = frame.ReadColor(ColorBinaryType.Alpha),
                LateSunset = frame.ReadColor(ColorBinaryType.Alpha),
            };
        }
    }

    public static partial void FillBinaryCloudsCustom(MutagenFrame frame, IWeatherInternal item, PreviousParse lastParsed)
    {
        FillBinaryCloudYSpeeds(frame, item.Clouds);
    }

    public static void FillBinaryCloudYSpeeds(MutagenFrame frame, CloudLayer[] clouds)
    {
        frame.ReadSubrecordHeader();
        for (int i = 0; i < NumLayersNew; i++)
        {
            clouds[i].YSpeed = ConvertToSpeed(frame.ReadUInt8());
        }
    }

    public static partial ParseResult FillBinaryCloudXSpeedsCustom(MutagenFrame frame, IWeatherInternal item, PreviousParse lastParsed)
    {
        FillBinaryCloudXSpeeds(frame, item.Clouds);
        return null;
    }

    public static void FillBinaryCloudXSpeeds(MutagenFrame frame, CloudLayer[] clouds)
    {
        frame.ReadSubrecordHeader();
        for (int i = 0; i < NumLayersNew; i++)
        {
            clouds[i].XSpeed = ConvertToSpeed(frame.ReadUInt8());
        }
    }

    public static partial ParseResult FillBinaryDisabledCloudLayersCustom(MutagenFrame frame, IWeatherInternal item, PreviousParse lastParsed)
    {
        FillBinaryDisabledCloudLayers(frame, item.Clouds);
        return null;
    }

    public static void FillBinaryDisabledCloudLayers(MutagenFrame frame, CloudLayer[] clouds)
    {
        var subRec = frame.ReadSubrecord();
        var raw = BinaryPrimitives.ReadUInt32LittleEndian(subRec.Content);
        uint index = 1;
        for (int i = 0; i < NumLayersNew; i++)
        {
            // Inverse because we're exposing as enabled
            clouds[i].Enabled = !Enums.HasFlag(raw, index);
            index <<= 1;
        }
    }

    public static partial void FillBinaryDirectionalAmbientLightingColorsCustom(MutagenFrame frame, IWeatherInternal item, PreviousParse lastParsed)
    {
        item.DirectionalAmbientLightingColors = GetBinaryDirectionalAmbientLightingColors(frame);
    }

    public static partial void FillBinaryCloudTexturesParseCustom(MutagenFrame frame, IWeatherInternal item)
    {
    }

    public static WeatherAmbientColorSet GetBinaryDirectionalAmbientLightingColors(MutagenFrame frame)
    {
        AmbientColors Parse()
        {
            var subMeta = frame.GetSubrecordHeader();
            if (subMeta.RecordType != RecordTypes.DALC)
            {
                return new AmbientColors();
            }
            frame.ReadSubrecordHeader();
            return AmbientColors.CreateFromBinary(frame.SpawnWithLength(subMeta.ContentLength, checkFraming: false));
        }

        return new WeatherAmbientColorSet()
        {
            Sunrise = Parse(),
            Day = Parse(),
            Sunset = Parse(),
            Night = Parse(),
            EarlySunrise = Parse(),
            LateSunrise = Parse(),
            EarlySunset = Parse(),
            LateSunset = Parse(),
        };
    }

    public static ParseResult CustomRecordFallback(
        IWeatherInternal item,
        MutagenFrame frame,
        PreviousParse lastParsed,
        Dictionary<RecordType, int>? recordParseCount,
        RecordType nextRecordType,
        int contentLength,
        TypedParseParams translationParams = default)
    {
        if (nextRecordType == RecordTypes.EDID)
        {
            return Fallout4MajorRecordBinaryCreateTranslation.FillBinaryRecordTypes(
                item: item,
                frame: frame,
                lastParsed: lastParsed,
                recordParseCount: recordParseCount,
                nextRecordType: nextRecordType,
                contentLength: contentLength,
                translationParams: translationParams);
        }
        WeatherBinaryCreateTranslation.FillCloudTexture(frame, nextRecordType, item.CloudTextures);
        return default(int?);
    }
}

partial class WeatherBinaryWriteTranslation
{
    public static byte ConvertFromSpeed(float f)
    {
        return (byte)Math.Round((f * 10 * 127) + 127);
    }

    public static partial void WriteBinaryCloudTexturesParseCustom(MutagenWriter writer, IWeatherGetter item)
    {
        var cloudTex = item.CloudTextures;
        for (int i = 0; i < cloudTex.Length; i++)
        {
            if (cloudTex[i] is not { } tex) continue;
            using (HeaderExport.Subrecord(writer, new RecordType(WeatherBinaryCreateTranslation.TextureIntBase + i)))
            {
                writer.Write(tex, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
            }
        }
    }

    public static partial void WriteBinaryCloudsCustom(MutagenWriter writer, IWeatherGetter item)
    {
        bool HasAny(Span<float?> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i].HasValue) return true;
            }
            return false;
        }

        Span<float?> xSpeeds = stackalloc float?[WeatherBinaryCreateTranslation.NumLayersNew];
        Span<float?> ySpeeds = stackalloc float?[WeatherBinaryCreateTranslation.NumLayersNew];
        var alphas = new IWeatherAlphaGetter?[WeatherBinaryCreateTranslation.NumLayersNew];
        var colors = new IWeatherColorGetter?[WeatherBinaryCreateTranslation.NumLayersNew];

        foreach (var cloud in item.Clouds.WithIndex())
        {
            xSpeeds[cloud.Index] = cloud.Item.XSpeed;
            ySpeeds[cloud.Index] = cloud.Item.YSpeed;
            alphas[cloud.Index] = cloud.Item.Alphas;
            colors[cloud.Index] = cloud.Item.Colors;
        }

        if (HasAny(ySpeeds)
            || HasAny(xSpeeds))
        {
            // Write YSpeeds
            using (HeaderExport.Subrecord(writer, RecordTypes.RNAM))
            {
                for (int i = 0; i < ySpeeds.Length; i++)
                {
                    writer.Write(ConvertFromSpeed(ySpeeds[i] ?? default(byte)));
                }
            }

            // Write XSpeeds
            using (HeaderExport.Subrecord(writer, RecordTypes.QNAM))
            {
                for (int i = 0; i < xSpeeds.Length; i++)
                {
                    writer.Write(ConvertFromSpeed(xSpeeds[i] ?? default(byte)));
                }
            }
        }

        var numLayers = WeatherBinaryCreateTranslation.NumLayers(item.FormVersion);

        // Write colors
        if (colors.Any(a => a != null))
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.PNAM))
            {
                for (int i = 0; i < numLayers; i++)
                {
                    var color = colors[i];
                    if (color == null)
                    {
                        writer.WriteZeros((uint)numLayers);
                    }
                    else
                    {
                        ColorBinaryTranslation.Instance.Write(writer, color.Sunrise);
                        ColorBinaryTranslation.Instance.Write(writer, color.Day);
                        ColorBinaryTranslation.Instance.Write(writer, color.Sunset);
                        ColorBinaryTranslation.Instance.Write(writer, color.Night);
                        ColorBinaryTranslation.Instance.Write(writer, color.EarlySunrise);
                        ColorBinaryTranslation.Instance.Write(writer, color.LateSunrise);
                        ColorBinaryTranslation.Instance.Write(writer, color.EarlySunset);
                        ColorBinaryTranslation.Instance.Write(writer, color.LateSunset);
                    }
                }
            }
        }

        // Write alphas
        if (alphas.Any(a => a != null))
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.JNAM))
            {
                for (int i = 0; i < numLayers; i++)
                {
                    var alpha = alphas[i];
                    if (alpha == null)
                    {
                        writer.WriteZeros(32);
                    }
                    else
                    {
                        writer.Write(alpha.Sunrise);
                        writer.Write(alpha.Day);
                        writer.Write(alpha.Sunset);
                        writer.Write(alpha.Night);
                        writer.Write(alpha.EarlySunrise);
                        writer.Write(alpha.LateSunrise);
                        writer.Write(alpha.EarlySunset);
                        writer.Write(alpha.LateSunset);
                    }
                }
            }
        }

    }

    // Other partials handled in clouds custom ^
    #region Unused
    public static partial void WriteBinaryCloudAlphasCustom(MutagenWriter writer, IWeatherGetter item)
    {
    }

    public static partial void WriteBinaryCloudColorsCustom(MutagenWriter writer, IWeatherGetter item)
    {
    }

    public static partial void WriteBinaryCloudXSpeedsCustom(MutagenWriter writer, IWeatherGetter item)
    {
    }
    #endregion

    public static partial void WriteBinaryDisabledCloudLayersCustom(MutagenWriter writer, IWeatherGetter item)
    {
        var version = item.FormVersion;
        var clouds = item.Clouds;
        Span<bool?> enabled = stackalloc bool?[WeatherBinaryCreateTranslation.NumLayersNew];
        bool any = false;
        for (int i = 0; i < WeatherBinaryCreateTranslation.NumLayersNew; i++)
        {
            enabled[i] = clouds[i].Enabled;
            any |= enabled[i].HasValue;
        }
        if (!any) return;

        using (HeaderExport.Subrecord(writer, RecordTypes.NAM1))
        {
            uint raw = 0;
            uint index = 1;
            for (int i = 0; i < WeatherBinaryCreateTranslation.NumLayersNew; i++)
            {
                var enable = enabled[i] ?? true;
                // Inverse because we're exposing as enabled
                raw = Enums.SetFlag(raw, index, !enable);
                index <<= 1;
            }
            writer.Write(raw);
        }
    }

    public static partial void WriteBinaryDirectionalAmbientLightingColorsCustom(MutagenWriter writer, IWeatherGetter item)
    {
        if (item.DirectionalAmbientLightingColors is not { } colors) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.Sunrise.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.Day.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.Sunset.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.Night.WriteToBinary(writer);
        }
        if (item.FormVersion <= WeatherBinaryCreateTranslation.OldVersion) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.EarlySunrise.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.LateSunrise.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.EarlySunset.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DALC))
        {
            colors.LateSunset.WriteToBinary(writer);
        }
    }
}

partial class WeatherBinaryOverlay
{
    private readonly string?[] _cloudTextures = new string?[29];
    public ReadOnlyMemorySlice<string?> CloudTextures => _cloudTextures;

    private readonly CloudLayer[] _clouds = ArrayExt.Create(WeatherBinaryCreateTranslation.NumLayersNew, (i) => new CloudLayer());
    public ReadOnlyMemorySlice<ICloudLayerGetter> Clouds => _clouds;

    int? _directionalLoc;

    partial void CloudsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        WeatherBinaryCreateTranslation.FillBinaryCloudYSpeeds(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            _clouds);
    }

    public partial ParseResult CloudXSpeedsCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        WeatherBinaryCreateTranslation.FillBinaryCloudXSpeeds(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            _clouds);
        return null;
    }

    public partial ParseResult CloudAlphasCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        WeatherBinaryCreateTranslation.FillBinaryCloudAlphas(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            _clouds,
            FormVersion);
        return null;
    }

    public partial ParseResult CloudColorsCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        WeatherBinaryCreateTranslation.FillBinaryCloudColors(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            _clouds,
            FormVersion);
        return null;
    }

    public partial ParseResult DisabledCloudLayersCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        WeatherBinaryCreateTranslation.FillBinaryDisabledCloudLayers(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            _clouds);
        return null;
    }

    partial void DirectionalAmbientLightingColorsCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        if (_directionalLoc.HasValue) return;
        _directionalLoc = (ushort)(stream.Position - offset);
    }

    public partial IWeatherAmbientColorSetGetter? GetDirectionalAmbientLightingColorsCustom()
    {
        if (!_directionalLoc.HasValue) return null;
        return WeatherBinaryCreateTranslation.GetBinaryDirectionalAmbientLightingColors(
            new MutagenFrame(new MutagenMemoryReadStream(_recordData.Slice(_directionalLoc.Value), _package.MetaData)));
    }

    private ParseResult CustomRecordFallback(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        TypedParseParams translationParams = default)
    {
        if (type == RecordTypes.EDID)
        {
            return base.FillRecordType(
                stream: stream,
                finalPos: finalPos,
                offset: offset,
                type: type,
                recordParseCount: null,
                lastParsed: lastParsed,
                translationParams: translationParams);
        }
        WeatherBinaryCreateTranslation.FillCloudTexture(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            type,
            _cloudTextures);
        return default(int?);
    }
}
