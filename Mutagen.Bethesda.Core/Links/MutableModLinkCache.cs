using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
    /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
    /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
    /// <br/>
    /// If being used in a multithreaded scenario,<br/>
    /// this cache must be locked alongside any mutations to the mod the cache wraps
    /// </summary>
    /// <typeparam name="TMod">Mod type</typeparam>
    public class MutableModLinkCache<TMod> : ILinkCache
        where TMod : IModGetter
    {
        private readonly TMod _sourceMod;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

        /// <summary>
        /// Constructs a link cache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public MutableModLinkCache(TMod sourceMod)
        {
            this._sourceMod = sourceMod;
            this.ListedOrder = new List<IModGetter>()
            {
                sourceMod
            };
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryLookup(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var item in this._sourceMod.EnumerateMajorRecords())
            {
                if (item.FormKey == formKey)
                {
                    majorRec = item;
                    return true;
                }
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryLookup<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords<TMajor>())
            {
                if (major.FormKey == formKey)
                {
                    majorRec = major;
                    return true;
                }
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryLookup(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords(type))
            {
                if (major.FormKey == formKey)
                {
                    majorRec = major;
                    return true;
                }
            }

            majorRec = default;
            return false;
        }
    }
}
