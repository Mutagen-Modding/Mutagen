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

        public ModContext(TMajorGetter record, Func<TMod, TMajorGetter, TMajorSetter> getter)
        {
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
    }
}
