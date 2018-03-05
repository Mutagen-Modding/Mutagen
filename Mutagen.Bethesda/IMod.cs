using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IMod
    {
        INotifyingListGetter<MasterReference> MasterReferences { get; }
        bool TryGetRecord<T>(uint id, out T record);
        INotifyingDictionaryGetter<FormID, MajorRecord> MajorRecords { get; }
    }
}
