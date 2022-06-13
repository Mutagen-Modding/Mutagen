using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

/// <summary> 
/// For use when a previously encountered record is seen again 
/// </summary> 
public class AlignmentSubRule : AlignmentRule 
{ 
    public List<RecordType> SubTypes; 
 
    public AlignmentSubRule( 
        params RecordType[] types) 
    { 
        SubTypes = types.ToList(); 
    } 
 
    public override IEnumerable<RecordType> RecordTypes => SubTypes; 
 
    public override ReadOnlyMemorySlice<byte> GetBytes(IMutagenReadStream inputStream) 
    { 
        Dictionary<RecordType, byte[]> dataDict = new Dictionary<RecordType, byte[]>(); 
        MutagenWriter stream; 
        while (!inputStream.Complete) 
        { 
            var subType = HeaderTranslation.ReadNextSubrecordType( 
                inputStream, 
                out var subLen); 
            if (!SubTypes.Contains(subType)) 
            { 
                inputStream.Position -= 6; 
                break; 
            } 
            var data = new byte[subLen + 6]; 
            stream = new MutagenWriter(new MemoryStream(data), inputStream.MetaData.Constants); 
            using (HeaderExport.Subrecord(stream, subType)) 
            { 
                inputStream.CopyTo(stream.BaseStream, subLen); 
            } 
            dataDict[subType] = data; 
        } 
        byte[] ret = new byte[dataDict.Values.Sum((d) => d.Length)]; 
        stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData.Constants); 
        foreach (var alignment in SubTypes) 
        { 
            if (dataDict.TryGetValue( 
                    alignment, 
                    out var data)) 
            { 
                stream.Write(data); 
            } 
        } 
        return ret; 
    } 
} 