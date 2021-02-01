using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for retriving records given a FormKey.
    /// </summary>
    public interface ILinkCache
    {
        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false. <br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target FormKey of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false. <br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target EditorID of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false. <br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target FormKey of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false. <br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target EditorID of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a KeyNotFoundException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IMajorRecordCommonGetter Resolve(FormKey formKey);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a KeyNotFoundException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IMajorRecordCommonGetter Resolve(string editorId);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given type, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(FormKey formKey, Type type);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given type, it will be seen as not a match.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(string editorId, Type type);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from one of the given types, it will be seen as not a match.<br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target FormKey of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from one of the given types, it will be seen as not a match.<br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target EditorID of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(string editorId, params Type[] types);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from one of the given types, it will be seen as not a match.<br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target FormKey of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from one of the given types, it will be seen as not a match.<br />
        /// <br />
        /// NOTE: <br />
        /// In the case of two records existing with the same target EditorID of different types exist, the first found to match will be returned.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="types">The types of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <returns>Matching record</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <returns>Matching record</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Iterates all records that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Enumerable of all located records that match the FormKey in the cache</returns>
        IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Iterates all records that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Enumerable of all located records that match the FormKey in the cache</returns>
        IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type);

        /// <summary>
        /// Iterates all records that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <returns>Enumerable of all located records that match the FormKey in the cache</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey);

        /// <summary>
        /// Iterates through the contained mods in the order they were listed, with the least prioritized mod first.
        /// </summary>
        IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <summary>
        /// Iterates through the contained mods in priority order (reversed), with the most prioritized mod first.
        /// </summary>
        IReadOnlyList<IModGetter> PriorityOrder { get; }
    }

    public interface ILinkCache<TModSetter> : ILinkCache
        where TModSetter : class, IMod
    {
        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <returns>True if a matching record was found</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        bool TryResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TModSetter, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <returns>True if a matching record was found</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        bool TryResolveContext<TMajorSetter, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TModSetter, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a KeyNotFoundException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record with context</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a KeyNotFoundException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record with context</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given type, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record with context</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type);

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given type, it will be seen as not a match.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Matching record with context</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajorSetter">The setter type of Major Record to look up</typeparam>
        /// <typeparam name="TMajorGetter">The getter type of Major Record to look up</typeparam>
        /// <returns>Matching record with context</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TModSetter, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the EditorID relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <typeparam name="TMajorSetter">The setter type of Major Record to look up</typeparam>
        /// <typeparam name="TMajorGetter">The getter type of Major Record to look up</typeparam>
        /// <returns>Matching record with context</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TModSetter, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(string editorId)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Iterates all record contexts that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajorSetter">The setter type of Major Record to look up</typeparam>
        /// <typeparam name="TMajorGetter">The getter type of Major Record to look up</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Enumerable of all located record contexts that match the FormKey in the cache</returns>
        IEnumerable<IModContext<TModSetter, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Iterates all record contexts that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Enumerable of all located record contexts that match the FormKey in the cache</returns>
        IEnumerable<IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type);

        /// <summary>
        /// Iterates all record contexts that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <returns>Enumerable of all located record contexts that match the FormKey in the cache</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IEnumerable<IModContext<TModSetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey);
    }

    public interface ILinkCache<TModSetter, TModGetter> : ILinkCache<TModSetter>
        where TModSetter : class, IMod, TModGetter
        where TModGetter : class, IModGetter
    {
    }

    public static class ILinkCacheExt
    {
        /// <summary>
        /// Creates a Link Cache using a single mod as its link target. <br/>
        /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
        /// modifications occur on content already cached.
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static ImmutableModLinkCache ToUntypedImmutableLinkCache(this IModGetter mod)
        {
            return new ImmutableModLinkCache(mod);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache ToUntypedImmutableLinkCache(
            this LoadOrder<IModGetter> loadOrder)
        {
            return new ImmutableLoadOrderLinkCache(loadOrder.ListedOrder);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache ToUntypedImmutableLinkCache(
            this LoadOrder<IModListing<IModGetter>> loadOrder)
        {
            return new ImmutableLoadOrderLinkCache(
                loadOrder
                    .Select(listing => listing.Value.Mod)
                    .NotNull());
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache ToUntypedImmutableLinkCache(
            this IEnumerable<IModListing<IModGetter>> loadOrder)
        {
            return new ImmutableLoadOrderLinkCache(
                loadOrder
                    .Select(listing => listing.Mod)
                    .NotNull());
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of mods on the load order is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">Enumerable of mods to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache ToImmutableLinkCache(
            this IEnumerable<IModGetter> loadOrder)
        {
            return new ImmutableLoadOrderLinkCache(loadOrder);
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target. <br/>
        /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
        /// modifications occur on content already cached.
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static ImmutableModLinkCache<TMod, TModGetter> ToImmutableLinkCache<TMod, TModGetter>(this TModGetter mod)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new ImmutableModLinkCache<TMod, TModGetter>(mod);
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(this TModGetter mod)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new MutableModLinkCache<TMod, TModGetter>(mod);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TMod, TModGetter> ToImmutableLinkCache<TMod, TModGetter>(
            this LoadOrder<TModGetter> loadOrder)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new ImmutableLoadOrderLinkCache<TMod, TModGetter>(loadOrder.ListedOrder);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TMod, TModGetter> ToImmutableLinkCache<TMod, TModGetter>(
            this LoadOrder<IModListing<TModGetter>> loadOrder)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new ImmutableLoadOrderLinkCache<TMod, TModGetter>(
                loadOrder
                    .Select(listing => listing.Value.Mod)
                    .NotNull());
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TMod, TModGetter> ToImmutableLinkCache<TMod, TModGetter>(
            this IEnumerable<IModListing<TModGetter>> loadOrder)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new ImmutableLoadOrderLinkCache<TMod, TModGetter>(
                loadOrder
                    .Select(listing => listing.Mod)
                    .NotNull());
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of mods on the load order is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">Enumerable of mods to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TMod, TModGetter> ToImmutableLinkCache<TMod, TModGetter>(
            this IEnumerable<TModGetter> loadOrder)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new ImmutableLoadOrderLinkCache<TMod, TModGetter>(loadOrder);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(
            this LoadOrder<TModGetter> immutableBaseCache,
            params TMod[] mutableMods)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new MutableLoadOrderLinkCache<TMod, TModGetter>(
                immutableBaseCache.ToImmutableLinkCache<TMod, TModGetter>(),
                mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(
            this LoadOrder<IModListing<TModGetter>> immutableBaseCache,
            params TMod[] mutableMods)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new MutableLoadOrderLinkCache<TMod, TModGetter>(
                immutableBaseCache.ToImmutableLinkCache<TMod, TModGetter>(),
                mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(
            this IEnumerable<TModGetter> immutableBaseCache,
            params TMod[] mutableMods)
            where TMod : class, IContextMod<TMod>, TModGetter
            where TModGetter : class, IContextGetterMod<TMod>
        {
            return new MutableLoadOrderLinkCache<TMod, TModGetter>(
                immutableBaseCache.ToImmutableLinkCache<TMod, TModGetter>(),
                mutableMods);
        }
    }
}
