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

public class PluginListingsParser : IPluginListingsParser
{
    private readonly ILoadOrderListingParser _listingParser;

    public PluginListingsParser(
        ILoadOrderListingParser listingParser)
    {
        _listingParser = listingParser;
    }
        
    /// <inheritdoc />
    public IEnumerable<ILoadOrderListingGetter> Parse(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var str = streamReader.ReadLine().AsSpan();
            var commentIndex = str.IndexOf('#');
            if (commentIndex != -1)
            {
                str = str.Slice(0, commentIndex);
            }
            if (MemoryExtensions.IsWhiteSpace(str) || str.Length == 0) continue;
            yield return _listingParser.FromString(str);
        }
    }
}