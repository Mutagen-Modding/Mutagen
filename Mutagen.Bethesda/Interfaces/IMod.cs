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
    public interface IModGetter
    {
        GameMode GameMode { get; }
        IReadOnlyList<IMasterReferenceGetter> MasterReferences { get; }
        IReadOnlyCache<IMajorRecordInternalGetter, FormKey> MajorRecords { get; }
        IReadOnlyCache<T, FormKey> GetGroupGetter<T>() where T : IMajorRecordInternalGetter;
        void WriteToBinary(
            string path,
            ModKey modKey);
        ModKey ModKey { get; }
    }

    public interface IMod : IModGetter, ILinkContainer
    {
        new ISourceList<MasterReference> MasterReferences { get; }
        new IObservableCache<IMajorRecord, FormKey> MajorRecords { get; }
        ISourceCache<T, FormKey> GetGroup<T>() where T : IMajorRecordInternal;
        FormKey GetNextFormKey();
        void SyncRecordCount();
    }
}
