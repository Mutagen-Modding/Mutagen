using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CellSubBlock : IDuplicatable
    {
        public static CellSubBlock_CopyMask duplicateMask = new CellSubBlock_CopyMask(true)
        {
            Items = new Loqui.MaskItem<Loqui.CopyOption, Cell_CopyMask>(Loqui.CopyOption.Skip, null)
        };

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            var ret = new CellSubBlock();
            ret.CopyFieldsFrom(this, duplicateMask);
            ret.Items.SetTo(this.Items.Select(i => (Cell)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }
    }
}
