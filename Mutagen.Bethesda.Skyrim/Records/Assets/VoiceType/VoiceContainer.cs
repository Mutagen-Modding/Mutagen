using System.Text;
using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

public class VoiceContainer : ICloneable, IEquatable<VoiceContainer>
{
    /// <summary>
    /// Voice type names mapped to form keys of npcs using that voice type 
    /// </summary>
    private readonly Dictionary<string, HashSet<FormKey>> _voices = new();
    public bool IsDefault { get; private set; }

    #region Constructors
    public VoiceContainer(bool isDefault = false)
    {
        IsDefault = isDefault;
    }

    public VoiceContainer(FormKey npc, IEnumerable<string> voiceTypes)
    {
        foreach (var voiceType in voiceTypes)
        {
            _voices.Add(voiceType, new HashSet<FormKey> { npc });
        }
    }

    public VoiceContainer(Dictionary<FormKey, IEnumerable<string>> npcVoices)
    {
        foreach (var (npc, voiceTypes) in npcVoices)
        {
            foreach (var voiceType in voiceTypes)
            {
                if (_voices.TryGetValue(voiceType, out var npcs))
                {
                    npcs.Add(npc);
                } else
                {
                    _voices.Add(voiceType, new HashSet<FormKey> { npc });
                }
            }
        }
    }

    public VoiceContainer(Dictionary<FormKey, HashSet<string>> npcVoices)
    {
        foreach (var (npc, voiceTypes) in npcVoices)
        {
            foreach (var voiceType in voiceTypes)
            {
                if (_voices.TryGetValue(voiceType, out var npcs))
                {
                    npcs.Add(npc);
                } else
                {
                    _voices.Add(voiceType, new HashSet<FormKey> { npc });
                }
            }
        }
    }

    public VoiceContainer(string voiceType)
    {
        _voices.Add(voiceType, new HashSet<FormKey>());
    }

    public VoiceContainer(IEnumerable<string> voiceTypes)
    {
        foreach (var voiceType in voiceTypes)
        {
            _voices.Add(voiceType, new HashSet<FormKey>());
        }
    }
    #endregion

    #region ExplicitOperators
    public void InvertVoiceTypes(HashSet<string> voiceTypesToKeep)
    {
        var voiceTypes = new List<string>(_voices.Keys);

        foreach (var voiceType in voiceTypes.Where(voiceType => !voiceTypesToKeep.Contains(voiceType)))
        {
            _voices.Remove(voiceType);
        }
    }
    #endregion

    #region BinaryOperators
    public void IntersectWith(VoiceContainer other)
    {
        if (IsDefault && other.IsDefault) return;

        var removeVoiceTypes = new HashSet<string>();

        foreach (var (voiceType, npcs) in _voices)
        {
            if (other._voices.TryGetValue(voiceType, out var otherNpcs))
            {
                if (npcs.Any())
                {
                    //We don't have all NPCs of this voice type
                    if (otherNpcs.Any())
                    {
                        //Only intersect if other doesn't have all NPCs, otherwise it stays the same
                        npcs.IntersectWith(otherNpcs);
                    }
                } else
                {
                    //We have all NPCs of this voice type => limit with other voice type
                    foreach (var otherNpc in otherNpcs) npcs.Add(otherNpc);
                }
            } else
            {
                //They don't have this voice type => remove ours
                removeVoiceTypes.Add(voiceType);
            }
        }

        foreach (var removeVoiceType in removeVoiceTypes)
        {
            _voices.Remove(removeVoiceType);
        }

        IsDefault = false;
    }

    public void Insert(VoiceContainer other)
    {
        if (IsDefault && other.IsDefault) return;

        foreach (var (voiceType, npcs) in other._voices)
        {
            if (_voices.TryGetValue(voiceType, out var otherNpcs))
            {
                //Don't add anything when we have all NPCs of this voice type
                if (!otherNpcs.Any()) continue;

                if (npcs.Any())
                {
                    //Insert as usual
                    foreach (var npc in npcs)
                    {
                        otherNpcs.Add(npc);
                    }
                } else
                {
                    //We have all NPCs of this voice type
                    otherNpcs.Clear();
                }
            } else
            {
                _voices.Add(voiceType, new HashSet<FormKey>(npcs));
            }
        }

        IsDefault = false;
    }

    /// <summary>
    /// Only possible for non-default voice containers sets
    /// </summary>
    /// <param name="other"></param>
    public void Remove(VoiceContainer other)
    {
        if (other.IsEmpty()) return;

        var removeVoiceTypes = new HashSet<string>();

        foreach (var (voiceType, npcs) in _voices)
        {
            if (other._voices.TryGetValue(voiceType, out var otherNpcs))
            {
                if (otherNpcs.Count == 0)
                {
                    //Other covers whole voice type => remove it
                    removeVoiceTypes.Add(voiceType);
                } else
                {
                    //Remove all npcs
                    foreach (var otherNpc in otherNpcs)
                    {
                        npcs.Remove(otherNpc);
                    }

                    //If all npcs are gone, remove the voice type
                    if (!npcs.Any())
                    {
                        removeVoiceTypes.Add(voiceType);
                    }
                }
            }
        }

        foreach (var removeVoiceType in removeVoiceTypes)
        {
            _voices.Remove(removeVoiceType);
        }

        IsDefault = false;
    }

    public void Merge(VoiceContainer newVoices)
    {
        if (newVoices.IsDefault) return;

        if (IsDefault)
        {
            Insert(newVoices);
        } else
        {
            IntersectWith(newVoices);
        }
    }
    #endregion

    public HashSet<string> GetVoiceTypes(HashSet<string> defaultVoiceTypes)
    {
        return IsDefault ? defaultVoiceTypes : _voices.Keys.ToHashSet();
    }

    public bool IsEmpty()
    {
        return _voices.Count == 0;
    }

    public object Clone()
    {
        var clone = new VoiceContainer
        {
            IsDefault = IsDefault
        };

        foreach (var (voice, npcs) in _voices)
        {
            clone._voices.Add(voice, new HashSet<FormKey>(npcs));
        }

        return clone;
    }

    public bool Equals(VoiceContainer? other) => other != null && IsDefault == other.IsDefault && _voices.Count == other._voices.Count && _voices.Keys.All(voiceType => other._voices.ContainsKey(voiceType));

    public override string ToString()
    {
        var voices = _voices.Keys.ToList();
        voices.Sort();

        StringBuilder sb = new();
        sb.Append($"IsDefault: {IsDefault}: ");
        foreach (var voiceType in voices)
        {
            sb.Append(voiceType);
            sb.Append(", ");
        }

        return sb.ToString();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VoiceContainer);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_voices);
    }
}
