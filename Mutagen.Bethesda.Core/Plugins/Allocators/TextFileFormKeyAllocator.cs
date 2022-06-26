using Mutagen.Bethesda.Plugins.Records;
using System.IO.Abstractions;
using Noggog;
using Path = System.IO.Path;
using DirectoryNotFoundException = System.IO.DirectoryNotFoundException;
using StreamReader = System.IO.StreamReader;
using StreamWriter = System.IO.StreamWriter;
using FileMode = System.IO.FileMode;

namespace Mutagen.Bethesda.Plugins.Allocators;

/// <summary>
/// A FormKey allocator that utilizes a folder of text files to persist and sync.
/// 
/// This class is made thread safe by locking internally on the Mod object.
/// </summary>
public class TextFileFormKeyAllocator : BasePersistentFormKeyAllocator
{
    private readonly object _lock = new();
    private readonly uint _initialNextFormID;
    private Lazy<InternalState> _state;
    private readonly bool _preload;
    private readonly IFileSystem _fileSystem;
    private readonly ModKey _modKey;

    class InternalState
    {
        public readonly Dictionary<string, FormKey> Cache = new();
        public readonly HashSet<uint> FormIDSet = new();
    }

    public TextFileFormKeyAllocator(IMod mod, string saveLocation, bool preload = false, IFileSystem? fileSystem = null) 
        : base(mod, saveLocation)
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
        InternalState ret = new();
        if (!_fileSystem.File.Exists(_saveLocation)) return ret;
        using var streamReader = new StreamReader(_fileSystem.FileStream.Create(_saveLocation, FileMode.Open));
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
            if (!ret.FormIDSet.Add(formKey.ID))
            {
                throw new ArgumentException($"Duplicate formKey loaded from {_saveLocation}.");
            }
            ret.Cache.Add(edidStr, formKey);
        }
        return ret;
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
        lock (_lock)
        {
            lock (Mod)
            {
                var candidateFormID = Mod.NextFormID;
                if (candidateFormID > 0xFFFFFF)
                    throw new OverflowException();

                while (_state.Value.FormIDSet.Contains(candidateFormID))
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
            if (_state.Value.Cache.TryGetValue(editorID, out var id))
                return id;

            var formKey = GetNextFormKey();

            _state.Value.Cache.Add(editorID, formKey);

            return formKey;
        }
    }

    internal static void WriteToFile(string saveLocation, IEnumerable<KeyValuePair<string, FormKey>> data, IFileSystem? fileSystem = null)
    {
        fileSystem ??= IFileSystemExt.DefaultFilesystem;
        var tempFile = saveLocation + ".tmp";
        {
            using var fs = fileSystem.FileStream.Create(tempFile, FileMode.Create);
            using var streamWriter = new StreamWriter(fs);
            foreach (var pair in data)
            {
                streamWriter.WriteLine(pair.Key);
                streamWriter.WriteLine(pair.Value.ID);
            }
        }
        if (fileSystem.File.Exists(saveLocation))
            fileSystem.File.Replace(tempFile, saveLocation, null);
        else
            fileSystem.File.Move(tempFile, saveLocation);
    }

    public override void Commit()
    {
        lock (_lock)
        {
            if (!_state.IsValueCreated) return;
            WriteToFile(_saveLocation, _state.Value.Cache, _fileSystem);
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
}