using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
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


    //Global variables
    private IQuestGetter _currentQuest = new Quest(FormKey.Null, SkyrimRelease.SkyrimSE);
    private ModKey _currentMod = ModKey.Null;

    public void Prep(IAssetLinkCache linkCache)
    {
        _formLinkCache = linkCache.FormLinkCache;
        _formLinkCache.Warmup<INpcGetter>();

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
                    if (_factionNPCs.ContainsKey(factionKey))
                    {
                        _factionNPCs[factionKey].Add(npc.FormKey);
                    } else
                    {
                        _factionNPCs.Add(factionKey, new HashSet<FormKey> { npc.FormKey });
                    }
                }

                foreach (var classKey in GetClasses(npc))
                {
                    if (_classNPCs.ContainsKey(classKey))
                    {
                        _classNPCs[classKey].Add(npc.FormKey);
                    } else
                    {
                        _classNPCs.Add(classKey, new HashSet<FormKey> { npc.FormKey });
                    }
                }

                foreach (var gender in GetGenders(npc))
                {
                    if (_genderNPCs.ContainsKey(gender))
                    {
                        _genderNPCs[gender].Add(npc.FormKey);
                    } else
                    {
                        _genderNPCs.Add(gender, new HashSet<FormKey> { npc.FormKey });
                    }
                }

                foreach (var raceKey in GetRaces(npc))
                {
                    if (_raceNPCs.ContainsKey(raceKey))
                    {
                        _raceNPCs[raceKey].Add(npc.FormKey);
                    } else
                    {
                        _raceNPCs.Add(raceKey, new HashSet<FormKey> { npc.FormKey });
                    }
                }
            }

            foreach (var response in mod.EnumerateMajorRecords<IDialogResponsesGetter>())
            {
                if (!response.ResponseData.IsNull)
                {
                    if (_sharedInfosCache.ContainsKey(response.ResponseData.FormKey))
                    {
                        _sharedInfosCache[response.ResponseData.FormKey].Add(response.FormKey);
                    } else
                    {
                        _sharedInfosCache.Add(response.ResponseData.FormKey, new HashSet<FormKey> { response.FormKey });
                    }
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
            .Where(r => _raceNPCs.ContainsKey(r))
            .SelectMany(r => _raceNPCs[r])
            .ToHashSet();

        //Add master voice types
        var defaultVoicesCopy = new Dictionary<ModKey, HashSet<string>>(_defaultVoiceTypes);
        foreach (var mod in _formLinkCache.PriorityOrder)
        {
            foreach (var master in mod.MasterReferences)
            {
                if (!defaultVoicesCopy.ContainsKey(master.Master)) continue;

                foreach (var voiceType in defaultVoicesCopy[master.Master])
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

        //Assign global variables
        _currentQuest = quest;
        _currentMod = topic.FormKey.ModKey;

        //Get quest voices
        VoiceContainer questVoices;
        if (_questCache.ContainsKey(quest.FormKey))
        {
            questVoices = _questCache[quest.FormKey];
        } else
        {
            questVoices = GetVoices(quest);
            _questCache.Add(quest.FormKey, questVoices);
        }

        //If this is a shared info and it's not used, return no voices
        if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && !_sharedInfosCache.ContainsKey(response.FormKey)) return new VoiceContainer();

        //If we have selected default voices, make sure the quest voices are being checked first - they might not be part of default voices
        var voices = GetVoices(topic, response);
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

        if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && _sharedInfosCache.ContainsKey(response.FormKey))
        {
            var userConditions = new VoiceContainer(true);
            foreach (var responseKey in _sharedInfosCache[response.FormKey])
            {
                var responseContext = _formLinkCache.ResolveSimpleContext<IDialogResponsesGetter>(responseKey);
                if (responseContext is not { Parent.Record: {} }) continue;

                var currentTopic = (IDialogTopicGetter) responseContext.Parent.Record;
                var currentQuest = currentTopic.Quest.TryResolve(_formLinkCache);
                if (currentQuest == null) continue;

                _currentQuest = currentQuest;
                userConditions.Insert(GetVoices(responseContext.Record.Conditions));
            }

            voices.Merge(userConditions);
        }

        return voices;
    }

    public IEnumerable<string> GetVoiceTypePaths(IDialogTopicGetter topic)
    {
        var quest = topic.Quest.TryResolve(_formLinkCache);
        if (quest == null) yield break;

        //Assign global variables
        _currentQuest = quest;
        _currentMod = topic.FormKey.ModKey;

        //Get quest voices
        VoiceContainer questVoices;
        if (_questCache.ContainsKey(quest.FormKey))
        {
            questVoices = _questCache[quest.FormKey];
        } else
        {
            questVoices = GetVoices(quest);
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
            var voices = GetVoices(topic, responses);

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

            if (topic.Subtype == DialogTopic.SubtypeEnum.SharedInfo && _sharedInfosCache.ContainsKey(responses.FormKey))
            {
                var userConditions = new VoiceContainer(true);
                foreach (var responseKey in _sharedInfosCache[responses.FormKey])
                {
                    var responseContext = _formLinkCache.ResolveSimpleContext<IDialogResponsesGetter>(responseKey);
                    if (responseContext is not { Parent.Record: {} }) continue;

                    var currentTopic = (IDialogTopicGetter) responseContext.Parent.Record;
                    var currentQuest = currentTopic.Quest.TryResolve(_formLinkCache);
                    if (currentQuest == null) continue;

                    _currentQuest = currentQuest;
                    userConditions.Insert(GetVoices(responseContext.Record.Conditions));
                }

                voices.Merge(userConditions);
            }


            var responseFormID = responses.FormKey.ID.ToString("X8");
            if (voices.IsEmpty()) Console.WriteLine($"WARNING: {responseFormID} has no valid voice types {topic.EditorID}");

            foreach (var response in responses.Responses)
            {
                var responseNumber = response.ResponseNumber;
                foreach (var voiceType in voices.GetVoiceTypes(_defaultVoiceTypes[_currentMod]))
                {
                    yield return Path.Combine
                    (
                        "Data",
                        "Sound",
                        "Voice",
                        _currentMod.FileName,
                        voiceType,
                        $"{questString}_{topicString}_{responseFormID}_{responseNumber}.fuz"
                    );
                }
            }
        }
    }

    private VoiceContainer GetVoices(IDialogTopicGetter topic, IDialogResponsesGetter response)
    {
        var voices = new VoiceContainer(true);

        //Use speaker only if we have one
        if (!response.Speaker.IsNull) return GetVoices(response.Speaker.FormKey);

        //Check scene
        if (topic.Category == DialogTopic.CategoryEnum.Scene && _dialogueSceneAliasIndex.ContainsKey(topic.FormKey))
        {
            voices.Merge(GetVoices(_currentQuest, _dialogueSceneAliasIndex[topic.FormKey]));
        }

        //Search conditions
        if (response.Conditions.Any()) voices.Merge(GetVoices(response.Conditions));

        return voices;
    }

    private VoiceContainer GetVoices(IEnumerable<IConditionGetter> conditions)
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
                var voices = GetVoiceTypesOrBlock(currentConditions);
                if (!voices.IsDefault) voiceTypesOrBlock.Add(voices);

                currentConditions.Clear();
            }
        }

        //Merge OR blocks
        return voiceTypesOrBlock.Any() ? voiceTypesOrBlock.MergeAll() : new VoiceContainer(true);
    }

    private VoiceContainer GetVoiceTypesOrBlock(IEnumerable<IConditionGetter> conditions)
    {
        var voices = new VoiceContainer(true);

        foreach (var condition in conditions)
        {
            var conditionVoices = GetVoices(condition);
            if (conditionVoices.IsDefault) continue;

            voices.Insert(conditionVoices);
        }

        return voices;
    }

    private VoiceContainer GetVoices(IConditionGetter condition)
    {
        var voices = new VoiceContainer();

        if (condition.Data.RunOnType != Condition.RunOnType.Subject || condition.Data is not IFunctionConditionDataGetter functionConditionData) return new VoiceContainer(true);

        switch (functionConditionData.Function)
        {
            case Condition.Function.GetIsID:
                if (!functionConditionData.ParameterOneRecord.FormKey.IsNull)
                {
                    voices = new VoiceContainer(functionConditionData.ParameterOneRecord.FormKey, _speakerVoices[functionConditionData.ParameterOneRecord.FormKey]);
                }

                break;
            case Condition.Function.GetIsVoiceType:
                if (_formLinkCache.TryResolveIdentifier<IVoiceTypeGetter>(functionConditionData.ParameterOneRecord.FormKey, out var voiceTypeEditorId))
                {
                    //One voice type
                    voices = new VoiceContainer(voiceTypeEditorId!);
                } else
                {
                    //Multiple voice types
                    var formList = functionConditionData.ParameterOneRecord.TryResolve<IFormListGetter>(_formLinkCache);
                    if (formList != null)
                    {
                        voices = new VoiceContainer(formList.Items.SelectWhere(link =>
                        {
                            _formLinkCache.TryResolveIdentifier<IVoiceTypeGetter>(link.FormKey, out var linkVoiceTypeEditorId);
                            return linkVoiceTypeEditorId == null ? TryGet<string>.Failure : TryGet<string>.Succeed(linkVoiceTypeEditorId);
                        }).ToHashSet());
                    }
                }

                break;
            case Condition.Function.GetIsAliasRef:
                voices = GetVoices(_currentQuest, functionConditionData.ParameterOneNumber);

                break;
            case Condition.Function.GetInFaction:
                if (_factionNPCs.ContainsKey(functionConditionData.ParameterOneRecord.FormKey))
                {
                    voices = new VoiceContainer(_factionNPCs[functionConditionData.ParameterOneRecord.FormKey].ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case Condition.Function.GetIsClass:
                if (_classNPCs.ContainsKey(functionConditionData.ParameterOneRecord.FormKey))
                {
                    voices = new VoiceContainer(_classNPCs[functionConditionData.ParameterOneRecord.FormKey].ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case Condition.Function.GetIsRace:
                if (_raceNPCs.ContainsKey(functionConditionData.ParameterOneRecord.FormKey))
                {
                    voices = new VoiceContainer(_raceNPCs[functionConditionData.ParameterOneRecord.FormKey].ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case Condition.Function.GetIsSex:
                var isFemale = functionConditionData.ParameterOneNumber == 1;
                if (_genderNPCs.ContainsKey(isFemale))
                {
                    voices = new VoiceContainer(_genderNPCs[isFemale].ToDictionary(npc => npc, GetVoiceTypes));
                }

                break;
            case Condition.Function.IsInList:
                if (!functionConditionData.ParameterOneRecord.FormKey.IsNull)
                {
                    var formList = functionConditionData.ParameterOneRecord.FormKey.ToLinkGetter<IFormListGetter>().TryResolve(_formLinkCache);
                    //Only look at speakers in the form list
                    if (formList != null) voices = formList.Items.Select(link => GetVoices(link.FormKey)).MergeInsert();
                }

                break;
            case Condition.Function.IsChild:
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

            voices = Invert(voices, functionConditionData.Function == Condition.Function.GetIsVoiceType);
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

    private VoiceContainer GetVoices(IQuestGetter quest, int aliasIndex)
    {
        var alias = quest.Aliases.FirstOrDefault(a => a.ID == aliasIndex);
        return alias == null ? new VoiceContainer(true) : GetVoices(alias);
    }

    private VoiceContainer GetVoices(IQuestAliasGetter alias)
    {
        //External Alias
        if (alias.External != null)
        {
            var quest = alias.External.Quest.TryResolve(_formLinkCache);
            var aliasIndex = alias.External.AliasID;
            if (quest != null && aliasIndex != null)
            {
                return GetVoices(quest, aliasIndex.Value);
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
            var voices = GetVoices(alias.Conditions);
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
            var locationAlias = _currentQuest.Aliases.FirstOrDefault(a => a.ID == alias.Location.AliasID.Value);
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

    private VoiceContainer GetVoices(IQuestGetter quest) => GetVoices(quest.DialogConditions);

    private VoiceContainer GetDefaultVoices(ModKey mod)
    {
        if (_defaultSpeakerVoices.ContainsKey(mod)) return _defaultSpeakerVoices[mod];

        var vc = new VoiceContainer(_speakerVoices);
        vc.InvertVoiceTypes(_defaultVoiceTypes[mod]);
        _defaultSpeakerVoices.Add(mod, vc);
        return vc;
    }

    private VoiceContainer Invert(VoiceContainer voiceContainer, bool invertDefaultVoices)
    {
        VoiceContainer baseVoices;
        if (invertDefaultVoices)
        {
            baseVoices = (VoiceContainer) GetDefaultVoices(_currentMod).Clone();
        } else
        {
            baseVoices = new VoiceContainer(_speakerVoices);
        }

        baseVoices.Remove(voiceContainer);
        return baseVoices;
    }

    private HashSet<string> GetVoiceTypes(FormKey speaker)
    {
        return _speakerVoices.ContainsKey(speaker) ? _speakerVoices[speaker] : new HashSet<string>();
    }

    #region Voice Parser
    private HashSet<string> GetVoiceTypes(INpcGetter npc)
    {
        if (_speakerVoices.ContainsKey(npc.FormKey)) return _speakerVoices[npc.FormKey];

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
        if (_speakerVoices.ContainsKey(talkingActivator.FormKey)) return _speakerVoices[talkingActivator.FormKey];

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
