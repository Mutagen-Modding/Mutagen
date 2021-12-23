using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda
{
    public static class IGroupMixIns
    {

        /// <summary>
        /// Convenience function to instantiate a new Major Record and add it to the Group<br />
        /// </summary>
        /// <param name="group">Group to add a record to</param>
        /// <param name="formKey">FormKey assign the new record.</param>
        /// <returns>New record already added to the Group</returns>
        public static TMajor AddNew<TMajor>(this IGroup<TMajor> group, FormKey formKey)
            where TMajor : IMajorRecordInternal
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(
                formKey,
                group.SourceMod.GameRelease);
            group.Set(ret);
            return ret;
        }

        /// <summary>
        /// Convenience function to instantiate a new Major Record and add it to the Group.<br />
        /// FormKey will be automatically assigned.
        /// </summary>
        /// <param name="group">Group to add a record to</param>
        /// <returns>New record already added to the Group</returns>
        public static TMajor AddNew<TMajor>(this IGroup<TMajor> group)
            where TMajor : IMajorRecordInternal
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(
                group.SourceMod.GetNextFormKey(),
                group.SourceMod.GameRelease);
            group.Set(ret);
            return ret;
        }

        /// <summary>
        /// Convenience function to instantiate a new Major Record and add it to the Group.<br />
        /// FormKey will be automatically assigned based on the editorID given
        /// </summary>
        /// <param name="group">Group to add a record to</param>
        /// <param name="editorID">Editor ID to assign the new record, and use in any FormKey persistence logic.</param>
        /// <returns>New record already added to the Group</returns>
        public static TMajor AddNew<TMajor>(this IGroup<TMajor> group, string? editorID)
            where TMajor : IMajorRecordInternal
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(
                group.SourceMod.GetNextFormKey(editorID),
                group.SourceMod.GameRelease);
            ret.EditorID = editorID;
            group.Set(ret);
            return ret;
        }

        /// <summary>
        /// Duplicates a given record (giving it a new FormID) adding it to the group and returning it.
        /// </summary>
        /// <param name="group">Group to add to</param>
        /// <param name="source">Source record to duplicate</param>
        /// <returns>Duplicated and added record</returns>
        public static TMajor DuplicateInAsNewRecord<TMajor, TMajorGetter>(this IGroup<TMajor> group, TMajorGetter source)
            where TMajor : class, IMajorRecordInternal, TMajorGetter
            where TMajorGetter : IMajorRecordGetter
        {
            return DuplicateInAsNewRecord<TMajor, TMajorGetter, TMajorGetter>(group, source);
        }

        /// <summary>
        /// Duplicates a given record (giving it a new FormID) adding it to the group and returning it.
        /// </summary>
        /// <param name="group">Group to add to</param>
        /// <param name="source">Source record to duplicate</param>
        /// <returns>Duplicated and added record</returns>
        public static TMajor DuplicateInAsNewRecord<TMajor, TMajorGetter, TSharedParent>(this IGroup<TMajor> group, TMajorGetter source)
            where TMajor : class, IMajorRecordInternal, TSharedParent
            where TMajorGetter : TSharedParent
            where TSharedParent : IMajorRecordGetter
        {
            try
            {
                var newRec = (source.Duplicate(group.SourceMod.GetNextFormKey()) as TMajor)!;
                group.Add(newRec);
                return newRec;
            }
            catch (Exception ex)
            {
                throw RecordException.Enrich<TMajor>(ex, source.FormKey, source.EditorID);
            }
        }

        /// <summary>
        /// Duplicates a given record (giving it a new FormID) adding it to the group and returning it.
        /// </summary>
        /// <param name="group">Group to add to</param>
        /// <param name="source">Source record to duplicate</param>
        /// <param name="edid">EditorID to drive the FormID assignment off any persistence systems</param>
        /// <returns>Duplicated and added record</returns>
        public static TMajor DuplicateInAsNewRecord<TMajor, TMajorGetter>(this IGroup<TMajor> group, TMajorGetter source, string? edid)
            where TMajor : class, IMajorRecordInternal, TMajorGetter
            where TMajorGetter : IMajorRecordGetter
        {
            return DuplicateInAsNewRecord<TMajor, TMajorGetter, TMajorGetter>(group, source, edid);
        }

        /// <summary>
        /// Duplicates a given record (giving it a new FormID) adding it to the group and returning it.
        /// </summary>
        /// <param name="group">Group to add to</param>
        /// <param name="source">Source record to duplicate</param>
        /// <param name="edid">EditorID to drive the FormID assignment off any persistence systems</param>
        /// <returns>Duplicated and added record</returns>
        public static TMajor DuplicateInAsNewRecord<TMajor, TMajorGetter, TSharedParent>(this IGroup<TMajor> group, TMajorGetter source, string? edid)
            where TMajor : class, IMajorRecordInternal, TSharedParent
            where TMajorGetter : TSharedParent
            where TSharedParent : IMajorRecordGetter
        {
            try
            {
                var newRec = (source.Duplicate(group.SourceMod.GetNextFormKey(edid)) as TMajor)!;
                group.Add(newRec);
                return newRec;
            }
            catch (Exception ex)
            {
                throw RecordException.Enrich<TMajor>(ex, source.FormKey, source.EditorID);
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
            this IGroupGetter<TMajor> group,
            FormKey formKey,
            [MaybeNullWhen(false)] out TMajor record)
            where TMajor : IMajorRecordGetter
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
            this IGroupGetter<TMajor> group,
            FormKey formKey)
            where TMajor : IMajorRecordGetter
        {
            if (group.RecordCache.TryGetValue(formKey, out var record))
            {
                return record;
            }
            return default;
        }
    }
}
