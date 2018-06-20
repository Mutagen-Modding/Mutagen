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
    public partial class LeveledItem
    {
        static partial void SpecialParse_Vestigial(LeveledItem item, MutagenFrame frame, ErrorMaskBuilder errorMask)
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
                out var parseVal,
                errorMask)
                && parseVal > 0)
            {
                item.Flags |= LeveledFlag.CalculateForEachItemInCount;
            }
        }
    }
}
