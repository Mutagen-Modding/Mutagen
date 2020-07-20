using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Core
{
    public interface IModHeaderCommon : IBinaryItem
    {
        IExtendedList<MasterReference> MasterReferences { get; }
        int RawFlags { get; set; }
        uint NumRecords { get; set; }
    }
}
