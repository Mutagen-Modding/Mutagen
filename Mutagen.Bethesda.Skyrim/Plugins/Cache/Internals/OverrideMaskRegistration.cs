using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim.Plugins.Cache.Internals
{
    public class OverrideMaskRegistration : IOverrideMaskRegistration
    {
        public IEnumerable<(ILoquiRegistration, object)> Masks
        {
            get
            {
                yield return (Cell_Registration.Instance, ModContextExt.CellCopyMask);
                yield return (Worldspace_Registration.Instance, ModContextExt.WorldspaceCopyMask);
                yield return (DialogTopic_Registration.Instance, ModContextExt.DialogTopicCopyMask);
            }
        }
    }
}