namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

public static class VoiceContainerExtension
{
    public static VoiceContainer MergeInsert(this IEnumerable<VoiceContainer> voiceContainers, bool isDefaultIfEmpty)
    {
        if (!voiceContainers.Any()) return new VoiceContainer(isDefaultIfEmpty);

        var output = new VoiceContainer();
        foreach (var voiceContainer in voiceContainers)
        {
            output.Insert(voiceContainer);
        }

        return output;
    }

    public static VoiceContainer MergeIntersect(this IEnumerable<VoiceContainer> voiceContainers)
    {
        var voiceContainerList = voiceContainers.ToList();

        switch (voiceContainerList) {
            case []: return new VoiceContainer();
            case [var voiceContainer]: return voiceContainer;
            default:
                var output = new VoiceContainer(true);
                foreach (var voiceContainer in voiceContainerList)
                {
                    output.IntersectWith(voiceContainer);
                }

                return output;
        }
    }
}
