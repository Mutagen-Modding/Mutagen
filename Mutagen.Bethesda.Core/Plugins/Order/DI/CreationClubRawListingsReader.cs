namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ICreationClubRawListingsReader
{
    IEnumerable<ILoadOrderListingGetter> Read(Stream stream);
}

public sealed class CreationClubRawListingsReader : ICreationClubRawListingsReader
{
    public IEnumerable<ILoadOrderListingGetter> Read(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var str = streamReader.ReadLine().AsSpan();
            var modKey = ModKey.FromNameAndExtension(str);
            yield return new LoadOrderListing(modKey, enabled: true);
        }
    }
}