using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

public record MasterStyleIndex(ModIndex Index, MasterStyle Style);

public interface ISeparatedMasterPackage
{
    ModKey CurrentMod { get; }
    IReadOnlyMasterReferenceCollection Raw { get; }
    bool TryLookupModKey(ModKey modKey, [MaybeNullWhen(false)] out MasterStyleIndex index);
    ILoadOrderGetter<ModKey> GetLoadOrder(ModIndex modIndex);
    internal ILoadOrderGetter<ModKey> Normal { get; }
    internal ILoadOrderGetter<ModKey>? Light { get; }
    internal ILoadOrderGetter<ModKey>? Medium { get; }
}

public class SeparatedMasterPackage : ISeparatedMasterPackage
{
    public ILoadOrderGetter<ModKey> Normal { get; private set; } = null!;
    public ILoadOrderGetter<ModKey>? Light { get; private set; }
    public ILoadOrderGetter<ModKey>? Medium { get; private set; }
    public ModKey CurrentMod { get; private set; }
    public IReadOnlyMasterReferenceCollection Raw { get; private set; } = null!;

    private IReadOnlyDictionary<ModKey, MasterStyleIndex> _lookup = null!;

    internal static readonly ISeparatedMasterPackage EmptyNull = NotSeparate(new MasterReferenceCollection(ModKey.Null));
    
    public static ISeparatedMasterPackage Factory(
        GameRelease release,
        ModKey currentModKey,
        IReadOnlyMasterReferenceCollection masters, 
        ILoadOrderGetter<IModFlagsGetter>? loadOrder)
    {
        var constants = GameConstants.Get(release);
        if (constants.SeparateMasterLoadOrders)
        {
            return SeparatedMasterPackage.Separate(currentModKey, masters, loadOrder);
        }
        else
        {
            return SeparatedMasterPackage.NotSeparate(masters);
        }
    }
    
    public static ISeparatedMasterPackage Factory(
        GameRelease release,
        ModPath modPath,
        ILoadOrderGetter<IModFlagsGetter>? loadOrder,
        IFileSystem? fileSystem = null)
    {
        var masters = MasterReferenceCollection.FromPath(modPath, release, fileSystem: fileSystem);
        return Factory(release, modPath.ModKey, masters, loadOrder);
    }

    internal static ISeparatedMasterPackage NotSeparate(IReadOnlyMasterReferenceCollection masters)
    {
        var normal = new List<ModKey>(masters.Masters.Select((x => x.Master)));
        normal.Add(masters.CurrentMod);
        var ret = new SeparatedMasterPackage()
        {
            Normal = new LoadOrder<ModKey>(normal, disposeItems: false),
            Raw = masters,
            CurrentMod = masters.CurrentMod,
        };
        ret.SetupLookup();
        return ret;
    }
    
    internal static ISeparatedMasterPackage Separate(
        ModKey currentModKey,
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

        normal.Add(currentModKey);

        var ret = new SeparatedMasterPackage()
        {
            Normal = new LoadOrder<ModKey>(normal, disposeItems: false),
            Medium = new LoadOrder<ModKey>(medium, disposeItems: false),
            Light = new LoadOrder<ModKey>(light, disposeItems: false),
            Raw = masters,
            CurrentMod = masters.CurrentMod,
        };
        ret.SetupLookup();
        return ret;

    }

    public static ISeparatedMasterPackage FromConstants(GameConstants constants, ModPath path, IFileSystem? fileSystem = null)
    {
        if (constants.SeparateMasterLoadOrders)
        {
            throw new ArgumentException(
                $"Cannot make {nameof(SeparatedMasterPackage)} just a path on a game that has separated masters: {constants.SeparateMasterLoadOrders}");
        }

        return NotSeparate(MasterReferenceCollection.FromPath(path, constants.Release, fileSystem: fileSystem));
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

    public bool TryLookupModKey(ModKey modKey, [MaybeNullWhen(false)] out MasterStyleIndex index)
    {
        return _lookup.TryGetValue(modKey, out index);
    }
    
    public ILoadOrderGetter<ModKey> GetLoadOrder(ModIndex modIndex)
    {
        if (modIndex == ModIndex.LightMaster)
        {
            return Light ?? Normal;
        }
        else if (modIndex == ModIndex.MediumMaster)
        {
            return Medium ?? Normal;
        }
        else
        {
            return Normal;
        }
    }
}