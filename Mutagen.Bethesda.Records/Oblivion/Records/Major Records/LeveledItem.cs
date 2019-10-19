using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    namespace Internals
    {
        public partial class LeveledItemSetterCommon
        {
            static partial void SpecialParse_Vestigial(ILeveledItemInternal item, MutagenFrame frame, ErrorMaskBuilder errorMask)
            {
                var rec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var length);
                if (length != 1)
                {
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException($"Unexpected length: {length}"));
                    return;
                }
                if (ByteBinaryTranslation.Instance.Parse(
                    frame,
                    out var parseVal)
                    && parseVal > 0)
                {
                    item.Flags |= LeveledFlag.CalculateForEachItemInCount;
                }
            }
        }

        public partial class LeveledItemBinaryWrapper
        {
            private bool _vestigialMarker;

            private int? _FlagsLocation;
            bool GetFlagsIsSetCustom() => _FlagsLocation.HasValue || _vestigialMarker;
            public LeveledFlag GetFlagsCustom()
            {
                var ret = _FlagsLocation.HasValue ? (LeveledFlag)HeaderTranslation.ExtractSubrecordSpan(_data.Span, _FlagsLocation.Value, _package.Meta)[0] : default;
                if (_vestigialMarker)
                {
                    ret |= LeveledFlag.CalculateForEachItemInCount;
                }
                return ret;
            }
            partial void FlagsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _FlagsLocation = (ushort)(stream.Position - offset);
            }

            public void VestigialSpecialParse(
                BinaryMemoryReadStream stream,
                int offset,
                RecordType type,
                int? lastParsed)
            {
                var subMeta = _package.Meta.ReadSubRecord(stream);
                if (subMeta.RecordLength != 1)
                {
                    throw new ArgumentException($"Unexpected length: {subMeta.RecordLength}");
                }
                if (stream.ReadByte() > 0)
                {
                    this._vestigialMarker = true;
                }
            }
        }
    }
}
