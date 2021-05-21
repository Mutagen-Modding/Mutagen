using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Plugins.Allocators
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
        private Lazy<InternalState> _state;
        private readonly bool _preload;
        private readonly ModKey _modKey;
        public const string MarkerFileName = $"{nameof(TextFileSharedFormKeyAllocator)}.marker";

        class InternalState
        {
            public readonly Dictionary<string, (string patcherName, FormKey formKey)> Cache = new();
            public readonly HashSet<uint> FormIDSet = new();
        }

        public TextFileSharedFormKeyAllocator(IMod mod, string saveFolder, string activePatcherName, bool preload = false)
            : base(mod, saveFolder, activePatcherName)
        {
            _initialNextFormID = mod.NextFormID;
            _modKey = mod.ModKey;
            if (!Directory.Exists(Path.GetDirectoryName(_saveLocation))) throw new DirectoryNotFoundException();
            _preload = preload;
            _state = GetLazyInternalState();
        }

        private Lazy<InternalState> GetLazyInternalState()
        {
            if (_preload)
            {
                return new Lazy<InternalState>(GetInternalState());
            }
            else
            {
                return new Lazy<InternalState>(() => GetInternalState());
            }
        }

        private InternalState GetInternalState()
        {
            var ret = new InternalState();
            if (Directory.Exists(_saveLocation))
            {
                if (!File.Exists(Path.Combine(_saveLocation, MarkerFileName)))
                {
                    throw new InvalidDataException("Tried to load from a folder that did not have marker file");
                }
                foreach (var file in Directory.GetFiles(_saveLocation, "*.txt"))
                {
                    ReadFile(file, Path.GetFileNameWithoutExtension(file), ret);
                }
            }
            return ret;
        }

        private void ReadFile(string filePath, string patcherName, InternalState state)
        {
            using var streamReader = new StreamReader(filePath);
            while (true)
            {
                var edidStr = streamReader.ReadLine();
                var formIdStr = streamReader.ReadLine();
                if (edidStr == null) break;
                if (formIdStr == null)
                {
                    throw new ArgumentException("Unexpected odd number of lines.");
                }
                var formKey = new FormKey(_modKey, uint.Parse(formIdStr));
                if (!state.FormIDSet.Add(formKey.ID))
                {
                    throw new ArgumentException($"Duplicate formKey loaded from {filePath}.");
                }
                state.Cache.Add(edidStr, new(patcherName, formKey));
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

                    var set = _state.Value.FormIDSet;
                    while (set.Contains(candidateFormID))
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
                if (_state.Value.Cache.TryGetValue(editorID, out var rec))
                {
                    if (rec.patcherName != ActivePatcherName)
                        throw new ConstraintException($"Attempted to allocate a unique FormKey for {editorID} when it was previously allocated by {rec.patcherName}");
                    return rec.formKey;
                }

                var formKey = GetNextFormKey();

                _state.Value.Cache.Add(editorID, (ActivePatcherName, formKey));

                return formKey;
            }
        }

        public override void Commit()
        {
            lock (this._lock)
            {
                if (!_state.IsValueCreated) return;
                Initialize(_saveLocation);
                var data = _state.Value.Cache
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
                _state = GetLazyInternalState();
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
                    streamWriter.WriteLine(Value.ID);
                }
            }
            if (File.Exists(targetFile))
            {
                File.Replace(tempFile, targetFile, null);
            }
            else
            {
                File.Move(tempFile, targetFile);
            }
        }

        public static bool IsPathOfAllocatorType(string path)
        {
            if (!Directory.Exists(path)) return false;
            return File.Exists(Path.Combine(path, MarkerFileName));
        }

        public static void Initialize(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var markerPath = Path.Combine(path, MarkerFileName);
            if (!File.Exists(markerPath))
            {
                File.WriteAllText(markerPath, null);
            }
        }
    }
}
