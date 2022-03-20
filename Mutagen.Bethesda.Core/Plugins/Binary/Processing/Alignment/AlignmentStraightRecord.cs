using System;
using System.Collections.Generic;
using System.IO;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

public class AlignmentStraightRecord : AlignmentRule 
{ 
    private RecordType _recordType; 
 
    public AlignmentStraightRecord(string str) 
    { 
        _recordType = new RecordType(str); 
    }

    public override IEnumerable<RecordType> RecordTypes => _recordType.AsEnumerable(); 
 
    public override ReadOnlyMemorySlice<byte> GetBytes(IMutagenReadStream inputStream) 
    { 
        var subType = HeaderTranslation.ReadNextSubrecordType( 
            inputStream, 
            out var subLen); 
        if (!subType.Equals(_recordType)) 
        { 
            throw new ArgumentException(); 
        } 
        var ret = new byte[subLen + 6]; 
        MutagenWriter stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData.Constants); 
        using (HeaderExport.Subrecord(stream, _recordType)) 
        { 
            inputStream.WriteTo(stream.BaseStream, subLen); 
        } 
        return ret; 
    } 
} 