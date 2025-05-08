using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Meta;

public sealed record GroupCellConstants(int TopGroupType, int[] SubTypes);
public sealed record GroupWorldConstants(int TopGroupType, int[] CellGroupTypes, int[] CellSubGroupTypes);
public sealed record GroupTopicConstants(int TopGroupType);
public sealed record GroupQuestConstants(int TopGroupType);

public sealed record GroupNesting(int GroupType, params GroupNesting[] Underneath)
{
    public bool HasTopLevelRecordType { get; init; }

    public GroupNesting(bool HasTopLevelRecordType, int GroupType, params GroupNesting[] Underneath)
        : this(GroupType, Underneath)
    {
        this.HasTopLevelRecordType = HasTopLevelRecordType;
    }
}

public sealed record GroupConstants : RecordHeaderConstants
{
    public GroupCellConstants Cell { get; init; }
    public GroupWorldConstants World { get; init; }
    public GroupTopicConstants? Topic { get; init; }
    public GroupQuestConstants? Quest { get; init; }
    public IReadOnlyCollection<int> HasSubGroups { get; }
    public IReadOnlyCollection<int> HasParentFormId { get; }
    private readonly GroupNesting[] _nesting;

    public GroupConstants(
        ObjectType type,
        byte headerLength,
        byte lengthLength,
        GroupCellConstants cell,
        GroupWorldConstants world,
        GroupTopicConstants? topic,
        int[] hasSubGroups,
        int[] hasParentFormId,
        GroupNesting[] nesting)
        : base(type, headerLength, lengthLength)
    {
        Cell = cell;
        World = world;
        Topic = topic;
        HasSubGroups = hasSubGroups.ToHashSet();
        HasParentFormId = hasParentFormId.ToHashSet();
        var max = nesting.Select(x => x.GroupType).StartWith(0).Max(x => x);
        _nesting = new GroupNesting[max + 1];
        for (int i = 0; i < _nesting.Length; i++)
        {
            _nesting[i] = new GroupNesting(i);
        }
        foreach (var nest in nesting)
        {
            _nesting[nest.GroupType] = nest;
        }
    }

    public bool TryGetNesting(int groupType, [MaybeNullWhen(false)] out GroupNesting nesting)
    {
        return _nesting.TryGet(groupType, out nesting);
    }

    public GroupNesting? TryGetNesting(int groupType)
    {
        return TryGetNesting(groupType, out var nesting) ? nesting : default;
    }

    public bool CanHaveSubGroups(int groupType)
    {
        return HasSubGroups.Contains(groupType);
    }
}