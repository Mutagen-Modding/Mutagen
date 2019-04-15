using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IMajorRecordCommon : IFormKey, IDuplicatable, IXmlFolderItem, ILinkSubContainer
    {
        string EditorID { get; }
        bool IsCompressed { get; set; }
        new FormKey FormKey { get; }
    }

}
