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
    public IReadOnlyMasterReferenceCollection Normal { get; private set; } = null!;
    public IReadOnlyMasterReferenceCollection? Light { get; private set; }
    public IReadOnlyMasterReferenceCollection? Medium { get; private set; }
    public IReadOnlyMasterReferenceCollection Original { get; private set; } = null!;

    public IReadOnlyDictionary<ModKey, (ModIndex Index, MasterStyle Style)> Lookup { get; private set; } = null!;

    public static SeparatedMasterPackage Factory(IReadOnlyMasterReferenceCollection masters)
    {
        var ret = new SeparatedMasterPackage()
        {
            Normal = masters,
            Original = masters
        };
        ret.SetupLookup();
        return ret;
    }
    
    public static SeparatedMasterPackage Factory(
        IReadOnlyMasterReferenceCollection masters, 
        ILoadOrderGetter<IModFlagsGetter> loadOrder)
    {
        var normal = new List<IMasterReferenceGetter>();
        var medium = new List<IMasterReferenceGetter>();
        var light = new List<IMasterReferenceGetter>();
        
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

            if (mod.IsHalfMaster)
            {
                medium.Add(master);
            }
            else if (mod.IsLightMaster)
            {
                light.Add(master);
            }
            else
            {
                normal.Add(master);
            }
        }

        var normalRefs = new MasterReferenceCollection(masters.CurrentMod);
        var mediumRefs = new MasterReferenceCollection(masters.CurrentMod);
        var lightRefs = new MasterReferenceCollection(masters.CurrentMod);
        
        normalRefs.SetTo(normal);
        mediumRefs.SetTo(medium);
        lightRefs.SetTo(light);

        var ret = new SeparatedMasterPackage()
        {
            Normal = normalRefs,
            Medium = mediumRefs,
            Light = lightRefs,
            Original = masters,
        };
        ret.SetupLookup();
        return ret;
    }
    
    private void SetupLookup()
    {
        var lookup = new Dictionary<ModKey, (ModIndex Index, MasterStyle Style)>();
        var index = FillLookup(Normal, lookup, MasterStyle.Normal);
        lookup.Set(Normal.CurrentMod, (new ModIndex(index), MasterStyle.Normal));
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
        IReadOnlyMasterReferenceCollection masters,
        Dictionary<ModKey, (ModIndex Index, MasterStyle Style)> dict,
        MasterStyle style)
    {
        byte index = 0;
        foreach (var master in masters.Masters)
        {
            dict.Set(master.Master, (new ModIndex(index), style));
            index++;
        }

        return index;
    }
}