using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Core.Persistance
{
    /// <summary>
    /// A FormKey allocator that utilizes a folder of text files to persist and sync.
    /// 
    /// This class is made thread safe by locking internally on the Mod object.
    /// </summary>
    [DebuggerDisplay("TextFileFormKeyAllocatorv2 {patcherName}")]
    public class TextFileFormKeyAllocatorv2 : IFormKeyAllocator
    {
        private readonly Dictionary<string, (string patcherName, FormKey formKey)> _cache = new();

        private readonly HashSet<string> allocatedEditorIDs = new();

        private readonly HashSet<uint> FormIDSet = new();

        private readonly string patcherName;

        /// <summary>
        /// Associated Mod
        /// </summary>
        public IMod Mod { get; }

        public TextFileFormKeyAllocatorv2(IMod mod) : this(mod, "") { }

        public TextFileFormKeyAllocatorv2(IMod mod, string patcherName)
        {
            this.Mod = mod;
            this.patcherName = patcherName;
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
                if (!FormIDSet.Add(formKey.ID))
                {
                    throw new ArgumentException("Duplicate formKey loaded from {filePath}.");
                }
                _cache.Add(edidStr, new(patcherName, formKey));
            }
        }

        public static TextFileFormKeyAllocatorv2 FromFile(IMod mod, string filePath)
        {
            // should it be okay to read from an empty/missing file?
            var self = new TextFileFormKeyAllocatorv2(mod, "");
            self.ReadFile(filePath, self.patcherName);
            return self;
        }

        public static TextFileFormKeyAllocatorv2 FromFolder(IMod mod, string folderPath, string patcherName)
        {
            var self = new TextFileFormKeyAllocatorv2(mod, patcherName);
            foreach (var file in Directory.GetFiles(folderPath))
                self.ReadFile(file, Path.GetFileName(file));
            return self;
        }

        /// <summary>
        /// Returns a FormKey with the next listed ID in the Mod's header.
        /// No checks will be done that this is truly a unique key; It is assumed the header is in a correct state.
        ///
        /// The Mod's header will be incremented to mark the allocated key as "used".
        /// </summary>
        /// <returns>The next FormKey from the Mod</returns>
        public FormKey GetNextFormKey()
        {
            lock (this.Mod)
            {
                var candidateFormID = this.Mod.NextFormID;
                if (candidateFormID > 0xFFFFFF)
                    throw new OverflowException();

                while (FormIDSet.Contains(candidateFormID))
                {
                    candidateFormID++;
                    if (candidateFormID > 0xFFFFFF)
                        throw new OverflowException();
                }

                Mod.NextFormID = candidateFormID + 1;

                return new FormKey(
                    this.Mod.ModKey,
                    checked(candidateFormID));
            }
        }

        public FormKey GetNextFormKey(string? editorID)
        {
            if (editorID == null) return GetNextFormKey();

            lock (_cache)
            {
                if (!allocatedEditorIDs.Add(editorID))
                    throw new ConstraintException($"Attempted to allocate a duplicate unique FormKey for {editorID}");

                if (_cache.TryGetValue(editorID, out var rec))
                {
                    if (rec.patcherName != this.patcherName)
                        throw new ConstraintException($"Attempted to allocate a unique FormKey for {editorID} when it was previously allocated by {rec.patcherName}");
                    return rec.formKey;
                }

                var formKey = GetNextFormKey();

                _cache.Add(editorID, (patcherName, formKey));

                return formKey;
            }
        }

        internal static void WriteToFile(string path, IEnumerable<(string Key, FormKey Value)> edidFormKeyPairs)
        {
            using var streamWriter = new StreamWriter(path);
            foreach (var (Key, Value) in edidFormKeyPairs)
            {
                streamWriter.WriteLine(Key);
                streamWriter.WriteLine(Value);
            }
        }

        public void WriteToFile(string path)
        {
            if (patcherName != "")
                throw new InvalidOperationException("Use WriteToFolder instead.");

            var data = this._cache
                .Select(p => (p.Key, p.Value.formKey));

            WriteToFile(path, data);
        }

        public void WriteToFolder(string path)
        {
            if (patcherName == "")
                throw new InvalidOperationException("Use WriteToFile instead.");

            var data = this._cache
                .Where(p => p.Value.patcherName == patcherName)
                .Select(p => (p.Key, p.Value.formKey));

            WriteToFile(Path.Combine(path, patcherName), data);
        }
    }
}
