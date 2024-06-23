using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

public record MasterStyleIndex(ModIndex Index, MasterStyle Style);

public interface IReadOnlySeparatedMasterPackage
{
    ModKey CurrentMod { get; }
    IReadOnlyMasterReferenceCollection Raw { get; }
    bool TryLookupModKey(ModKey modKey, [MaybeNullWhen(false)] out MasterStyleIndex index);
    ILoadOrderGetter<ModKey> GetLoadOrder(ModIndex modIndex);
}

public class SeparatedMasterPackage : IReadOnlySeparatedMasterPackage
{
    public ILoadOrderGetter<ModKey> Normal { get; private set; } = null!;
    public ILoadOrderGetter<ModKey>? Light { get; private set; }
    public ILoadOrderGetter<ModKey>? Medium { get; private set; }
    public ModKey CurrentMod { get; private set; }
    public IReadOnlyMasterReferenceCollection Raw { get; private set; } = null!;

    private IReadOnlyDictionary<ModKey, MasterStyleIndex> _lookup = null!;

    internal static readonly IReadOnlySeparatedMasterPackage EmptyNull = NotSeparate(new MasterReferenceCollection(ModKey.Null));

    internal SeparatedMasterPackage()
    {
    }
    
    public static IReadOnlySeparatedMasterPackage Factory(
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
    
    public static IReadOnlySeparatedMasterPackage Factory(
        GameRelease release,
        ModPath modPath,
        ILoadOrderGetter<IModFlagsGetter>? loadOrder,
        IFileSystem? fileSystem = null)
    {
        var masters = MasterReferenceCollection.FromPath(modPath, release, fileSystem: fileSystem);
        return Factory(release, modPath.ModKey, masters, loadOrder);
    }

    internal class NotSeparatedMasterPackage : IReadOnlySeparatedMasterPackage
    {
        public ModKey CurrentMod { get; }
        public IReadOnlyMasterReferenceCollection Raw { get; }

        public ILoadOrderGetter<ModKey> Normal { get; }
        public ILoadOrderGetter<ModKey>? Light => null;
        public ILoadOrderGetter<ModKey>? Medium => null;

        internal IReadOnlyDictionary<ModKey, MasterStyleIndex> _lookup = null!;

        public NotSeparatedMasterPackage(
            ModKey currentMod,
            IReadOnlyMasterReferenceCollection masters,
            LoadOrder<ModKey> normal)
        {
            CurrentMod = currentMod;
            Raw = masters;
            Normal = normal;
        }
    
        public bool TryLookupModKey(ModKey modKey, [MaybeNullWhen(false)] out MasterStyleIndex index)
        {
            return _lookup.TryGetValue(modKey, out index);
        }

        public ILoadOrderGetter<ModKey> GetLoadOrder(ModIndex modIndex)
        {
            return Normal;
        }
    }
    
    internal static IReadOnlySeparatedMasterPackage NotSeparate(IReadOnlyMasterReferenceCollection masters)
    {
        var normal = new List<ModKey>(masters.Masters.Select((x => x.Master)));
        normal.Add(masters.CurrentMod);
        var ret = new NotSeparatedMasterPackage(
            masters.CurrentMod,
            masters,
            new LoadOrder<ModKey>(normal, disposeItems: false));
        var lookup = new Dictionary<ModKey, MasterStyleIndex>();
        FillLookup(ret.Normal, lookup, MasterStyle.Normal);
        ret._lookup = lookup;
        return ret;
    }
    
    internal static IReadOnlySeparatedMasterPackage Separate(
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
        var lookup = new Dictionary<ModKey, MasterStyleIndex>();
        FillLookup(ret.Normal, lookup, MasterStyle.Normal);
        
        if (ret.Light != null)
        {
            FillLookup(ret.Light, lookup, MasterStyle.Light);
        }

        if (ret.Medium != null)
        {
            FillLookup(ret.Medium, lookup, MasterStyle.Medium);
        }
        ret._lookup = lookup;
        return ret;

    }

    public static IReadOnlySeparatedMasterPackage FromConstants(GameConstants constants, ModPath path, IFileSystem? fileSystem = null)
    {
        if (constants.SeparateMasterLoadOrders)
        {
            throw new ArgumentException(
                $"Cannot make {nameof(SeparatedMasterPackage)} just a path on a game that has separated masters: {constants.SeparateMasterLoadOrders}");
        }

        return NotSeparate(MasterReferenceCollection.FromPath(path, constants.Release, fileSystem: fileSystem));
    }

    internal static void FillLookup(
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