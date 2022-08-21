using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Skyrim.Assets;

public class VoiceTypeAssetLookupTestSkyrim
{
    private static readonly ILinkCache<ISkyrimMod, ISkyrimModGetter> LinkCache = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE).LinkCache;
    private readonly VoiceTypeAssetLookup _searcher = new();

    public VoiceTypeAssetLookupTestSkyrim()
    {
        _searcher.Prep(LinkCache.CreateImmutableAssetLinkCache());
    }

    [Fact]
    public void TestAliasAdditionalVoicesNPCList()
    {
        Assert.True(LinkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0A3E66:Skyrim.esm"), out var responses), "Response not resolved");
        Assert.True(LinkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0A3E2B:Skyrim.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "FemaleCommander",
                "MaleBrute"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAliasAdditionalVoicesVoiceTypeList()
    {
        Assert.True(LinkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0E3039:Skyrim.esm"), out var responses), "Response not resolved");
        Assert.True(LinkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0E2D1E:Skyrim.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "MaleCommander",
                "MaleNord"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestNotEqualToZero()
    {
        Assert.True(LinkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("074A09:Skyrim.esm"), out var responses), "Response not resolved");
        Assert.True(LinkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("074772:Skyrim.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "FemaleEvenToned",
                "FemaleYoungEager",
                "MaleNord"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAdditionalVoicesNPCAndCreatedNPC()
    {
        Assert.True(LinkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("05C9DB:Skyrim.esm"), out var responses), "Response not resolved");
        Assert.True(LinkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("05C9C8:Skyrim.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "MaleGuard",
                "MaleNordCommander"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAdditionalVoicesNPCAndForcedRef()
    {
        Assert.True(LinkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0D0514:Skyrim.esm"), out var responses), "Response not resolved");
        Assert.True(LinkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0D04FB:Skyrim.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "FemaleNord",
                "MaleGuard",
                "MaleNordCommander"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestMultipleFactionsAndVoiceTypesOr()
    {
        Assert.True(LinkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("07D905:Skyrim.esm"), out var responses), "Response not resolved");
        Assert.True(LinkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("02A3DB:Skyrim.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "MaleUniqueHadvar",
                "MaleNordCommander",
                "MaleSoldier",
                "MaleNord",
                "FemaleCommander",
                "MaleCommander",
                "MaleYoungEager"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }
}
