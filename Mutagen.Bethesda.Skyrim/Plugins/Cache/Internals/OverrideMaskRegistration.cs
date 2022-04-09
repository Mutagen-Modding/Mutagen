using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;

namespace Mutagen.Bethesda.Skyrim;

internal class SkyrimOverrideMaskRegistration : IOverrideMaskRegistration
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