using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

public record AlignmentRepeatedSubrule(RecordType RecordType, bool Single)
{
    public bool Ender { get; init; }
}

/// <summary> 
/// For use when a set of records is repeated. 
/// Does not currently enforce order within sub-group, but could be upgraded in the future 
/// </summary> 
public class AlignmentRepeatedRule : AlignmentRule 
{ 
    public Dictionary<RecordType, AlignmentRepeatedSubrule> SubTypes;
    public bool SortContents;
 
    private AlignmentRepeatedRule( 
        params RecordType[] types) 
        : this(types.Select(x => new AlignmentRepeatedSubrule(x, false)).ToArray())
    { 
    } 
 
    private AlignmentRepeatedRule( 
        params AlignmentRepeatedSubrule[] types) 
    { 
        SubTypes = types.ToDictionary(x => x.RecordType, x => x); 
    } 

    public static AlignmentRule Basic(params RecordType[] recordTypes)
    {
        return new AlignmentRepeatedRule(recordTypes);
    }

    public static AlignmentRule Sorted(params AlignmentRepeatedSubrule[] recordTypes)
    {
        return new AlignmentRepeatedRule(recordTypes)
        {
            SortContents = true
        };
    }
 
    public override IEnumerable<RecordType> RecordTypes => SubTypes.Keys; 
 
    public override ReadOnlyMemorySlice<byte> GetBytes(IMutagenReadStream inputStream)
    {
        if (inputStream.Complete) return Array.Empty<byte>();
        var dataList = new List<List<ReadOnlyMemorySlice<byte>>>();
        var latestList = new List<ReadOnlyMemorySlice<byte>>();
        var encountered = new HashSet<RecordType>(RecordTypes);
        RecordType? lastEncountered = null;
        MutagenWriter stream; 
        while (!inputStream.Complete) 
        { 
            var frame = inputStream.GetSubrecordFrame(readSafe: true);
            var subType = frame.RecordType;
            if (!SubTypes.TryGetValue(subType, out var rule)) 
            { 
                break; 
            }

            if (lastEncountered == subType
                || encountered.Remove(subType))
            {
            }
            else
            {
                dataList.Add(latestList);
                latestList = new List<ReadOnlyMemorySlice<byte>>();
                encountered.Add(SubTypes.Keys);
            }

            if (rule.Single)
            {
                lastEncountered = null;
            }
            else
            {
                lastEncountered = subType;
            }
            latestList.Add(frame.HeaderAndContentData);
            inputStream.Position += frame.TotalLength;

            if (rule.Ender)
            {
                lastEncountered = null;
                encountered.Clear();
            }
        } 
        dataList.Add(latestList);

        Sort(dataList, inputStream.MetaData.Constants);
        
        byte[] ret = new byte[dataList.SelectMany(x => x).Sum((d) => d.Length)]; 
        stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData.Constants); 
        foreach (var data in dataList.SelectMany(x => x)) 
        { 
            stream.Write(data); 
        } 
        return ret; 
    }

    private void Sort(List<List<ReadOnlyMemorySlice<byte>>> data, GameConstants constants)
    {
        if (!SortContents) return;
        foreach (var item in data)
        {
            Dictionary<RecordType, List<ReadOnlyMemorySlice<byte>>> mapping = new();
            foreach (var subrecord in item)
            {
                var frame = new SubrecordFrame(constants, subrecord);
                mapping.GetOrAdd(frame.RecordType).Add(subrecord);
            }
            
            item.Clear();
            foreach (var archetype in SubTypes)
            {
                if (mapping.TryGetValue(archetype.Key, out var content))
                {
                    item.AddRange(content);
                }
            }
        }
    }
} 