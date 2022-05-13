using Mutagen.Bethesda.Plugins.Binary.Headers;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis;

public record RecordLocationMarker(FormKey FormKey, RangeInt64 Location, RecordType Record);

public record GroupLocationMarker(RangeInt64 Location, RecordType ContainedRecordType, int GroupType)
{
    public GroupLocationMarker(GroupPinHeader pinHeader) : this(
        RangeInt64.FromLength(pinHeader.Location, pinHeader.TotalLength),
        pinHeader.ContainedRecordType,
        pinHeader.GroupType)
    {
    }
}