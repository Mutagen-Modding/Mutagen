using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.AutoData;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Skyrim.Assets;

public class VoiceTypeAssetLookupTestSkyrim
{
    private readonly ILinkCache _linkCache;
    private readonly VoiceTypeAssetLookup _searcher = new();

    public VoiceTypeAssetLookupTestSkyrim()
    {
        var mod = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.VoiceTypeTesting, SkyrimRelease.SkyrimSE);
        _linkCache = mod.ToImmutableLinkCache();
        
        _searcher.Prep(_linkCache.CreateImmutableAssetLinkCache());
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
    public void TestGetIsID()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("15549C:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("155352:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "CYRaaaPLACEHOLDERVoicetype"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestGetInFaction()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0CE3BE:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0CE39F:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "CYRMaleEvenToned",
                "CYRMaleStandard",
                "CYRMaleHonorable",
                "CYRFemaleRich",
                "CYRFemaleSultry",
                "CYRFemaleEnergetic",
                "CYRaaaPLACEHOLDERVoicetype"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestGetInFactionQuestConditions()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0724F4:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0724D7:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "CYRMaleArgonian",
                "CYRMaleArgonianAccented",
                "CYRFemaleDeepToned",
                "CYRFemaleKhajiit",
                "CYRFemaleSoftToned",
                "CYRFemaleNord",
                "CYRMaleBrute",
                "CYRMaleEvenToned",
                "CYRMaleElfHaughty",
                "CYRMaleGuttural",
                "CYRMaleLightToned",
                "CYRMaleDunmer",
                "CYRMaleRoughshod",
                "CYRMaleStandard",
                "CYRMaleNord",
                "CYRFemaleArgonian",
                "CYRMaleNordThick",
                "CYRFemaleRich",
                "CYRMaleKhajiitMercurial",
                "CYRFemaleEnergetic",
                "CYRMaleEnglishRich",
                "CYRMaleOrcAlexC"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAliasForcedRef()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0AF3C5:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0AF395:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "CYRMaleSemiUniqueTES4MaleImperialVoiceMatch"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAliasExternal()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("07EFF0:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("07EFDC:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "CYRFemaleEnergetic",
                "CYRMaleHonorable",
                "CYRMaleStandard"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestEmptyLeveledChar()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("16EAE5:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("16EAE2:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>()),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestGetIsVoiceTypeQuestConditions()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("063C34:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("063B3F:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<string>
            {
                "CYRMaleArgonian",
                "CYRMaleArgonianAccented",
                "CYRFemaleDeepToned",
                "CYRFemaleKhajiit",
                "CYRFemaleSoftToned",
                "CYRFemaleNord",
                "CYRMaleBrute",
                "CYRMaleEvenToned",
                "CYRMaleElfHaughty",
                "CYRMaleGuttural",
                "CYRMaleLightToned",
                "CYRMaleDunmer",
                "CYRMaleRoughshod",
                "CYRMaleStandard",
                "CYRMaleNord",
                "CYRFemaleArgonian",
                "CYRMaleHonorable",
                "CYRMaleNordThick",
                "CYRFemaleRich",
                "CYRMaleKhajiitMercurial",
                "CYRFemaleSultry",
                "CYRFemaleEnergetic",
                "CYRMaleEnglishRich",
                "CYRMaleOrcAlexC"
            }),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }
}
