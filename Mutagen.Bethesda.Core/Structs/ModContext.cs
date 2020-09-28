using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public struct ModContext<TMod, TMajorSetter, TMajorGetter>
        where TMod : IModGetter
        where TMajorSetter : IMajorRecordCommon, TMajorGetter
        where TMajorGetter : IMajorRecordCommonGetter
    {
        private readonly Func<TMod, TMajorGetter, TMajorSetter> _getOrAddAsOverride;
        public readonly TMajorGetter Record;
        public readonly ModKey ModKey;

        public ModContext(ModKey modKey, TMajorGetter record, Func<TMod, TMajorGetter, TMajorSetter> getter)
        {
            ModKey = modKey;
            Record = record;
            _getOrAddAsOverride = getter;
        }

        public static implicit operator TMajorGetter(ModContext<TMod, TMajorSetter, TMajorGetter> context)
        {
            return context.Record;
        }

        public TMajorSetter GetOrAddAsOverride(TMod mod)
        {
            try
            {
                return _getOrAddAsOverride(mod, Record);
            }
            catch (Exception ex)
            {
                throw RecordException.Factory(ex, Record);
            }
        }

        internal ModContext<TMod, TMajorSetter, TMajorGetter> AddModKey(ModKey modKey)
        {
            return new ModContext<TMod, TMajorSetter, TMajorGetter>(
                modKey: modKey,
                record: Record,
                getter: _getOrAddAsOverride);
        }
    }

    namespace Internals
    {
        public static class ModContextExt
        {
            public static ModContext<TMod, TMajorSetter, TMajorGetter> AddModKey<TMod, TMajorSetter, TMajorGetter>(ModContext<TMod, TMajorSetter, TMajorGetter> context, ModKey key)
                where TMod : IModGetter
                where TMajorSetter : IMajorRecordCommon, TMajorGetter
                where TMajorGetter : IMajorRecordCommonGetter
            {
                return context.AddModKey(key);
            }
        }
    }
}
