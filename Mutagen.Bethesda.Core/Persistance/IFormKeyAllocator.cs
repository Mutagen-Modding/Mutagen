using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Mutagen.Bethesda
{
    [DebuggerDisplay("FormKeyAllocator {Mod.ModKey}")]
    public abstract class BaseFormKeyAllocator : IFormKeyAllocator
    {
        /// <summary>
        /// Attached Mod that will be used as reference when allocating new keys
        /// </summary>
        public IMod Mod { get; }

        private readonly HashSet<string> _allocatedEditorIDs = new();

        protected BaseFormKeyAllocator(IMod mod)
        {
            this.Mod = mod;
        }

        public abstract FormKey GetNextFormKey();

        public FormKey GetNextFormKey(string? editorID)
        {
            if (editorID is null) return GetNextFormKey();

            lock (_allocatedEditorIDs)
            {
                if (!_allocatedEditorIDs.Add(editorID))
                {
                    throw new ConstraintException($"Attempted to allocate a duplicate unique FormKey for {editorID}");
                }
            }

            return GetNextFormKeyNotNull(editorID);
        }

        protected abstract FormKey GetNextFormKeyNotNull(string editorID);
    }

    /// <summary>
    /// An interface for something that can allocate new FormKeys when requested
    /// </summary>
    public interface IFormKeyAllocator
    {
        /// <summary>
        /// Requests a new unused FormKey, with no other requirements
        /// </summary>
        /// <returns>An unused FormKey</returns>
        FormKey GetNextFormKey();

        /// <summary>
        /// Requests a new unused FormKey, given an EditorID to be used for syncronization purposes.
        /// The EditorID can be used to provide persistance syncronization by the implementation.
        /// </summary>
        /// <returns>An unused FormKey</returns>
        FormKey GetNextFormKey(string? editorID);
    }
}
