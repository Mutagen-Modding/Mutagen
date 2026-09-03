using AutoFixture;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
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

    public DialogResponses CreateResponses()
    {
        var response = _fixture.Create<DialogResponses>();
        Topic.Responses.Add(response);
        return response;
    }

    public void AssertSceneSpeakersEqual(uint aliasId, IEnumerable<Condition> conditions, IEnumerable<Npc> expectedSpeakers)
    {
        var scene = _fixture.Create<Scene>();
        scene.Actors.Add(new() { ID = aliasId });
        scene.Actions.Add(new() { ActorID = (int)aliasId, Topic = Topic.ToNullableLink() });

        var response = CreateResponses();
        Topic.Category = DialogTopic.CategoryEnum.Scene;
        response.Conditions.AddRange(conditions);

        AssertSpeakersEqualImpl(response, expectedSpeakers);
    }

    public void AssertSpeakersEqual(IEnumerable<Condition> conditions, IEnumerable<Npc> expectedSpeakers)
    {
        var response = CreateResponses();
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
        // TODO: Test for combining with OR and AND
        public static ConditionFloat Create(ConditionData data, float compareValue, CompareOperator op = CompareOperator.EqualTo, bool or = false)
        {
            return new ConditionFloat
            {
                Data = data,
                ComparisonValue = compareValue,
                CompareOperator = op,
                Flags = or ? Condition.Flag.OR : 0,
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

        public static ConditionData GetIsRace(IRaceGetter race)
        {
            var data = new GetIsRaceConditionData();
            data.Race.Link.SetTo(race);
            return data;
        }

        public static ConditionData HasKeyword(IKeywordGetter keyword)
        {
            var data = new HasKeywordConditionData();
            data.Keyword.Link.SetTo(keyword);
            return data;
        }

        public static ConditionData GetIsClass(IFormLinkGetter<IClassGetter> npcClass)
        {
            var data = new GetIsClassConditionData();
            data.Class.Link.SetTo(npcClass);
            return data;
        }

        public static ConditionData IsInList(IFormListGetter list)
        {
            var data = new IsInListConditionData();
            data.FormList.Link.SetTo(list);
            return data;
        }
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
    public void TestGetIsVoice(VoiceTypeAssetLookupTestFixture fixture, FormList list)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(npc1.Voice), 1)],
            [npc1]);

        // A form list can be used to check for any voice in a list
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
    public void TestRaceDefaultVoice(
        VoiceTypeAssetLookupTestFixture fixture,
        Race race,
        VoiceType maleVoice,
        string maleEdid,
        VoiceType femaleVoice,
        string femaleEdid)
    {
        var male = fixture.CreateSpeaker("male");
        male.Race.SetTo(race);
        var female = fixture.CreateSpeaker("female");
        female.Race.SetTo(race);
        female.Configuration.Flags |= NpcConfiguration.Flag.Female;

        maleVoice.EditorID = maleEdid;
        femaleVoice.EditorID = femaleEdid;
        race.Voices = new GenderedItem<IFormLinkGetter<IVoiceTypeGetter>>(maleVoice.ToLink(), femaleVoice.ToLink());

        // Race voices should only be used as a fallback
        male.Voice.ShouldNotBeNull();
        female.Voice.ShouldNotBeNull();
        fixture.AssertSpeakersEqual([ConditionFactory.Create(ConditionFactory.GetIsVoice(maleVoice.ToLink()), 1)], []);
        fixture.AssertSpeakersEqual([ConditionFactory.Create(ConditionFactory.GetIsVoice(femaleVoice.ToLink()), 1)], []);

        // If no explicit voice is provided, fall back to the race's default for the NPCs gender
        male.Voice.SetToNull();
        female.Voice.SetToNull();
        fixture.AssertSpeakersEqual([ConditionFactory.Create(ConditionFactory.GetIsVoice(maleVoice.ToLink()), 1)], [male]);
        fixture.AssertSpeakersEqual([ConditionFactory.Create(ConditionFactory.GetIsVoice(femaleVoice.ToLink()), 1)], [female]);
    }

    [Theory, MutagenModAutoData]
    public void TestGetIsAliasRef(
        VoiceTypeAssetLookupTestFixture fixture,
        uint locAliasId,
        uint externalAliasId,
        uint aliasId,
        Quest externalQuest,
        FormList additionalVoices,
        Cell cell,
        PlacedNpc placedNpc,
        Location location,
        LocationReferenceType refType)
    {
        var npc1 = fixture.CreateSpeaker("npc");
        var npc2 = fixture.CreateSpeaker("npc2");

        void CheckAlias(QuestAlias alias, IEnumerable<Npc>? speakers = null)
        {
            alias.ID = aliasId;
            fixture.Quest.Aliases.Add(alias);
            fixture.AssertSpeakersEqual(
                [ConditionFactory.Create(new GetIsAliasRefConditionData() { ReferenceAliasIndex = (int)aliasId }, 1)],
                speakers ?? [npc1]);
            fixture.Quest.Aliases.Clear();
        }

        CheckAlias(new() { UniqueActor = npc1.ToNullableLink() });

        // Created
        CheckAlias(new() { CreateReferenceToObject = new() { Object =  npc1.ToLink() }  });

        // Forced ref
        cell.Flags |= Cell.Flag.IsInteriorCell;
        fixture.Mod.Cells.AddInteriorCell(cell);
        placedNpc.Base.SetTo(npc1);
        cell.Persistent.Add(placedNpc);
        CheckAlias(new() { ForcedReference = placedNpc.ToNullableLink() });

        // Location alias ref
        location.LocationRefTypeReferencesAdded = [new()
        {
            Ref = placedNpc.ToLink(),
            LocationRefType = refType.ToLink(),
        }];
        fixture.Quest.Aliases.Add(new()
        {
            ID = locAliasId,
            SpecificLocation = location.ToNullableLink(),
        });
        CheckAlias(new()
        {
            Location = new()
            {
                AliasID = (int)locAliasId,
                RefType = refType.ToNullableLink()
            }
        });

        // External alias
        externalQuest.Aliases.Add(new() { UniqueActor = npc1.ToNullableLink(), ID = externalAliasId });
        CheckAlias(new() { External = new() { AliasID = (int)externalAliasId, Quest = externalQuest.ToNullableLink() } });

        // Matching ref from event doesn't provide enough context for filtering
        CheckAlias(new() { FindMatchingRefFromEvent = new() }, [npc1, npc2]);
        // Matching ref near alias event would require knowing every ref linked to every possible value of the other alias
        CheckAlias(new() { FindMatchingRefNearAlias = new() }, [npc1, npc2]);

        // If conditions are present, they are applied as an intersection before additional voices
        CheckAlias(new()
        {
            FindMatchingRefFromEvent = new(),
            Conditions = [ConditionFactory.Create(ConditionFactory.GetIsVoice(npc2.Voice), 1)]
        }, [npc2]);

        // Additional voices are applied as a union after additional voices
        additionalVoices.Items.AddRange(npc1.ToLink(), npc2.ToLink());
        CheckAlias(new()
        {
            FindMatchingRefFromEvent = new(),
            VoiceTypes = additionalVoices.ToNullableLink(),
            Conditions = [ConditionFactory.Create(ConditionFactory.GetIsVoice(npc2.Voice), 1)]
        }, [npc1, npc2]);
    }

    [Theory, MutagenModAutoData]
    public void TestGetInFaction(VoiceTypeAssetLookupTestFixture fixture, Faction faction)
    {
        var member = fixture.CreateSpeaker("member");
        member.Factions.Add(new() { Faction = faction.ToLink() });
        var nonmember = fixture.CreateSpeaker("nonmember");

        // Factions may be applied via aliases
        var questmember = fixture.CreateSpeaker("questmember");
        fixture.Quest.Aliases.Add(new()
        {
            UniqueActor = questmember.ToNullableLink(),
            Factions = [faction.ToLink()],
        });

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetInFaction(faction), 1)],
            [member, questmember]);

    }

    [Theory, MutagenModAutoData]
    public void TestGetIsClass(
        VoiceTypeAssetLookupTestFixture fixture,
        Class npcClass)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        npc1.Class.SetTo(npcClass);
        fixture.CreateSpeaker("dummy");

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsClass(npcClass.ToLink()), 1)],
            [npc1]);
    }

    [Theory, MutagenModAutoData]
    public void TestMultipleConditions(
        VoiceTypeAssetLookupTestFixture fixture,
        FormList list)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");
        var npc3 = fixture.CreateSpeaker("npc3");

        // Multiple OR in a block is combined as a union
        // npc1 || npc2
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsId(npc1), 1, or: true), ConditionFactory.Create(ConditionFactory.GetIsId(npc2), 1)],
            [npc1, npc2]);

        // AND is combined as the intersection
        // (npc1 || npc2 || npc3) && !npc3
        list.Items.AddRange(npc1.Voice, npc2.Voice, npc3.Voice);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(list.ToLink()), 1, or: false), ConditionFactory.Create(ConditionFactory.GetIsVoice(npc3.Voice), 0, or: false)],
            [npc1, npc2]);
    }

    [Theory, MutagenModAutoData]
    public void TestHasKeyword(
        VoiceTypeAssetLookupTestFixture fixture,
        Keyword actorKeyword,
        Keyword raceKeyword,
        Keyword questKeyword,
        Race race,
        Quest quest)
    {
        var actorKey = fixture.CreateSpeaker("actorKey");
        actorKey.Keywords = [actorKeyword.ToLink()];
        
        var raceKey = fixture.CreateSpeaker("raceKey");
        raceKey.Race.SetTo(race);
        race.Keywords = [raceKeyword.ToLink()];

        var questKey = fixture.CreateSpeaker("questKey");
        quest.Aliases.Add(new()
        {
            UniqueActor = questKey.ToNullableLink(),
            Keywords = [questKeyword.ToLink()],
        });

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(actorKeyword), 1)],
            [actorKey]);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(raceKeyword), 1)],
            [raceKey]);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(questKeyword), 1)],
            [questKey]);
    }

    [Theory, MutagenModAutoData]
    public void TestGetIsRace(
        VoiceTypeAssetLookupTestFixture fixture,
        Race race1,
        Race race2)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        npc1.Race.SetTo(race1);
        var npc2 = fixture.CreateSpeaker("npc2");
        npc2.Race.SetTo(race2);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsRace(race1), 1)],
            [npc1]);
    }

    [Theory, MutagenModAutoData]
    public void TestGetIsSex(VoiceTypeAssetLookupTestFixture fixture)
    {
        var male = fixture.CreateSpeaker("male");
        male.Configuration.Flags &= ~NpcConfiguration.Flag.Female;
        var female = fixture.CreateSpeaker("female");
        female.Configuration.Flags |= NpcConfiguration.Flag.Female;

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(new GetIsSexConditionData() { MaleFemaleGender = MaleFemaleGender.Male }, 1)],
            [male]);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(new GetIsSexConditionData() { MaleFemaleGender = MaleFemaleGender.Female }, 1)],
            [female]);
    }

    [Theory, MutagenModAutoData]
    public void TestIsInList(VoiceTypeAssetLookupTestFixture fixture, FormList list)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        fixture.CreateSpeaker("dummy");

        list.Items.Add(npc1.ToLink());
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.IsInList(list), 1)],
            [npc1]);
    }

    [Theory, MutagenModAutoData]
    public void TestIsChild(
        VoiceTypeAssetLookupTestFixture fixture,
        Race child,
        Race adult)
    {
        child.Flags |= Race.Flag.Child;
        var childNpc = fixture.CreateSpeaker("child");
        childNpc.Race.SetTo(child);


        adult.Flags &= ~Race.Flag.Child;
        var adultNpc = fixture.CreateSpeaker("adult");
        adultNpc.Race.SetTo(adult);

        fixture.AssertSpeakersEqual([ConditionFactory.Create(new IsChildConditionData(), 1)], [childNpc]);
        fixture.AssertSpeakersEqual([ConditionFactory.Create(new IsChildConditionData(), 0)], [adultNpc]);
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
    public void TestSceneSpeaker(VoiceTypeAssetLookupTestFixture fixture, uint aliasId)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");
        fixture.Quest.Aliases.Add(new() { ID = aliasId, UniqueActor = npc1.ToNullableLink() });

        fixture.AssertSceneSpeakersEqual(aliasId, [], [npc1]);
        fixture.AssertSceneSpeakersEqual(aliasId, [ConditionFactory.Create(ConditionFactory.GetIsId(npc2), 1)], []);
    }

    [Theory, MutagenModAutoData]
    public void TestSpecificSpeaker(VoiceTypeAssetLookupTestFixture fixture)
    {
        var npc = fixture.CreateSpeaker("npc");
        var speaker = fixture.CreateSpeaker("speaker");

        var responses = fixture.CreateResponses();
        responses.Conditions.Add(ConditionFactory.Create(ConditionFactory.GetIsId(npc), 1));
        fixture.GetLookup().GetSpeakers(responses).ShouldBe([npc.ToLink()]);

        responses.Speaker.SetTo(speaker);
        fixture.GetLookup().GetSpeakers(responses).ShouldBe([speaker.ToLink()]);
    }

    [Theory, MutagenModAutoData]
    public void TestCompareOperators(VoiceTypeAssetLookupTestFixture fixture, GlobalFloat global)
    {
        var npc1 = fixture.CreateSpeaker("npc1");
        var npc2 = fixture.CreateSpeaker("npc2");

        var data = ConditionFactory.GetIsId(npc1);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(data, 1, CompareOperator.EqualTo)],
            [npc1]);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(data, 1, CompareOperator.NotEqualTo)],
            [npc2]);

        var conditionGlobal = new ConditionGlobal()
        {
            Data = data,
            ComparisonValue = global.ToLink()
        };
        global.Data = 1;
        fixture.AssertSpeakersEqual([conditionGlobal], [npc1]);
        global.Data = 0;
        fixture.AssertSpeakersEqual([conditionGlobal], [npc2]);
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

    [Theory, MutagenModAutoData]
    public void TestInheritedTraits(
        VoiceTypeAssetLookupTestFixture fixture,
        LeveledNpc leveledNpc,
        Race race1,
        Race race2,
        Race dummyRace)
    {
        var base1 = fixture.CreateSpeaker("base1");
        base1.Race.SetTo(race1);
        base1.Configuration.Flags &= ~NpcConfiguration.Flag.Female;
        var base2 = fixture.CreateSpeaker("base2");
        base2.Race.SetTo(race2);
        base2.Configuration.Flags |= NpcConfiguration.Flag.Female;
        leveledNpc.Entries =
        [
            new() {Data = new() {Reference = base1.ToLink() } },
            new() {Data = new() {Reference = base2.ToLink() } },
        ];

        var derived = fixture.CreateSpeaker("derived");
        derived.Race.SetTo(dummyRace);
        derived.Template.SetTo(leveledNpc);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(base1.Voice), 1)],
            [base1]);
        derived.Configuration.TemplateFlags |= NpcConfiguration.TemplateFlag.Traits;

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(base1.Voice), 1)],
            [base1, derived]);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsRace(race2), 1)],
            [base2, derived]);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(new GetIsSexConditionData() { MaleFemaleGender = MaleFemaleGender.Male }, 1)],
            [base1, derived]);

        // Derived data should be disregarded if a template is used
        derived.Race.SetTo(dummyRace);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsRace(dummyRace), 1)],
            []);
        derived.Voice.ShouldNotBeNull();
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(derived.Voice), 1)],
            []);

        // Template flags should be ignored if template is null
        derived.Template.SetToNull();
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsRace(dummyRace), 1)],
            [derived]);
    }

    [Theory, MutagenModAutoData]
    public void TestInheritedStats(
        VoiceTypeAssetLookupTestFixture fixture,
        LeveledNpc leveledNpc,
        Class class1,
        Class class2,
        Class dummyClass)
    {
        var base1 = fixture.CreateSpeaker("base1");
        base1.Class.SetTo(class1);
        var base2 = fixture.CreateSpeaker("base2");
        base2.Class.SetTo(class2);
        leveledNpc.Entries =
        [
            new() {Data = new() {Reference = base1.ToLink() } },
            new() {Data = new() {Reference = base2.ToLink() } },
        ];
        var derived = fixture.CreateSpeaker("derived");
        derived.Template.SetTo(leveledNpc);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsClass(base1.Class), 1)],
            [base1]);
        derived.Configuration.TemplateFlags |= NpcConfiguration.TemplateFlag.Stats;

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsClass(base1.Class), 1)],
            [base1, derived]);

        // Derived data should be disregarded if a template is used
        derived.Class.SetTo(dummyClass);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsClass(dummyClass.ToLink()), 1)],
            []);

        // Template flags should be ignored if template is null
        derived.Template.SetToNull();
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsClass(dummyClass.ToLink()), 1)],
            [derived]);
    }

    [Theory, MutagenModAutoData]
    public void TestInheritedFactions(
        VoiceTypeAssetLookupTestFixture fixture,
        LeveledNpc leveledNpc,
        Faction faction1,
        Faction faction2)
    {
        var base1 = fixture.CreateSpeaker("base1");
        base1.Factions.Add(new() { Faction = faction1.ToLink() });
        var base2 = fixture.CreateSpeaker("base2");
        base1.Factions.Add(new() { Faction = faction2.ToLink() });
        leveledNpc.Entries =
        [
            new() {Data = new() {Reference = base1.ToLink() } },
            new() {Data = new() {Reference = base2.ToLink() } },
        ];
        var derived = fixture.CreateSpeaker("derived");
        derived.Template.SetTo(leveledNpc);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetInFaction(faction1), 1)],
            [base1]);
        derived.Configuration.TemplateFlags |= NpcConfiguration.TemplateFlag.Factions;

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetInFaction(faction1), 1)],
            [base1, derived]);
    }

    [Theory, MutagenModAutoData]
    public void TestInheritedKeywords(
        VoiceTypeAssetLookupTestFixture fixture,
        LeveledNpc leveledNpc,
        Keyword keyword1,
        Keyword keyword2,
        Race race,
        Keyword raceKeyword)
    {
        var base1 = fixture.CreateSpeaker("base1");
        base1.Keywords = [keyword1.ToLink()];
        var base2 = fixture.CreateSpeaker("base2");
        base2.Keywords = [keyword2.ToLink()];
        var raceBase = fixture.CreateSpeaker("raceBase");
        raceBase.Race.SetTo(race);
        race.Keywords = [raceKeyword.ToLink()];
        leveledNpc.Entries =
        [
            new() {Data = new() {Reference = base1.ToLink() } },
            new() {Data = new() {Reference = base2.ToLink() } },
            new() {Data = new() {Reference = raceBase.ToLink() } },
        ];
        var derived = fixture.CreateSpeaker("derived");
        derived.Template.SetTo(leveledNpc);

        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(keyword1), 1)],
            [base1]);
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(raceKeyword), 1)],
            [raceBase]);


        // Keywords are inherited via the UseKeywords flag
        derived.Configuration.TemplateFlags |= NpcConfiguration.TemplateFlag.Keywords;
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(keyword1), 1)],
            [base1, derived]);

        // They can also be inheited via the race, this time with UseTraits
        derived.Configuration.TemplateFlags &= ~NpcConfiguration.TemplateFlag.Keywords;
        derived.Configuration.TemplateFlags |= NpcConfiguration.TemplateFlag.Traits;
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.HasKeyword(raceKeyword), 1)],
            [raceBase, derived]);
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
    public void EditorIdEqual(
        VoiceTypeAssetLookupTestFixture fixture,
        VoiceType voice1,
        Npc npc1,
        VoiceType voice2,
        Npc npc2,
        string edid)
    {
        voice1.EditorID = edid;
        npc1.Voice.SetTo(voice1);
        voice2.EditorID = edid;
        npc2.Voice.SetTo(voice2);

        // Voice types with the same editor ID should be considered as separate.
        // Conditions care about form IDs, duplicated editor IDs are rare, but valid
        // such as in patchless integration of follower mods
        fixture.AssertSpeakersEqual(
            [ConditionFactory.Create(ConditionFactory.GetIsVoice(voice1.ToLink()), 1)],
            [npc1]);
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
            new VoiceContainer([voiceType.FormKey, voiceType2.FormKey]),
            sut.GetVoicesWithQuest(topic, dialogResponses)
        );
    }

    VoiceContainer CreateVoiceIdContainer(IEnumerable<string> editorIds)
    {
        return new VoiceContainer(editorIds.Select(e => _linkCache.Resolve<IVoiceTypeGetter>(e).FormKey));
    }

    [Fact]
    public void TestGetIsID()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("15549C:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("155352:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            CreateVoiceIdContainer(["CYRaaaPLACEHOLDERVoicetype"]),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestGetInFactionOLD()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0CE3BE:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0CE39F:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            CreateVoiceIdContainer(["CYRMaleEvenToned",
                "CYRMaleStandard",
                "CYRMaleHonorable",
                "CYRFemaleRich",
                "CYRFemaleSultry",
                "CYRFemaleEnergetic",
                "CYRaaaPLACEHOLDERVoicetype"]),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestGetInFactionQuestConditions()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0724F4:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0724D7:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            CreateVoiceIdContainer(["CYRMaleArgonian",
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
                "CYRMaleOrcAlexC"]),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAliasForcedRef()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("0AF3C5:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("0AF395:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            CreateVoiceIdContainer(["CYRMaleSemiUniqueTES4MaleImperialVoiceMatch"]),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestAliasExternal()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("07EFF0:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("07EFDC:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            CreateVoiceIdContainer(["CYRFemaleEnergetic",
                "CYRMaleHonorable",
                "CYRMaleStandard"
            ]),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestEmptyLeveledChar()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("16EAE5:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("16EAE2:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            new VoiceContainer(new HashSet<FormKey>()),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }

    [Fact]
    public void TestGetIsVoiceTypeQuestConditions()
    {
        Assert.True(_linkCache.TryResolve<IDialogResponsesGetter>(FormKey.Factory("063C34:VoiceTypeTestPlugin.esm"), out var responses), "Response not resolved");
        Assert.True(_linkCache.TryResolve<IDialogTopicGetter>(FormKey.Factory("063B3F:VoiceTypeTestPlugin.esm"), out var topic), "Topic not resolved");

        Assert.Equal(
            CreateVoiceIdContainer([
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
            ]),
            _searcher.GetVoicesWithQuest(topic!, responses!)
        );
    }
}
