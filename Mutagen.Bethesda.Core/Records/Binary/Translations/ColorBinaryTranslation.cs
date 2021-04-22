using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public static class ColorBinaryTranslationExt
    {
        public static void Write(this IBinaryWriteStream writer, Color color, ColorBinaryType binaryType)
        {
            switch (binaryType)
            {
                case ColorBinaryType.NoAlpha:
                    writer.Write(color.R);
                    writer.Write(color.G);
                    writer.Write(color.B);
                    break;
                case ColorBinaryType.Alpha:
                    writer.Write(color.R);
                    writer.Write(color.G);
                    writer.Write(color.B);
                    writer.Write(color.A);
                    break;
                case ColorBinaryType.NoAlphaFloat:
                    writer.Write((float)(color.R / 255d));
                    writer.Write((float)(color.G / 255d));
                    writer.Write((float)(color.B / 255d));
                    break;
                case ColorBinaryType.AlphaFloat:
                    writer.Write((float)(color.R / 255d));
                    writer.Write((float)(color.G / 255d));
                    writer.Write((float)(color.B / 255d));
                    writer.Write((float)(color.A / 255d));
                    break;
                default:
                    break;
            }
        }

        public static void Write(
            this PrimitiveBinaryTranslation<Color, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            Color item,
            RecordType header,
            ColorBinaryType binaryType)
        {
            transl.Write(
                writer,
                item,
                header,
                write: ColorBinaryTranslation.GetWriter(binaryType));
        }

        public static void WriteNullable(
            this PrimitiveBinaryTranslation<Color, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            Color? item,
            RecordType header,
            ColorBinaryType binaryType)
        {
            transl.WriteNullable(
                writer,
                item,
                header,
                write: ColorBinaryTranslation.GetWriter(binaryType));
        }
    }

    public class ColorBinaryTranslation : PrimitiveBinaryTranslation<Color, MutagenFrame, MutagenWriter>
    {
        public readonly static ColorBinaryTranslation Instance = new ColorBinaryTranslation();
        public override int ExpectedLength => 3;

        public bool Parse(
            MutagenFrame reader,
            [MaybeNullWhen(false)]out Color item,
            ColorBinaryType binaryType)
        {
            if (binaryType != ColorBinaryType.NoAlpha)
            {
                throw new NotImplementedException();
            }
            item = this.Parse(reader);
            return true;
        }

        public Color Parse(
            MutagenFrame reader,
            ColorBinaryType binaryType)
        {
            return reader.ReadColor(binaryType);
        }

        public override Color Parse(MutagenFrame reader)
        {
            throw new NotImplementedException();
        }

        public override void Write(MutagenWriter writer, Color item)
        {
            writer.Write(item, ColorBinaryType.Alpha);
        }

        public static Action<MutagenWriter, Color> GetWriter(ColorBinaryType binaryType)
        {
            switch (binaryType)
            {
                case ColorBinaryType.NoAlpha:
                    return (w, c) =>
                    {
                        w.Write(c, ColorBinaryType.NoAlpha);
                    };
                case ColorBinaryType.Alpha:
                    return (w, c) =>
                    {
                        w.Write(c, ColorBinaryType.Alpha);
                    };
                case ColorBinaryType.NoAlphaFloat:
                    return (w, c) =>
                    {
                        w.Write(c, ColorBinaryType.NoAlphaFloat);
                    };
                case ColorBinaryType.AlphaFloat:
                    return (w, c) =>
                    {
                        w.Write(c, ColorBinaryType.AlphaFloat);
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(
            MutagenWriter writer,
            Color item,
            ColorBinaryType binaryType)
        {
            this.Write(
                writer,
                item,
                write: GetWriter(binaryType));
        }

        public void WriteNullable(
            MutagenWriter writer,
            Color? item,
            ColorBinaryType binaryType)
        {
            this.WriteNullable(
                writer,
                item,
                write: GetWriter(binaryType));
        }
    }
}
