using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IMod<M> : ILinkContainer
        where M : IMod<M>
    {
        INotifyingListGetter<MasterReference> MasterReferences { get; }
        INotifyingDictionaryGetter<FormID, IMajorRecord> MajorRecords { get; }
        INotifyingKeyedCollection<FormID, T> GetGroup<T>() where T : IMajorRecord;
        void Link(
            ModList<M> modList,
            NotifyingFireParameters cmds = null);
    }
}
