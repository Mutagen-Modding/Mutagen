using DynamicData;
using System.Collections;

namespace Mutagen.Bethesda.Plugins.Internals;

public interface IRecordCollection : IReadOnlyCollection<RecordType>
{
    bool Contains(RecordType type);
    int IndexOf(RecordType type);
}

public class RecordCollection : IRecordCollection
{
    private readonly IReadOnlyList<RecordType> _ordered;
    private readonly IReadOnlySet<RecordType> _set;

    public int Count => _ordered.Count;

    private RecordCollection(IReadOnlyList<RecordType> ordered)
    {
        _ordered = ordered;
        _set = ordered.ToHashSet();
    }

    public bool Contains(RecordType type) => _set.Contains(type);

    public int IndexOf(RecordType type) => _ordered.IndexOf(type);

    public static IRecordCollection Factory(params RecordType[] types)
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
            return new RecordCollection(types.ToList());
        }
    }

    public static IRecordCollection FactoryFromOneAndArray(RecordType type, params RecordType[] types)
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
            return new RecordCollection(list);
        }
    }

    public IEnumerator<RecordType> GetEnumerator()
    {
        return _ordered.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class SingleRecordCollection : IRecordCollection
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

public class EmptyRecordCollection : IRecordCollection
{
    public static readonly EmptyRecordCollection Instance = new();

    public int Count => 0;

    public bool Contains(RecordType type) => false;

    public IEnumerator<RecordType> GetEnumerator() => Enumerable.Empty<RecordType>().GetEnumerator();

    public int IndexOf(RecordType type) => -1;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
