namespace Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;

public static class VoiceContainerExtension
{
    public static VoiceContainer MergeInsert(this IEnumerable<VoiceContainer> voiceContainers)
    {
        if (!voiceContainers.Any()) return new VoiceContainer();

        var output = new VoiceContainer(true);
        foreach (var voiceContainer in voiceContainers)
        {
            output.Insert(voiceContainer);
        }

        return output;
    }

    public static VoiceContainer MergeAll(this IEnumerable<VoiceContainer> voiceContainers)
    {
        var voiceContainerList = voiceContainers.ToList();

        if (!voiceContainerList.Any()) return new VoiceContainer();
        if (voiceContainerList.Count == 1) return voiceContainerList[0];

        var output = new VoiceContainer(true);
        foreach (var voiceContainer in voiceContainerList)
        {
            output.Merge(voiceContainer);
        }

        return output;
    }
}
