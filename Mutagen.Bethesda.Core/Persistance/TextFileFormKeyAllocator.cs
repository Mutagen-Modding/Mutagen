using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Core.Persistance
{
    /// <summary>
    /// A FormKey allocator that utilizes a folder of text files to persist and sync.
    /// 
    /// This class is made thread safe by locking internally on the Mod object.
    /// </summary>
    public class TextFileFormKeyAllocator : IFormKeyAllocator
    {
        private readonly Dictionary<string, FormKey> _cache = new Dictionary<string, FormKey>();

        /// <summary>
        /// Associated Mod
        /// </summary>
        public IMod Mod { get; }

        private TextFileFormKeyAllocator(IMod mod, string filePath)
        {
            this.Mod = mod;
            using var streamReader = new StreamReader(filePath);
            var idLine = streamReader.ReadLine();
            if (!uint.TryParse(idLine, out var nextID))
            {
                throw new ArgumentException($"Unconvertable next ID line: {idLine}");
            }
            this.Mod.NextFormID = nextID;
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
                _cache.Add(edidStr, formKey);
            }
        }

        public static TextFileFormKeyAllocator FromFile(IMod mod, string filePath)
        {
            return new TextFileFormKeyAllocator(mod, filePath);
        }

        public static TextFileFormKeyAllocator FromFolder(IMod mod, string folderPath)
        {
            return new TextFileFormKeyAllocator(mod, Path.Combine(folderPath, mod.ModKey.FileName));
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
                return new FormKey(
                    this.Mod.ModKey,
                    checked(this.Mod.NextFormID++));
            }
        }

        public FormKey GetNextFormKey(string? editorID)
        {
            if (editorID != null)
            {
                if (_cache.TryGetValue(editorID, out var id)) return id;
            }
            return GetNextFormKey();
        }

        public static void WriteToFile(string path, uint nextID, IEnumerable<KeyValuePair<string, FormKey>> edidFormKeyPairs)
        {
            using var streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(nextID.ToString());
            foreach (var pair in edidFormKeyPairs)
            {
                streamWriter.WriteLine(pair.Key);
                streamWriter.WriteLine(pair.Value);
            }
        }

        public static void WriteToFile(string path, IModGetter mod)
        {
            WriteToFile(
                path,
                mod.NextFormID, 
                mod.EnumerateMajorRecords()
                    .SelectWhere(m =>
                    {
                        var edid = m.EditorID;
                        if (edid == null) return TryGet<KeyValuePair<string, FormKey>>.Failure;
                        return TryGet<KeyValuePair<string, FormKey>>.Succeed(
                            new KeyValuePair<string, FormKey>(edid, m.FormKey));
                    }));
        }
    }
}
