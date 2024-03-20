using Mutagen.Bethesda.Plugins.Records;
using System.Data;
using System.IO.Abstractions;
using Noggog;
using Path = System.IO.Path;
using DirectoryNotFoundException = System.IO.DirectoryNotFoundException;
using StreamReader = System.IO.StreamReader;
using StreamWriter = System.IO.StreamWriter;
using InvalidDataException = System.IO.InvalidDataException;

namespace Mutagen.Bethesda.Plugins.Allocators;

/// <summary>
/// A FormKey allocator that utilizes a folder of text files to persist and sync.
/// 
/// This class is made thread safe by locking internally on the Mod object.
/// </summary>
public sealed class TextFileSharedFormKeyAllocator : BaseSharedFormKeyAllocator
{
    private readonly object _lock = new();
    private readonly uint _initialNextFormID;
    private Lazy<InternalState> _state;
    private readonly bool _preload;
    private readonly IFileSystem _fileSystem;
    private readonly ModKey _modKey;
    public const string MarkerFileName = $"{nameof(TextFileSharedFormKeyAllocator)}.marker";

    class InternalState
    {
        public readonly Dictionary<string, (string patcherName, FormKey formKey)> Cache = new();
        public readonly HashSet<uint> FormIDSet = new();
    }

    public TextFileSharedFormKeyAllocator(IMod mod, string saveFolder, string activePatcherName, bool preload = false, IFileSystem? fileSystem = null) 
        : base(mod, saveFolder, activePatcherName)
    {
        _fileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
        _initialNextFormID = mod.NextFormID;
        _modKey = mod.ModKey;
        if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(_saveLocation))) throw new DirectoryNotFoundException();
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
        if (_fileSystem.Directory.Exists(_saveLocation))
        {
            if (!_fileSystem.File.Exists(Path.Combine(_saveLocation, MarkerFileName)))
            {
                throw new InvalidDataException("Tried to load from a folder that did not have marker file");
            }
            foreach (var file in _fileSystem.Directory.GetFiles(_saveLocation, "*.txt"))
            {
                ReadFile(file, Path.GetFileNameWithoutExtension(file), ret);
            }
        }
        return ret;
    }

    private void ReadFile(string filePath, string patcherName, InternalState state)
    {
        using var fs = _fileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var streamReader = new StreamReader(fs);
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
        lock (_lock)
        {
            lock (Mod)
            {
                var candidateFormID = Mod.NextFormID;
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

                return new FormKey(Mod.ModKey, candidateFormID);
            }
        }
    }

    protected override FormKey GetNextFormKeyNotNull(string editorID)
    {
        lock (_lock)
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
        lock (_lock)
        {
            if (!_state.IsValueCreated) return;
            Initialize(_saveLocation, _fileSystem);
            var data = _state.Value.Cache
                .Where(p => p.Value.patcherName == ActivePatcherName)
                .Select(p => (p.Key, p.Value.formKey));
            WriteToFile(Path.Combine(_saveLocation, ActivePatcherName), data, _fileSystem);
        }
    }

    public override void Rollback()
    {
        lock (_lock)
        {
            lock (Mod)
            {
                Mod.NextFormID = _initialNextFormID;
            }
            _state = GetLazyInternalState();
        }
    }

    internal static void WriteToFile(string thePath, IEnumerable<(string Key, FormKey formKey)> data, IFileSystem fileSystem)
    {
        var tempFile = thePath + ".tmp";
        var targetFile = thePath + ".txt";
        {
            using var fs = fileSystem.FileStream.New(tempFile, FileMode.Create);
            using var streamWriter = new StreamWriter(fs);
            foreach (var (Key, Value) in data)
            {
                streamWriter.WriteLine(Key);
                streamWriter.WriteLine(Value.ID);
            }
        }
        if (fileSystem.File.Exists(targetFile))
        {
            fileSystem.File.Replace(tempFile, targetFile, null);
        }
        else
        {
            fileSystem.File.Move(tempFile, targetFile);
        }
    }

    public static bool IsPathOfAllocatorType(string path, IFileSystem? fileSystem = null)
    {
        fileSystem ??= IFileSystemExt.DefaultFilesystem;
        if (!fileSystem.Directory.Exists(path)) return false;
        return fileSystem.File.Exists(Path.Combine(path, MarkerFileName));
    }

    public static void Initialize(string path, IFileSystem? fileSystem = null)
    {
        fileSystem ??= IFileSystemExt.DefaultFilesystem;
        if (!fileSystem.Directory.Exists(path))
        {
            fileSystem.Directory.CreateDirectory(path);
        }
        var markerPath = Path.Combine(path, MarkerFileName);
        if (!fileSystem.File.Exists(markerPath))
        {
            fileSystem.File.WriteAllText(markerPath, null);
        }
    }
}