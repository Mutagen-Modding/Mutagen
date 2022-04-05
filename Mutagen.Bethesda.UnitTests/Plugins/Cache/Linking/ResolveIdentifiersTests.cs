using System.Linq;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void ResolveLinkIdentifiers(LinkCachePreferences.RetentionType cacheType)
    {
        var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var armor = prototype.Armors.AddNew();
        var llist = prototype.LeveledItems.AddNew();
        llist.Entries = new ExtendedList<LeveledItemEntry>()
        {
            new LeveledItemEntry()
            {
                Data = new LeveledItemEntryData()
                {
                    Reference = armor.AsLink()
                }
            }
        };
        using var disp = ConvertMod(prototype, out var mod);
        var (style, package) = GetLinkCache(mod, cacheType);
        Assert.True(package.TryResolveIdentifier(mod.LeveledItems.First().Entries[0].Data.Reference.FormKey, out _));
    }
}