using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CellBlock
    {
        public static CellBlock.TranslationMask duplicateMask = new CellBlock.TranslationMask(true)
        {
            SubBlocks = new MaskItem<bool, CellSubBlock.TranslationMask?>(false, default)
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
