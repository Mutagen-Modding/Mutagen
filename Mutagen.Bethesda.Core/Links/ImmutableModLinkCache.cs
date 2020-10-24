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
    /// A Link Cache using a single mod as its link target. <br/>
    /// <br/>
    /// Internal caching will only occur for the types required to serve the requested link. <br/>
    /// <br/>
    /// All functionality is multithread safe. <br/>
    /// <br/>
    /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
    /// modifications occur on content already cached.
    /// </summary>
    /// <typeparam name="TMod">Mod type</typeparam>
    public class ImmutableModLinkCache<TMod> : ILinkCache
        where TMod : IModGetter
    {
        private readonly TMod _sourceMod;

        private readonly Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _untypedMajorRecords;
        private readonly Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _majorRecords = new Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>();

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

        /// <summary>
        /// Constructs a link cache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public ImmutableModLinkCache(TMod sourceMod)
        {
            this._sourceMod = sourceMod;
            this._untypedMajorRecords = new Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructCache());
            this.ListedOrder = new List<IModGetter>()
            {
                sourceMod
            };
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryLookup(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _untypedMajorRecords.Value.TryGetValue(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryLookup<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            IReadOnlyCache<IMajorRecordCommonGetter, FormKey> cache;
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

        /// <inheritdoc />
        public bool TryLookup(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            IReadOnlyCache<IMajorRecordCommonGetter, FormKey> cache;
            lock (_majorRecords)
            {
                cache = GetCache(type);
            }
            if (!cache.TryGetValue(formKey, out majorRec))
            {
                majorRec = default!;
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Lookup(FormKey formKey)
        {
            if (TryLookup<IMajorRecordCommonGetter>(formKey, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Lookup(FormKey formKey, Type type)
        {
            if (TryLookup(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Lookup<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryLookup<TMajor>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
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
                    var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_sourceMod.GameRelease.ToCategory());
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
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
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
    }
}
