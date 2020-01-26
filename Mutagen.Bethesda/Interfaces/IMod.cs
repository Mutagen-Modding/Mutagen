using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IModGetter : IMajorRecordGetterEnumerable
    {
        GameMode GameMode { get; }
        IReadOnlyList<IMasterReferenceGetter> MasterReferences { get; }
        IReadOnlyCache<T, FormKey> GetGroupGetter<T>() where T : IMajorRecordCommonGetter;
        void WriteToBinary(string path, ModKey? modKeyOverride = null);
        Task WriteToBinaryAsync(string path, ModKey? modKeyOverride = null);
        void WriteToBinaryParallel(string path, ModKey? modKeyOverride = null);
        ModKey ModKey { get; }
    }

    public interface IMod : IModGetter, ILinkContainer, IMajorRecordEnumerable
    {
        new IList<MasterReference> MasterReferences { get; }
        ICache<T, FormKey> GetGroup<T>() where T : IMajorRecordCommon;
        FormKey GetNextFormKey();
        void SyncRecordCount();
    }
}
