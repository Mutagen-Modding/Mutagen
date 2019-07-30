using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IMajorRecordCommon : IMajorRecordCommonGetter, ILinkSubContainer
    {
        new bool IsCompressed { get; set; }
        new int MajorRecordFlagsRaw { get; set; }
    }

    public interface IMajorRecordCommonGetter : IFormKey, IDuplicatable, IXmlFolderItem
    {
        string EditorID { get; }
        bool IsCompressed { get; }
        int MajorRecordFlagsRaw { get; }
        new FormKey FormKey { get; }
    }
}
