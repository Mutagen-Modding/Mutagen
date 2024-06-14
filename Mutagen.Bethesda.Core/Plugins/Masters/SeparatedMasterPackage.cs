using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

public record MasterStyleIndex(ModIndex Index, MasterStyle Style);

public class SeparatedMasterPackage
{
    public ILoadOrderGetter<ModKey> Normal { get; private set; } = null!;
    public ILoadOrderGetter<ModKey>? Light { get; private set; }
    public ILoadOrderGetter<ModKey>? Medium { get; private set; }
    public IReadOnlyMasterReferenceCollection Raw { get; private set; } = null!;

    private IReadOnlyDictionary<ModKey, MasterStyleIndex> _lookup = null!;

    public static SeparatedMasterPackage NotSeparate(IReadOnlyMasterReferenceCollection masters)
    {
        var normal = new List<ModKey>(masters.Masters.Select((x => x.Master)));
        normal.Add(masters.CurrentMod);
        var ret = new SeparatedMasterPackage()
        {
            Normal = new LoadOrder<ModKey>(normal, disposeItems: false),
            Raw = masters
        };
        ret.SetupLookup();
        return ret;
    }
    
    public static SeparatedMasterPackage Separate(
        IModFlagsGetter currentMod,
        IReadOnlyMasterReferenceCollection masters, 
        ILoadOrderGetter<IModFlagsGetter>? loadOrder)
    {
        var normal = new List<ModKey>();
        var medium = new List<ModKey>();
        var light = new List<ModKey>();
        
        void AddToList(IModFlagsGetter mod, ModKey modKey)
        {
            if (mod.IsMediumMaster)
            {
                medium.Add(modKey);
            }
            else if (mod.IsLightMaster)
            {
                light.Add(modKey);
            }
            else
            {
                normal.Add(modKey);
            }
        }

        foreach (var master in masters.Masters)
        {
            if (loadOrder != null)
            {
                if (!loadOrder.TryGetValue(master.Master, out var mod))
                {
                    throw new MissingModException(master.Master,
                        "Mod was missing from load order when constructing the separate mod lists needed for FormID translation.");
                }

                if (mod.IsLightMaster && mod.IsMediumMaster)
                {
                    throw new ModHeaderMalformedException(mod.ModKey, "Mod had both Light and Medium master flags enabled");
                }

                AddToList(mod, master.Master);
            }
            // Don't have a load order, assume normal
            // Viewed as user error if this turns out to be wrong
            // They should provide load order unless they're sure it's not needed
            else
            {
                normal.Add(master.Master);
            }
        }

        normal.Add(currentMod.ModKey);

        var ret = new SeparatedMasterPackage()
        {
            Normal = new LoadOrder<ModKey>(normal, disposeItems: false),
            Medium = new LoadOrder<ModKey>(medium, disposeItems: false),
            Light = new LoadOrder<ModKey>(light, disposeItems: false),
            Raw = masters,
        };
        ret.SetupLookup();
        return ret;

    }
    
    private void SetupLookup()
    {
        var lookup = new Dictionary<ModKey, MasterStyleIndex>();
        FillLookup(Normal, lookup, MasterStyle.Normal);
        
        if (Light != null)
        {
            FillLookup(Light, lookup, MasterStyle.Light);
        }

        if (Medium != null)
        {
            FillLookup(Medium, lookup, MasterStyle.Medium);
        }

        _lookup = lookup;
    }

    private void FillLookup(
        ILoadOrderGetter<ModKey> masters,
        Dictionary<ModKey, MasterStyleIndex> dict,
        MasterStyle style)
    {
        byte index = 0;
        foreach (var modKey in masters.ListedOrder)
        {
            dict.Set(modKey, new (new ModIndex(index), style));
            index++;
        }
    }

    public bool TryLookup(ModKey modKey, [MaybeNullWhen(false)] out MasterStyleIndex index)
    {
        return _lookup.TryGetValue(modKey, out index);
    }
}