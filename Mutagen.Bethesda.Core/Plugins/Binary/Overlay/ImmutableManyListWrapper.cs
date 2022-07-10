using System.Collections;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

public sealed class ImmutableManyListWrapper<TValue> : IReadOnlyList<TValue>
{
    private int _count;
    private readonly List<IReadOnlyList<TValue>> _listOfLists = new();

    public void AddList(IReadOnlyList<TValue> val)
    {
        _count += val.Count;
        _listOfLists.Add(val);
    }
    
    public IEnumerator<TValue> GetEnumerator()
    {
        foreach (var list in _listOfLists)
        {
            foreach (var item in list)
            {
                yield return item;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _count;

    public TValue this[int index]
    {
        get
        {
            if (_listOfLists.Count == 0) throw new IndexOutOfRangeException();
            foreach (var list in _listOfLists)
            {
                if (index < list.Count)
                {
                    return list[index];
                }

                index -= list.Count;
            }

            throw new IndexOutOfRangeException();
        }
    }
}