using Loqui;
using Mutagen.Bethesda.Plugins.Allocators;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Plugins.Records;

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
    /// Read only list of listed overridden forms.<br />
    /// Note this is only the listed overridden forms in the mod header, and may
    /// deviate from the reality of contained records
    /// </summary>
    IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms { get; }
    
    /// <summary>
    /// The next FormID to be allocated
    /// </summary>
    uint NextFormID { get; }

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
    void WriteToBinary(FilePath path, BinaryWriteParameters? param = null);

    /// <summary>
    /// Exports to disk in Bethesda binary format.
    /// Access and iterates through the mod object's contents in a single thread.
    /// </summary>
    /// <param name="stream">Path to export to</param>
    /// <param name="param">Optional customization parameters</param>
    void WriteToBinary(Stream stream, BinaryWriteParameters? param = null);

    /// <summary>
    /// Retrieves the recommended lowest starting FormID, based on current mod header flags. <br />
    /// Note that this might be lower than the currently contained records, as it is just reporting
    /// what the starting lowest ID should be.
    /// </summary>
    /// <param name="forceUseLowerFormIDRanges">Whether to force using the lower FormID ranges.
    /// Default of null refers to the mod header flags</param>
    /// <returns>Lowest suggested FormID given current mod header flags</returns>
    uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false);
    
    IBinaryModdedWriteBuilderTargetChoice BeginWrite { get; }

    /// <summary>
    /// Calculates the record count by enumerating all records
    /// </summary>
    /// <returns>Calculated record count of the contained records</returns>
    uint GetRecordCount();
}

/// <summary>
/// An interface that Mod objects implement to hook into the common systems
/// </summary>
public interface IMod : IModGetter, IMajorRecordEnumerable, IFormKeyAllocator, IFormLinkContainer
{
    /// <summary>
    /// The associated ModKey
    /// </summary>
    new ModKey ModKey { get; internal set; }
    
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
    /// Whether a mod has Small Master flag enabled
    /// </summary>
    new bool IsSmallMaster { get; set; }

    /// <summary>
    /// Whether a mod has Medium Master flag enabled
    /// </summary>
    new bool IsMediumMaster { get; set; }

    /// <summary>
    /// Whether a mod has Master flag enabled
    /// </summary>
    new bool IsMaster { get; set; }

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