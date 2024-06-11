using Loqui;
using Mutagen.Bethesda.Plugins.Allocators;
using Noggog;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Parameters;

namespace Mutagen.Bethesda.Plugins.Records;

public interface IModFlagsGetter : IModKeyed
{
    /// <summary>
    /// Whether a mod supports localization features
    /// </summary>
    bool CanUseLocalization { get; }

    /// <summary>
    /// Whether a mod has localization enabled
    /// </summary>
    bool UsingLocalization { get; }
    
    /// <summary>
    /// Whether a mod supports Light Master features
    /// </summary>
    bool CanBeLightMaster { get; }

    /// <summary>
    /// Whether a mod has Light Master flag enabled
    /// </summary>
    bool IsLightMaster { get; }
    
    /// <summary>
    /// Whether a mod supports Half Master features
    /// </summary>
    bool CanBeMediumMaster { get; }

    /// <summary>
    /// Whether a mod has Half Master flag enabled
    /// </summary>
    bool IsMediumMaster { get; }
}

/// <summary>
/// An interface that Mod objects implement to hook into the common getter systems
/// </summary>
public interface IModGetter : 
    IModFlagsGetter,
    IMajorRecordGetterEnumerable,
    IMajorRecordSimpleContextEnumerable,
    IFormLinkContainerGetter, 
    IEqualsMask
{
    /// <summary>
    /// The associated game release
    /// </summary>
    GameRelease GameRelease { get; }

    /// <summary>
    /// Read only list of master reference getters.
    /// </summary>
    /// <returns>Read only list of master reference getters</returns>
    IReadOnlyList<IMasterReferenceGetter> MasterReferences { get; }

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    IGroupGetter<TMajor>? TryGetTopLevelGroup<TMajor>() where TMajor : IMajorRecordGetter;

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <param name="type">The type of Major Record to get the Group for</param>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    IGroupGetter? TryGetTopLevelGroup(Type type);

    /// <summary>
    /// Exports to disk in Bethesda binary format.
    /// Access and iterates through the mod object's contents in a single thread.
    /// </summary>
    /// <param name="path">Path to export to</param>
    /// <param name="param">Optional customization parameters</param>
    /// <param name="fileSystem">Optional filesystem substitution</param>
    void WriteToBinary(FilePath path, BinaryWriteParameters? param = null, IFileSystem? fileSystem = null);

    /// <summary>
    /// Exports to disk in Bethesda binary format.
    /// Access and iterates through the mod groups in separate threads.  All provided mod objects
    /// are thread safe to use with this function.
    /// </summary>
    /// <param name="path">Path to export to</param>
    /// <param name="param">Optional customization parameters</param>
    /// <param name="fileSystem">Optional filesystem substitution</param>
    /// <param name="parallelWriteParameters">Optional customization parameters related to parallelization</param>
    void WriteToBinaryParallel(FilePath path, BinaryWriteParameters? param = null, IFileSystem? fileSystem = null, ParallelWriteParameters? parallelWriteParameters = null);

    /// <summary>
    /// Exports to disk in Bethesda binary format.
    /// Access and iterates through the mod object's contents in a single thread.
    /// </summary>
    /// <param name="stream">Path to export to</param>
    /// <param name="param">Optional customization parameters</param>
    void WriteToBinary(Stream stream, BinaryWriteParameters? param = null);

    /// <summary>
    /// Exports to disk in Bethesda binary format.
    /// Access and iterates through the mod groups in separate threads.  All provided mod objects
    /// are thread safe to use with this function.
    /// </summary>
    /// <param name="stream">Path to export to</param>
    /// <param name="param">Optional customization parameters</param>
    /// <param name="parallelWriteParameters">Optional customization parameters related to parallelization</param>
    void WriteToBinaryParallel(Stream stream, BinaryWriteParameters? param = null, ParallelWriteParameters? parallelWriteParameters = null);

    /// <summary>
    /// The next FormID to be allocated
    /// </summary>
    uint NextFormID { get; }

    uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false);
}

/// <summary>
/// An interface that Mod objects implement to hook into the common systems
/// </summary>
public interface IMod : IModGetter, IMajorRecordEnumerable, IFormKeyAllocator, IFormLinkContainer
{
    /// <summary>
    /// List of master references.
    /// </summary>
    /// <returns>List of master references</returns>
    new IList<MasterReference> MasterReferences { get; }

    /// <summary>
    /// Returns the Group object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group object associated with the given Major Record Type</returns>
    /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.
    /// Unexpected types include:
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    new IGroup<TMajor>? TryGetTopLevelGroup<TMajor>() where TMajor : IMajorRecord;

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <param name="type">The type of Major Record to get the Group for</param>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    new IGroup? TryGetTopLevelGroup(Type type);

    /// <summary>
    /// The next FormID to be allocated
    /// </summary>
    new uint NextFormID { get; set; }

    /// <summary>
    /// Whether a mod has localization enabled
    /// </summary>
    new bool UsingLocalization { get; set; }

    /// <summary>
    /// Whether a mod has Light Master flag enabled
    /// </summary>
    new bool IsLightMaster { get; set; }

    /// <summary>
    /// Assigns a new allocator to the mod.  This will be used whenever a new FormKey is requested from the mod.
    /// </summary>
    /// <param name="allocator">Allocator to use</param>
    /// <typeparam name="TAlloc">Allocator type</typeparam>
    /// <returns>The same Allocator given is returned</returns>
    TAlloc SetAllocator<TAlloc>(TAlloc allocator)
        where TAlloc : IFormKeyAllocator;
}

/// <summary>
/// An interface for Overlay mod systems
/// </summary>
public interface IModDisposeGetter : IModGetter, IDisposable
{
}