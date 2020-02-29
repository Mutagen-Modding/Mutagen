using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class ColorBinaryTranslation : PrimitiveBinaryTranslation<Color>
    {
        public readonly static ColorBinaryTranslation Instance = new ColorBinaryTranslation();
        public override int ExpectedLength => 3;

        public bool Parse(
            MutagenFrame frame,
            [MaybeNullWhen(false)]out Color item,
            bool extraByte)
        {
            if (!extraByte)
            {
                throw new NotImplementedException();
            }
            return this.Parse(
                frame,
                out item);
        }

        public Color Parse(
            MutagenFrame frame,
            bool extraByte)
        {
            if (!extraByte)
            {
                throw new NotImplementedException();
            }
            return this.ParseValue(frame);
        }

        public override Color ParseValue(MutagenFrame reader)
        {
            return reader.ReadColor();
        }

        public override void Write(MutagenWriter writer, Color item)
        {
            writer.Write(item, false);
        }

        protected Action<MutagenWriter, Color> GetWriter(bool extraByte)
        {
            if (extraByte)
            {
                return (w, c) =>
                {
                    w.Write(c, true);
                };
            }
            else
            {
                return (w, c) =>
                {
                    w.Write(c, false);
                };
            }
        }

        public void Write(
            MutagenWriter writer,
            Color item,
            RecordType header,
            bool extraByte)
        {
            this.Write(
                writer,
                item,
                header,
                write: GetWriter(extraByte));
        }

        public void Write(
            MutagenWriter writer,
            Color item,
            bool extraByte)
        {
            this.Write(
                writer,
                item,
                write: GetWriter(extraByte));
        }

        public void WriteNullable(
            MutagenWriter writer,
            Color? item,
            RecordType header,
            bool extraByte)
        {
            this.WriteNullable(
                writer,
                item,
                header,
                write: GetWriter(extraByte));
        }

        public void WriteNullable(
            MutagenWriter writer,
            Color? item,
            bool extraByte)
        {
            this.WriteNullable(
                writer,
                item,
                write: GetWriter(extraByte));
        }
    }
}
