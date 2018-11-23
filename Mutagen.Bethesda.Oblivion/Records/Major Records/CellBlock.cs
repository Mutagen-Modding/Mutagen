using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CellBlock : IDuplicatable
    {
        public static CellBlock_CopyMask duplicateMask = new CellBlock_CopyMask(true)
        {
            Items = new Loqui.MaskItem<Loqui.CopyOption, CellSubBlock_CopyMask>(Loqui.CopyOption.Skip, null)
        };

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            var ret = new CellBlock();
            ret.CopyFieldsFrom(this, duplicateMask);
            ret.Items.SetTo(this.Items.Select(i => (CellSubBlock)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }
    }
}
