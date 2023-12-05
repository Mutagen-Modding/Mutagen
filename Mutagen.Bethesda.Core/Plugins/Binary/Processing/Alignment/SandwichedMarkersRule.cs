using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

public class SandwichedMarkersRule : AlignmentRule
{
    private AlignmentRule _internalRule;
    private RecordType _marker;
    private HashSet<RecordType> _internalTypes;
    
    public SandwichedMarkersRule(RecordType marker, params RecordType[] recordTypes)
    {
        _marker = marker;
        RecordTypes = new [] { marker };
        _internalRule = AlignmentRepeatedRule.Basic(recordTypes);
        _internalTypes = recordTypes.ToHashSet();
    }

    public override IEnumerable<RecordType> RecordTypes { get; }
    
    public override ReadOnlyMemorySlice<byte> ReadBytes(IMutagenReadStream inputStream, int? lengthOverride)
    {
        if (lengthOverride != null)
        {
            throw new NotImplementedException();
        }
        if (inputStream.Complete) return Array.Empty<byte>();
        ReadOnlyMemorySlice<byte>? data = null;
        MutagenWriter stream;
        bool second = false;
        while (!inputStream.Complete) 
        { 
            var frame = inputStream.GetSubrecord(readSafe: true);
            var subType = frame.RecordType;

            if (inputStream.MetaData.Constants.HeaderOverflow.Contains(subType))
            {
                throw new NotImplementedException();
            }

            if (subType == _marker)
            {
                inputStream.Position += frame.TotalLength;
                if (!second)
                {
                    second = true;
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (data != null || !_internalTypes.Contains(subType))
            {
                throw new NotImplementedException();
            }

            data = _internalRule.ReadBytes(inputStream, null);
        }

        if (data == null) return Array.Empty<byte>();
        
        byte[] ret = new byte[data.Value.Length + 2 * inputStream.MetaData.Constants.SubConstants.HeaderLength]; 
        stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData.Constants);
        using (HeaderExport.Subrecord(stream, _marker))
        {
        }
        stream.Write(data.Value); 
        using (HeaderExport.Subrecord(stream, _marker))
        {
        }
        return ret; 
    }
}