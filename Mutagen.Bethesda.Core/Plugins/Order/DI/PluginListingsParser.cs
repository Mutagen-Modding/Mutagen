namespace Mutagen.Bethesda.Plugins.Order.DI;

/// <summary>
/// Converts a stream into raw enumerable of ModListings
/// </summary>
public interface IPluginListingsParser
{
    /// <summary>
    /// Parses a stream to retrieve all ModKeys in expected plugin file format
    /// </summary>
    /// <param name="stream">Stream to read from</param>
    /// <returns>List of ModKeys representing a load order</returns>
    /// <exception cref="ArgumentException">Line in plugin stream is unexpected</exception>
    IEnumerable<ILoadOrderListingGetter> Parse(Stream stream);
}

public sealed class PluginListingsParser : IPluginListingsParser
{
    private readonly IPluginListingCommentTrimmer _commentTrimmer;
    private readonly ILoadOrderListingParser _listingParser;

    public PluginListingsParser(
        IPluginListingCommentTrimmer commentTrimmer,
        ILoadOrderListingParser listingParser)
    {
        _commentTrimmer = commentTrimmer;
        _listingParser = listingParser;
    }
        
    /// <inheritdoc />
    public IEnumerable<ILoadOrderListingGetter> Parse(Stream stream)
    {
        uint currentLine = 0;
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            currentLine++;
            var str = streamReader.ReadLine().AsSpan();
            str = _commentTrimmer.Trim(str);
            if (MemoryExtensions.IsWhiteSpace(str) || str.Length == 0) continue;

            if (_listingParser.TryFromString(str, out var listing))
            {
                yield return listing;
            }
            else
            {
                throw new InvalidDataException($"Load order file had malformed entry at line {currentLine}: \"{str}\"");
            }
        }
    }
}