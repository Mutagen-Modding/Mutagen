using Loqui.Internal;
using Loqui.Xml;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CellSubBlock
    {
        public static CellSubBlock_CopyMask duplicateMask = new CellSubBlock_CopyMask(true)
        {
            Items = new Loqui.MaskItem<Loqui.CopyOption, Cell_CopyMask>(Loqui.CopyOption.Skip, null)
        };

        public void Write_Xml_Folder(
            string path,
            ErrorMaskBuilder errorMask)
        {
            this.Write_Xml(
                path, 
                errorMask,
                translationMask: null);
        }

        public static CellSubBlock Create_Xml_Folder(FilePath file, int index)
        {
            return CellSubBlock.Create_Xml(file.Path);
        }

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            var ret = new CellSubBlock();
            ret.CopyFieldsFrom(this, duplicateMask);
            ret.Items.SetTo(this.Items.Select(i => (Cell)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }
    }
}
