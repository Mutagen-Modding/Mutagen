using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface that Group Record objects implement to hook into the common systems
    /// </summary>
    public interface IGroupCommonGetter<out TMajor> : IEnumerable<TMajor>
        where TMajor : IMajorRecordCommonGetter, IBinaryItem
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

    public interface IGroupCommon<TMajor> : IGroupCommonGetter<TMajor>
        where TMajor : IMajorRecordCommonGetter, IBinaryItem
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

        /// <summary>
        /// Clears all items
        /// </summary>
        void Clear();

        TMajor AddNew(FormKey formKey);

        /// <summary>
        /// Convenience function to instantiate a new Major Record and add it to the Group.<br />
        /// FormKey will be automatically assigned.
        /// </summary>
        /// <returns>New record already added to the Group</returns>
        TMajor AddNew();

        /// <summary>
        /// Convenience function to instantiate a new Major Record and add it to the Group.<br />
        /// FormKey will be automatically assigned based on the editorID given
        /// </summary>
        /// <param name="editorID">Editor ID to assign the new record.</param>
        /// <returns>New record already added to the Group</returns>
        TMajor AddNew(string? editorID);
    }

    public static class GroupExt
    {
        /// <summary>
        /// Duplicates a given record (giving it a new FormID) adding it to the group and returning it.
        /// </summary>
        /// <param name="group">Group to add to</param>
        /// <param name="source">Source record to duplicate</param>
        /// <returns>Duplicated and added record</returns>
        public static TMajor DuplicateInAsNewRecord<TMajor, TMajorGetter>(this IGroupCommon<TMajor> group, TMajorGetter source)
            where TMajor : IMajorRecordInternal, TMajorGetter
            where TMajorGetter : IMajorRecordGetter, IBinaryItem
        {
            try
            {
                var newRec = group.AddNew();
                var mask = OverrideMixIns.AddAsOverrideMasks.GetValueOrDefault(typeof(TMajor));
                newRec.DeepCopyIn(source, mask as MajorRecord.TranslationMask);
                group.RecordCache.Set(newRec);
                return newRec;
            }
            catch (Exception ex)
            {
                throw RecordException.Factory(ex, source.FormKey, source.EditorID);
            }
        }

        /// <summary>
        /// Duplicates a given record (giving it a new FormID) adding it to the group and returning it.
        /// </summary>
        /// <param name="group">Group to add to</param>
        /// <param name="source">Source record to duplicate</param>
        /// <param name="edid">EditorID to drive the FormID assignment off any persistance systems</param>
        /// <returns>Duplicated and added record</returns>
        public static TMajor DuplicateInAsNewRecord<TMajor, TMajorGetter>(this IGroupCommon<TMajor> group, TMajorGetter source, string? edid)
            where TMajor : IMajorRecordInternal, TMajorGetter
            where TMajorGetter : IMajorRecordGetter, IBinaryItem
        {
            try
            {
                var newRec = group.AddNew(edid);
                var mask = OverrideMixIns.AddAsOverrideMasks.GetValueOrDefault(typeof(TMajor));
                newRec.DeepCopyIn(source, mask as MajorRecord.TranslationMask);
                group.RecordCache.Set(newRec);
                return newRec;
            }
            catch (Exception ex)
            {
                throw RecordException.Factory(ex, source.FormKey, source.EditorID);
            }
        }

        /// <summary>
        /// Tries to retrieve a record from the group.
        /// </summary>
        /// <typeparam name="TMajor">Record type of the group</typeparam>
        /// <param name="group">Group to retrieve from</param>
        /// <param name="formKey">FormKey to query for</param>
        /// <param name="record">Record object, if located</param>
        /// <returns>True if record retreived from group</returns>
        public static bool TryGetValue<TMajor>(
            this IGroupCommonGetter<TMajor> group,
            FormKey formKey,
            [MaybeNullWhen(false)] out TMajor record)
            where TMajor : IMajorRecordCommonGetter, IBinaryItem
        {
            return group.RecordCache.TryGetValue(formKey, out record);
        }

        /// <summary>
        /// Tries to retrieve a record from the group.
        /// </summary>
        /// <typeparam name="TMajor">Record type of the group</typeparam>
        /// <param name="group">Group to retrieve from</param>
        /// <param name="formKey">FormKey to query for</param>
        /// <returns>Record object, if located</returns>
        public static TMajor? TryGetValue<TMajor>(
            this IGroupCommonGetter<TMajor> group,
            FormKey formKey)
            where TMajor : IMajorRecordCommonGetter, IBinaryItem
        {
            if (group.RecordCache.TryGetValue(formKey, out var record))
            {
                return record;
            }
            return default;
        }
    }
}
