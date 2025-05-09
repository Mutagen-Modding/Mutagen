using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
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
        foundRecord.ShouldNotBeNull();
        foundRecord = linkCache.Resolve(formLink);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public async Task ResolvePassingRecordResolvesGetter(
        IFileSystem fileSystem,
        Fallout4Mod mod,
        Npc n,
        DirectoryPath existingPath)
    {
        var path = Path.Combine(existingPath, mod.ModKey.FileName);
        await mod.BeginWrite
            .ToPath(path)
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .WithFileSystem(fileSystem)
            .WriteAsync();

        using var modRead = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();
        
        INpcGetter getter = n;

        var cache = modRead.ToImmutableLinkCache();
        var resolved = cache.Resolve(getter);
        resolved.FormKey.ShouldBe(n.FormKey);
    }
}
