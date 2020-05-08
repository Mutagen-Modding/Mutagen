using Mutagen.Bethesda.Persistance;
using System;
using Noggog;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An abstract base class for Mods to inherit from for some common functionality
    /// </summary>
    public abstract class AMod : IMod
    {
        /// <summary>
        /// The key associated with the mod
        /// </summary>
        public ModKey ModKey { get; }
        /// <summary>
        /// The game associated with the mod
        /// </summary>
        public abstract GameMode GameMode { get; }
        
        private IFormKeyAllocator allocator;

        protected AMod()
        {
            this.ModKey = ModKey.Null;
            this.allocator = new SimpleFormKeyAllocator(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modKey">Key to assign the mod</param>
        /// <param name="allocator">Optional custom FormKey allocator logic</param>
        public AMod(ModKey modKey, IFormKeyAllocator? allocator = null)
        {
            this.ModKey = modKey;
            this.allocator = allocator ?? new SimpleFormKeyAllocator(this);
        }

        #region NonImplemented IMod
        IEnumerable<FormKey> ILinkedFormKeyContainer.LinkFormKeys => throw new NotImplementedException();
        void ILinkedFormKeyContainer.RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => throw new NotImplementedException();
        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => throw new NotImplementedException();
        IList<MasterReference> IMod.MasterReferences => throw new NotImplementedException();
        uint IMod.NextObjectID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ICache<T, FormKey> IMod.GetGroup<T>() => throw new NotImplementedException();
        public abstract void SyncRecordCount();
        IReadOnlyCache<T, FormKey> IModGetter.GetGroupGetter<T>() => throw new NotImplementedException();
        void IModGetter.WriteToBinary(string path, BinaryWriteParameters? param) => throw new NotImplementedException();
        void IModGetter.WriteToBinaryParallel(string path, BinaryWriteParameters? param) => throw new NotImplementedException();
        IEnumerable<IMajorRecordCommon> IMajorRecordEnumerable.EnumerateMajorRecords() => throw new NotImplementedException();
        IEnumerable<T> IMajorRecordEnumerable.EnumerateMajorRecords<T>() => throw new NotImplementedException();
        IEnumerable<IMajorRecordCommonGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords() => throw new NotImplementedException();
        IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>() => throw new NotImplementedException();
        #endregion

        /// <summary>
        /// Requests a new unused FormKey from the alloctor specified in the mod's construction
        /// </summary>
        /// <returns>An unused FormKey</returns>
        public FormKey GetNextFormKey()
        {
            return allocator.GetNextFormKey();
        }

        /// <summary>
        /// Requests a new unused FormKey from the alloctor specified in the mod's construction
        /// </summary>
        /// <param name="editorID">The target EditorID that may potentially be used for synchronization</param>
        /// <returns>An unused FormKey</returns>
        public FormKey GetNextFormKey(string editorID)
        {
            return allocator.GetNextFormKey(editorID);
        }
    }
}
