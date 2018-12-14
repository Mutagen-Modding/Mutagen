using Loqui.Internal;
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
    }
}
