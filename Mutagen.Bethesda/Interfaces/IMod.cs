using CSharpExt.Rx;
using DynamicData;
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
        ISourceList<MasterReference> MasterReferences { get; }
        IObservableCache<IMajorRecord, FormKey> MajorRecords { get; }
        ISourceCache<T, FormKey> GetGroup<T>() where T : IMajorRecordInternalGetter;
        void WriteToBinary(
            string path,
            ModKey modKey);
        ModKey ModKey { get; }
        FormKey GetNextFormKey();
        void SyncRecordCount();
    }
}
