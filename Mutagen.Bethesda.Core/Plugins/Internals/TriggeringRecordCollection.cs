using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;

namespace Mutagen.Bethesda.Plugins.Internals;

public interface ITriggeringRecordCollection
{
    bool Contains(RecordType type);
    int IndexOf(RecordType type);
}

public class TriggeringRecordCollection : ITriggeringRecordCollection
{
    private readonly IReadOnlyList<RecordType> _ordered;
    private readonly IReadOnlySet<RecordType> _set;

    private TriggeringRecordCollection(params RecordType[] types)
    {
        _ordered = types;
        _set = types.ToHashSet();
    }

    public bool Contains(RecordType type) => _set.Contains(type);

    public int IndexOf(RecordType type) => _ordered.IndexOf(type);

    public static ITriggeringRecordCollection Factory(params RecordType[] types)
    {
        if (types.Length == 0)
        {
            return EmptyTriggeringRecordCollection.Instance;
        }
        else if (types.Length == 1)
        {
            return new SingleTriggeringRecordCollection(types[0]);
        }
        else
        {
            return new TriggeringRecordCollection(types);
        }
    }
}

public class SingleTriggeringRecordCollection : ITriggeringRecordCollection
{
    private readonly RecordType _type;

    public SingleTriggeringRecordCollection(RecordType type)
    {
        _type = type;
    }

    public bool Contains(RecordType type) => _type == type;

    public int IndexOf(RecordType type) => _type == type ? 0 : -1;
}

public class EmptyTriggeringRecordCollection : ITriggeringRecordCollection
{
    public static readonly EmptyTriggeringRecordCollection Instance = new();

    public bool Contains(RecordType type) => false;

    public int IndexOf(RecordType type) => -1;
}
