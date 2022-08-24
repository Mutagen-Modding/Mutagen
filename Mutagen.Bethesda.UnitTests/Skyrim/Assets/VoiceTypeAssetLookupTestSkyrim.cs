using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Mutagen.Bethesda.UnitTests.AutoData;
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

    [Theory]
    [MutagenModAutoData]
    public void TestAliasAdditionalVoicesNPCList(
        SkyrimMod mod,
        Scene scene,
        DialogTopic topic,
        Quest quest,
        DialogResponses dialogResponses,
        VoiceType voiceType,
        VoiceType voiceType2,
        Npc npc,
        Npc npc2,
        FormList formList,
        string edid1,
        string edid2,
        uint aliasId)
    {
        topic.Responses.Add(dialogResponses);
        topic.Category = DialogTopic.CategoryEnum.Scene;
        topic.Quest.SetTo(quest);
        
        scene.Actions.Add(new SceneAction()
        {
            Type = SceneAction.TypeEnum.Dialog,
            Topic = topic.ToNullableLink<IDialogTopicGetter>(),
            ActorID = (int)aliasId,
        });
        
        voiceType.EditorID = edid1;
        npc.Voice.SetTo(voiceType);
        
        voiceType2.EditorID = edid2;
        npc2.Voice.SetTo(voiceType2);
        
        formList.Items.Add(npc);
        formList.Items.Add(npc2);
        
        var alias = new QuestAlias()
        {
            ID = aliasId
        };
        alias.VoiceTypes.SetTo(formList);
        quest.Aliases.Add(alias);
        
        var linkCache = mod.ToImmutableLinkCache();
        var sut = new VoiceTypeAssetLookup();
        sut.Prep(linkCache.CreateImmutableAssetLinkCache());
        
        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                edid1,
                edid2
            }),
            sut.GetVoicesWithQuest(topic, dialogResponses)
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
