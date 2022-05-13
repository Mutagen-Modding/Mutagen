namespace Mutagen.Bethesda.Plugins.Meta;

public record GroupCellConstants(int TopGroupType, int[] SubTypes);
public record GroupWorldConstants(int TopGroupType, int[] CellGroupTypes, int[] CellSubGroupTypes);
public record GroupTopicConstants(int TopGroupType);
public record GroupQuestConstants(int TopGroupType);

public record GroupNesting(int GroupType, params GroupNesting[] Underneath)
{
    public RecordType? ContainedRecordType { get; init; }
}

public record GroupConstants : RecordHeaderConstants
{
    public GroupCellConstants Cell { get; init; }
    public GroupWorldConstants World { get; init; }
    public GroupTopicConstants? Topic { get; init; }
    public GroupQuestConstants? Quest { get; init; }
    public IReadOnlyCollection<int> HasSubGroups { get; }
    public GroupNesting[] Nesting { get; init; }

    public GroupConstants(
        ObjectType type, 
        byte headerLength,
        byte lengthLength,
        GroupCellConstants cell,
        GroupWorldConstants world,
        GroupTopicConstants? topic,
        int[] hasSubGroups,
        GroupNesting[] nesting) 
        : base(type, headerLength, lengthLength)
    {
        Cell = cell;
        World = world;
        Topic = topic;
        HasSubGroups = hasSubGroups.ToHashSet();
        Nesting = nesting;
    }
}