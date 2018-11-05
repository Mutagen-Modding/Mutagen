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
    public interface IMod<M> : ILinkContainer
        where M : IMod<M>
    {
        ISourceList<MasterReference> MasterReferences { get; }
        IObservableCache<IMajorRecord, FormKey> MajorRecords { get; }
        ISourceCache<T, FormKey> GetGroup<T>() where T : IMajorRecord;
        void Link(
            ModList<M> modList,
            NotifyingFireParameters cmds = null);
        void Write_Binary(
            string path,
            ModKey modKey);
        ModKey ModKey { get; }
        //FormKey GetNextAvailableFormKey();
    }
}
