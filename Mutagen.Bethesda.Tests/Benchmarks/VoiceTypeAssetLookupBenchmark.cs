#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Noggog;

namespace Mutagen.Bethesda.Tests.Benchmarks;

[MemoryDiagnoser, InProcess]
public class VoiceTypeAssetLookupBenchmark
{
    VoiceTypeAssetLookup lookup;
    SkyrimMod outMod;
    Quest quest;
    DialogTopic topic;
    IAssetLinkCache assetCache;
    ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache;
    ILinkUsageCache usageCache;
    Consumer consumer = new();

    public VoiceTypeAssetLookupBenchmark()
    {
        // Load Skyrim.esm to get a large pool of real data
        outMod = new("Temp.esp", SkyrimRelease.SkyrimSE);
        var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .TransformLoadOrderListings(x => x.Where(m => m.ModKey == "Skyrim.esm"))
            .WithOutputMod(outMod)
            .Build();

        quest = outMod.Quests.AddNew();
        topic = outMod.DialogTopics.AddNew();
        topic.Quest.SetTo(quest);

        var actor = env.LinkCache.Resolve<INpcGetter>("Faendal");

        // Set up all responses with their conditions
        // Use EDID resolves as we don't have formkeys here
        getIsIdResponse = new(outMod);
        var idData = new GetIsIDConditionData();
        idData.Object.Link.SetTo(actor);
        getIsIdResponse.Conditions.Add(new ConditionFloat()
        {
            Data = idData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsIdResponse);

        getIsVoiceResponse = new(outMod);
        var voiceData = new GetIsVoiceTypeConditionData();
        voiceData.VoiceTypeOrList.Link.SetTo(env.LinkCache.Resolve<IVoiceTypeGetter>("MaleEvenToned"));
        getIsVoiceResponse.Conditions.Add(new ConditionFloat()
        {
            Data = voiceData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsVoiceResponse);

        getIsVoiceListResponse = new(outMod);
        var voiceListData = new GetIsVoiceTypeConditionData();
        voiceListData.VoiceTypeOrList.Link.SetTo(env.LinkCache.Resolve<IFormListGetter>("DefaultNPCVoiceTypes"));
        getIsVoiceListResponse.Conditions.Add(new ConditionFloat()
        {
            Data = voiceListData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsVoiceListResponse);

        getIsAliasResponse = new(outMod);
        quest.Aliases.Add(new() { ID = 1, UniqueActor = actor.ToNullableLink() });
        getIsAliasResponse.Conditions.Add(new ConditionFloat()
        {
            Data = new GetIsAliasRefConditionData() { ReferenceAliasIndex = (int)quest.Aliases[0].ID },
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsAliasResponse);

        getInFactionResponse = new(outMod);
        var factionData = new GetInFactionConditionData();
        factionData.Faction.Link.SetTo(env.LinkCache.Resolve<IFactionGetter>("BanditFaction"));
        getInFactionResponse.Conditions.Add(new ConditionFloat()
        {
            Data = factionData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(getInFactionResponse);

        getIsClassResponse = new(outMod);
        var classData = new GetIsClassConditionData();
        classData.Class.Link.SetTo(env.LinkCache.Resolve<IClassGetter>("VendorPawnbroker"));
        getIsClassResponse.Conditions.Add(new ConditionFloat()
        {
            Data = classData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsClassResponse);

        hasKeywordResponse = new(outMod);
        var keywordData = new HasKeywordConditionData();
        keywordData.Keyword.Link.SetTo(env.LinkCache.Resolve<IKeywordGetter>("FarmerCabbage"));
        hasKeywordResponse.Conditions.Add(new ConditionFloat()
        {
            Data = keywordData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(hasKeywordResponse);

        getIsRaceResponse = new(outMod);
        var raceData = new GetIsRaceConditionData();
        raceData.Race.Link.SetTo(env.LinkCache.Resolve<IRaceGetter>("NordRace"));
        getIsRaceResponse.Conditions.Add(new ConditionFloat()
        {
            Data = raceData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsRaceResponse);

        getIsSexResponse = new(outMod);
        getIsSexResponse.Conditions.Add(new ConditionFloat()
        {
            Data = new GetIsSexConditionData() { MaleFemaleGender = Plugins.Records.MaleFemaleGender.Male },
            ComparisonValue = 1,
        });
        topic.Responses.Add(getIsSexResponse);

        isInListResponse = new(outMod);
        var inListData = new IsInListConditionData();
        inListData.FormList.Link.SetTo(env.LinkCache.Resolve<IFormListGetter>("CompanionsLeaders"));
        isInListResponse.Conditions.Add(new ConditionFloat()
        {
            Data = inListData,
            ComparisonValue = 1,
        });
        topic.Responses.Add(isInListResponse);

        isChildResponse = new(outMod);
        isChildResponse.Conditions.Add(new ConditionFloat()
        {
            Data = new IsChildConditionData(),
            ComparisonValue = 1
        });
        topic.Responses.Add(isChildResponse);

        nonFilteringResponse = new(outMod);
        nonFilteringResponse.DeepCopyIn(getIsIdResponse);
        var nonFilterData = new GetStageConditionData();
        nonFilterData.Quest.Link.SetTo(quest);
        nonFilteringResponse.Conditions.Add(new ConditionFloat()
        {
            Data = nonFilterData,
        });
        topic.Responses.Add(nonFilteringResponse);

        invertedResponse = new(outMod);
        invertedResponse.DeepCopyIn(getIsVoiceResponse);
        invertedResponse.Conditions.Add(new ConditionFloat()
        {
            Data = idData,
            ComparisonValue = 0
        });
        topic.Responses.Add(invertedResponse);

        scene = outMod.Scenes.AddNew();
        scene.Quest.SetTo(quest);
        sceneTopic = outMod.DialogTopics.AddNew();
        sceneTopic.Quest.SetTo(quest);
        sceneTopic.Subtype = DialogTopic.SubtypeEnum.Scene;

        // Add a few extra actions to search through
        // No impact on baseline since this wasn't used on the SceneAlias codepath previously
        for (int i = 0; i < 32; i++)
        {
            scene.Actors.Add(new() { });
        }

        scene.Actions.Add(new()
        {
            Type = SceneAction.TypeEnum.Dialog,
            ActorID = (int)quest.Aliases[0].ID,
            Topic = sceneTopic.ToNullableLink(),
        });
        sceneResponse = new(outMod); // No need for conditions
        sceneTopic.Responses.Add(sceneResponse);

        linkCache = env.LinkCache;
        assetCache = linkCache.CreateImmutableAssetLinkCache();
        usageCache = new ImmutableLoadOrderLinkUsageCache(linkCache);
        lookup = new();
        lookup.Prep(assetCache, usageCache);
    }

    [Benchmark]
    public void Prep()
    {
        var lookup = new VoiceTypeAssetLookup();
        lookup.Prep(assetCache, usageCache);
    }

    void RunBench(DialogResponses response)
    {
        //Debug.Assert(lookup.GetSpeakers(response).Any());
        lookup.GetSpeakers(response).Consume(consumer);
    }

    [Benchmark, WarmupCount(1), IterationCount(5)]
    public void GetAllSpeakers()
    {
        foreach (var info in linkCache.WinningOverrides<IDialogResponsesGetter>())
        {
            lookup.GetSpeakers(info).Consume(consumer);
        }
    }

    [Benchmark, WarmupCount(1), IterationCount(5)]
    public void GetAllPaths()
    {
        foreach (var info in linkCache.WinningOverrides<IDialogResponsesGetter>())
        {
            lookup.GetVoiceLineFilePaths(info).Consume(consumer);
        }
    }

    DialogResponses getIsIdResponse;
    [Benchmark]
    public void GetIsId()
    {
        RunBench(getIsIdResponse);
    }

    DialogResponses getIsVoiceResponse;
    [Benchmark]
    public void GetIsVoiceType()
    {
        RunBench(getIsVoiceResponse);
    }

    DialogResponses getIsVoiceListResponse;
    [Benchmark]
    public void GetIsVoiceTypeList()
    {
        RunBench(getIsVoiceListResponse);
    }

    DialogResponses getIsAliasResponse;
    [Benchmark]
    public void GetIsAliasRef()
    {
        RunBench(getIsAliasResponse);
    }

    DialogResponses getInFactionResponse;
    [Benchmark]
    public void GetInFaction()
    {
        RunBench(getInFactionResponse);
    }

    DialogResponses getIsClassResponse;
    [Benchmark]
    public void GetIsClass()
    {
        RunBench(getIsClassResponse);
    }

    DialogResponses hasKeywordResponse;
    [Benchmark]
    public void HasKeyword()
    {
        RunBench(hasKeywordResponse);
    }

    DialogResponses getIsRaceResponse;
    [Benchmark]
    public void GetIsRace()
    {
        RunBench(getIsRaceResponse);
    }

    DialogResponses getIsSexResponse;
    [Benchmark]
    public void GetIsSex()
    {
        RunBench(getIsSexResponse);
    }

    DialogResponses isInListResponse;
    [Benchmark]
    public void IsInList()
    {
        RunBench(isInListResponse);
    }

    DialogResponses isChildResponse;
    [Benchmark]
    public void IsChild()
    {
        RunBench(isChildResponse);
    }

    DialogResponses nonFilteringResponse;
    [Benchmark]
    public void NonFilteringCondition()
    {
        RunBench(nonFilteringResponse);
    }

    DialogResponses invertedResponse;
    [Benchmark]
    public void InvertedCondition()
    {
        RunBench(invertedResponse);
    }

    Scene scene;
    DialogTopic sceneTopic;
    DialogResponses sceneResponse;
    [Benchmark]
    public void SceneResponse()
    {
        RunBench(sceneResponse);
    }
}