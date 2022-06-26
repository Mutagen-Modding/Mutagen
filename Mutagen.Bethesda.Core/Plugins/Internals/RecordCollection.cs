using DynamicData;
using System.Collections;

namespace Mutagen.Bethesda.Plugins.Internals;

public interface IReadOnlyRecordCollection : IReadOnlyCollection<RecordType>
{
    bool Contains(RecordType type);
    int IndexOf(RecordType type);
}

public interface IRecordCollection : IReadOnlyRecordCollection
{
    bool Add(RecordType recordType);
}

internal class ReadOnlyRecordCollection : IReadOnlyRecordCollection
{
    private readonly IReadOnlyList<RecordType> _ordered;
    private readonly IReadOnlySet<RecordType> _set;

    public int Count => _ordered.Count;

    public ReadOnlyRecordCollection(IReadOnlyList<RecordType> ordered)
    {
        _ordered = ordered;
        _set = ordered.ToHashSet();
    }

    public bool Contains(RecordType type) => _set.Contains(type);

    public int IndexOf(RecordType type) => _ordered.IndexOf(type);

    public IEnumerator<RecordType> GetEnumerator()
    {
        return _ordered.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class RecordCollection : IRecordCollection
{
    private readonly List<RecordType> _ordered;
    private readonly HashSet<RecordType> _set;

    public int Count => _ordered.Count;

    public RecordCollection()
    {
        _ordered = new List<RecordType>();
        _set = new HashSet<RecordType>();
    }

    public bool Contains(RecordType type) => _set.Contains(type);

    public int IndexOf(RecordType type) => _ordered.IndexOf(type);

    public static IReadOnlyRecordCollection Factory(params RecordType[] types)
    {
        if (types.Length == 0)
        {
            return EmptyRecordCollection.Instance;
        }
        else if (types.Length == 1)
        {
            return new SingleRecordCollection(types[0]);
        }
        else
        {
            return new ReadOnlyRecordCollection(types.ToList());
        }
    }

    public static IReadOnlyRecordCollection FactoryFromOneAndArray(RecordType type, params RecordType[] types)
    {
        if (types.Length == 0)
        {
            return new SingleRecordCollection(type);
        }
        else
        {
            var list = new List<RecordType>();
            list.Add(type);
            list.AddRange(types);
            return new ReadOnlyRecordCollection(list);
        }
    }

    public IEnumerator<RecordType> GetEnumerator()
    {
        return _ordered.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Add(RecordType recordType)
    {
        if (!_set.Add(recordType)) return false;
        _ordered.Add(recordType);
        return true;
    }

    public void Add(IEnumerable<RecordType> recordTypes)
    {
        foreach (var item in recordTypes)
        {
            Add(item);
        }
    }

    public void Clear()
    {
        _set.Clear();
        _ordered.Clear();
    }
}

public class SingleRecordCollection : IReadOnlyRecordCollection
{
    private readonly RecordType _type;

    public SingleRecordCollection(RecordType type)
    {
        _type = type;
    }

    public int Count => 1;

    public bool Contains(RecordType type) => _type == type;

    public IEnumerator<RecordType> GetEnumerator()
    {
        yield return _type;
    }

    public int IndexOf(RecordType type) => _type == type ? 0 : -1;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class EmptyRecordCollection : IReadOnlyRecordCollection
{
    public static readonly EmptyRecordCollection Instance = new();

    public int Count => 0;

    public bool Contains(RecordType type) => false;

    public IEnumerator<RecordType> GetEnumerator() => Enumerable.Empty<RecordType>().GetEnumerator();

    public int IndexOf(RecordType type) => -1;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
