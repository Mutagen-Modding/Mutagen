using Loqui;
using Loqui.Internal;
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
    public partial class CellBlock : IXmlFolderItem
    {
        public static CellBlock_CopyMask duplicateMask = new CellBlock_CopyMask(true)
        {
            Items = new Loqui.MaskItem<Loqui.CopyOption, CellSubBlock_CopyMask>(Loqui.CopyOption.Skip, null)
        };

        public void Write_Xml_Folder(
            DirectoryPath? dir,
            string name,
            XElement node,
            int counter,
            ErrorMaskBuilder errorMask)
        {
            var subDir = Path.Combine(dir.Value.Path, $"Cells/{this.BlockNumber}");
            Directory.CreateDirectory(subDir);
            int blockCounter = 0;
            foreach (var item in this.Items)
            {
                using (errorMask.PushIndex(blockCounter++))
                {
                    try
                    {
                        item.Write_Xml_Folder(
                            path: subDir,
                            errorMask: errorMask);
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            var ret = new CellBlock();
            ret.CopyFieldsFrom(this, duplicateMask);
            ret.Items.SetTo(this.Items.Select(i => (CellSubBlock)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }
    }
}

