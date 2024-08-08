using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

internal static class FormIDTranslator
{
    public static FormKey GetFormKey(
        IReadOnlySeparatedMasterPackage masterReferences,
        FormID formId,
        bool reference)
    {
        return masterReferences.GetFormKey(formId, reference);
    }
    
    public static FormID GetFormID(
        IReadOnlySeparatedMasterPackage masters,
        IFormLinkIdentifier key,
        bool reference)
    {
        if (key.FormKey == FormKey.Null)
        {
            return FormID.Null;
        }

        if (!masters.TryLookupModKey(key.FormKey.ModKey, reference, out var style, out var index))
        {
            throw new UnmappableFormIDException(
                key,
                masters.Raw.Masters
                    .Select(x => x.Master)
                    .ToArray());
        }

        return FormID.Factory(style, index, key.FormKey.ID);
    }
}