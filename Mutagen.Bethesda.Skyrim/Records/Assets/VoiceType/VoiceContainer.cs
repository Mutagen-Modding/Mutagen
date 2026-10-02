using System.Text;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

public class VoiceContainer : ICloneable, IEquatable<VoiceContainer>
{
    public static VoiceContainer Empty => new VoiceContainer();

    /// <summary>
    /// Voice type names mapped to form keys of npcs or talking activators using that voice type 
    /// </summary>
    private Dictionary<FormKey, HashSet<FormKey>> _voices = new();
    public IReadOnlyDictionary<FormKey, HashSet<FormKey>> Voices => _voices;
    [Obsolete("Represent as null")]
    public bool IsDefault { get; private set; }

    #region Constructors
    public VoiceContainer(bool isDefault = false)
    {
        IsDefault = isDefault;
    }

    public VoiceContainer(FormKey npc, IEnumerable<FormKey> voiceTypes)
    {
        foreach (var voiceType in voiceTypes)
        {
            _voices.Add(voiceType, [npc]);
        }
    }

    public VoiceContainer(IEnumerable<FormKey> npcs, Dictionary<FormKey, HashSet<FormKey>> speakerVoices)
    {
        foreach (var npc in npcs)
        {
            if (!speakerVoices.TryGetValue(npc, out var voices))
                continue;
            foreach (var voice in voices)
            {
                _voices.GetOrAdd(voice).Add(npc);
            }
        }
    }

    public VoiceContainer(FormKey voiceType)
    {
        _voices.Add(voiceType, []);
    }

    public VoiceContainer(IEnumerable<FormKey> voiceTypes)
    {
        foreach (var voiceType in voiceTypes)
        {
            _voices.Add(voiceType, []);
        }
    }
    #endregion

    #region BinaryOperators
    public void Invert(Dictionary<FormKey, HashSet<FormKey>> voiceSpeakers)
    {
        // We had everything default -> become empty
        if (IsDefault)
        {
            IsDefault = false;
            return;
        }
        // We had nothing -> become default
        if (_voices.Count == 0)
        {
            IsDefault = true;
            return;
        }

        var originalVoices = _voices;
        _voices = [];
        foreach (var (voice, allSpeakers) in voiceSpeakers)
        {
            // Had none of these speakers, gain all of them
            if (!originalVoices.TryGetValue(voice, out var previousSpeakers))
            {
                _voices.Add(voice, []);
            }
            // Had all of these speakers, lose the whole voice
            else if (previousSpeakers.Count == 0 || previousSpeakers.Count >= allSpeakers.Count)
            {
                // Do nothing
            }
            // Invert set of speakers
            else
            {
                _voices.Add(voice, allSpeakers.Except(previousSpeakers).ToHashSet());
            }
        }
    }

    public void IntersectWith(VoiceContainer other)
    {
        // If the other is default, we can stay as we are
        if (other.IsDefault) return;

        // If we are default, but the other is not, we become non-default and take over all voices
        if (IsDefault)
        {
            IsDefault = false;
            foreach (var (voiceType, npcs) in other.Voices)
            {
                _voices.Add(voiceType, [..npcs]);
            }
            return;
        }

        // If both are non-default, we need to intersect the voice types and their NPCs
        var removeVoiceTypes = new List<FormKey>();

        foreach (var (voiceType, npcs) in _voices)
        {
            if (other._voices.TryGetValue(voiceType, out var otherNpcs))
            {
                if (npcs.Count > 0)
                {
                    // We don't have all NPCs of this voice type
                    // If other doesn't have all NPCs, intersect, otherwise it stays the same
                    if (otherNpcs.Count > 0)
                    {
                        npcs.IntersectWith(otherNpcs);
                        // If we lose all speakers here, lose the voice since an empty set is treated as having all speakers
                        if (npcs.Count == 0)
                        {
                            removeVoiceTypes.Add(voiceType);
                        }
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
    }

    public void Insert(VoiceContainer other)
    {
        if (IsDefault || other.IsDefault)
        {
            IsDefault = true;
            _voices.Clear();
            return;
        }

        foreach (var (voiceType, npcs) in other._voices)
        {
            if (_voices.TryGetValue(voiceType, out var otherNpcs))
            {
                //Don't add anything when we have all NPCs of this voice type
                if (otherNpcs.Count == 0) continue;

                if (npcs.Count > 0)
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
                _voices.Add(voiceType, [..npcs]);
            }
        }

        IsDefault = false;
    }

    #endregion

    #region Mutation
    public void AddSpeaker(FormKey speaker, HashSet<FormKey> voices)
    {
        foreach (var voice in voices)
        {
            _voices.GetOrAdd(voice).Add(speaker);
        }
    }
    // Add all NPCs with a voice type
    public void AddFullVoice(FormKey voice)
    {
        _voices[voice] = [];
    }
    #endregion

    public IEnumerable<FormKey> GetVoiceTypes(HashSet<FormKey> allVoices)
    {
        return IsDefault ? allVoices : _voices.Keys;
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
            clone._voices.Add(voice, [..npcs]);
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
