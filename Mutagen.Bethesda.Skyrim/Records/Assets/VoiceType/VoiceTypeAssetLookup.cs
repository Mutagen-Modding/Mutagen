using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

/// <summary>
/// Lookup for potential speakers and voice types of dialogue. Attempts to determine potential speakers at runtime to the extent that it can be statically determined.
/// Considers winning overrides only.
/// 
/// Intentionally differs from CK behaviour in that it ignores the `AllowDefaultDialog` flag of voice types, which has no effect at runtime but removes a response with
/// no explicit inclusion condition from voice export consideration in the CK.
/// 
/// Behaviour is undefined if a filter condition is compared against something other than a boolean, or using operators other than == or !=
/// </summary>
public class VoiceTypeAssetLookup : IAssetCacheComponent
{
    private ILinkCache _formLinkCache = null!;
    private ILinkUsageCache _usageCache = null!;

    // Databases. These aren't good candidates for usage caches as data here can be inherited
    private HashSet<FormKey> _allVoiceTypes = null!;
    // TODO: Is this necessary? Can we look up when retrieving speakers?
    // TOOD: Most unique NPCs have only one voice. Is an enumerable faster?
    private readonly Dictionary<FormKey, HashSet<FormKey>> _speakerVoices = new();

    // Inverse lookup of voice type -> speakers for GetIsVoiceType conditions and inversions
    private readonly Dictionary<FormKey, HashSet<FormKey>> _voiceSpeakers = [];

    // NPCs who start as members of a faction
    private readonly Dictionary<FormKey, HashSet<FormKey>> _staticFactionNPCs = [];
    // NPCs who start as members of a faction (quest alias or rank -1)
    private readonly Dictionary<FormKey, HashSet<FormKey>> _potentialFactionNPCs = [];

    private readonly Dictionary<FormKey, HashSet<FormKey>> _classNPCs = new();
    private readonly Dictionary<FormKey, HashSet<FormKey>> _raceNPCs = new();
    private readonly Dictionary<FormKey, HashSet<FormKey>> _keywordNPCs = new();
    private readonly Dictionary<MaleFemaleGender, HashSet<FormKey>> _genderNPCs = new();
    private HashSet<FormKey> _childNPCs = null!;

    //Caches
    private readonly Lock _questCacheLock = new();
    private readonly Dictionary<FormKey, VoiceContainer?> _questCache = new();

    [Obsolete("Provide usage cache for better performance")]
    public void Prep(IAssetLinkCache linkCache) => Prep(linkCache, new ImmutableLoadOrderLinkUsageCache(linkCache.FormLinkCache));

    public void Prep(IAssetLinkCache linkCache, ILinkUsageCache usageCache)
    {
        _formLinkCache = linkCache.FormLinkCache;
        _usageCache = usageCache;

        foreach (var quest in _formLinkCache.WinningOverrides<IQuestGetter>())
        {
            foreach (var alias in quest.Aliases)
            {
                var uniqueActor = alias.UniqueActor.FormKey;
                if (uniqueActor.IsNull) continue;

                foreach (var faction in alias.Factions)
                {
                    if (!faction.IsNull)
                    {
                        _potentialFactionNPCs
                            .GetOrAdd(faction.FormKey)
                            .Add(uniqueActor);
                    }
                }
                if (alias.Keywords != null)
                {
                    foreach (var keyword in alias.Keywords)
                    {
                        _keywordNPCs.GetOrAdd(keyword.FormKey).Add(uniqueActor);
                    }
                }
            }
        }

        foreach (var npc in _formLinkCache.WinningOverrides<INpcGetter>())
        {
            // The traits flag is used for multiple filters we're interested in, no need to loop multiple times
            var voices = new HashSet<FormKey>();
            foreach (var traits in GetTemplateActors(npc, NpcConfiguration.TemplateFlag.Traits))
            {
                var female = traits.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female);
                var voice = traits.Voice.IsNull ? GetDefaultVoice(traits.Race, female) : traits.Voice.FormKey;
                voices.Add(voice);
                _voiceSpeakers.GetOrAdd(voice).Add(npc.FormKey);

                _genderNPCs.GetOrAdd(female ? MaleFemaleGender.Female : MaleFemaleGender.Male).Add(npc.FormKey);

                if (!traits.Race.IsNull)
                {
                    _raceNPCs.GetOrAdd(traits.Race.FormKey).Add(npc.FormKey);
                    if (traits.Race.TryResolve(_formLinkCache, out var race) && race.Keywords != null)
                    {
                        foreach (var keyword in race.Keywords)
                        {
                            _keywordNPCs.GetOrAdd(keyword.FormKey).Add(npc.FormKey);
                        }
                    }
                }
            }
            _speakerVoices.Add(npc.FormKey, voices);

            foreach (var factionKey in GetFactions(npc))
            {
                _potentialFactionNPCs
                    .GetOrAdd(factionKey.Faction.FormKey)
                    .Add(npc.FormKey);
                if (factionKey.Rank >= 0)
                {
                    _staticFactionNPCs
                        .GetOrAdd(factionKey.Faction.FormKey)
                        .Add(npc.FormKey);
                }
            }

            foreach (var classKey in GetClasses(npc))
            {
                _classNPCs
                    .GetOrAdd(classKey.FormKey)
                    .Add(npc.FormKey);
            }

            foreach (var keyword in GetDirectKeywords(npc))
            {
                _keywordNPCs.GetOrAdd(keyword.FormKey).Add(npc.FormKey);
            }
        }

        foreach (var talkingActivator in _formLinkCache.WinningOverrides<ITalkingActivatorGetter>())
        {
            if (!talkingActivator.Voice.IsNull)
                _speakerVoices.Add(talkingActivator.FormKey, [talkingActivator.Voice.FormKey]);
        }

        // Build caches
        _childNPCs = _raceNPCs.Where(raceNpcs => _formLinkCache.TryResolve<IRaceGetter>(raceNpcs.Key, out var race) && race.Flags.HasFlag(Race.Flag.Child))
            .SelectMany(raceNpcs => raceNpcs.Value)
            .ToHashSet();

        _allVoiceTypes = _formLinkCache.WinningOverrides<IVoiceTypeGetter>()
            .Select(v => v.FormKey)
            .ToHashSet();
    }

    /// <summary>
    /// Used for testing mainly
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    public VoiceContainer? GetVoicesWithQuest(IDialogTopicGetter topic, IDialogResponsesGetter response)
    {
        var quest = topic.Quest.TryResolve(_formLinkCache);
        if (quest == null) return null;

        //When the quest doesn't allow export return no voices  
        if ((quest.Flags & Quest.Flag.ExcludeFromDialogExport) != 0) return new VoiceContainer();

        //When all responses use a sound override, return no voices
        if (response.Responses.All(r => !r.Sound.IsNull)) return new VoiceContainer();

        //If this is a shared info and it's not used, return no voices
        if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && !GetSharedInfoUsages(response).Any()) return new VoiceContainer();

        //Get quest voices
        var questVoices = GetQuestVoices(topic, quest);

        //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voices
        var voices = GetVoices(topic, response, quest);
        voices.IntersectWith(questVoices);

        LimitVoicesToSharedInfoUsages(voices, topic, response);

        return voices;
    }

    public IEnumerable<DataRelativePath> GetVoiceLineFilePaths(IDialogTopicGetter topic)
    {
        var quest = topic.Quest.TryResolve(_formLinkCache);
        if (quest == null) yield break;

        //Get quest voices
        var questVoices = GetQuestVoices(topic, quest);

        var (questString, topicString) = GetQuestAndTopicStrings(topic, quest);
        foreach (var responses in topic.Responses)
        {
            foreach (var path in GetVoiceLineFilePaths(topic, responses, quest, questVoices, questString, topicString))
            {
                yield return path;
            }
        }
    }

    public IEnumerable<DataRelativePath> GetVoiceLineFilePaths(IDialogResponsesGetter responses)
    {
        var responsesContext = _formLinkCache.ResolveSimpleContext<IDialogResponsesGetter>(responses.FormKey);
        if (!responsesContext.TryGetParent<IDialogTopicGetter>(out var topic)) yield break;

        var quest = topic.Quest.TryResolve(_formLinkCache);
        if (quest == null) yield break;

        //Get quest voices
        var questVoices = GetQuestVoices(topic, quest);

        var (questString, topicString) = GetQuestAndTopicStrings(topic, quest);
        foreach (var path in GetVoiceLineFilePaths(topic, responses, quest, questVoices, questString, topicString))
        {
            yield return path;
        }
    }

    private VoiceContainer GetVoiceContainer(IDialogResponsesGetter responses)
    {
        if (!_formLinkCache.TryResolveSimpleContext(responses, out var context))
            return new();
        if (!context.TryGetParent<IDialogTopicGetter>(out var topic))
            return new();
        if (!topic.Quest.TryResolve(_formLinkCache, out var quest))
            return new();

        //Get quest voices
        var questVoices = GetQuestVoices(topic, quest);

        //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voices
        var voiceContainer = GetVoices(topic, responses, quest);
        voiceContainer.IntersectWith(questVoices);

        return voiceContainer;
    }

    /// <summary>
    /// Get all voice types for a given dialog that can speak it based on the conditions of the dialog.
    /// </summary>
    /// <param name="responses">Dialog responses to get speakers for</param>
    /// <returns>List of voice types</returns>
    public IEnumerable<IFormLinkGetter<IVoiceTypeGetter>> GetVoiceTypes(IDialogResponsesGetter responses)
    {
        return GetVoiceContainer(responses).Voices.Keys
            .Select(v => v.ToLink<IVoiceTypeGetter>());
    }

    /// <summary>
    /// Get all NPCs for a given dialog that can speak it based on the conditions of the dialog.
    /// </summary>
    /// <param name="responses">Dialog responses to get speakers for</param>
    /// <returns>List of NPC speakers, including npcs or talking activators</returns>
    public IEnumerable<IFormLinkGetter<IHasVoiceTypeGetter>> GetSpeakers(IDialogResponsesGetter responses)
    {
        return GetVoiceContainer(responses).Voices.SelectMany(x =>
        {
            // A subset of speakers is used
            if (x.Value.Count > 0) return x.Value as IEnumerable<FormKey>;

            // The whole voice type is used
            return _voiceSpeakers.GetOrDefault(x.Key) ?? [];
        }).Distinct().Select(speaker => speaker.ToLink<IHasVoiceTypeGetter>());
    }

    private IEnumerable<DataRelativePath> GetVoiceLineFilePaths(
        IDialogTopicGetter topic,
        IDialogResponsesGetter responses,
        IQuestGetter quest,
        VoiceContainer? questVoices,
        string questString,
        string topicString)
    {
        var voices = GetDialogVoiceContainer(topic, responses, quest, questVoices);

        var responseFormID = responses.FormKey.ID.ToString("X8");

        foreach (var response in responses.Responses)
        {
            // Skip responses with sound override
            if (!response.Sound.IsNull) continue;

            var responseNumber = response.ResponseNumber;
            foreach (var voiceType in voices.GetVoiceTypes())
            {
                if (!_formLinkCache.TryResolve<IVoiceTypeGetter>(voiceType, out var voice) || voice.EditorID == null) continue;
                yield return Path.Combine
                (
                    "Sound",
                    "Voice",
                    responses.FormKey.ModKey.FileName,
                    voice.EditorID,
                    $"{questString}_{topicString}_{responseFormID}_{responseNumber}.fuz"
                );
            }
        }
    }

    private VoiceContainer GetDialogVoiceContainer(
        IDialogTopicGetter topic,
        IDialogResponsesGetter responses,
        IQuestGetter quest,
        VoiceContainer? questVoices)
    {
        //Don't process responses with response data
        if (!responses.ResponseData.IsNull)
        {
            return VoiceContainer.Empty;
        }

        //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voices
        var voices = GetVoices(topic, responses, quest);
        voices.IntersectWith(questVoices);

        LimitVoicesToSharedInfoUsages(voices, topic, responses);

        return voices;
    }

    IEnumerable<IModContext<IDialogResponsesGetter>> GetSharedInfoUsages(IDialogResponsesGetter response)
    {
        return _usageCache.GetUsagesOf<IDialogResponsesGetter>(response).UsageLinks
            .Select(u => u.ResolveSimpleContext(_formLinkCache)).WhereNotNull()
            .Where(u => u.Record.ResponseData.Equals(response));
    }

    private void LimitVoicesToSharedInfoUsages(VoiceContainer voices, IDialogTopicGetter topic, IDialogResponsesGetter responses) {
        if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo)
        {
            var userConditions = GetSharedInfoUsages(responses)
                .Select(responseContext =>
                {
                    if (!responseContext.TryGetParent<IDialogTopicGetter>(out var currentTopic)) return null;
                    var currentQuest = currentTopic.Quest.TryResolve(_formLinkCache);
                    if (currentQuest == null) return null;

                    return GetVoices(responseContext.Record.Conditions, currentQuest);
                })
                .WhereNotNull()
                .ToList()
                .MergeInsert();

            // The user has conditions
            if (userConditions != null)
            {
                voices.IntersectWith(userConditions);
            }
        }
    }

    private VoiceContainer? GetQuestVoices(IDialogTopicGetter topic, IQuestGetter quest)
    {
        lock (_questCacheLock)
        {
            if (!_questCache.TryGetValue(quest.FormKey, out var questVoices))
            {
                questVoices = GetVoices(quest);
                _questCache.TryAdd(quest.FormKey, questVoices);
            }

            return questVoices;
        }
    }

    private static (string questString, string topicString) GetQuestAndTopicStrings(IDialogTopicGetter topic, IQuestGetter quest)
    {
        //Voice line variables
        var questID = quest.EditorID ?? "";
        var topicID = topic.EditorID ?? "";

        //Evaluate string length
        var questLength = questID.Length;
        var topicLength = topicID.Length;
        if (questLength + topicLength > 25)
        {
            if (questLength > 10)
            {
                questLength = 10;
                topicLength = 15;
            }
            else
            {
                topicLength = 25 - questLength;
            }
        }

        var questString = questID[..questLength];
        var topicString = topicID.Length > topicLength ? topicID[..topicLength] : topicID;
        return (questString, topicString);
    }

    // Get alias index of a scene topic. Returns null for orphaned topics
    bool GetSceneAliasIndex(IDialogTopicGetter topic, [MaybeNullWhen(false)] out int? index)
    {
        index = _usageCache.GetUsagesOf<ISceneGetter>(topic).UsageLinks
            .SelectMany(u => u.Resolve(_formLinkCache).Actions)
            .FirstOrDefault(action => action.Topic.Equals(topic))?.ActorID;
        return index != null;
    }

    private VoiceContainer GetVoices(IDialogTopicGetter topic, IDialogResponsesGetter response, IQuestGetter quest)
    {
        //Use speaker only if we have one
        if (!response.Speaker.IsNull) return GetSpeakerVoiceContainer(response.Speaker.FormKey);

        VoiceContainer? voices = null;

        //Check scene
        if (topic.Subtype == DialogTopic.SubtypeEnum.Scene && GetSceneAliasIndex(topic, out var aliasIndex))
        {
            voices = GetVoices(quest, aliasIndex!.Value);
        }

        //Search conditions
        if (response.Conditions.Any())
        {
            var conditionVoices = GetVoices(response.Conditions, quest);
            if (voices == null)
            {
                voices = conditionVoices;
            }
            else
            {
                voices.IntersectWith(conditionVoices);
            }
        }

        // If there is no filtering from scene or conditions, response is valid for any speaker
        if (voices == null)
        {
            voices = new VoiceContainer(_allVoiceTypes);
        }

        return voices;
    }

    private VoiceContainer? GetVoices(IEnumerable<IConditionGetter> conditions, IQuestGetter quest)
    {
        var voiceTypesOrBlock = new List<VoiceContainer>();
        var currentConditions = new List<IConditionGetter>();

        //Calculate OR blocks
        var conditionsList = conditions.ToList();
        for (var i = 0; i < conditionsList.Count; i++)
        {
            var condition = conditionsList[i];
            currentConditions.Add(condition);

            //At every new AND or at the end of the conditions, finish the current block
            if ((condition.Flags & Condition.Flag.OR) == 0 || i == conditionsList.Count - 1)
            {
                var voices = GetVoiceTypesOrBlock(currentConditions, quest);
                if (voices != null) voiceTypesOrBlock.Add(voices);

                currentConditions.Clear();
            }
        }

        //Merge OR blocks
        return voiceTypesOrBlock.Any() ? voiceTypesOrBlock.MergeIntersect() : null;
    }

    private VoiceContainer? GetVoiceTypesOrBlock(IEnumerable<IConditionGetter> conditions, IQuestGetter quest)
    {
        return conditions
            .Select(condition => GetVoices(condition, quest))
            .WhereNotNull()
            .ToList()
            .MergeInsert();
    }

    /// <summary>
    /// Create a voice container for a condition data.
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="data"></param>
    /// <param name="inverted">Context for functions that needs it. Does NOT invert the result</param>
    /// <returns>Container for condition, or null if condition does not filter</returns>
    private VoiceContainer? GetConditionDataVoices(IQuestGetter quest, IConditionDataGetter data, bool inverted)
    {
        switch (data)
        {
            case IGetIsIDConditionDataGetter getIsId:
                if (getIsId.Object.UsesLink())
                {
                    var getIsIdFormKey = getIsId.Object.Link.FormKey;
                    if (_speakerVoices.TryGetValue(getIsIdFormKey, out var idVoices))
                    {
                        return new VoiceContainer(getIsIdFormKey, idVoices);
                    }
                }

                break;
            case IGetIsVoiceTypeConditionDataGetter isVoiceType:
                var voiceTypeRecord = isVoiceType.VoiceTypeOrList.Link.TryResolve(_formLinkCache);
                switch (voiceTypeRecord)
                {
                    case IVoiceTypeGetter voiceType:
                        return new VoiceContainer(voiceType.FormKey);
                    case IFormListGetter formList:
                        return new VoiceContainer(formList.Items.Select(i => i.FormKey).Where(_allVoiceTypes.Contains));
                }
                break;
            case IGetIsAliasRefConditionDataGetter aliasRef:
                return GetVoices(quest, aliasRef.ReferenceAliasIndex);
            case IGetInFactionConditionDataGetter getInFaction:
                // Inverting a GetInFaction condition requires special handling of potential members to account for cases such as `PotentialFollowerFaction == 1 && CurrentFollowerFaction == 0`
                // Actual inversion of the container is handled below
                if (getInFaction.Faction.UsesLink() && (inverted ? _staticFactionNPCs : _potentialFactionNPCs).TryGetValue(getInFaction.Faction.Link.FormKey, out var factionNpcFormKeys))
                {
                    return new VoiceContainer(factionNpcFormKeys, _speakerVoices);
                }

                break;
            case IGetFactionRankConditionDataGetter getFactionRank:
                // Assume the actor can be in any rank as long they are in the faction - they might shift ranks later on
                if (getFactionRank.Faction.UsesLink() && (inverted ? _staticFactionNPCs : _potentialFactionNPCs).TryGetValue(getFactionRank.Faction.Link.FormKey, out var factionNpcFormKeys2))
                {
                    return new VoiceContainer(factionNpcFormKeys2, _speakerVoices);
                }

                break;
            case IGetIsClassConditionDataGetter getIsClass:
                if (getIsClass.Class.UsesLink() && _classNPCs.TryGetValue(getIsClass.Class.Link.FormKey, out var classNpcFormKeys))
                {
                    return new VoiceContainer(classNpcFormKeys, _speakerVoices);
                }

                break;
            case IHasKeywordConditionDataGetter hasKeyword:
                if (_keywordNPCs.TryGetValue(hasKeyword.Keyword.Link.FormKey, out var keywordNpcs))
                {
                    return new VoiceContainer(keywordNpcs, _speakerVoices);
                }
                break;
            case IGetIsRaceConditionDataGetter getIsRace:
                if (getIsRace.Race.UsesLink() && _raceNPCs.TryGetValue(getIsRace.Race.Link.FormKey, out var raceNpcFormKeys))
                {
                    return new VoiceContainer(raceNpcFormKeys, _speakerVoices);
                }

                break;
            case IGetIsSexConditionDataGetter sexConditionDataGetter:
                if (_genderNPCs.TryGetValue(sexConditionDataGetter.MaleFemaleGender, out var genderNpcFormKeys))
                {
                    return new VoiceContainer(genderNpcFormKeys, _speakerVoices);
                }

                break;
            case IIsInListConditionDataGetter isInList:
                if (isInList.FormList.Link.TryResolve(_formLinkCache, out var formList2))
                {
                    // Container will skip entries that are not speakers
                    return new VoiceContainer(formList2.Items.Select(i => i.FormKey), _speakerVoices);

                }
                break;
            case IIsChildConditionDataGetter isChild:
                return new VoiceContainer(_childNPCs, _speakerVoices);
            default:
                // Condition does not filter
                return null;
        }
        // Condition has invalid argument or no NPCs meet requirement
        return new VoiceContainer();
    }

    // Returns null if condition does not filter
    private VoiceContainer? GetVoices(IConditionGetter condition, IQuestGetter quest)
    {
        var data = condition.Data;

        if (data.RunOnType != Condition.RunOnType.Subject) return null;

        var inverted = IsConditionInverted(condition);
        var voices = GetConditionDataVoices(quest, data, inverted);

        if (voices != null && inverted)
        {
            //Can't invert alias according to CK calculation
            if (data.Function == Condition.Function.GetIsAliasRef)
            {
                return null;
            }

            voices.Invert(_voiceSpeakers);
        }

        return voices;
    }

    private bool IsConditionInverted(IConditionGetter condition)
    {
        var compareValue = condition switch
        {
            IConditionFloatGetter conditionFloat => conditionFloat.ComparisonValue,
            IConditionGlobalGetter conditionGlobal => conditionGlobal.ComparisonValue.TryResolve(_formLinkCache) switch
            {
                IGlobalFloat globalFloat => globalFloat.Data,
                IGlobalInt globalInt => globalInt.Data,
                IGlobalShortGetter globalShort => globalShort.Data,
                _ => 0,
            },
            _ => 0
        } ?? 0;

        switch (condition.CompareOperator)
        {
            case CompareOperator.EqualTo:
                return compareValue == 0;
            case CompareOperator.NotEqualTo:
                return compareValue == 1;
            case CompareOperator.GreaterThan:
            case CompareOperator.GreaterThanOrEqualTo:
            case CompareOperator.LessThan:
            case CompareOperator.LessThanOrEqualTo:
            default:
                return false;
        }
    }

    private VoiceContainer? GetVoices(IQuestGetter quest, int aliasIndex)
    {
        var alias = quest.Aliases.FirstOrDefault(a => a.ID == aliasIndex);
        return alias == null ? null : GetVoices(alias, quest);
    }

    private VoiceContainer? GetVoices(IQuestAliasGetter alias, IQuestGetter quest)
    {
        //External Alias
        if (alias.External != null)
        {
            var externalQuest = alias.External.Quest.TryResolve(_formLinkCache);
            var aliasIndex = alias.External.AliasID;
            if (externalQuest != null && aliasIndex != null)
            {
                return GetVoices(externalQuest, aliasIndex.Value);
            }
        }

        //Additional voice types
        var additionalVoices = alias.VoiceTypes.TryResolve(_formLinkCache) switch
        {
            INpcGetter npc => GetSpeakerVoiceContainer(npc.FormKey),
            IFormListGetter formList => GetFormListVoices(formList),
            _ => null
        };

        //Forced Ref
        if (!alias.ForcedReference.IsNull)
        {
            var placedNPC = alias.ForcedReference.TryResolve<IPlacedNpcGetter>(_formLinkCache);
            if (placedNPC != null)
            {
                var voices = GetSpeakerVoiceContainer(placedNPC.Base.FormKey);
                if (additionalVoices != null) voices.Insert(additionalVoices);
                return voices;
            }

            var placedObject = alias.ForcedReference.TryResolve<IPlacedObjectGetter>(_formLinkCache);
            if (placedObject != null)
            {
                var voices = GetSpeakerVoiceContainer(placedObject.Base.FormKey);
                if (additionalVoices != null) voices.Insert(additionalVoices);
                return voices;
            }
        }

        //Created object
        if (alias.CreateReferenceToObject != null)
        {
            var voices = GetSpeakerVoiceContainer(alias.CreateReferenceToObject.Object.FormKey);
            if (additionalVoices != null) voices.Insert(additionalVoices);
            return voices;
        }

        //Conditions
        // These do not allow a unique actor alias to fill with someone else
        if (alias.Conditions.Any() && alias.UniqueActor.IsNull)
        {
            var voices = GetVoices(alias.Conditions, quest);
            if (voices != null && additionalVoices != null) voices.Insert(additionalVoices);
            return voices;
        }

        //Additional voices overwrites location alias and unique npc
        if (additionalVoices != null)
        {
            return additionalVoices;
        }

        //Location alias. Currently only SpecificLocation is supported
        if (alias.Location is { AliasID: {} })
        {
            var locationAlias = quest.Aliases.FirstOrDefault(a => a.ID == alias.Location.AliasID.Value);
            if (locationAlias != null)
            {
                if (!locationAlias.SpecificLocation.IsNull)
                {
                    var location = locationAlias.SpecificLocation.TryResolve(_formLinkCache);
                    if (location is not null)
                    {
                        foreach (var locRef in location.LocationRefTypesReferences().Where(r => r.LocationRefType.FormKey == alias.Location.RefType.FormKey))
                        {
                            var linkedRef = locRef.Ref.TryResolve(_formLinkCache);
                            if (linkedRef == null) continue;

                            return linkedRef switch
                            {
                                IPlacedNpcGetter placedNpc => GetSpeakerVoiceContainer(placedNpc.Base.FormKey),
                                IPlacedObjectGetter placedObject => GetSpeakerVoiceContainer(placedObject.Base.FormKey),
                                _ => null
                            };
                        }
                    }
                }
            }
        }

        //Unique NPC
        if (!alias.UniqueActor.IsNull) return new VoiceContainer(alias.UniqueActor.FormKey, GetVoiceTypes(alias.UniqueActor.FormKey));

        //Find matching from event => default voices
        if (alias.FindMatchingRefFromEvent != null || alias.FindMatchingRefNearAlias != null)
        {
            return null;
        }

        //Nothing is valid => no voices for this alias
        return VoiceContainer.Empty;
    }

    private VoiceContainer GetSpeakerVoiceContainer(FormKey speaker) => new(speaker, GetVoiceTypes(speaker));

    private VoiceContainer GetFormListVoices(IFormListGetter formList)
    {
        var voices = new VoiceContainer();
        foreach (var entry in formList.Items)
        {
            if (_formLinkCache.TryResolveIdentifier<IVoiceTypeGetter>(entry.FormKey, out var _))
            {
                voices.AddFullVoice(entry.FormKey);
            }
            else if (_speakerVoices.TryGetValue(entry.FormKey, out var speakerVoiceTypes))
            {
                voices.AddSpeaker(entry.FormKey, speakerVoiceTypes);
            }
        }
        return voices;
    }

    private VoiceContainer? GetVoices(IQuestGetter quest) => GetVoices(quest.DialogConditions, quest);

    private IEnumerable<FormKey> GetVoiceTypes(FormKey speaker)
    {
        return _speakerVoices.TryGetValue(speaker, out var speakerVoiceTypes) ? speakerVoiceTypes : [];
    }

    private IEnumerable<INpcGetter> GetTemplateActors(INpcSpawnGetter spawn, NpcConfiguration.TemplateFlag inheritFlag)
    {
        switch (spawn)
        {
            case INpcGetter npc:
                if (npc.Configuration.TemplateFlags.HasFlag(inheritFlag) && npc.Template.TryResolve(_formLinkCache, out var template))
                    return GetTemplateActors(template, inheritFlag);
                else
                    return [npc];
            case ILeveledNpcGetter leveledNpc:
                if (leveledNpc.Entries == null) return [];

                return leveledNpc.Entries
                    .Select(e => e.Data?.Reference?.TryResolve(_formLinkCache))
                    .WhereNotNull()
                    .SelectMany(e => GetTemplateActors(e, inheritFlag));
            default: return [];
        }
    }

    private FormKey GetDefaultVoice(IFormLinkGetter<IRaceGetter> raceLink, bool female)
    {
        if (!raceLink.TryResolve(_formLinkCache, out var race))
            return FormKey.Null;
        var link = female ? race.Voices.Female : race.Voices.Male;
        return link.FormKey;
    }

    // Getters return enumerable as a temporary hash set would be excessive
    private IEnumerable<IRankPlacementGetter> GetFactions(INpcSpawnGetter npc)
    {
        return GetTemplateActors(npc, NpcConfiguration.TemplateFlag.Factions)
            .SelectMany(n => n.Factions);
    }

    private IEnumerable<IFormLinkGetter<IClassGetter>> GetClasses(INpcSpawnGetter npc)
    {
        return GetTemplateActors(npc, NpcConfiguration.TemplateFlag.Stats)
            .Select(n => n.Class)
            .Where(c => !c.IsNull);
    }

    // Does not include keywords from race
    private IEnumerable<IFormLinkGetter<IKeywordGetter>> GetDirectKeywords(INpcSpawnGetter npc)
    {
        return GetTemplateActors(npc, NpcConfiguration.TemplateFlag.Keywords)
            .SelectMany(n => n.Keywords ?? []);
    }
}