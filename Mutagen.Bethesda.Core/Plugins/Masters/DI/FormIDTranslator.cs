using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

internal static class FormIDTranslator
{
    public static FormKey GetFormKey(IReadOnlyMasterReferenceCollection masterReferences, uint idWithModID)
    {
        var modID = ModIndex.GetModIndexByteFromUInt(idWithModID);

        if (modID >= masterReferences.Masters.Count)
        {
            return new FormKey(
                masterReferences.CurrentMod,
                idWithModID);
        }

        var justId = idWithModID & 0xFFFFFF;
        if (modID == 0 && justId == 0)
        {
            return FormKey.Null;
        }

        var master = masterReferences.Masters[modID];
        return new FormKey(
            master.Master,
            idWithModID);
    }
    
    public static FormID GetFormID(
        SeparatedMasterPackage masters,
        IFormLinkIdentifier key)
    {
        if (!masters.TryLookup(key.FormKey.ModKey, out var index))
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