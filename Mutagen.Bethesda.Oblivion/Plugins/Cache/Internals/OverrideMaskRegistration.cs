using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins.Cache.Internals;

namespace Mutagen.Bethesda.Oblivion.Plugins.Cache.Internals
{
    public class OverrideMaskRegistration : IOverrideMaskRegistration
    {
        public IEnumerable<(ILoquiRegistration, object)> Masks
        {
            get
            {
                yield return (Cell_Registration.Instance, ModContextExt.CellCopyMask);
                yield return (Worldspace_Registration.Instance, ModContextExt.WorldspaceCopyMask);
                yield return (DialogResponse_Registration.Instance, ModContextExt.DialogResponsesCopyMask);
            }
        }
    }
}