using System;
using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>
    /// An interface that Group Record objects implement to hook into the common systems
    /// </summary>
    public interface IGroupGetter<out TMajor> : IEnumerable<TMajor>
        where TMajor : IMajorRecordGetter
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

    public interface IGroup : IGroupGetter<IMajorRecordGetter>
    {
        
    }

    public interface IGroup<TMajor> : IGroupGetter<TMajor>, IClearable
        where TMajor : IMajorRecordGetter
    {
        /// <summary>
        /// Access to records in an ICache interface
        /// </summary>
        new ICache<TMajor, FormKey> RecordCache { get; }

        /// <summary>
        /// Adds a major record to the group
        /// </summary>
        /// <param name="record">The record</param>
        /// <exception cref="ArgumentException">
        /// A record with the same FormKey already exists in the group
        /// </exception>
        void Add(TMajor record);

        /// <summary>
        /// Adds
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="ArgumentException">
        /// A record with the same FormKey already exists in the group, or is of the wrong type.
        /// </exception>
        void AddUntyped(IMajorRecord record);

        /// <summary>
        /// Adds or replaces the major record
        /// </summary>
        /// <param name="record">The record</param>
        void Set(TMajor record);

        /// <summary>
        /// Adds or replaces the major record
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Record was the wrong type
        /// </exception>
        /// <param name="record">The record</param>
        void SetUntyped(IMajorRecord record);

        /// <summary>
        /// Adds or updates the major records given
        /// </summary>
        /// <param name="records">The records</param>
        void Set(IEnumerable<TMajor> records);

        /// <summary>
        /// Adds or updates the major records given
        /// </summary>
        /// <exception cref="ArgumentException">
        /// A record was the wrong type.  The contents of the group will be undefined.  Some records may have been added.
        /// </exception>
        /// <param name="records">The records</param>
        void SetUntyped(IEnumerable<IMajorRecord> records);

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
