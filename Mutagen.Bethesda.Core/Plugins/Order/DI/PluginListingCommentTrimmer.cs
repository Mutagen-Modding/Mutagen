namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingCommentTrimmer
{
    ReadOnlySpan<char> Trim(ReadOnlySpan<char> str);
}

public class PluginListingCommentTrimmer : IPluginListingCommentTrimmer
{
    private int LastIndexOfSuffix(ReadOnlySpan<char> str)
    {
        var index = str.LastIndexOf(".esp", StringComparison.OrdinalIgnoreCase);
        var highest = index;
        index = str.LastIndexOf(".esm", StringComparison.OrdinalIgnoreCase);
        highest = Math.Max(highest, index);
        index = str.LastIndexOf(".esl", StringComparison.OrdinalIgnoreCase);
        highest = Math.Max(highest, index);
        if (highest == -1) return -1;
        return highest + 4;
    }
    
    public ReadOnlySpan<char> Trim(ReadOnlySpan<char> str)
    {
        int startIndex = 0;
        var suffixEnding = LastIndexOfSuffix(str);
        if (suffixEnding != -1)
        {
            startIndex = suffixEnding;
        }
        var commentIndex = str.Slice(startIndex).IndexOf('#');
        if (commentIndex != -1)
        {
            return str.Slice(0, commentIndex + startIndex);
        }

        return str;
    }
}