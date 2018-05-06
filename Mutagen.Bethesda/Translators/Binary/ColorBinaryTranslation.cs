using Loqui;
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

        protected override Color ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadColor();
        }

        protected override void WriteValue(MutagenWriter writer, Color item)
        {
            writer.Write(item);
        }

        protected Func<MutagenWriter, Color?, bool, Exception> GetWriter(bool extraByte)
        {
            if (!extraByte) return Write_Internal;
            return (w, c, doM) =>
            {
                var ret = Write_Internal(w, c, doM);
                w.WriteZeros(1);
                return ret;
            };
        }
        
        public TryGet<Color> Parse<M>(
            MutagenFrame frame,
            int fieldIndex,
            bool extraByte,
            Func<M> errorMask)
            where M : IErrorMask
        {
            var ret = this.Parse(
                frame,
                fieldIndex,
                errorMask);
            frame.Position += 1;
            return ret;
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<Color> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            bool extraByte,
            Func<M> errorMask)
            where M : IErrorMask
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

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<Color> item,
            int fieldIndex,
            bool extraByte,
            Func<M> errorMask)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask,
                write: GetWriter(extraByte));
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<Color> item,
            int fieldIndex,
            bool extraByte,
            Func<M> errorMask)
            where M : IErrorMask
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
