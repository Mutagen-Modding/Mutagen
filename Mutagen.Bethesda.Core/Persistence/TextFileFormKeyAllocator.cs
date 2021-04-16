using Noggog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Persistence
{
    /// <summary>
    /// A FormKey allocator that utilizes a folder of text files to persist and sync.
    /// 
    /// This class is made thread safe by locking internally on the Mod object.
    /// </summary>
    public class TextFileFormKeyAllocator : BasePersistentFormKeyAllocator
    {
        private readonly object _lock = new();
        private readonly Dictionary<string, FormKey> _cache = new();
        private readonly uint _initialNextFormID;

        private readonly HashSet<uint> _formIDSet = new();

        public TextFileFormKeyAllocator(IMod mod, string saveLocation) 
            : base(mod, saveLocation)
        {
            _initialNextFormID = mod.NextFormID;
            Load();
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
            lock (this._lock)
            {
                lock (this.Mod)
                {
                    var candidateFormID = this.Mod.NextFormID;
                    if (candidateFormID > 0xFFFFFF)
                        throw new OverflowException();

                    while (_formIDSet.Contains(candidateFormID))
                    {
                        candidateFormID++;
                        if (candidateFormID > 0xFFFFFF)
                            throw new OverflowException();
                    }

                    Mod.NextFormID = candidateFormID + 1;

                    return new FormKey(this.Mod.ModKey, candidateFormID);
                }
            }
        }

        protected override FormKey GetNextFormKeyNotNull(string editorID)
        {
            lock (this._lock)
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

        public override void Commit()
        {
            lock (this._lock)
            {
                WriteToFile(_saveLocation, _cache);
            }
        }

        private void Load()
        {
            if (!Directory.Exists(Path.GetDirectoryName(_saveLocation))) throw new DirectoryNotFoundException();
            if (!File.Exists(_saveLocation)) return;
            using var streamReader = new StreamReader(_saveLocation);
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
                if (formKey.ModKey != Mod.ModKey)
                {
                    throw new ArgumentException($"Attempted to load a FormKey belonging to {formKey.ModKey} into the FormKey allocator for {Mod.ModKey}.");
                }
                if (!_formIDSet.Add(formKey.ID))
                {
                    throw new ArgumentException($"Duplicate formKey loaded from {_saveLocation}.");
                }
                _cache.Add(edidStr, formKey);
            }
        }

        public override void Rollback()
        {
            lock (this._lock)
            {
                lock (this.Mod)
                {
                    this.Mod.NextFormID = _initialNextFormID;
                }
                _cache.Clear();
                Load();
            }
        }
    }
}
