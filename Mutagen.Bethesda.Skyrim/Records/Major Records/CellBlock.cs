using Loqui;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class CellBlock
    {
        public static CellBlock.TranslationMask duplicateMask = new CellBlock.TranslationMask(true)
        {
            SubBlocks = false
        };

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecordTracker = null)
        {
            var ret = new CellBlock();
            ret.DeepCopyIn(this, duplicateMask);
            ret.SubBlocks = this.SubBlocks.Select(i => (CellSubBlock)i.Duplicate(getNextFormKey, duplicatedRecordTracker)).ToExtendedList();
            return ret;
        }
    }
}
