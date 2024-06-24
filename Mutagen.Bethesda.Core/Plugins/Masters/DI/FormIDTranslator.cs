using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

internal static class FormIDTranslator
{
    public static FormKey GetFormKey(IReadOnlySeparatedMasterPackage masterReferences, uint idWithModID)
    {
        return masterReferences.GetFormKey(idWithModID);
    }
    
    public static FormID GetFormID(
        IReadOnlySeparatedMasterPackage masters,
        IFormLinkIdentifier key)
    {
        if (!masters.TryLookupModKey(key.FormKey.ModKey, out var index))
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

        switch (index.Style)
        {
            case MasterStyle.Normal:
                return new FormID(
                    index.Index,
                    key.FormKey.ID);
            case MasterStyle.Light:
            {
                var light = new LightMasterFormID(index.Index.ID, key.FormKey.ID);
                return new FormID(light.Raw);
            }
            case MasterStyle.Medium:
            {
                var medium = new MediumMasterFormID(index.Index.ID, key.FormKey.ID);
                return new FormID(medium.Raw);
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}