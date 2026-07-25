using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

namespace Mutagen.Bethesda.Tests.Benchmarks;

[MemoryDiagnoser, InProcess]
public class VoiceTypeAssetLookupBenchmark
{
    public static ILinkCache LinkCache;

    [GlobalSetup]
    public void Setup()
    {
        var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .Build();
        LinkCache = env.LinkCache;
    }

    [Benchmark]
    public void PrepareWithDefaultMasters()
    {
        var lookup = new VoiceTypeAssetLookup();
        var assetCache = LinkCache.CreateImmutableAssetLinkCache();
        lookup.Prep(assetCache);
    }
}

[MemoryDiagnoser, InProcess]
public class VoiceTypeAssetLookupRuntimeBenchmark
{
    public static ILinkCache LinkCache;
    public static VoiceTypeAssetLookup Lookup;

    public static IDialogResponsesGetter ResponsesSpecific = null!;
    public static IDialogResponsesGetter ResponsesGeneric = null!;

    [GlobalSetup]
    public void Setup()
    {
        var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .Build();
        LinkCache = env.LinkCache;
        Lookup = new();
        var assetCache = LinkCache.CreateImmutableAssetLinkCache();
        Lookup.Prep(assetCache);

        ResponsesSpecific = LinkCache.Resolve<IDialogResponsesGetter>(FormKey.Factory("0D5533:Skyrim.esm"));
        ResponsesGeneric = LinkCache.Resolve<IDialogResponsesGetter>(FormKey.Factory("0DBA2A:Skyrim.esm"));
    }

    [Benchmark]
    public IFormLinkGetter<IHasVoiceTypeGetter>[] LookupVoicesSpecificSpeaker()
    {
        return Lookup.GetSpeakers(ResponsesSpecific).ToArray();
    }

    [Benchmark]
    public IFormLinkGetter<IHasVoiceTypeGetter>[] LookupVoicesGenericSpeaker()
    {
        return Lookup.GetSpeakers(ResponsesGeneric).ToArray();
    }
}
