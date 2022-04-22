using System.Collections.Generic;
using System.Linq;
using DynamicData;

namespace Mutagen.Bethesda.Plugins.Internals;

public interface IRecordCollection
{
    bool Contains(RecordType type);
    int IndexOf(RecordType type);
}

public class RecordCollection : IRecordCollection
{
    private readonly IReadOnlyList<RecordType> _ordered;
    private readonly IReadOnlySet<RecordType> _set;

    private RecordCollection(params RecordType[] types)
    {
        _ordered = types;
        _set = types.ToHashSet();
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
            return new RecordCollection(types);
        }
    }
}

public class SingleRecordCollection : IRecordCollection
{
    private readonly RecordType _type;

    public SingleRecordCollection(RecordType type)
    {
        _type = type;
    }

    public bool Contains(RecordType type) => _type == type;

    public int IndexOf(RecordType type) => _type == type ? 0 : -1;
}

public class EmptyRecordCollection : IRecordCollection
{
    public static readonly EmptyRecordCollection Instance = new();

    public bool Contains(RecordType type) => false;

    public int IndexOf(RecordType type) => -1;
}
