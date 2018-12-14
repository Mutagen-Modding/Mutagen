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
    }
}
