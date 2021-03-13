using Noggog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Core.Persistance
{
    /// <summary>
    /// A FormKey allocator that utilizes a folder of text files to persist and sync.
    /// 
    /// This class is made thread safe by locking internally on the Mod object.
    /// </summary>

    public class TextFileSharedFormKeyAllocator : BaseSharedFormKeyAllocator
    {
        private readonly object _lock = new();
        private readonly uint _initialNextFormID;
        private readonly Dictionary<string, (string patcherName, FormKey formKey)> _cache = new();

        private readonly HashSet<uint> _formIDSet = new();

        public TextFileSharedFormKeyAllocator(IMod mod, string saveFolder, string activePatcherName)
            : base(mod, saveFolder, activePatcherName)
        {
            _initialNextFormID = mod.NextFormID;
            Load();
        }

        private void Load()
        {
            foreach (var file in Directory.GetFiles(_saveLocation, "*.txt"))
                ReadFile(file, Path.GetFileNameWithoutExtension(file));
        }

        private void ReadFile(string filePath, string patcherName)
        {
            using var streamReader = new StreamReader(filePath);
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
                    throw new ArgumentException($"Duplicate formKey loaded from {filePath}.");
                }
                _cache.Add(edidStr, new(patcherName, formKey));
            }
        }

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
                if (_cache.TryGetValue(editorID, out var rec))
                {
                    if (rec.patcherName != ActivePatcherName)
                        throw new ConstraintException($"Attempted to allocate a unique FormKey for {editorID} when it was previously allocated by {rec.patcherName}");
                    return rec.formKey;
                }

                var formKey = GetNextFormKey();

                _cache.Add(editorID, (ActivePatcherName, formKey));

                return formKey;
            }
        }

        public override void Commit()
        {
            lock (this._lock)
            {
                var data = _cache
                    .Where(p => p.Value.patcherName == ActivePatcherName)
                    .Select(p => (p.Key, p.Value.formKey));
                WriteToFile(Path.Combine(_saveLocation, ActivePatcherName), data);
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
                _formIDSet.Clear();
                Load();
            }
        }

        internal static void WriteToFile(string thePath, IEnumerable<(string Key, FormKey formKey)> data)
        {
            var tempFile = thePath + ".tmp";
            var targetFile = thePath + ".txt";
            {
                using var streamWriter = new StreamWriter(tempFile);
                foreach (var (Key, Value) in data)
                {
                    streamWriter.WriteLine(Key);
                    streamWriter.WriteLine(Value);
                }
            }
            if (File.Exists(targetFile))
                File.Replace(tempFile, targetFile, null);
            else
                File.Move(tempFile, targetFile);
        }
    }
}
