using Noggog;

namespace Mutagen.Bethesda.Plugins.Order;

internal sealed class LoadOrderTimestampComparer : IComparer<(ModKey ModKey, DateTime Write)>
{
    private readonly IReadOnlyList<ModKey> _modKeys;

    public LoadOrderTimestampComparer(IReadOnlyList<ModKey> modKeys)
    {
        _modKeys = modKeys;
    }

    public int Compare((ModKey ModKey, DateTime Write) x, (ModKey ModKey, DateTime Write) y)
    {
        int compare = x.Write.CompareTo(y.Write);
        if (compare != 0) return compare;
        var xIndex = _modKeys.IndexOf(x.ModKey);
        var yIndex = _modKeys.IndexOf(y.ModKey);
        return xIndex.CompareTo(yIndex);
    }
}