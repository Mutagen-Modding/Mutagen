using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>
    /// An interface that Group Record objects implement to hook into the common systems
    /// </summary>
    public interface IGroupCommonGetter<out TMajor> : IEnumerable<TMajor>
        where TMajor : IMajorRecordCommonGetter
    {
        /// <summary>
        /// Mod object the Group belongs to
        /// </summary>
        IMod SourceMod { get; }

        /// <summary>
        /// Access to records in an IReadOnlyCache interface
        /// </summary>
        IReadOnlyCache<TMajor, FormKey> RecordCache { get; }

        /// <summary>
        /// Number of contained records
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the record associated with the specified key
        /// </summary>
        /// <param name="key">FormKey to retrieve</param>
        /// <exception cref="KeyNotFoundException">A record with the given FormKey does not exist</exception>
        /// <returns>Record associated with the specified key</returns>
        TMajor this[FormKey key] { get; }

        /// <summary>
        /// Enumerable containing all the FormKeys present in the group
        /// </summary>
        IEnumerable<FormKey> FormKeys { get; }

        /// <summary>
        /// Checks if a record with the specified key exists in the group
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>True if record found with given key</returns>
        bool ContainsKey(FormKey key);
    }

    public interface IGroupCommon<TMajor> : IGroupCommonGetter<TMajor>, IClearable
        where TMajor : IMajorRecordCommonGetter
    {
        /// <summary>
        /// Access to records in an ICache interface
        /// </summary>
        new ICache<TMajor, FormKey> RecordCache { get; }

        /// <summary>
        /// Adds an item using the specified key
        /// </summary>
        /// <param name="item">The item.</param>
        void Add(TMajor item);

        /// <summary>
        /// Adds or replaces the major record
        /// </summary>
        /// <param name="record">The record</param>
        void Set(TMajor record);

        /// <summary>
        /// Adds or updates the major records given
        /// </summary>
        /// <param name="records">The records</param>
        void Set(IEnumerable<TMajor> records);

        /// <summary>
        /// Removes the item matching the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        bool Remove(FormKey key);

        /// <summary>
        /// Removes all items matching the specified keys
        /// </summary>
        /// <param name="keys">The keys.</param>
        void Remove(IEnumerable<FormKey> keys);
    }
}
