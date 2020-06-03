using DynamicData.Annotations;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Weather
    {
        public enum ColorType
        {
            SkyUpper,
            FogNear,
            Unknown,
            Ambient,
            Sunlight,
            Sun,
            Stars,
            SkyLower,
            Horizon,
            EffectLighting,
            CloudLodDiffuse,
            CloudLodAmbient,
            FogFar,
            SkyStatics,
            WaterMultiplier,
            SunGlare,
            MoonGlare
        }

        [Flags]
        public enum Flag
        {
            Pleasent = 0x001,
            Cloudy = 0x002,
            Rainy = 0x004,
            Snow = 0x008,
            SkyStaticsAlwaysVisible = 0x010,
            SkyStaticsFollowsSunPosition = 0x020,
        }
    }

    namespace Internals
    {
        public partial class WeatherBinaryCreateTranslation
        {
            public const int NumLayers = 32;
            public const int TextureIntBase = 0x58543030;

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
                var subRec = stream.ReadSubrecordFrame();
                textures[layer] = BinaryStringUtility.ProcessWholeToZString(subRec.Content);
            }

            static partial void FillBinaryCloudAlphasCustom(MutagenFrame frame, IWeatherInternal item)
            {
                FillBinaryCloudAlphas(frame, item.Clouds);
            }

            public static void FillBinaryCloudAlphas(MutagenFrame frame, CloudLayer[] clouds)
            {
                frame.ReadSubrecord();
                for (int i = 0; i < NumLayers; i++)
                {
                    clouds[i].Alphas = new WeatherAlpha()
                    {
                        Sunrise = frame.ReadFloat(),
                        Day = frame.ReadFloat(),
                        Sunset = frame.ReadFloat(),
                        Night = frame.ReadFloat(),
                    };
                }
            }

            static partial void FillBinaryCloudColorsCustom(MutagenFrame frame, IWeatherInternal item)
            {
                FillBinaryCloudColors(frame, item.Clouds);
            }

            public static void FillBinaryCloudColors(MutagenFrame frame, CloudLayer[] clouds)
            {
                var rec = frame.ReadSubrecord();
                frame = frame.SpawnWithLength(rec.ContentLength);
                for (int i = 0; i < NumLayers; i++)
                {
                    if (frame.Complete) return;
                    clouds[i].Colors = new WeatherColor()
                    {
                        Sunrise = frame.ReadColor(ColorBinaryType.Alpha),
                        Day = frame.ReadColor(ColorBinaryType.Alpha),
                        Sunset = frame.ReadColor(ColorBinaryType.Alpha),
                        Night = frame.ReadColor(ColorBinaryType.Alpha),
                    };
                }
            }

            static partial void FillBinaryCloudsCustom(MutagenFrame frame, IWeatherInternal item)
            {
                FillBinaryCloudYSpeeds(frame, item.Clouds);
            }

            public static void FillBinaryCloudYSpeeds(MutagenFrame frame, CloudLayer[] clouds)
            {
                frame.ReadSubrecord();
                for (int i = 0; i < NumLayers; i++)
                {
                    clouds[i].YSpeed = ConvertToSpeed(frame.ReadUInt8());
                }
            }

            static partial void FillBinaryCloudXSpeedsCustom(MutagenFrame frame, IWeatherInternal item)
            {
                FillBinaryCloudXSpeeds(frame, item.Clouds);
            }

            public static void FillBinaryCloudXSpeeds(MutagenFrame frame, CloudLayer[] clouds)
            {
                frame.ReadSubrecord();
                for (int i = 0; i < NumLayers; i++)
                {
                    clouds[i].XSpeed = ConvertToSpeed(frame.ReadUInt8());
                }
            }

            static partial void FillBinaryDisabledCloudLayersCustom(MutagenFrame frame, IWeatherInternal item)
            {
                FillBinaryDisabledCloudLayers(frame, item.Clouds);
            }

            public static void FillBinaryDisabledCloudLayers(MutagenFrame frame, CloudLayer[] clouds)
            {
                var subRec = frame.ReadSubrecordFrame();
                var raw = BinaryPrimitives.ReadUInt32LittleEndian(subRec.Content);
                uint index = 1;
                for (int i = 0; i < NumLayers; i++)
                {
                    // Inverse because we're exposing as enabled
                    clouds[i].Enabled = !EnumExt.HasFlag(raw, index);
                    index <<= 1;
                }
            }

            static partial void FillBinaryDirectionalAmbientLightingColorsCustom(MutagenFrame frame, IWeatherInternal item)
            {
                item.DirectionalAmbientLightingColors = GetBinaryDirectionalAmbientLightingColors(frame);
            }

            public static WeatherAmbientColorSet GetBinaryDirectionalAmbientLightingColors(MutagenFrame frame)
            {
                WeatherAmbientColors Parse()
                {
                    var subMeta = frame.ReadSubrecord();
                    if (subMeta.RecordType != Weather_Registration.DALC_HEADER)
                    {
                        throw new ArgumentException();
                    }
                    return WeatherAmbientColors.CreateFromBinary(frame.SpawnWithLength(subMeta.ContentLength, checkFraming: false));
                }

                return new WeatherAmbientColorSet()
                {
                    Sunrise = Parse(),
                    Day = Parse(),
                    Sunset = Parse(),
                    Night = Parse(),
                };
            }

            public static TryGet<int?> CustomRecordFallback(
                IWeatherInternal item,
                MutagenFrame frame,
                RecordType nextRecordType,
                int contentLength,
                RecordTypeConverter? recordTypeConverter = null)
            {
                if (nextRecordType.TypeInt == 0x44494445) // EDID
                {
                    return SkyrimMajorRecordBinaryCreateTranslation.FillBinaryRecordTypes(
                        item: item,
                        frame: frame,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        recordTypeConverter: recordTypeConverter);
                }
                WeatherBinaryCreateTranslation.FillCloudTexture(frame, nextRecordType, item.CloudTextures);
                return TryGet<int?>.Succeed(null);
            }
        }

        public partial class WeatherBinaryWriteTranslation
        {
            public static byte ConvertFromSpeed(float f)
            {
                return (byte)Math.Round((f * 10 * 127) + 127);
            }

            static partial void WriteBinaryCloudTexturesParseCustom(MutagenWriter writer, IWeatherGetter item)
            {
                var cloudTex = item.CloudTextures;
                for (int i = 0; i < cloudTex.Length; i++)
                {
                    if (!cloudTex[i].TryGet(out var tex)) continue;
                    using (HeaderExport.ExportSubrecordHeader(writer, new RecordType(WeatherBinaryCreateTranslation.TextureIntBase + i)))
                    {
                        writer.Write(tex, StringBinaryType.NullTerminate);
                    }
                }
            }

            static partial void WriteBinaryCloudsCustom(MutagenWriter writer, IWeatherGetter item)
            {
                bool HasAny(Span<float?> span)
                {
                    for (int i = 0; i < span.Length; i++)
                    {
                        if (span[i].HasValue) return true;
                    }
                    return false;
                }
                var version = item.FormVersion;

                Span<float?> xSpeeds = stackalloc float?[WeatherBinaryCreateTranslation.NumLayers];
                Span<float?> ySpeeds = stackalloc float?[WeatherBinaryCreateTranslation.NumLayers];
                var alphas = new IWeatherAlphaGetter?[WeatherBinaryCreateTranslation.NumLayers];
                var colors = new IWeatherColorGetter?[WeatherBinaryCreateTranslation.NumLayers];

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
                    using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.RNAM_HEADER))
                    {
                        for (int i = 0; i < ySpeeds.Length; i++)
                        {
                            writer.Write(ConvertFromSpeed(ySpeeds[i] ?? default(byte)));
                        }
                    }

                    // Write XSpeeds
                    using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.QNAM_HEADER))
                    {
                        for (int i = 0; i < xSpeeds.Length; i++)
                        {
                            writer.Write(ConvertFromSpeed(xSpeeds[i] ?? default(byte)));
                        }
                    }
                }

                // Write colors
                if (colors.Any(a => a != null))
                {
                    using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.PNAM_HEADER))
                    {
                        for (int i = 0; i < colors.Length; i++)
                        {
                            var color = colors[i];
                            if (color == null)
                            {
                                writer.WriteZeros(16);
                            }
                            else
                            {
                                ColorBinaryTranslation.Instance.Write(writer, color.Sunrise);
                                ColorBinaryTranslation.Instance.Write(writer, color.Day);
                                ColorBinaryTranslation.Instance.Write(writer, color.Sunset);
                                ColorBinaryTranslation.Instance.Write(writer, color.Night);
                            }

                            // Special case for older version
                            if (version == 31 && i == 3)
                            {
                                break;
                            }
                        }
                    }
                }

                // Write alphas
                if (alphas.Any(a => a != null))
                {
                    using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.JNAM_HEADER))
                    {
                        for (int i = 0; i < alphas.Length; i++)
                        {
                            var alpha = alphas[i];
                            if (alpha == null)
                            {
                                writer.WriteZeros(16);
                            }
                            else
                            {
                                writer.Write(alpha.Sunrise);
                                writer.Write(alpha.Day);
                                writer.Write(alpha.Sunset);
                                writer.Write(alpha.Night);
                            }
                        }
                    }
                }

            }

            // Other partials handled in clouds custom ^

            static partial void WriteBinaryDisabledCloudLayersCustom(MutagenWriter writer, IWeatherGetter item)
            {
                var clouds = item.Clouds;
                Span<bool?> enabled = stackalloc bool?[WeatherBinaryCreateTranslation.NumLayers];
                bool any = false;
                for (int i = 0; i < WeatherBinaryCreateTranslation.NumLayers; i++)
                {
                    enabled[i] = clouds[i].Enabled;
                    any |= enabled[i].HasValue;
                }
                if (!any) return;

                using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.NAM1_HEADER))
                {
                    uint raw = 0;
                    uint index = 1;
                    for (int i = 0; i < WeatherBinaryCreateTranslation.NumLayers; i++)
                    {
                        var enable = enabled[i] ?? true;
                        // Inverse because we're exposing as enabled
                        raw = EnumExt.SetFlag(raw, index, !enable);
                        index <<= 1;
                    }
                    writer.Write(raw);
                }
            }

            static partial void WriteBinaryDirectionalAmbientLightingColorsCustom(MutagenWriter writer, IWeatherGetter item)
            {
                if (!item.DirectionalAmbientLightingColors.TryGet(out var colors)) return;
                using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.DALC_HEADER))
                {
                    colors.Sunrise.WriteToBinary(writer);
                }
                using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.DALC_HEADER))
                {
                    colors.Day.WriteToBinary(writer);
                }
                using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.DALC_HEADER))
                {
                    colors.Sunset.WriteToBinary(writer);
                }
                using (HeaderExport.ExportSubrecordHeader(writer, Weather_Registration.DALC_HEADER))
                {
                    colors.Night.WriteToBinary(writer);
                }
            }
        }

        public partial class WeatherBinaryOverlay
        {
            private readonly string?[] _cloudTextures = new string?[29];
            public ReadOnlyMemorySlice<string?> CloudTextures => _cloudTextures;

            private readonly CloudLayer[] _clouds = ArrayExt.Create(WeatherBinaryCreateTranslation.NumLayers, (i) => new CloudLayer());
            public ReadOnlyMemorySlice<ICloudLayerGetter> Clouds => _clouds;

            int? _directionalLoc;

            partial void CloudsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                WeatherBinaryCreateTranslation.FillBinaryCloudYSpeeds(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
                    _clouds);
            }

            partial void CloudXSpeedsCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                WeatherBinaryCreateTranslation.FillBinaryCloudXSpeeds(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
                    _clouds);
            }

            partial void CloudAlphasCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                WeatherBinaryCreateTranslation.FillBinaryCloudAlphas(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
                    _clouds);
            }

            partial void CloudColorsCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                WeatherBinaryCreateTranslation.FillBinaryCloudColors(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
                    _clouds);
            }

            partial void DisabledCloudLayersCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                WeatherBinaryCreateTranslation.FillBinaryDisabledCloudLayers(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
                    _clouds);
            }

            partial void DirectionalAmbientLightingColorsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                if (_directionalLoc.HasValue) return;
                _directionalLoc = (ushort)(stream.Position - offset);
            }

            IWeatherAmbientColorSetGetter? GetDirectionalAmbientLightingColorsCustom()
            {
                if (!_directionalLoc.HasValue) return null;
                return WeatherBinaryCreateTranslation.GetBinaryDirectionalAmbientLightingColors(
                    new MutagenFrame(new MutagenMemoryReadStream(_data.Slice(_directionalLoc.Value), _package.MetaData)));
            }

            private TryGet<int?> CustomRecordFallback(
                BinaryMemoryReadStream stream,
                int finalPos,
                int offset,
                RecordType type,
                int? lastParsed,
                RecordTypeConverter? recordTypeConverter)
            {
                if (type.TypeInt == 0x44494445) // EDID
                {
                    return base.FillRecordType(
                        stream: stream,
                        finalPos: finalPos,
                        offset: offset,
                        type: type,
                        lastParsed: lastParsed,
                        recordTypeConverter: recordTypeConverter);
                }
                WeatherBinaryCreateTranslation.FillCloudTexture(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
                    type,
                    _cloudTextures);
                return TryGet<int?>.Succeed(null);
            }
        }
    }
}
