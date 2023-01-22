using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

public class VoiceTypeAssetLookup : IAssetCacheComponent
{
    private ILinkCache _formLinkCache = null!;

    //Databases
    private readonly Dictionary<ModKey, VoiceContainer> _defaultSpeakerVoices = new();
    private readonly Dictionary<ModKey, HashSet<string>> _defaultVoiceTypes = new();
    private readonly Dictionary<FormKey, HashSet<string>> _speakerVoices = new();

    private readonly Dictionary<FormKey, HashSet<FormKey>> _factionNPCs = new();
    private readonly Dictionary<FormKey, HashSet<FormKey>> _classNPCs = new();
    private readonly Dictionary<FormKey, HashSet<FormKey>> _raceNPCs = new();
    private readonly Dictionary<bool, HashSet<FormKey>> _genderNPCs = new();
    private HashSet<FormKey> _childNPCs = null!;
    private readonly Dictionary<FormKey, int> _dialogueSceneAliasIndex = new();


    //Caches
    private readonly Dictionary<FormKey, VoiceContainer> _questCache = new();
    private readonly Dictionary<FormKey, HashSet<FormKey>> _sharedInfosCache = new();

    public void Prep(IAssetLinkCache linkCache)
    {
        _formLinkCache = linkCache.FormLinkCache;

        var childRaces = new HashSet<FormKey>();
        foreach (var mod in _formLinkCache.PriorityOrder)
        {
            foreach (var npc in mod.EnumerateMajorRecords<INpcGetter>())
            {
                if (!_speakerVoices.ContainsKey(npc.FormKey))
                {
                    _speakerVoices.Add(npc.FormKey, GetVoiceTypes(npc));
                }

                foreach (var factionKey in GetFactions(npc))
                {
                    _factionNPCs
                        .GetOrAdd(factionKey)
                        .Add(npc.FormKey);
                }

                foreach (var classKey in GetClasses(npc))
                {
                    _classNPCs
                        .GetOrAdd(classKey)
                        .Add(npc.FormKey);
                }

                foreach (var gender in GetGenders(npc))
                {
                    _genderNPCs
                        .GetOrAdd(gender)
                        .Add(npc.FormKey);
                }

                foreach (var raceKey in GetRaces(npc))
                {
                    _raceNPCs
                        .GetOrAdd(raceKey)
                        .Add(npc.FormKey);
                }
            }

            foreach (var response in mod.EnumerateMajorRecords<IDialogResponsesGetter>())
            {
                if (!response.ResponseData.IsNull)
                {
                    _sharedInfosCache
                        .GetOrAdd(response.ResponseData.FormKey)
                        .Add(response.FormKey);
                }
            }

            foreach (var talkingActivator in mod.EnumerateMajorRecords<ITalkingActivatorGetter>())
            {
                if (!_speakerVoices.ContainsKey(talkingActivator.FormKey))
                {
                    _speakerVoices.Add(talkingActivator.FormKey, GetVoiceTypes(talkingActivator));
                }
            }

            foreach (var race in mod.EnumerateMajorRecords<IRaceGetter>())
            {
                if ((race.Flags & Race.Flag.Child) != 0)
                {
                    childRaces.Add(race.FormKey);
                }
            }

            foreach (var scene in mod.EnumerateMajorRecords<ISceneGetter>())
            {
                foreach (var action in scene.Actions)
                {
                    if (action.Type == SceneAction.TypeEnum.Dialog && !action.Topic.IsNull && action.ActorID != null && !_dialogueSceneAliasIndex.ContainsKey(action.Topic.FormKey))
                    {
                        _dialogueSceneAliasIndex.Add(action.Topic.FormKey, action.ActorID.Value);
                    }
                }
            }

            var defaultVoiceTypes = new HashSet<string>();
            _defaultVoiceTypes.Add(mod.ModKey, defaultVoiceTypes);
            foreach (var voiceType in mod.EnumerateMajorRecords<IVoiceTypeGetter>())
            {
                if (voiceType.EditorID != null)
                {
                    if ((voiceType.Flags & Skyrim.VoiceType.Flag.AllowDefaultDialog) != 0)
                    {
                        defaultVoiceTypes.Add(voiceType.EditorID);
                    }
                }
            }
        }

        //Build child cache
        _childNPCs = childRaces
            .SelectWhere(r => _raceNPCs.TryGetValue(r, out var raceNpcFormKeys) ? TryGet<HashSet<FormKey>>.Succeed(raceNpcFormKeys) : TryGet<HashSet<FormKey>>.Failure)
            .SelectMany(x => x)
            .ToHashSet();

        //Add master voice types
        var defaultVoicesCopy = new Dictionary<ModKey, HashSet<string>>(_defaultVoiceTypes);
        foreach (var mod in _formLinkCache.PriorityOrder)
        {
            foreach (var master in mod.MasterReferences)
            {
                if (!defaultVoicesCopy.TryGetValue(master.Master, out var defaultVoiceTypes)) continue;

                foreach (var voiceType in defaultVoiceTypes)
                {
                    _defaultVoiceTypes[mod.ModKey].Add(voiceType);
                }
            }
        }
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

        //Get quest voices
        VoiceContainer questVoices;
        if (_questCache.TryGetValue(quest.FormKey, out var questVoiceContainer))
        {
            questVoices = questVoiceContainer;
        } else
        {
            questVoices = GetVoices(quest, topic.FormKey.ModKey);
            _questCache.Add(quest.FormKey, questVoices);
        }

        //If this is a shared info and it's not used, return no voices
        if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && !_sharedInfosCache.ContainsKey(response.FormKey)) return new VoiceContainer();

        //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voices
        var voices = GetVoices(topic, response, quest);
        if (!questVoices.IsDefault)
        {
            if (voices.IsDefault)
            {
                voices = (VoiceContainer) questVoices.Clone();
            } else
            {
                voices.IntersectWith(questVoices);
            }
        }

        if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && _sharedInfosCache.TryGetValue(response.FormKey, out var responseFormKeys))
        {
            var userConditions = new VoiceContainer(true);
            foreach (var responseKey in responseFormKeys)
            {
                var responseContext = _formLinkCache.ResolveSimpleContext<IDialogResponsesGetter>(responseKey);
                if (responseContext is not { Parent.Record: {} }) continue;

                if (!responseContext.TryGetParent<IDialogTopicGetter>(out var currentTopic)) continue;
                var currentQuest = currentTopic.Quest.TryResolve(_formLinkCache);
                if (currentQuest == null) continue;

                userConditions.Insert(GetVoices(responseContext.Record.Conditions, quest, topic.FormKey.ModKey));
            }

            voices.Merge(userConditions);
        }

        return voices;
    }

    public IEnumerable<string> GetVoiceTypePaths(IDialogTopicGetter topic)
    {
        var quest = topic.Quest.TryResolve(_formLinkCache);
        if (quest == null) yield break;

        //Get quest voices
        VoiceContainer questVoices;
        if (_questCache.TryGetValue(quest.FormKey, out var questVoiceContainer))
        {
            questVoices = questVoiceContainer;
        } else
        {
            questVoices = GetVoices(quest, topic.FormKey.ModKey);
            _questCache.Add(quest.FormKey, questVoices);
        }

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
            } else
            {
                topicLength = 25 - questLength;
            }
        }

        var questString = questID[..questLength];
        var topicString = topicID.Length > topicLength ? topicID[..topicLength] : topicID;
        foreach (var responses in topic.Responses)
        {
            //Don't process responses with response data
            if (!responses.ResponseData.IsNull) continue;

            //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voices
            var voices = GetVoices(topic, responses, quest);

            if (!questVoices.IsDefault)
            {
                //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voice
                if (voices.IsDefault)
                {
                    voices = (VoiceContainer) questVoices.Clone();
                } else
                {
                    voices.IntersectWith(questVoices);
                }
            }

            if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && _sharedInfosCache.TryGetValue(responses.FormKey, out var responseFormKeys))
            {
                var userConditions = new VoiceContainer(true);
                foreach (var responseKey in responseFormKeys)
                {
                    var responseContext = _formLinkCache.ResolveSimpleContext<IDialogResponsesGetter>(responseKey);
                    if (responseContext is not { Parent.Record: {} }) continue;

                    if (!responseContext.TryGetParent<IDialogTopicGetter>(out var currentTopic)) continue;
                    var currentQuest = currentTopic.Quest.TryResolve(_formLinkCache);
                    if (currentQuest == null) continue;

                    userConditions.Insert(GetVoices(responseContext.Record.Conditions, quest, topic.FormKey.ModKey));
                }

                voices.Merge(userConditions);
            }


            var responseFormID = responses.FormKey.ID.ToString("X8");

            foreach (var response in responses.Responses)
            {
                var responseNumber = response.ResponseNumber;
                foreach (var voiceType in voices.GetVoiceTypes(_defaultVoiceTypes[topic.FormKey.ModKey]))
                {
                    yield return Path.Combine
                    (
                        "Sound",
                        "Voice",
                        topic.FormKey.ModKey.FileName,
                        voiceType,
                        $"{questString}_{topicString}_{responseFormID}_{responseNumber}.fuz"
                    );
                }
            }
        }
    }

    private VoiceContainer GetVoices(IDialogTopicGetter topic, IDialogResponsesGetter response, IQuestGetter quest)
    {
        var voices = new VoiceContainer(true);

        //Use speaker only if we have one
        if (!response.Speaker.IsNull) return GetVoices(response.Speaker.FormKey);

        //Check scene
        if (topic.Category == DialogTopic.CategoryEnum.Scene && _dialogueSceneAliasIndex.TryGetValue(topic.FormKey, out var aliasIndex))
        {
            voices.Merge(GetVoices(quest, aliasIndex, topic.FormKey.ModKey));
        }

        //Search conditions
        if (response.Conditions.Any()) voices.Merge(GetVoices(response.Conditions, quest, topic.FormKey.ModKey));

        return voices;
    }

    private VoiceContainer GetVoices(IEnumerable<IConditionGetter> conditions, IQuestGetter quest, ModKey currentMod)
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
                var voices = GetVoiceTypesOrBlock(currentConditions, quest, currentMod);
                if (!voices.IsDefault) voiceTypesOrBlock.Add(voices);

                currentConditions.Clear();
            }
        }

        //Merge OR blocks
        return voiceTypesOrBlock.Any() ? voiceTypesOrBlock.MergeAll() : new VoiceContainer(true);
    }

    private VoiceContainer GetVoiceTypesOrBlock(IEnumerable<IConditionGetter> conditions, IQuestGetter quest, ModKey currentMod)
    {
        var voices = new VoiceContainer(true);

        foreach (var condition in conditions)
        {
            var conditionVoices = GetVoices(condition, quest, currentMod);
            if (conditionVoices.IsDefault) continue;

            voices.Insert(conditionVoices);
        }

        return voices;
    }

    private VoiceContainer GetVoices(IConditionGetter condition, IQuestGetter quest, ModKey currentMod)
    {
        var voices = new VoiceContainer();

        if (condition.Data.RunOnType != Condition.RunOnType.Subject || condition.Data is not IFunctionConditionDataGetter functionConditionData) return new VoiceContainer(true);

        switch (functionConditionData)
        {
            case IGetIsIDConditionDataGetter getIsId:
                if (!getIsId.FirstParameter.Link.IsNull)
                {
                    voices = new VoiceContainer(getIsId.FirstParameter.Link.FormKey, _speakerVoices[getIsId.FirstParameter.Link.FormKey]);
                }

                break;
            case IGetIsVoiceTypeConditionDataGetter isVoiceType:
                if (isVoiceType.FirstParameter.Link.TryResolve(_formLinkCache, out var voiceTypeRecord))
                {
                    switch (voiceTypeRecord)
                    {
                        case IVoiceTypeGetter voiceType when voiceType.EditorID != null:
                            voices = new VoiceContainer(voiceType.EditorID);
                            break;
                        case IFormListGetter formList:
                            voices = new VoiceContainer(formList.Items.SelectWhere(link =>
                            {
                                _formLinkCache.TryResolveIdentifier<IVoiceTypeGetter>(link.FormKey, out var linkVoiceTypeEditorId);
                                return linkVoiceTypeEditorId == null ? TryGet<string>.Failure : TryGet<string>.Succeed(linkVoiceTypeEditorId);
                            }).ToHashSet());
                            break;
                    }
                }
                
                break;
            case IGetIsAliasRefConditionDataGetter aliasRef:
                voices = GetVoices(quest, aliasRef.FirstParameter, currentMod);

                break;
            case IGetInFactionConditionDataGetter getInFaction:
                if (_factionNPCs.TryGetValue(getInFaction.FirstParameter.Link.FormKey, out var factionNpcFormKeys))
                {
                    voices = new VoiceContainer(factionNpcFormKeys.ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case IGetIsClassConditionDataGetter getIsClass:
                if (_classNPCs.TryGetValue(getIsClass.FirstParameter.Link.FormKey, out var classNpcFormKeys))
                {
                    voices = new VoiceContainer(classNpcFormKeys.ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case IGetIsRaceConditionDataGetter getIsRace:
                if (_raceNPCs.TryGetValue(getIsRace.FirstParameter.Link.FormKey, out var raceNpcFormKeys))
                {
                    voices = new VoiceContainer(raceNpcFormKeys.ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case IGetIsSexConditionDataGetter sexConditionDataGetter:
                var isFemale = sexConditionDataGetter.FirstParameter == MaleFemaleGender.Female;
                if (_genderNPCs.TryGetValue(isFemale, out var genderNpcFormKeys))
                {
                    voices = new VoiceContainer(genderNpcFormKeys.ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case IIsInListConditionDataGetter isInList:
                if (!isInList.FirstParameter.Link.IsNull)
                {
                    var formList = isInList.FirstParameter.Link.TryResolve(_formLinkCache);
                    //Only look at speakers in the form list
                    if (formList != null) voices = formList.Items.Select(link => GetVoices(link.FormKey)).MergeInsert();
                }

                break;
            case IIsChildConditionDataGetter isChild:
                voices = new VoiceContainer(_childNPCs.ToDictionary(npc => npc, GetVoiceTypes));

                break;
            default:
                voices = new VoiceContainer(true);
                break;
        }

        if (!voices.IsDefault && !IsConditionValid(condition))
        {
            //Can't invert alias according to CK calculation
            if (functionConditionData.Function == Condition.Function.GetIsAliasRef)
            {
                return new VoiceContainer(true);
            }

            voices = Invert(voices, functionConditionData.Function == Condition.Function.GetIsVoiceType, currentMod);
        }

        return voices;
    }

    private bool IsConditionValid(IConditionGetter condition)
    {
        const double floatTolerance = 0.001;

        bool FloatEquals(float value, int expected) => Math.Abs(value - expected) < floatTolerance;

        bool GlobalEquals(ILink<IGlobalGetter> global, int expected)
        {
            var globalValue = global.TryResolve(_formLinkCache);
            if (globalValue == null) return false;

            return globalValue switch
            {
                IGlobalFloatGetter globalFloat => globalFloat.Data != null && Math.Abs(globalFloat.Data.Value - expected) < floatTolerance,
                IGlobalIntGetter globalInt => globalInt.Data != null && globalInt.Data.Value == expected,
                IGlobalShortGetter globalShort => globalShort.Data != null && globalShort.Data.Value == expected,
                _ => false
            };
        }

        switch (condition.CompareOperator)
        {
            case CompareOperator.EqualTo:
                return condition switch
                {
                    IConditionFloatGetter floatCondition => FloatEquals(floatCondition.ComparisonValue, 1),
                    IConditionGlobalGetter globalCondition => GlobalEquals(globalCondition.ComparisonValue, 1),
                    _ => false
                };
            case CompareOperator.NotEqualTo:
                return condition switch
                {
                    IConditionFloatGetter floatCondition => FloatEquals(floatCondition.ComparisonValue, 0),
                    IConditionGlobalGetter globalCondition => GlobalEquals(globalCondition.ComparisonValue, 0),
                    _ => false
                };
            case CompareOperator.GreaterThan:
            case CompareOperator.GreaterThanOrEqualTo:
            case CompareOperator.LessThan:
            case CompareOperator.LessThanOrEqualTo:
            default:
                return false;
        }
    }

    private VoiceContainer GetVoices(IQuestGetter quest, int aliasIndex, ModKey currentMod)
    {
        var alias = quest.Aliases.FirstOrDefault(a => a.ID == aliasIndex);
        return alias == null ? new VoiceContainer(true) : GetVoices(alias, quest, currentMod);
    }

    private VoiceContainer GetVoices(IQuestAliasGetter alias, IQuestGetter quest, ModKey currentMod)
    {
        //External Alias
        if (alias.External != null)
        {
            var externalQuest = alias.External.Quest.TryResolve(_formLinkCache);
            var aliasIndex = alias.External.AliasID;
            if (externalQuest != null && aliasIndex != null) 
            {
                return GetVoices(externalQuest, aliasIndex.Value, currentMod);
            }
        }

        //Additional voice types
        VoiceContainer? additionalVoices = null;
        if (!alias.VoiceTypes.IsNull)
        {
            var additionalVoiceTypes = alias.VoiceTypes.TryResolve(_formLinkCache);
            if (additionalVoiceTypes != null)
            {
                additionalVoices = additionalVoiceTypes switch
                {
                    INpcGetter npc => GetVoices(npc),
                    IFormListGetter formList => GetVoices(formList),
                    _ => new VoiceContainer(true)
                };
            }
        }

        //Forced Ref
        if (!alias.ForcedReference.IsNull)
        {
            var placedNPC = alias.ForcedReference.TryResolve<IPlacedNpcGetter>(_formLinkCache);
            if (placedNPC != null)
            {
                var voices = GetVoices(placedNPC.Base.FormKey);
                if (additionalVoices != null) voices.Insert(additionalVoices);
                return voices;
            }

            var placedObject = alias.ForcedReference.TryResolve<IPlacedObjectGetter>(_formLinkCache);
            if (placedObject != null)
            {
                var voices = GetVoices(placedObject.Base.FormKey);
                if (additionalVoices != null) voices.Insert(additionalVoices);
                return voices;
            }
        }

        //Conditions
        if (alias.Conditions.Any())
        {
            var voices = GetVoices(alias.Conditions, quest, currentMod);
            if (additionalVoices != null) voices.Insert(additionalVoices);
            return voices;
        }

        //Created object
        if (alias.CreateReferenceToObject != null)
        {
            var voices = GetVoices(alias.CreateReferenceToObject.Object.FormKey);
            if (additionalVoices != null) voices.Insert(additionalVoices);
            return voices;
        }

        //Additional voices overwrites location alias and unique npc
        if (additionalVoices != null)
        {
            return additionalVoices;
        }

        //Location alias
        if (alias.Location is { AliasID: {} })
        {
            var locationAlias = quest.Aliases.FirstOrDefault(a => a.ID == alias.Location.AliasID.Value);
            if (locationAlias != null)
            {
                if (!locationAlias.SpecificLocation.IsNull)
                {
                    var location = locationAlias.SpecificLocation.TryResolve(_formLinkCache);
                    if (location?.LocationCellStaticReferences != null)
                    {
                        foreach (var locRef in location.LocationCellStaticReferences.Where(r => r.LocationRefType.FormKey == alias.Location.RefType.FormKey))
                        {
                            var linkedRef = locRef.Marker.TryResolve(_formLinkCache);
                            if (linkedRef == null) continue;

                            return linkedRef switch
                            {
                                IPlacedNpcGetter placedNpc => GetVoices(placedNpc.Base.FormKey),
                                IPlacedObjectGetter placedObject => GetVoices(placedObject.Base.FormKey),
                                _ => new VoiceContainer(true)
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
            return new VoiceContainer(true);
        }

        //Nothing is valid => no voices for this alias
        return new VoiceContainer();
    }

    private VoiceContainer GetVoices(FormKey speaker) => new(speaker, GetVoiceTypes(speaker));
    private VoiceContainer GetVoices(INpcGetter npc) => new(npc.FormKey, GetVoiceTypes(npc.FormKey));

    private VoiceContainer GetVoices(IFormListGetter formList)
    {
        var voices = new List<VoiceContainer>();

        foreach (var item in formList.Items)
        {
            if (_formLinkCache.TryResolveIdentifier<IVoiceTypeGetter>(item.FormKey, out var voiceTypeEditorId))
            {
                //FormList entry is VoiceType
                voices.Add(new VoiceContainer(voiceTypeEditorId!));
            } else if (_speakerVoices.ContainsKey(item.FormKey))
            {
                //FormList entry is Npc
                voices.Add(GetVoices(item.FormKey));
            }
        }

        return voices.MergeInsert();
    }

    private VoiceContainer GetVoices(IQuestGetter quest, ModKey currentMod) => GetVoices(quest.DialogConditions, quest, currentMod);

    private VoiceContainer GetDefaultVoices(ModKey mod)
    {
        if (_defaultSpeakerVoices.TryGetValue(mod, out var defaultVoiceTypes)) return defaultVoiceTypes;

        var vc = new VoiceContainer(_speakerVoices);
        vc.InvertVoiceTypes(_defaultVoiceTypes[mod]);
        _defaultSpeakerVoices.Add(mod, vc);
        return vc;
    }

    private VoiceContainer Invert(VoiceContainer voiceContainer, bool invertDefaultVoices, ModKey currentMod)
    {
        VoiceContainer baseVoices;
        if (invertDefaultVoices)
        {
            baseVoices = (VoiceContainer) GetDefaultVoices(currentMod).Clone();
        } else
        {
            baseVoices = new VoiceContainer(_speakerVoices);
        }

        baseVoices.Remove(voiceContainer);
        return baseVoices;
    }

    private IEnumerable<string> GetVoiceTypes(FormKey speaker)
    {
        return _speakerVoices.TryGetValue(speaker, out var speakerVoiceTypes) ? speakerVoiceTypes : Array.Empty<string>();
    }

    #region Voice Parser
    private HashSet<string> GetVoiceTypes(INpcGetter npc)
    {
        if (_speakerVoices.TryGetValue(npc.FormKey, out var speakerVoiceTypes)) return speakerVoiceTypes;

        //Check voice type
        if (!npc.Voice.IsNull)
        {
            var voiceType = npc.Voice.TryResolve(_formLinkCache);
            if (voiceType is { EditorID: {} })
            {
                return new HashSet<string> { voiceType.EditorID };
            }
        }

        //Check template
        if (!npc.Template.IsNull && (npc.Configuration.TemplateFlags & NpcConfiguration.TemplateFlag.Traits) != 0)
        {
            return GetVoiceTypes(npc.Template).ToHashSet();
        }

        return new HashSet<string>();
    }

    private HashSet<string> GetVoiceTypes(IFormLinkGetter<INpcSpawnGetter> npcSpawn)
    {
        if (npcSpawn.IsNull) return new HashSet<string>();

        //NPC
        var npc = npcSpawn.TryResolve<INpcGetter>(_formLinkCache);
        if (npc != null)
        {
            return GetVoiceTypes(npc);
        }

        //Levelled NPC
        var leveledNpc = npcSpawn.TryResolve<ILeveledNpcGetter>(_formLinkCache);
        if (leveledNpc is { Entries: {} })
        {
            return leveledNpc.Entries
                .Select(entry => entry.Data?.Reference).NotNull()
                .SelectMany(GetVoiceTypes).ToHashSet();
        }

        return new HashSet<string>();
    }

    private HashSet<string> GetVoiceTypes(ITalkingActivatorGetter talkingActivator)
    {
        if (_speakerVoices.TryGetValue(talkingActivator.FormKey, out var speakerVoiceTypes)) return speakerVoiceTypes;

        if (!talkingActivator.VoiceType.IsNull)
        {
            var voiceTypeGetter = talkingActivator.VoiceType.TryResolve(_formLinkCache);
            if (voiceTypeGetter is { EditorID: {} })
            {
                return new HashSet<string> { voiceTypeGetter.EditorID };
            }
        }

        return new HashSet<string>();
    }
    #endregion

    #region Faction Parser
    private HashSet<FormKey> GetFactions(INpcGetter npc)
    {
        if ((npc.Configuration.TemplateFlags & NpcConfiguration.TemplateFlag.Factions) == 0)
        {
            return npc.Factions.Where(f => !f.Faction.IsNull).Select(f => f.Faction.FormKey).ToHashSet();
        }

        return npc.Template.IsNull ? new HashSet<FormKey>() : GetFactions(npc.Template);

    }

    private HashSet<FormKey> GetFactions(IFormLinkGetter<INpcSpawnGetter> npcTemplate)
    {
        if (npcTemplate.IsNull) return new HashSet<FormKey>();

        //NPC
        var npc = npcTemplate.TryResolve<INpcGetter>(_formLinkCache);
        if (npc != null) return GetFactions(npc);

        //Levelled NPC
        var leveledNpc = npcTemplate.TryResolve<ILeveledNpcGetter>(_formLinkCache);
        if (leveledNpc is { Entries: {} })
        {
            return leveledNpc.Entries
                .Select(entry => entry.Data?.Reference).NotNull()
                .SelectMany(GetFactions).ToHashSet();
        }

        return new HashSet<FormKey>();
    }
    #endregion

    #region Class Parser
    private HashSet<FormKey> GetClasses(INpcGetter npc)
    {
        if ((npc.Configuration.TemplateFlags & NpcConfiguration.TemplateFlag.Stats) == 0 && !npc.Class.IsNull)
        {
            return new HashSet<FormKey> { npc.Class.FormKey };
        }

        return npc.Template.IsNull ? new HashSet<FormKey>() : GetClasses(npc.Template);

    }

    private HashSet<FormKey> GetClasses(IFormLinkGetter<INpcSpawnGetter> npcTemplate)
    {
        if (npcTemplate.IsNull) return new HashSet<FormKey>();

        //NPC
        var npc = npcTemplate.TryResolve<INpcGetter>(_formLinkCache);
        if (npc != null) return GetClasses(npc);

        //Levelled NPC
        var leveledNpc = npcTemplate.TryResolve<ILeveledNpcGetter>(_formLinkCache);
        if (leveledNpc is { Entries: {} })
        {
            return leveledNpc.Entries
                .Select(entry => entry.Data?.Reference).NotNull()
                .SelectMany(GetClasses).ToHashSet();
        }

        return new HashSet<FormKey>();
    }
    #endregion

    #region Gender Parser
    private HashSet<bool> GetGenders(INpcGetter npc)
    {
        if ((npc.Configuration.TemplateFlags & NpcConfiguration.TemplateFlag.Traits) == 0)
        {
            return new HashSet<bool> { (npc.Configuration.Flags & NpcConfiguration.Flag.Female) != 0 };
        }

        return npc.Template.IsNull ? new HashSet<bool>() : GetGenders(npc.Template);

    }

    private HashSet<bool> GetGenders(IFormLinkGetter<INpcSpawnGetter> npcTemplate)
    {
        if (npcTemplate.IsNull) return new HashSet<bool>();

        //NPC
        var npc = npcTemplate.TryResolve<INpcGetter>(_formLinkCache);
        if (npc != null) return GetGenders(npc);

        //Levelled NPC
        var leveledNpc = npcTemplate.TryResolve<ILeveledNpcGetter>(_formLinkCache);
        if (leveledNpc is { Entries: {} })
        {
            return leveledNpc.Entries
                .Select(entry => entry.Data?.Reference).NotNull()
                .SelectMany(GetGenders).ToHashSet();
        }

        return new HashSet<bool>();
    }
    #endregion

    #region Race Parser
    private HashSet<FormKey> GetRaces(INpcGetter npc)
    {
        if ((npc.Configuration.TemplateFlags & NpcConfiguration.TemplateFlag.Traits) == 0 && !npc.Race.IsNull)
        {
            return new HashSet<FormKey> { npc.Race.FormKey };
        }

        return npc.Template.IsNull ? new HashSet<FormKey>() : GetRaces(npc.Template);

    }

    private HashSet<FormKey> GetRaces(IFormLinkGetter<INpcSpawnGetter> npcTemplate)
    {
        if (npcTemplate.IsNull) return new HashSet<FormKey>();

        //NPC
        var npc = npcTemplate.TryResolve<INpcGetter>(_formLinkCache);
        if (npc != null) return GetRaces(npc);

        //Levelled NPC
        var leveledNpc = npcTemplate.TryResolve<ILeveledNpcGetter>(_formLinkCache);
        if (leveledNpc is { Entries: {} })
        {
            return leveledNpc.Entries
                .Select(entry => entry.Data?.Reference).NotNull()
                .SelectMany(GetRaces).ToHashSet();
        }

        return new HashSet<FormKey>();
    }
    #endregion
}
