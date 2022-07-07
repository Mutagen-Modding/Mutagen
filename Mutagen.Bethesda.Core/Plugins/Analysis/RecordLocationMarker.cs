using System.Collections.Immutable;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis;

public sealed record RecordLocationMarker(FormKey FormKey, RangeInt64 Location, RecordType Record);

public sealed class GroupLocationMarker
{
    public ImmutableStack<GroupLocationMarker> Parents { get; }
    internal bool Registered { get; set; }
    public RangeInt64 Location { get; init; }
    public RecordType ContainedRecordType { get; init; }
    public int GroupType { get; init; }

    public GroupLocationMarker(RangeInt64 location, RecordType containedRecordType, int groupType, ImmutableStack<GroupLocationMarker> parents)
    {
        Location = location;
        ContainedRecordType = containedRecordType;
        GroupType = groupType;
        Parents = parents;
    }

    public GroupLocationMarker(RangeInt64 location, RecordType containedRecordType, int groupType)
    {
        Location = location;
        ContainedRecordType = containedRecordType;
        GroupType = groupType;
        Parents = ImmutableStack<GroupLocationMarker>.Empty;
    }

    public GroupLocationMarker(GroupPinHeader pinHeader, ImmutableStack<GroupLocationMarker> parents)
    {
        Parents = parents;
        Location = RangeInt64.FromLength(pinHeader.Location, pinHeader.TotalLength);
        ContainedRecordType = pinHeader.ContainedRecordType;
        GroupType = pinHeader.GroupType;
    }
    
    protected bool Equals(GroupLocationMarker other)
    {
        return Location.Equals(other.Location) && ContainedRecordType.Equals(other.ContainedRecordType) && GroupType == other.GroupType;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GroupLocationMarker)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Location, ContainedRecordType, GroupType);
    }
}