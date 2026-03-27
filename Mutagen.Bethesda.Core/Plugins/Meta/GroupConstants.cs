using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Meta;

public sealed record GroupCellConstants(int TopGroupType, int[] SubTypes);
public sealed record GroupWorldConstants(int TopGroupType, int[] CellGroupTypes, int[] CellSubGroupTypes);
public sealed record GroupTopicConstants(int TopGroupType);
public sealed record GroupQuestConstants(int TopGroupType);

/// <summary>
/// Represents a group nesting level in the plugin binary format.
/// </summary>
/// <param name="ParentRecordType">
/// The record type of the parent record that contains this group nesting, if applicable.
/// For example, WRLD for worldspace groups, CELL for cell sub-blocks, DIAL for dialog topic groups.
/// Null for leaf groups or groups without a specific parent record type.
/// </param>
/// <param name="GroupType">The integer group type identifier</param>
/// <param name="Underneath">Child nesting levels beneath this group</param>
public sealed record GroupNesting(RecordType? ParentRecordType, int GroupType, params GroupNesting[] Underneath)
{
    public bool HasTopLevelRecordType { get; init; }

    public GroupNesting(int GroupType, params GroupNesting[] Underneath)
        : this(null, GroupType, Underneath)
    {
    }

    public GroupNesting(bool HasTopLevelRecordType, int GroupType, params GroupNesting[] Underneath)
        : this(null, GroupType, Underneath)
    {
        this.HasTopLevelRecordType = HasTopLevelRecordType;
    }

    public GroupNesting(RecordType ParentRecordType, bool HasTopLevelRecordType, int GroupType, params GroupNesting[] Underneath)
        : this(ParentRecordType, GroupType, Underneath)
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

    /// <summary>
    /// Record types that are known parent/container types for this game.
    /// A record type is a "parent" if its nesting level contains child nesting levels
    /// that themselves hold records (i.e., major records nested under this record type).
    /// Examples: WRLD (contains CELLs), CELL (contains placed objects), DIAL (contains INFOs).
    /// </summary>
    public IReadOnlySet<RecordType> ParentRecordTypes { get; }

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
        ParentRecordTypes = ComputeParentRecordTypes(nesting);
    }

    /// <summary>
    /// Walks the nesting tree to find record types that are parent/container types.
    /// Any nesting node with a ParentRecordType set is a parent, because the node's
    /// existence in the tree means that record type contains sub-groups with child records.
    /// </summary>
    private static IReadOnlySet<RecordType> ComputeParentRecordTypes(GroupNesting[] nestings)
    {
        var parents = new HashSet<RecordType>();
        CollectParentRecordTypes(nestings, parents);
        return parents;
    }

    private static void CollectParentRecordTypes(GroupNesting[] nestings, HashSet<RecordType> parents)
    {
        foreach (var nesting in nestings)
        {
            if (nesting.ParentRecordType.HasValue)
            {
                parents.Add(nesting.ParentRecordType.Value);
            }
            CollectParentRecordTypes(nesting.Underneath, parents);
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