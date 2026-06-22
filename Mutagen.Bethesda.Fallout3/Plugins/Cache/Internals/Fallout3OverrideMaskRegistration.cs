using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;

namespace Mutagen.Bethesda.Fallout3;

internal class Fallout3OverrideMaskRegistration : IOverrideMaskRegistration
{
    public IEnumerable<(ILoquiRegistration, object)> Masks
    {
        get
        {
            yield return (Cell_Registration.Instance, ModContextExt.CellCopyMask);
            yield return (Worldspace_Registration.Instance, ModContextExt.WorldspaceCopyMask);
        }
    }
}
