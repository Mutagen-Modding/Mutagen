using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class LeveledItem
    {
        static partial void SpecialParse_Vestigial(LeveledItem item, MutagenFrame frame, Func<LeveledItem_ErrorMask> errorMask)
        {
            var rec = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var length);
            if (length != 1)
            {
                throw new ArgumentException($"Unexpected length: {length}");
            }
            var parse = ByteBinaryTranslation.Instance.Parse(
                frame,
                doMasks: false,
                errorMask: out var ex);
            if (parse.Value > 0)
            {
                item.Flags |= LeveledFlag.CalculateForEachItemInCount;
            }
        }
    }
}
