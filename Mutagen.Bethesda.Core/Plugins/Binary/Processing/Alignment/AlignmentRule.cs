using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

public abstract class AlignmentRule 
{ 
    public abstract IEnumerable<RecordType> RecordTypes { get; } 
 
    public abstract ReadOnlyMemorySlice<byte> ReadBytes(IMutagenReadStream inputStream, int? lengthOverride); 
 
    public static implicit operator AlignmentRule(RecordType recordType) 
    { 
        return new AlignmentStraightRecord(recordType.Type); 
    } 
} 