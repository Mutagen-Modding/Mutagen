using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Masters;

/// <summary>
/// A registry of master listings.
/// Generally used for reference when converting FormIDs to FormKeys
/// </summary>
public interface IReadOnlyMasterReferenceCollection
{
    /// <summary>
    /// List of masters in the registry
    /// </summary>
    IReadOnlyList<IMasterReferenceGetter> Masters { get; }
        
    /// <summary>
    /// ModKey that should be considered to be the current mod
    /// </summary>
    ModKey CurrentMod { get; }

    /// <summary>
    /// Attempts to look up index associated with a given ModKey
    /// </summary>
    bool TryGetIndex(ModKey modKey, out ModIndex index);
}

public interface IMasterReferenceCollection : IReadOnlyMasterReferenceCollection
{
    /// <summary>
    /// Clears and sets contained masters to given enumerable's contents
    /// </summary>
    /// <param name="masters">Masters to set to</param>
    void SetTo(IEnumerable<IMasterReferenceGetter> masters);
}

/// <summary>
/// A registry of master listings.
/// Generally used for reference when converting FormIDs to FormKeys
/// </summary>
public sealed class MasterReferenceCollection : IMasterReferenceCollection
{
    private readonly Dictionary<ModKey, ModIndex> _masterIndices = new();
        
    /// <summary>
    /// A static singleton that is an empty registry containing no masters
    /// </summary>
    public static IReadOnlyMasterReferenceCollection Empty { get; } = new MasterReferenceCollection(ModKey.Null);

    /// <inheritdoc />
    public IReadOnlyList<IMasterReferenceGetter> Masters { get; private set; } = Array.Empty<IMasterReferenceGetter>();
        
    /// <inheritdoc />
    public ModKey CurrentMod { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="modKey">Mod to associate as the "current" mod</param>
    public MasterReferenceCollection(ModKey modKey)
    {
        CurrentMod = modKey;
        SetTo(Enumerable.Empty<IMasterReferenceGetter>());
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="modKey">Mod to associate as the "current" mod</param>
    /// <param name="masters">Masters to add to the registrar</param>
    public MasterReferenceCollection(ModKey modKey, IEnumerable<IMasterReferenceGetter> masters)
    {
        CurrentMod = modKey;
        SetTo(masters);
    }

    /// <inheritdoc />
    public void SetTo(IEnumerable<IMasterReferenceGetter> masters)
    {
        // ToDo
        // Throw exceptions early before making modifications to members
        
        Masters = masters.ToList();
        _masterIndices.Clear();

        byte index = 0;
        foreach (var master in Masters)
        {
            var modKey = master.Master;
            if (index >= Constants.PluginMasterLimit)
            {
                throw new TooManyMastersException(CurrentMod, Masters.Select(x => x.Master).ToArray());
            }
            if (modKey == CurrentMod)
            {
                throw new SelfReferenceException(CurrentMod);
            }
            // Don't care about duplicates too much, just skip
            if (!_masterIndices.ContainsKey(modKey))
            {
                _masterIndices[modKey] = new ModIndex(index);
            }
            index++;
        }

        // Add current mod
        _masterIndices[CurrentMod] = new ModIndex(index);
    }

    /// <inheritdoc />
    public bool TryGetIndex(ModKey modKey, out ModIndex index)
    {
        return _masterIndices.TryGetValue(modKey, out index);
    }

    public static MasterReferenceCollection FromPath(ModPath path, GameRelease release, IFileSystem? fileSystem = null)
    {
        var fs = fileSystem.GetOrDefault().FileStream.New(path, FileMode.Open, FileAccess.Read);
        using var stream = new MutagenBinaryReadStream(fs, 
            new ParsingBundle(
                release, 
                path.ModKey,
                masterReferences: null!));
        return FromStream(stream);
    }

    public static MasterReferenceCollection FromStream(Stream stream, ModKey modKey, GameRelease release, bool disposeStream = true)
    {
        using var interf = new MutagenInterfaceReadStream(
            new BinaryReadStream(stream, dispose: disposeStream), 
            new ParsingBundle(
                release, 
                modKey,
                masterReferences: null!));
        return FromStream(interf);
    }

    public static MasterReferenceCollection FromStream<TStream>(TStream stream)
        where TStream : IMutagenReadStream
    {
        var mutaFrame = new MutagenFrame(stream);
        var header = stream.ReadModHeaderFrame(readSafe: true);
        return new MasterReferenceCollection(
            stream.MetaData.ModKey,
            header
                .Masters()
                .Select(mastPin =>
                {
                    stream.Position = mastPin.Location;
                    return MasterReference.CreateFromBinary(mutaFrame);
                }));
    }
}