namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

public static class VoiceContainerExtension
{
    // Perform a set union on zero or more containers. May mutate input containers
    public static VoiceContainer? MergeInsert(this List<VoiceContainer> voiceContainers, bool isDefaultIfEmpty)
    {
        switch (voiceContainers)
        {
            case []: return isDefaultIfEmpty ? null : VoiceContainer.Empty;
            case [var voiceContainer]: return voiceContainer;
            default:
                var first = voiceContainers.First();
                foreach (var other in voiceContainers.Skip(1))
                {
                    first.Insert(other);
                }
                return first;
        }
    }

    // Perform a set intersection on zero or more containers. May mutate input containers
    public static VoiceContainer MergeIntersect(this List<VoiceContainer> voiceContainers)
    {
        switch (voiceContainers) {
            case []: return new VoiceContainer();
            case [var voiceContainer]: return voiceContainer;
            default:
                var first = voiceContainers.First();
                foreach (var voiceContainer in voiceContainers.Skip(1))
                {
                    first.IntersectWith(voiceContainer);
                }
                return first;
        }
    }
}
