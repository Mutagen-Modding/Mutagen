using Loqui;
using Mutagen.Bethesda.Core;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A Link Cache using a single mod as its link target.
    ///
    /// Internal caching will only occur for the types required to serve the requested link.
    ///
    /// All functionality is multithread safe.
    ///
    /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
    /// modifications occur on content already cached.
    /// </summary>
    /// <typeparam name="TMod">Mod type</typeparam>
    public class DirectModLinkCache<TMod> : ILinkCache
        where TMod : IModGetter
    {
        private readonly TMod _sourceMod;

        private readonly Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _untypedMajorRecords;
        private readonly Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _majorRecords = new Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>();

        /// <summary>
        /// Constructs a DirectModLinkCache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public DirectModLinkCache(TMod sourceMod)
        {
            this._sourceMod = sourceMod;
            this._untypedMajorRecords = new Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructCache());
        }

        /// <summary>
        /// Looks up a given FormKey to try to locate the target record.
        ///
        /// This call is not as optimized as its generic typed counterpart.
        /// It does not know what type the record is limited to, and so much load and process
        /// all record types in order to do a proper search.
        /// </summary>
        /// <param name="formKey">FormKey to search for</param>
        /// <param name="majorRec">MajorRecord if found</param>
        /// <returns>True if record was found</returns>
        public bool TryLookup(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _untypedMajorRecords.Value.TryGetValue(formKey, out majorRec);
        }

        /// <summary>
        /// Looks up a given FormKey to try to locate the target record.
        ///
        /// Will only look into the Groups that are applicable to the given type.
        /// </summary>
        /// <param name="formKey">FormKey to search for</param>
        /// <param name="majorRec">MajorRecord if found</param>
        /// <typeparam name="TMajor">MajorRecod type or interface to look for</typeparam>
        /// <returns>True if record was found</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.
        /// Unexpected types include:
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        public bool TryLookup<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            IReadOnlyCache<object, FormKey> cache;
            lock (_majorRecords)
            {
                cache = GetCache(typeof(TMajor));
            }
            if (!cache.TryGetValue(formKey, out var majorRecObj))
            {
                majorRec = default!;
                return false;
            }
            majorRec = (majorRecObj as TMajor)!;
            return majorRec != null;
        }

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> GetCache(Type type)
        {
            if (!_majorRecords.TryGetValue(type, out var cache))
            {
                if (type.Equals(typeof(IMajorRecordCommon))
                    || type.Equals(typeof(IMajorRecordCommonGetter)))
                {
                    cache = ConstructCache(type);
                    _majorRecords[typeof(IMajorRecordCommon)] = cache;
                    _majorRecords[typeof(IMajorRecordCommonGetter)] = cache;
                }
                else if (LoquiRegistration.TryGetRegister(type, out var registration))
                {
                    cache = ConstructCache(type);
                    _majorRecords[registration.ClassType] = cache;
                    _majorRecords[registration.GetterType] = cache;
                    _majorRecords[registration.SetterType] = cache;
                    if (registration.InternalGetterType != null)
                    {
                        _majorRecords[registration.InternalGetterType] = cache;
                    }
                    if (registration.InternalSetterType != null)
                    {
                        _majorRecords[registration.InternalSetterType] = cache;
                    }
                }
                else
                {
                    var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(GameRelease.Skyrim);
                    if (!interfaceMappings.TryGetValue(type, out var objs))
                    {
                        throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                    }
                    var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
                    foreach (var objType in objs)
                    {
                        majorRecords.Set(GetCache(LoquiRegistration.GetRegister(objType).GetterType).Items);
                    }
                    _majorRecords[type] = majorRecords;
                    cache = majorRecords;
                }
            }
            return cache;
        }

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> ConstructCache(Type type)
        {
            var cache = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords(type))
            {
                cache.Set(majorRec);
            }
            return cache;
        }

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> ConstructCache()
        {
            var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords())
            {
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this._sourceMod;
        }

        IEnumerator<IModGetter> IEnumerable<IModGetter>.GetEnumerator()
        {
            yield return this._sourceMod;
        }
    }
}
