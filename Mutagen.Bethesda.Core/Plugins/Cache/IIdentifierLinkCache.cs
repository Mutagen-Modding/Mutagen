using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache;

public interface IIdentifierLinkCache : IDisposable
{
    /// <summary>
    /// Retrieves the winning EditorID that matches the FormKey relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// <br/>
    /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
    /// processed, rather than being able to scope the search to a specific area.
    /// </summary>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="editorId">Out parameter containing the EditorID if successful</param>
    /// <param name="target">Resolution target to look up</param>
    /// <returns>True if a matching record was found</returns>
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner);

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// <br/>
    /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
    /// processed, rather than being able to scope the search to a specific area.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <returns>True if a matching record was found</returns>
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey);

    /// <summary>
    /// Retrieves the winning EditorID that matches the FormKey relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">The type of Major Record to look up</param>
    /// <param name="editorId">Out parameter containing the EditorID if successful</param>
    /// <param name="target">Resolution target to look up</param>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner);

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">The type of Major Record to look up</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey);

    /// <summary>
    /// Retrieves the winning EditorID that matches the FormKey relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="editorId">Out parameter containing the EditorID if successful</param>
    /// <param name="target">Resolution target to look up</param>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter;

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier<TMajor>(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
        where TMajor : class, IMajorRecordQueryableGetter;

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="types">The types of Major Records to look up</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types);

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="types">The types of Major Records to look up</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types);

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="types">The types of Major Records to look up</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <param name="target">Resolution target to look up</param>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner);

    /// <summary>
    /// Retrieves the FormKey that matches the winning EditorID relative to the source the package was attached to.<br/>
    /// <br/>
    /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
    /// the function will return false.
    /// </summary>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="types">The types of Major Records to look up</param>
    /// <param name="formKey">Out parameter containing the FormKey if successful</param>
    /// <exception cref="ArgumentException">
    /// An unexpected type will throw an exception.<br/>
    /// Unexpected types include:<br/>
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    /// <returns>True if a matching record was found</returns>
    bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey);

    /// <summary>
    /// Returns all winning identifiers for a given type.
    /// </summary>
    /// <param name="type">Type to retrieve identifiers for</param>
    /// <param name="cancel">Optional cancel token</param>
    /// <returns>Winning identifiers for a given type</returns>
    IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null);

    /// <summary>
    /// Returns all winning identifiers for a given type.
    /// </summary>
    /// <typeparam name="TMajor">Type to retrieve identifiers for</typeparam>
    /// <param name="cancel">Optional cancel token</param>
    /// <returns>Winning identifiers for a given type</returns>
    IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
        where TMajor : class, IMajorRecordQueryableGetter;

    /// <summary>
    /// Returns all winning identifiers for the given types.
    /// </summary>
    /// <param name="types">Types to retrieve identifiers for</param>
    /// <param name="cancel">Optional cancel token</param>
    /// <returns>Winning identifiers for a given type</returns>
    IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null);

    /// <summary>
    /// Returns all winning identifiers for the given types.
    /// </summary>
    /// <param name="types">Types to retrieve identifiers for</param>
    /// <returns>Winning identifiers for a given type</returns>
    IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types);
}