using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Plugins.Implicit;

public class ImplicitModKeyCollection : IReadOnlyList<ModKey>
{
    private readonly IReadOnlyList<ModKey> _keys;
    private readonly HashSet<ModKey> _set;

    public ImplicitModKeyCollection(IEnumerable<ModKey> keys)
    {
        _keys = keys.ToList();
        _set = _keys.ToHashSet();
    }

    public IEnumerator<ModKey> GetEnumerator()
    {
        return _keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _keys).GetEnumerator();
    }

    public int Count => _keys.Count;

    public ModKey this[int index] => _keys[index];

    public bool Contains(ModKey key)
    {
        return _set.Contains(key);
    }
}