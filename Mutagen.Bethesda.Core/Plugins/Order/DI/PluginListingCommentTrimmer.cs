namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingCommentTrimmer
{
    ReadOnlySpan<char> Trim(ReadOnlySpan<char> str);
}

public class PluginListingCommentTrimmer : IPluginListingCommentTrimmer
{
    public ReadOnlySpan<char> Trim(ReadOnlySpan<char> str)
    {
        var commentIndex = str.IndexOf('#');
        if (commentIndex != -1)
        {
            return str.Slice(0, commentIndex);
        }

        return str;
    }
}