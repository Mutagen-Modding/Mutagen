using Loqui.Internal;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public interface IXmlFolderItem
    {
        void Write_Xml_Folder(
            DirectoryPath? dir, 
            string name, 
            XElement node, 
            int counter,
            ErrorMaskBuilder errorMask);
    }
}
