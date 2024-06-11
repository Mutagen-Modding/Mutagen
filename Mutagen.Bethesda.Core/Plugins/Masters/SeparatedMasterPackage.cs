using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

internal enum MasterStyle
{
    Normal,
    Light,
    Medium
}

internal class SeparatedMasterPackage
{
    public List<ModKey> Normal { get; private set; } = null!;
    public List<ModKey>? Light { get; private set; }
    public List<ModKey>? Medium { get; private set; }
    public IReadOnlyMasterReferenceCollection Original { get; private set; } = null!;

    public IReadOnlyDictionary<ModKey, (ModIndex Index, MasterStyle Style)> Lookup { get; private set; } = null!;

    public static SeparatedMasterPackage Factory(IReadOnlyMasterReferenceCollection masters)
    {
        var normal = new List<ModKey>(masters.Masters.Select((x => x.Master)));
        normal.Add(masters.CurrentMod);
        var ret = new SeparatedMasterPackage()
        {
            Normal = normal,
            Original = masters
        };
        ret.SetupLookup();
        return ret;
    }
    
    public static SeparatedMasterPackage Factory(
        IReadOnlyMasterReferenceCollection masters, 
        ILoadOrderGetter<IModFlagsGetter> loadOrder)
    {
        var normal = new List<ModKey>();
        var medium = new List<ModKey>();
        var light = new List<ModKey>();
        
        if (!loadOrder.TryGetValue(masters.CurrentMod, out var currentMod))
        {
            throw new MissingModException(masters.CurrentMod,
                "Mod was missing from load order when constructing the separate mod lists needed for FormID translation.");
        }
        
        void AddToList(IModFlagsGetter mod, ModKey modKey)
        {
            if (mod.IsHalfMaster)
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
            if (!loadOrder.TryGetValue(master.Master, out var mod))
            {
                throw new MissingModException(master.Master,
                    "Mod was missing from load order when constructing the separate mod lists needed for FormID translation.");
            }

            if (mod.IsLightMaster && mod.IsHalfMaster)
            {
                throw new ModHeaderMalformedException(mod.ModKey, "Mod had both Light and Medium master flags enabled");
            }

            AddToList(mod, master.Master);
        }
        
        AddToList(currentMod, masters.CurrentMod);

        var ret = new SeparatedMasterPackage()
        {
            Normal = normal,
            Medium = medium,
            Light = light,
            Original = masters,
        };
        ret.SetupLookup();
        return ret;

    }
    
    private void SetupLookup()
    {
        var lookup = new Dictionary<ModKey, (ModIndex Index, MasterStyle Style)>();
        FillLookup(Normal, lookup, MasterStyle.Normal);
        
        if (Light != null)
        {
            FillLookup(Light, lookup, MasterStyle.Light);
        }

        if (Medium != null)
        {
            FillLookup(Medium, lookup, MasterStyle.Medium);
        }

        Lookup = lookup;
    }

    private byte FillLookup(
        List<ModKey> masters,
        Dictionary<ModKey, (ModIndex Index, MasterStyle Style)> dict,
        MasterStyle style)
    {
        byte index = 0;
        foreach (var modKey in masters)
        {
            dict.Set(modKey, (new ModIndex(index), style));
            index++;
        }

        return index;
    }
}