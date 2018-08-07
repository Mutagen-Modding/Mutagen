using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Grass
    {
        public enum UnitFromWaterType
        {
            AboveAtLeast = 0,
            AboveAtMost = 1,
            BelowAtLeast = 2,
            BelowAtMost = 3,
            EitherAtLeast = 4,
            EitherAtMost = 5,
            EitherAtMostAbove = 6,
            EitherAtMostBelow = 7,
        }
        
        [Flags]
        public enum GrassFlag
        {
            VertexLighting = 0x01,
            UniformScaling = 0x02,
            FitToSlope = 0x04
        }

        private byte[] _buffer1;
        private byte[] _buffer2;

        static partial void FillBinary_MaxSlope_Custom(MutagenFrame frame, Grass item, ErrorMaskBuilder errorMask)
        {
            item.MaxSlope = Mutagen.Bethesda.Binary.ByteBinaryTranslation.Instance.Parse(
                frame: frame.Spawn(snapToFinalPosition: false),
                errorMask: errorMask);
            item._buffer1 = frame.Reader.ReadBytes(1);
        }

        static partial void WriteBinary_MaxSlope_Custom(MutagenWriter writer, Grass item, ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.ByteBinaryTranslation.Instance.Write(
                writer: writer,
                item: item.MaxSlope,
                fieldIndex: (int)Grass_FieldIndex.MaxSlope,
                errorMask: errorMask);
            if (item._buffer1 != null)
            {
                writer.Write(item._buffer1);
            }
            else
            {
                writer.WriteZeros(1);
            }
        }

        static partial void FillBinary_UnitFromWaterAmount_Custom(MutagenFrame frame, Grass item, ErrorMaskBuilder errorMask)
        {
            item.UnitFromWaterAmount = Mutagen.Bethesda.Binary.UInt16BinaryTranslation.Instance.Parse(
                frame: frame.Spawn(snapToFinalPosition: false),
                errorMask: errorMask);
            item._buffer2 = frame.Reader.ReadBytes(2);
        }

        static partial void WriteBinary_UnitFromWaterAmount_Custom(MutagenWriter writer, Grass item, ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.UInt16BinaryTranslation.Instance.Write(
                writer: writer,
                item: item.UnitFromWaterAmount,
                fieldIndex: (int)Grass_FieldIndex.UnitFromWaterAmount,
                errorMask: errorMask);
            if (item._buffer2 != null)
            {
                writer.Write(item._buffer2);
            }
            else
            {
                writer.WriteZeros(2);
            }
        }
    }
}
