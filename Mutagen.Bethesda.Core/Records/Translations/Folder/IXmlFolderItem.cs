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
        Task WriteToXmlFolder(
            DirectoryPath dir, 
            string name, 
            XElement node, 
            int counter,
            ErrorMaskBuilder? errorMask);
    }
}
