using Noggog;

namespace Mutagen.Bethesda.Plugins.Order;

internal static class OrderUtility
{
    public static string GetListingFilename(ModKey modKey, string ghostSuffix)
    {
        return $"{modKey.FileName}{(ghostSuffix.IsNullOrWhitespace() ? null: ".")}{ghostSuffix}";
    }
}