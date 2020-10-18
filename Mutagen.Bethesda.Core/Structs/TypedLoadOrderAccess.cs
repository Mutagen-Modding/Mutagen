using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Internals
{
    public struct TypedLoadOrderAccess<TModSetter, TMajorSetter, TMajorGetter>
        where TModSetter : IModGetter
        where TMajorSetter : IMajorRecordCommon, TMajorGetter
        where TMajorGetter : IMajorRecordCommonGetter
    {
        private Func<IEnumerable<TMajorGetter>> _winningOverrides;
        private Func<IEnumerable<ModContext<TModSetter, TMajorSetter, TMajorGetter>>> _winningContextOverrides;

        public TypedLoadOrderAccess(
            Func<IEnumerable<TMajorGetter>> winningOverrides,
            Func<IEnumerable<ModContext<TModSetter, TMajorSetter, TMajorGetter>>> winningContextOverrides)
        {
            _winningOverrides = winningOverrides;
            _winningContextOverrides = winningContextOverrides;
        }

        public IEnumerable<TMajorGetter> WinningOverrides() => _winningOverrides();
        public IEnumerable<ModContext<TModSetter, TMajorSetter, TMajorGetter>> WinningContextOverrides() => _winningContextOverrides();
    }
}
