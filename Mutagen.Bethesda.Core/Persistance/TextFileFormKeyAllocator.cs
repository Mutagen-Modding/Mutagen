using Noggog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Core.Persistance
{
    /// <summary>
    /// A FormKey allocator that utilizes a folder of text files to persist and sync.
    /// 
    /// This class is made thread safe by locking internally on the Mod object.
    /// </summary>
    public class TextFileFormKeyAllocator : BasePersistentFormKeyAllocator
    {
        private readonly Dictionary<string, FormKey> _cache = new();

        public TextFileFormKeyAllocator(IMod mod, FilePath saveLocation) : base(mod, saveLocation)
        {
            if (!saveLocation.Exists) return;
            using var streamReader = new StreamReader(saveLocation.Path);
            while (true)
            {
                var edidStr = streamReader.ReadLine();
                var formKeyStr = streamReader.ReadLine();
                if (edidStr == null) break;
                if (formKeyStr == null)
                {
                    throw new ArgumentException("Unexpected odd number of lines.");
                }
                var formKey = FormKey.Factory(formKeyStr);
                if (formKey.ModKey != mod.ModKey)
                {
                    throw new ArgumentException($"Attempted to load a FormKey belonging to {formKey.ModKey} into the FormKey allocator for {mod.ModKey}.");
                }
                _cache.Add(edidStr, formKey);
            }
        }

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

        protected override FormKey GetNextFormKeyNotNull(string editorID)
        {
            lock (this.Mod)
            {
                if (_cache.TryGetValue(editorID, out var id))
                    return id;

                var formKey = GetNextFormKey();

                _cache.Add(editorID, formKey);

                return formKey;
            }
        }

        internal static void WriteToFile(string saveLocation, IEnumerable<KeyValuePair<string, FormKey>> data)
        {
            var tempFile = saveLocation + ".tmp";
            {
                using var streamWriter = new StreamWriter(tempFile);
                foreach (var pair in data)
                {
                    streamWriter.WriteLine(pair.Key);
                    streamWriter.WriteLine(pair.Value);
                }
            }
            if (File.Exists(saveLocation))
                File.Replace(tempFile, saveLocation, null);
            else
                File.Move(tempFile, saveLocation);
        }

        public override void Save()
        {
            WriteToFile(SaveLocation.Path, _cache);
        }
    }
}
