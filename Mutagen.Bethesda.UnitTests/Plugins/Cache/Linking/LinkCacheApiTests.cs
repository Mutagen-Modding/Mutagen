using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public class LinkCacheApiTests
{
    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void FormLinkTryResolve(
        Fallout4Mod mod, 
        InstanceNamingRules rules)
    {
        var linkCache = mod.ToImmutableLinkCache();
        var formLink = new FormLink<IInstanceNamingRulesGetter>(rules.FormKey);

        IInstanceNamingRulesGetter foundRecord;
        linkCache.TryResolve(formLink, out foundRecord);
        foundRecord.Should().NotBeNull();
        foundRecord = linkCache.Resolve(formLink);
    }
}
