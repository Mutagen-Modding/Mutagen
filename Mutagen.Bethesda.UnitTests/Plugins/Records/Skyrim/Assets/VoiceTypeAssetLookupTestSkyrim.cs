using AutoFixture;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using Shouldly;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim.Assets;

public class VoiceTypeAssetLookupTestFixture
{
    private readonly IFixture _fixture;
    public readonly ILinkCache LinkCache;
    public readonly SkyrimMod Mod;
    public readonly Quest Quest;
    public readonly DialogTopic Topic;

    public VoiceTypeAssetLookup GetLookup()
    {
        var assetCache = LinkCache.CreateImmutableAssetLinkCache();
        var lookup = new VoiceTypeAssetLookup();
        lookup.Prep(assetCache);
        return lookup;
    }

    public VoiceTypeAssetLookupTestFixture(
        IFixture fixture,
        SkyrimMod mod,
        DialogTopic topic,
        Quest quest)
    {
        _fixture = fixture;
        LinkCache = mod.ToMutableLinkCache();
        Mod = mod;
        Quest = quest;
        Topic = topic;
        topic.Quest.SetTo(quest);
    }

    /// <summary>
    /// Create and configure an NPC with a unique voice type
    /// </summary>
    public Npc CreateSpeaker(string name)
    {
        var npc = _fixture.Create<Npc>();
        var voice = _fixture.Create<VoiceType>();
        // Name is included here to provide additional context in errors
        npc.EditorID = name;
        voice.EditorID = name;
        npc.Voice.SetTo(voice);
        return npc;
    }

    public void AssertSceneSpeakersEqual(uint aliasId, IEnumerable<Condition> conditions, IEnumerable<Npc> expectedSpeakers)
    {
        var scene = _fixture.Create<Scene>();
        scene.Actors.Add(new() { ID = aliasId });
        scene.Actions.Add(new() { ActorID = (int)aliasId, Topic = Topic.ToNullableLink() });

        var response = _fixture.Create<DialogResponses>();
        Topic.Responses.Add(response);
        Topic.Category = DialogTopic.CategoryEnum.Scene;
        response.Conditions.AddRange(conditions);

        AssertSpeakersEqualImpl(response, expectedSpeakers);
    }

    public void AssertSpeakersEqual(IEnumerable<Condition> conditions, IEnumerable<Npc> expectedSpeakers)
    {
        var response = _fixture.Create<DialogResponses>();
        Topic.Responses.Add(response);
        response.Conditions.AddRange(conditions);

        AssertSpeakersEqualImpl(response, expectedSpeakers);
    }

    void AssertSpeakersEqualImpl(DialogResponses response, IEnumerable<Npc> expectedSpeakers)
    {
        // We compare against a resolved list to provide more context in errors
        var actual = GetLookup().GetSpeakers(response).Select(s => s.Resolve(LinkCache));
        actual.ShouldBe(expectedSpeakers, ignoreOrder: true);
    }
}

public class VoiceTypeAssetLookupTestSkyrim
{
    public static class ConditionFactory
    {
        public static ConditionFloat Create(ConditionData data, float compareValue, Condition.Flag flags = 0)
        {
            return new ConditionFloat
            {
                Data = data,
                ComparisonValue = compareValue,
                Flags = flags
            };
        }

        public static ConditionData GetIsId(IReferenceableObjectGetter target)
        {
            var data = new GetIsIDConditionData();
            data.Object.Link.SetTo(target);
            return data;
        }

        public static ConditionData GetInFaction(IFactionGetter faction)
        {
            var data = new GetInFactionConditionData();
            data.Faction.Link.SetTo(faction);
            return data;
        }

        public static ConditionData GetIsVoice(IFormLinkGetter<IVoiceTypeOrListGetter> voice)
        {
            var data = new GetIsVoiceTypeConditionData();
            data.VoiceTypeOrList.Link.SetTo(voice);
            return data;
        }
    }

    [Theory, MutagenModAutoData]
    public void TestWinningOverride(
        SkyrimMod mod1,
        SkyrimMod mod2,
        string edid)
    {
        var quest = mod1.Quests.AddNew();
        var topic = mod1.DialogTopics.AddNew();
        topic.Quest.SetTo(quest);
        var voice = mod1.VoiceTypes.AddNew();
        voice.EditorID = edid;

        var npc = mod1.Npcs.AddNew();
        npc.Voice.SetTo(voice);
        var faction = mod1.Factions.AddNew();
        npc.Factions.Add(new() { Faction = faction.ToLink() });

        var response = new DialogResponses(mod1);
        topic.Responses.Add(response);
        response.Conditions.Add(ConditionFactory.Create(ConditionFactory.GetInFaction(faction), 1));

        using var loadOrder = new LoadOrder<ISkyrimModGetter> { mod1 };

        // Baseline: Mod1 adds a topic conditioned to NPC1's voice
        var lookup = new VoiceTypeAssetLookup();
        lookup.Prep(loadOrder.ToImmutableLinkCache().CreateImmutableAssetLinkCache());
        lookup.GetSpeakers(response).ShouldBe([npc.ToLink()]);

        // User adds a second mod that removes the NPC from their faction
        var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
        npcOverride.Factions.Clear();

        loadOrder.Add(mod2);
        lookup = new();
        lookup.Prep(loadOrder.ToImmutableLinkCache().CreateImmutableAssetLinkCache());
        lookup.GetSpeakers(response).ShouldBeEmpty();
    }

    [Theory, MutagenModAutoData]
    public void TestGetIsId(VoiceTypeAssetLookupTestFixture fixture)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsId(npc1), 1)],
            [npc1]);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsId(npc1), 0)],
            [npc2]);
    }

    [Theory, MutagenModAutoData]
    public void TestSceneSpeaker(VoiceTypeAssetLookupTestFixture fixture, uint aliasId)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");
        fixture.Quest.Aliases.Add(new() { ID = aliasId, UniqueActor = npc1.ToNullableLink() });

        fixture.AssertSceneSpeakersEqual(aliasId, [], [npc1]);
        fixture.AssertSceneSpeakersEqual(aliasId, [ConditionFactory.Create(ConditionFactory.GetIsId(npc2), 1)], []);
    }

    [Theory, MutagenModAutoData]
    public void TestGetIsVoice(VoiceTypeAssetLookupTestFixture fixture, FormList list)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(npc1.Voice), 1)],
            [npc1]);

        list.Items.AddRange(npc1.Voice, npc2.Voice);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(list.ToLink()), 1)],
            [npc1, npc2]);

        // (Voice1 || Voice2) && !Voice1
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(list.ToLink()), 1), ConditionFactory.Create(ConditionFactory.GetIsVoice(npc2.Voice), 0)],
            [npc1]);
    }

    [Theory, MutagenModAutoData]
    public void TestUnfiltered(VoiceTypeAssetLookupTestFixture fixture)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");

        // The AllowDefaultDialog flag is not used at runtime
        npc1.Voice.Resolve<IVoiceType>(fixture.LinkCache).Flags &= ~VoiceType.Flag.AllowDefaultDialog;
        npc2.Voice.Resolve<IVoiceType>(fixture.LinkCache).Flags &= ~VoiceType.Flag.AllowDefaultDialog;
        fixture.AssertSpeakersEqual([], [npc1, npc2]);
    }

    [Theory, MutagenModAutoData]
    public void TestGetPaths(VoiceTypeAssetLookupTestFixture fixture, FormKey sharedInfo)
    {
        // Smoke test based on response in DialogueGeneric
        fixture.Quest.EditorID = "DialogueGeneric";
        fixture.Topic.EditorID = "DialogueGenericHello";
        var npc = fixture.CreateSpeaker("MaleEvenToned");

        var response = new DialogResponses(FormKey.Factory("0142C2:Skyrim.esm"), SkyrimRelease.SkyrimSE);
        response.Responses.Add(new() { ResponseNumber = 1 });
        fixture.Topic.Responses.Add(response);
        response.Conditions.Add(ConditionFactory.Create(ConditionFactory.GetIsVoice(npc.Voice), 1));
        response.FormKey.ModKey.ShouldNotBe(fixture.Topic.FormKey.ModKey, "Voice paths depend on the ID of the response, and must be different for this test to cover that edge case");
        fixture.GetLookup().GetVoiceLineFilePaths(response).ShouldBe([new DataRelativePath("Sound/Voice/Skyrim.esm/MaleEvenToned/DialogueGe_DialogueGeneric_000142C2_1.fuz")]);

        // Should also work for unfiltered response
        response.Conditions.Clear();
        fixture.GetLookup().GetVoiceLineFilePaths(response).ShouldBe([new DataRelativePath("Sound/Voice/Skyrim.esm/MaleEvenToned/DialogueGe_DialogueGeneric_000142C2_1.fuz")]);

        // A shared info should have no associated paths
        response.ResponseData.SetTo(sharedInfo);
        fixture.GetLookup().GetVoiceLineFilePaths(response).ShouldBeEmpty();
    }

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
        DialogResponse dialogResponse,
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
        dialogResponses.Responses.Add(dialogResponse);
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
                "CYRaaaPLACEHOLDERVoicetype",
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
