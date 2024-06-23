using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

internal static class FormIDTranslator
{
    public static FormKey GetFormKey(ISeparatedMasterPackage masterReferences, uint idWithModID)
    {
        var modID = ModIndex.GetModIndexFromUInt(idWithModID);

        ILoadOrderGetter<ModKey> loadOrder = masterReferences.GetLoadOrder(modID);

        if (modID.ID >= loadOrder.Count)
        {
            return new FormKey(
                masterReferences.CurrentMod,
                idWithModID);
        }

        var justId = idWithModID & 0xFFFFFF;
        if (modID.ID == 0 && justId == 0)
        {
            return FormKey.Null;
        }

        var master = loadOrder[modID.ID];
        return new FormKey(
            master,
            idWithModID);
    }
    
    public static FormID GetFormID(
        ISeparatedMasterPackage masters,
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