using System.Collections.Generic;

namespace Mutagen.Bethesda.Persistance
{
    /// <summary>
    /// A simple FormKey allocator that simply leverages a Mod's NextObjectID tracker to allocate.
    /// No safety checks or syncronization is provided.
    ///
    /// This class is thread safe.
    /// </summary>
    public class SimpleFormKeyAllocator : BaseFormKeyAllocator, IFormKeyAllocator
    {
        private readonly Dictionary<string, FormKey> _cache = new();

        /// <summary>
        /// Constructs a new SimpleNextIDAllocator that looks to a given Mod for the next key
        /// </summary>
        public SimpleFormKeyAllocator(IMod mod) : base(mod) { }

        /// <summary>
        /// Returns a FormKey with the next listed ID in the Mod's header.
        /// No checks will be done that this is truly a unique key; It is assumed the header is in a correct state.
        ///
        /// The Mod's header will be incremented to mark the allocated key as "used".
        /// </summary>
        /// <returns>The next FormKey from the Mod</returns>
        public override FormKey GetNextFormKey()
        {
            lock (this.Mod)
            {
                return new FormKey(
                    this.Mod.ModKey,
                    checked(this.Mod.NextFormID++));
            }
        }

        public bool TryRegister(string edid, FormKey formKey)
        {
            lock (_cache)
            {
                return _cache.TryAdd(edid, formKey);
            }
        }

        public void Register(string edid, FormKey formKey)
        {
            lock (_cache)
            {
                _cache.Add(edid, formKey);
            }
        }

        protected override FormKey GetNextFormKeyNotNull(string editorID)
        {
            lock (_cache)
            {
                if (_cache.TryGetValue(editorID, out var id))
                    return id;
            }
            return GetNextFormKey();
        }
    }
}
