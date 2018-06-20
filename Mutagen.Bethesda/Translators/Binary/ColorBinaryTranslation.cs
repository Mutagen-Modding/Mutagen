using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mutagen.Bethesda.Binary
{
    public class ColorBinaryTranslation : PrimitiveBinaryTranslation<Color>
    {
        public readonly static ColorBinaryTranslation Instance = new ColorBinaryTranslation();
        public override int? ExpectedLength => 3;
        
        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<Color> item,
            bool extraByte,
            ErrorMaskBuilder errorMask)
        {
            this.ParseInto(
                frame,
                fieldIndex,
                item,
                errorMask);
        }

        protected override Color ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadColor();
        }

        protected override void WriteValue(MutagenWriter writer, Color item)
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
            int fieldIndex,
            bool nullable,
            bool extraByte,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask,
                write: GetWriter(extraByte));
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<Color> item,
            int fieldIndex,
            bool extraByte,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask,
                write: GetWriter(extraByte));
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<Color> item,
            int fieldIndex,
            bool extraByte,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask,
                write: GetWriter(extraByte));
        }
    }
}
