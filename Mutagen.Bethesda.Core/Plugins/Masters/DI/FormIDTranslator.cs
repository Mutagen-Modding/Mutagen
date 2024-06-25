using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

internal static class FormIDTranslator
{
    public static FormKey GetFormKey(IReadOnlySeparatedMasterPackage masterReferences, FormID formId)
    {
        return masterReferences.GetFormKey(formId);
    }
    
    public static FormID GetFormID(
        IReadOnlySeparatedMasterPackage masters,
        IFormLinkIdentifier key)
    {
        if (!masters.TryLookupModKey(key.FormKey.ModKey, out var style, out var index))
        {
            if (key.FormKey == FormKey.Null)
            {
                return FormID.Null;
            }

            throw new UnmappableFormIDException(
                key,
                masters.Raw.Masters
                    .Select(x => x.Master)
                    .ToArray());
        }

        return FormID.Factory(style, index, key.FormKey.ID);
    }
}