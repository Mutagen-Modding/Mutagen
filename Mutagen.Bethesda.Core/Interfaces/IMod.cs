using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IModGetter : IMajorRecordGetterEnumerable, ILinkContainer
    {
        GameMode GameMode { get; }
        IReadOnlyList<IMasterReferenceGetter> MasterReferences { get; }
        IReadOnlyCache<T, FormKey> GetGroupGetter<T>() where T : IMajorRecordCommonGetter;
        void WriteToBinary(string path, BinaryWriteParameters? param = null);
        void WriteToBinaryParallel(string path, BinaryWriteParameters? param = null);
        ModKey ModKey { get; }
    }

    public interface IMod : IModGetter, IMajorRecordEnumerable
    {
        new IList<MasterReference> MasterReferences { get; }
        ICache<T, FormKey> GetGroup<T>() where T : IMajorRecordCommon;
        FormKey GetNextFormKey();
        void SyncRecordCount();
    }

    public interface IModDisposeGetter : IModGetter, IDisposable
    {
    }
}
