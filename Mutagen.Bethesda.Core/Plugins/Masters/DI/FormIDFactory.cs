using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

public class FormIDFactory
{
    public FormID GetFormID(
        GameConstants constants,
        IReadOnlyMasterReferenceCollection masterIndices,
        IFormLinkIdentifier key)
    {
        if (masterIndices.TryGetIndex(key.FormKey.ModKey, out var index))
        {
            return new FormID(
                index,
                key.FormKey.ID);
        }

        if (key.FormKey == FormKey.Null)
        {
            return FormID.Null;
        }

        throw new UnmappableFormIDException(
            key,
            masterIndices.Masters
                .Select(x => x.Master)
                .ToArray());
    }
}