using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis
{
    public record RecordLocationMarker(FormKey FormKey, RangeInt64 Location, RecordType Record);
}