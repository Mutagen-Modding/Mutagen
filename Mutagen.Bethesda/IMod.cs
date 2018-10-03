using CSharpExt.Rx;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IMod : ILinkContainer
    {
        IObservableSetList<MasterReference> MasterReferences { get; }
        INotifyingDictionaryGetter<FormID, MajorRecord> MajorRecords { get; }
        INotifyingKeyedCollection<FormID, T> GetGroup<T>() where T : IMajorRecord;
    }
}
