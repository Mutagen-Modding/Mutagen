using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;

namespace Mutagen.Bethesda.Fallout4;

internal class Fallout4OverrideMaskRegistration : IOverrideMaskRegistration
{
    public IEnumerable<(ILoquiRegistration, object)> Masks
    {
        get
        {
            yield return (Cell_Registration.Instance, ModContextExt.CellCopyMask);
            yield return (Worldspace_Registration.Instance, ModContextExt.WorldspaceCopyMask);
            yield return (Quest_Registration.Instance, ModContextExt.QuestCopyMask);
            yield return (DialogTopic_Registration.Instance, ModContextExt.DialogTopicCopyMask);
        }
    }
}