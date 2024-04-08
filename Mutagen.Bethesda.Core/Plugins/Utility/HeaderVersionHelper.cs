namespace Mutagen.Bethesda.Plugins.Utility;

internal static class HeaderVersionHelper
{
    public static uint GetNextFormId(
        GameRelease release,
        HashSet<GameRelease>? allowedReleases,
        float headerVersion,
        float? useLowerRangesVersion,
        bool? forceUseLowerFormIDRanges,
        uint higherFormIdRange)
    {
        if (forceUseLowerFormIDRanges.HasValue)
        {
            if (forceUseLowerFormIDRanges.Value)
            {
                return 1;
            }
            else
            {
                return higherFormIdRange;
            }
        }
        if (allowedReleases == null
            || allowedReleases.Contains(release))
        {
            if (useLowerRangesVersion.HasValue
                && headerVersion >= useLowerRangesVersion.Value)
            {
                return 1;
            }
        }
        return higherFormIdRange;
    }
}
