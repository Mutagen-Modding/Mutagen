using Loqui.Internal;
using Noggog;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Plugins.Xml;

public interface IXmlFolderItem
{
    Task WriteToXmlFolder(
        DirectoryPath dir, 
        string name, 
        XElement node, 
        int counter,
        ErrorMaskBuilder? errorMask);
}