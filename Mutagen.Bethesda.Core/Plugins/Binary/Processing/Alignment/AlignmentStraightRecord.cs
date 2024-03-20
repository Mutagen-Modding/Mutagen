using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

public sealed class AlignmentStraightRecord : AlignmentRule 
{ 
    private RecordType _recordType; 
 
    public AlignmentStraightRecord(string str) 
    { 
        _recordType = new RecordType(str); 
    }

    public override IEnumerable<RecordType> RecordTypes => _recordType.AsEnumerable(); 
 
    public override ReadOnlyMemorySlice<byte> ReadBytes(IMutagenReadStream inputStream, int? lengthOverride) 
    { 
        var subType = HeaderTranslation.ReadNextSubrecordType( 
            inputStream, 
            out var subLen);
        if (lengthOverride != null)
        {
            subLen = lengthOverride.Value;
        }
        if (!subType.Equals(_recordType)) 
        { 
            throw new ArgumentException(); 
        } 
        var ret = new byte[subLen + 6]; 
        MutagenWriter stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData.Constants);
        if (lengthOverride == null)
        {
            using (HeaderExport.Subrecord(stream, _recordType)) 
            { 
                inputStream.WriteTo(stream.BaseStream, subLen); 
            } 
        }
        else
        {
            using (HeaderExport.Subrecord(stream, _recordType)) 
            { 
            } 
            inputStream.WriteTo(stream.BaseStream, subLen); 
        }
        return ret; 
    } 
} 