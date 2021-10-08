using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache.Implementations
{
    internal class LinkCacheItem : IMajorRecordIdentifier
    {
        private readonly IMajorRecordCommonGetter? _record;
        private readonly string? _editorId;

        public FormKey FormKey { get; }
        public string? EditorID => _record?.EditorID ?? _editorId;
        public IMajorRecordCommonGetter Record => _record ?? throw new ArgumentException("Queried for record on a simple cache");

        public LinkCacheItem(
            IMajorRecordCommonGetter? record,
            FormKey formKey,
            string? editorId)
        {
            _record = record;
            _editorId = editorId;
            FormKey = formKey;
        }

        public static LinkCacheItem Factory(IMajorRecordCommonGetter record, bool simple)
        {
            return new LinkCacheItem(
                simple ? null : record,
                record.FormKey,
                simple ? record.EditorID : null);
        }
    }
}
