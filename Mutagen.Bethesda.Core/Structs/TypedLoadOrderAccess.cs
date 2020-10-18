using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public struct TypedLoadOrderAccess<TMajorGetter>
        where TMajorGetter : IMajorRecordCommonGetter
    {
        private Func<IEnumerable<TMajorGetter>> _getter;

        public TypedLoadOrderAccess(Func<IEnumerable<TMajorGetter>> getter)
        {
            _getter = getter;
        }

        public IEnumerable<TMajorGetter> WinningOverrides() => _getter();
    }
}
