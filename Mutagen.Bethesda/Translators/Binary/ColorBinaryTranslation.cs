using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
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
            out Color item,
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

        public void ParseInto(
            MutagenFrame frame,
            IHasItem<Color> item,
            bool extraByte)
        {
            if (!extraByte)
            {
                throw new NotImplementedException();
            }
            this.ParseInto(
                frame: frame,
                item: item);
        }

        public override Color ParseValue(MutagenFrame reader)
        {
            return reader.ReadColor();
        }

        public override void WriteValue(MutagenWriter writer, Color item)
        {
            writer.Write(item);
        }

        protected Action<MutagenWriter, Color?> GetWriter(bool extraByte)
        {
            if (!extraByte) return WriteValue;
            return (w, c) =>
            {
                WriteValue(w, c);
                w.WriteZeros(1);
            };
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<Color> item,
            RecordType header,
            bool nullable,
            bool extraByte)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                nullable,
                write: GetWriter(extraByte));
        }

        public void Write(
            MutagenWriter writer,
            Color item,
            RecordType header,
            bool nullable,
            bool extraByte)
        {
            this.Write(
                writer,
                item,
                header,
                nullable,
                write: GetWriter(extraByte));
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<Color> item,
            bool extraByte)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                write: GetWriter(extraByte));
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<Color> item,
            bool extraByte)
        {
            this.Write(
                writer,
                item.Item,
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
    }
}
