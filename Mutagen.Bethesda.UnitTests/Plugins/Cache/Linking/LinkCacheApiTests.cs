using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public class LinkCacheApiTests
{
    [Theory, MutagenAutoData]
    public void FormLinkTryResolve(
        FormKey formKey, 
        Fallout4Mod mod, 
        InstanceNamingRules rules)
    {
        Bethesda.Plugins.Cache.Internals.Implementations.ImmutableModLinkCache<IFallout4Mod, IFallout4ModGetter> linkCache = mod.ToImmutableLinkCache();

        var formLink = new FormLink<IInstanceNamingRulesGetter>(formKey);

        IInstanceNamingRulesGetter foundRecord;
        linkCache.TryResolve(formLink, out foundRecord);
        foundRecord = linkCache.Resolve(formLink);
    }
}
