using Loqui.Internal;
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
            XElement topNode = new XElement("topnode");
            int counter = 0;
            foreach (var cell in this.Items)
            {
                using (errorMask.PushIndex(counter))
                {
                    try
                    {
                        cell.Write_Xml(
                            topNode,
                            errorMask: errorMask,
                            translationMask: null);
                    }
                    catch (Exception ex)
                    {
                        errorMask.ReportException(ex);
                    }
                }
                counter++;
            }
            if (topNode.HasElements)
            {
                topNode.SaveIfChanged(Path.Combine(path, $"{this.BlockNumber.ToString()}.xml"));
            }
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
