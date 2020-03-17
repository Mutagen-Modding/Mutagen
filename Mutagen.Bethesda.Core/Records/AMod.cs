using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public abstract class AMod : IMod
    {
        public ModKey ModKey { get; }
        public abstract GameMode GameMode { get; }

        IEnumerable<ILinkGetter> ILinkContainer.Links => throw new NotImplementedException();

        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => throw new NotImplementedException();
        IList<MasterReference> IMod.MasterReferences => throw new NotImplementedException();

        protected AMod()
        {
            this.ModKey = ModKey.Null;
        }

        public AMod(ModKey modKey)
            : this()
        {
            this.ModKey = modKey;
        }

        ICache<T, FormKey> IMod.GetGroup<T>() => throw new NotImplementedException();
        public abstract FormKey GetNextFormKey();
        public abstract void SyncRecordCount();
        IReadOnlyCache<T, FormKey> IModGetter.GetGroupGetter<T>() => throw new NotImplementedException();
        void IModGetter.WriteToBinary(string path, BinaryWriteParameters? param) => throw new NotImplementedException();
        void IModGetter.WriteToBinaryParallel(string path, BinaryWriteParameters? param) => throw new NotImplementedException();
        IEnumerable<IMajorRecordCommon> IMajorRecordEnumerable.EnumerateMajorRecords() => throw new NotImplementedException();

        IEnumerable<T> IMajorRecordEnumerable.EnumerateMajorRecords<T>()
        {
            throw new NotImplementedException();
        }

        IEnumerable<IMajorRecordCommonGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords()
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>()
        {
            throw new NotImplementedException();
        }
    }
}
