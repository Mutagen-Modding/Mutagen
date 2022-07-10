using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal;

internal sealed class DepthCache<K, T>
    where K : notnull
{
    private readonly Dictionary<K, T> _dictionary = new Dictionary<K, T>();
    public HashSet<ModKey> PassedMods = new HashSet<ModKey>();
    public int Depth;
    public bool Done;

    public IReadOnlyCollection<T> Values => _dictionary.Values;

    public bool TryGetValue(K key, [MaybeNullWhen(false)] out T value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public void AddIfMissing(K key, T item)
    {
        if (!_dictionary.ContainsKey(key))
        {
            _dictionary[key] = item;
        }
    }

    public void Add(K key, T item)
    {
        _dictionary.Add(key, item);
    }

    public void Set(K key, T item)
    {
        _dictionary[key] = item;
    }
}