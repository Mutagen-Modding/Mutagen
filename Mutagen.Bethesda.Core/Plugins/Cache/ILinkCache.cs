using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache
{
    /// <summary>
    /// An interface for retriving records given a FormKey.
    /// </summary>
    public interface ILinkCache : IIdentifierLinkCache
    {
        /// <summary>
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// Retrieves the winning record that matches the FormKey relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a MissingRecordException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <exception cref="MissingRecordException">
        /// When the FormKey cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IMajorRecordCommonGetter Resolve(FormKey formKey);

        /// <summary>
        /// Retrieves the winning record that matches the EditorID relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a MissingRecordException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <exception cref="MissingRecordException">
        /// When the EditorID cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IMajorRecordCommonGetter Resolve(string editorId);

        /// <summary>
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(FormKey formKey, Type type);

        /// <summary>
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(string editorId, Type type);

        /// <summary>
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types);

        /// <summary>
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(string editorId, params Type[] types);

        /// <summary>
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types);

        /// <summary>
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types);

        /// <summary>
        /// Retrieves the winning record that matches the FormKey relative to the source the package was attached to.
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
        /// <exception cref="MissingRecordException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the winning record that matches the EditorID relative to the source the package was attached to.
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
        /// <exception cref="MissingRecordException">
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
        /// Queries and caches all winning overrides of the given type within the cache
        /// </summary>
        /// <param name="type">Type to query and warm up for</param>
        void Warmup(Type type);

        /// <summary>
        /// Queries and caches all winning overrides of the given type within the cache
        /// </summary>
        /// <typeparam name="TMajor">Type to query and warm up for</typeparam>
        void Warmup<TMajor>();

        /// <summary>
        /// Queries and caches all winning overrides of the given types within the cache
        /// </summary>
        /// <param name="types">Types to query and warm up for</param>
        void Warmup(params Type[] types);

        /// <summary>
        /// Queries and caches all winning overrides of the given types within the cache
        /// </summary>
        /// <param name="types">Types to query and warm up for</param>
        void Warmup(IEnumerable<Type> types);

        /// <summary>
        /// Iterates through the contained mods in the order they were listed, with the least prioritized mod first.
        /// </summary>
        IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <summary>
        /// Iterates through the contained mods in priority order (reversed), with the most prioritized mod first.
        /// </summary>
        IReadOnlyList<IModGetter> PriorityOrder { get; }
    }

    public interface ILinkCache<TMod, TModGetter> : ILinkCache
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        /// <summary>
        /// Retrieves the winning record context that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the winning record context that matches the EditorID relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <param name="majorRec">Out parameter containing the record with context if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the winning record context that matches the FormKey relative to the source the package was attached to.<br/>
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
        bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the winning record context that matches the EditorID relative to the source the package was attached to.<br/>
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
        bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the winning record context that matches the FormKey relative to the source the package was attached to.<br/>
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
        bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the winning record context that matches the EditorID relative to the source the package was attached to.<br/>
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
        bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);

        /// <summary>
        /// Retrieves the winning record context that matches the FormKey relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a MissingRecordException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <exception cref="MissingRecordException">
        /// When the FormKey cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record with context</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey);

        /// <summary>
        /// Retrieves the winning record context that matches the EditorID relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a MissingRecordException.<br />
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <exception cref="MissingRecordException">
        /// When the EditorID cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>Matching record with context</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId);

        /// <summary>
        /// Retrieves the winning record context that matches the FormKey relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type);

        /// <summary>
        /// Retrieves the winning record context that matches the EditorID relative to the source the package was attached to.<br/>
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
        /// <exception cref="MissingRecordException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type);

        /// <summary>
        /// Retrieves the winning record context that matches the FormKey relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajor">The setter type of Major Record to look up</typeparam>
        /// <typeparam name="TMajorGetter">The getter type of Major Record to look up</typeparam>
        /// <returns>Matching record with context</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="MissingRecordException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the winning record context that matches the EditorID relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the EditorID, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="editorId">EditorID to look for</param>
        /// <typeparam name="TMajor">The setter type of Major Record to look up</typeparam>
        /// <typeparam name="TMajorGetter">The getter type of Major Record to look up</typeparam>
        /// <returns>Matching record with context</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="MissingRecordException">
        /// When the EditorID having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Iterates all record contexts that match the FormKey relative to the source the package was attached to.<br />
        /// If attached to a single mod, at most a single record can be found.<br />
        /// If attached to a load order, many records may be returned, depending on how many mods overrode the FormKey.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajor">The setter type of Major Record to look up</typeparam>
        /// <typeparam name="TMajorGetter">The getter type of Major Record to look up</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>Enumerable of all located record contexts that match the FormKey in the cache</returns>
        IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
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
        IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type);

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
        IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey);
    }
}
