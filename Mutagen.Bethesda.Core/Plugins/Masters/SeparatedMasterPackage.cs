using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

public interface IReadOnlySeparatedMasterPackage
{
    ModKey CurrentMod { get; }
    IReadOnlyMasterReferenceCollection Raw { get; }
    bool TryLookupModKey(ModKey modKey, bool reference, out MasterStyle style, out uint index);
    FormKey GetFormKey(FormID formId, bool reference);
}

public class SeparatedMasterPackage : IReadOnlySeparatedMasterPackage
{
    internal record MasterStyleIndex(uint Index, MasterStyle Style);

    public IReadOnlyList<ModKey> Full { get; private set; } = null!;
    public IReadOnlyList<ModKey> Light { get; private set; } = null!;
    public IReadOnlyList<ModKey> Medium { get; private set; } = null!;
    public ModKey CurrentMod { get; private set; }
    public IReadOnlyMasterReferenceCollection Raw { get; private set; } = null!;

    private IReadOnlyDictionary<ModKey, MasterStyleIndex> _lookup = null!;

    internal static readonly IReadOnlySeparatedMasterPackage EmptyNull =
        NotSeparate(new MasterReferenceCollection(ModKey.Null));

    // Hack until this is better understood
    private static HashSet<ModKey> _starfieldMasters = new()
    {
        "OldMars.esm", 
        "Constellation.esm", 
        "SFBGS003.esm", 
        "SFBGS006.esm", 
        "SFBGS007.esm", 
        "SFBGS008.esm"
    };

    internal SeparatedMasterPackage()
    {
    }

    public static IReadOnlySeparatedMasterPackage Factory(
        GameRelease release,
        ModKey currentModKey,
        MasterStyle style,
        IReadOnlyMasterReferenceCollection masters,
        ILoadOrderGetter<IModMasterFlagsGetter>? loadOrder)
    {
        var constants = GameConstants.Get(release);
        if (constants.SeparateMasterLoadOrders)
        {
            return SeparatedMasterPackage.Separate(currentModKey, style, masters, loadOrder);
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
        var header = ModHeaderFrame.FromPath(modPath, release, fileSystem: fileSystem);
        var masters = MasterReferenceCollection.FromModHeader(modPath.ModKey, header);
        return Factory(release, modPath.ModKey, header.MasterStyle, masters, loadOrder);
    }

    internal class NotSeparatedMasterPackage : IReadOnlySeparatedMasterPackage
    {
        public ModKey CurrentMod { get; }
        public IReadOnlyMasterReferenceCollection Raw { get; }

        public IReadOnlyList<ModKey> Normal { get; }

        internal IReadOnlyDictionary<ModKey, MasterStyleIndex> _lookup = null!;

        public NotSeparatedMasterPackage(
            ModKey currentMod,
            IReadOnlyMasterReferenceCollection masters,
            IReadOnlyList<ModKey> normal)
        {
            CurrentMod = currentMod;
            Raw = masters;
            Normal = normal;
        }

        public bool TryLookupModKey(ModKey modKey, bool reference, out MasterStyle style, out uint index)
        {
            if (_lookup.TryGetValue(modKey, out var x))
            {
                style = x.Style;
                index = x.Index;
                return true;
            }

            style = default;
            index = default;
            return false;
        }

        public FormKey GetFormKey(FormID formId, bool reference)
        {
            var loadOrder = Normal;
            var modID = formId.FullMasterIndex;

            if (modID >= loadOrder.Count)
            {
                return new FormKey(
                    CurrentMod,
                    formId.FullId);
            }

            var id = formId.FullId;
            if (modID == 0 && id == 0)
            {
                return FormKey.Null;
            }

            var master = loadOrder[checked((int)modID)];
            return new FormKey(
                master,
                id);
        }
    }

    internal static IReadOnlySeparatedMasterPackage NotSeparate(IReadOnlyMasterReferenceCollection masters)
    {
        var normal = new List<ModKey>(masters.Masters.Select((x => x.Master)));
        normal.Add(masters.CurrentMod);
        var ret = new NotSeparatedMasterPackage(
            masters.CurrentMod,
            masters,
            normal);
        var lookup = new Dictionary<ModKey, MasterStyleIndex>();
        FillLookup(ret.Normal, lookup, MasterStyle.Full);
        ret._lookup = lookup;
        return ret;
    }

    internal static IReadOnlySeparatedMasterPackage Separate(
        ModKey currentModKey,
        MasterStyle style,
        IReadOnlyMasterReferenceCollection masters,
        ILoadOrderGetter<IModMasterFlagsGetter>? loadOrder)
    {
        var normal = new List<ModKey>();
        var medium = new List<ModKey>();
        var light = new List<ModKey>();

        void AddToList(IModMasterFlagsGetter mod, ModKey modKey)
        {
            if (mod.IsMediumMaster)
            {
                AddToListViaStyle(MasterStyle.Medium, modKey);
            }
            else if (mod.IsSmallMaster)
            {
                AddToListViaStyle(MasterStyle.Small, modKey);
            }
            else
            {
                AddToListViaStyle(MasterStyle.Full, modKey);
            }
        }

        void AddToListViaStyle(MasterStyle style, ModKey modKey)
        {
            switch (style)
            {
                case MasterStyle.Full:
                    normal.Add(modKey);
                    break;
                case MasterStyle.Small:
                    light.Add(modKey);
                    break;
                case MasterStyle.Medium:
                    medium.Add(modKey);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
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

                if (mod.IsSmallMaster && mod.IsMediumMaster)
                {
                    throw new ModHeaderMalformedException(mod.ModKey,
                        "Mod had both Light and Medium master flags enabled");
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

        if (_starfieldMasters.Contains(currentModKey))
        {
            AddToListViaStyle(style, currentModKey);
        }
        else
        {
            normal.Add(currentModKey);
        }

        var ret = new SeparatedMasterPackage()
        {
            Full = normal,
            Medium = medium,
            Light = light,
            Raw = masters,
            CurrentMod = masters.CurrentMod,
        };
        var lookup = new Dictionary<ModKey, MasterStyleIndex>();
        FillLookup(ret.Full, lookup, MasterStyle.Full);
        FillLookup(ret.Light, lookup, MasterStyle.Small);
        FillLookup(ret.Medium, lookup, MasterStyle.Medium);
        ret._lookup = lookup;
        return ret;
    }

    public static IReadOnlySeparatedMasterPackage FromConstants(GameConstants constants, ModPath path,
        IFileSystem? fileSystem = null)
    {
        if (constants.SeparateMasterLoadOrders)
        {
            throw new ArgumentException(
                $"Cannot make {nameof(SeparatedMasterPackage)} just a path on a game that has separated masters: {constants.SeparateMasterLoadOrders}");
        }

        return NotSeparate(MasterReferenceCollection.FromPath(path, constants.Release, fileSystem: fileSystem));
    }

    internal static void FillLookup(
        IReadOnlyList<ModKey> masters,
        Dictionary<ModKey, MasterStyleIndex> dict,
        MasterStyle style)
    {
        byte index = 0;
        foreach (var modKey in masters)
        {
            dict.Set(modKey, new(index, style));
            index++;
        }
    }

    public bool TryLookupModKey(ModKey modKey, bool reference, out MasterStyle style, out uint index)
    {
        if (_lookup.TryGetValue(modKey, out var x))
        {
            style = x.Style;
            index = x.Index;
            return true;
        }

        style = default;
        index = default;
        return false;
    }

    private void ExtractFormIdInfo(
        FormID formId,
        out MasterStyle style,
        out uint index,
        out uint id,
        out IReadOnlyList<ModKey> loadOrder)
    {
        var fullMasterIndex = formId.FullMasterIndex;
        switch (fullMasterIndex)
        {
            case FormID.SmallMasterMarker:
            {
                index = formId.LightMasterIndex;
                id = formId.LightId;
                style = MasterStyle.Small;
                loadOrder = Light;
                break;
            }
            case FormID.MediumMasterMarker:
            {
                index = formId.MediumMasterIndex;
                id = formId.MediumId;
                style = MasterStyle.Medium;
                loadOrder = Medium;
                break;
            }
            default:
            {
                index = formId.FullMasterIndex;
                id = formId.FullId;
                style = MasterStyle.Full;
                loadOrder = Full;
                break;
            }
        }
    }

    public FormKey GetFormKey(FormID formId, bool reference)
    {
        ExtractFormIdInfo(
            formId,
            out var style,
            out var index,
            out var id,
            out var loadOrder);

        if (index == 0 && id == 0)
        {
            return FormKey.Null;
        }

        if (index >= loadOrder.Count)
        {
            return new FormKey(
                CurrentMod,
                id);
        }

        var master = loadOrder[checked((int)index)];
        return new FormKey(
            master,
            id);
    }
}