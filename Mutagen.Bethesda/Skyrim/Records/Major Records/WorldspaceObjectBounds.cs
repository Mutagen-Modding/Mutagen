using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class WorldspaceObjectBoundsBinaryCreateTranslation
        {
            static partial void FillBinaryMaxCustom(MutagenFrame frame, IWorldspaceObjectBounds item)
            {
                var subHeader = frame.ReadSubrecord();
                if (subHeader.ContentLength != 8)
                {
                    throw new ArgumentException("Unexpected length");
                }
                item.Max = new P2Float(
                    frame.ReadFloat() / 4096f,
                    frame.ReadFloat() / 4096f);
            }

            static partial void FillBinaryMinCustom(MutagenFrame frame, IWorldspaceObjectBounds item)
            {
                var subHeader = frame.ReadSubrecord();
                if (subHeader.ContentLength != 8)
                {
                    throw new ArgumentException("Unexpected length");
                }
                item.Min = new P2Float(
                    frame.ReadFloat() / 4096f,
                    frame.ReadFloat() / 4096f);
            }
        }

        public partial class WorldspaceObjectBoundsBinaryWriteTranslation
        {
            static partial void WriteBinaryMaxCustom(MutagenWriter writer, IWorldspaceObjectBoundsGetter item)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, Worldspace_Registration.NAM9_HEADER))
                {
                    var max = item.Max;
                    writer.Write(max.X * 4096f);
                    writer.Write(max.Y * 4096f);
                }
            }

            static partial void WriteBinaryMinCustom(MutagenWriter writer, IWorldspaceObjectBoundsGetter item)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, Worldspace_Registration.NAM0_HEADER))
                {
                    var min = item.Min;
                    writer.Write(min.X * 4096f);
                    writer.Write(min.Y * 4096f);
                }
            }
        }

        public partial class WorldspaceObjectBoundsBinaryOverlay
        {
            private int? _minLoc;
            public P2Float GetMinCustom() => _minLoc.HasValue 
                ? new P2Float(
                    SpanExt.GetFloat(_data.Slice(_minLoc.Value + 6)) / 4096f,
                    SpanExt.GetFloat(_data.Slice(_minLoc.Value + 10)) / 4096f)
                : default;

            private int? _maxLoc;
            public P2Float GetMaxCustom() => _maxLoc.HasValue 
                ? new P2Float(
                    SpanExt.GetFloat(_data.Slice(_maxLoc.Value + 6)) / 4096f,
                    SpanExt.GetFloat(_data.Slice(_maxLoc.Value + 10)) / 4096f)
                : default;

            partial void MinCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                this._minLoc = stream.Position - offset;
            }

            partial void MaxCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                this._maxLoc = stream.Position - offset;
            }
        }
    }
}
