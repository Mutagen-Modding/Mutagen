using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;

public static class ModRecordAligner 
{
    public static void Align( 
        ModPath inputPath, 
        FilePath outputPath, 
        GameRelease gameMode, 
        AlignmentRules alignmentRules, 
        DirectoryPath temp) 
    { 
        var interest = new RecordInterest(alignmentRules.Alignments.Keys) 
        { 
            EmptyMeansInterested = false 
        }; 
        // Always interested in parent record types 
        interest.InterestingTypes.Add("CELL"); 
        interest.InterestingTypes.Add("WRLD"); 
        interest.InterestingTypes.Add("QUST"); 

        using (var inputStream = new MutagenBinaryReadStream(inputPath, gameMode))
        {
            var fileLocs = RecordLocator.GetLocations(inputPath, gameMode, interest);
            var alignedMajorRecordsFile = new ModPath(inputPath.ModKey, Path.Combine(temp, "alignedRules"));
            using var writer = new MutagenWriter(alignedMajorRecordsFile, gameMode);
            AlignMajorRecordsByRules(inputStream, writer, alignmentRules, fileLocs);
            inputPath = alignedMajorRecordsFile;
        }

        using (var inputStream = new MutagenBinaryReadStream(inputPath, gameMode))
        {
            var fileLocs = RecordLocator.GetLocations(inputPath, gameMode);
            var alignedGroupsFile = new ModPath(inputPath.ModKey, Path.Combine(temp, "alignedGroups"));
            using var writer = new MutagenWriter(alignedGroupsFile, gameMode);
            AlignGroupsByRules(inputStream, writer, alignmentRules, fileLocs);
            inputPath = alignedGroupsFile;
        }

        if (gameMode is GameRelease.Oblivion or GameRelease.Fallout4)
        {
            var fileLocs = RecordLocator.GetLocations(inputPath, gameMode, interest);
            var alignedCellsFile = new ModPath(inputPath.ModKey, Path.Combine(temp, "alignedCells"));
            using (var mutaReader = new MutagenBinaryReadStream(inputPath, gameMode))
            {
                using var writer = new MutagenWriter(alignedCellsFile, gameMode);
                foreach (var grup in fileLocs.GrupLocations.Keys)
                {
                    if (grup <= mutaReader.Position) continue;
                    var noRecordLength = grup - mutaReader.Position;
                    mutaReader.WriteTo(writer.BaseStream, (int)noRecordLength);

                    // If complete overall, return 
                    if (mutaReader.Complete) break;

                    var nextGrup = mutaReader.GetGroupHeader();
                    if (writer.MetaData.Constants.GroupConstants.Cell.TopGroupType == nextGrup.GroupType)
                    {
                        mutaReader.WriteTo(writer.BaseStream, nextGrup.HeaderLength);
                        AlignCellChildren(mutaReader, writer);
                    }
                }
                mutaReader.WriteTo(writer.BaseStream, checked((int)mutaReader.Remaining));
            }
            inputPath = alignedCellsFile;
        }

        if (gameMode is GameRelease.Oblivion)
        {
            var alignedCellsFile = new ModPath(inputPath.ModKey, Path.Combine(temp, "alignedWorldspaces"));
            var fileLocs = RecordLocator.GetLocations(inputPath, gameMode, interest); 
            using (var mutaReader = new MutagenBinaryReadStream(inputPath, gameMode)) 
            { 
                using var writer = new MutagenWriter(alignedCellsFile, gameMode); 
                foreach (var grup in fileLocs.GrupLocations.Keys) 
                { 
                    if (grup <= mutaReader.Position) continue; 
                    var noRecordLength = grup - mutaReader.Position; 
                    mutaReader.WriteTo(writer.BaseStream, (int)noRecordLength); 
 
                    // If complete overall, return 
                    if (mutaReader.Complete) break; 
 
                    mutaReader.WriteTo(writer.BaseStream, 12); 
                    var grupType = mutaReader.ReadInt32(); 
                    writer.Write(grupType); 
                    if (writer.MetaData.Constants.GroupConstants.World.TopGroupType == grupType) 
                    { 
                        AlignWorldChildren(mutaReader, writer); 
                    } 
                } 
                mutaReader.WriteTo(writer.BaseStream, checked((int)mutaReader.Remaining)); 
            } 
        }

        File.Copy(inputPath, outputPath, true);
    } 
 
    private static void AlignMajorRecordsByRules( 
        IMutagenReadStream inputStream, 
        MutagenWriter writer, 
        AlignmentRules alignmentRules, 
        RecordLocatorResults fileLocs) 
    { 
        while (!inputStream.Complete) 
        { 
            // Import until next listed major record 
            long noRecordLength; 
            if (fileLocs.ListedRecords.TryGetInDirection( 
                    inputStream.Position, 
                    higher: true, 
                    result: out var nextRec)) 
            { 
                var recordLocation = fileLocs.ListedRecords.Keys[nextRec.Key]; 
                noRecordLength = recordLocation - inputStream.Position; 
            } 
            else 
            { 
                noRecordLength = inputStream.Remaining; 
            } 
            inputStream.WriteTo(writer.BaseStream, (int)noRecordLength); 
 
            // If complete overall, return 
            if (inputStream.Complete) break;

            var majorHeader = inputStream.GetMajorRecordHeader();
            var recType = HeaderTranslation.ReadNextRecordType( 
                inputStream, 
                out var len); 
            if (!alignmentRules.StopMarkers.TryGetValue(recType, out var stopMarkers)) 
            { 
                stopMarkers = null; 
            } 
 
            bool started = false; 
            if (!alignmentRules.StartMarkers.TryGetValue(recType, out var startMarkers)) 
            { 
                startMarkers = null; 
                started = true; 
            } 
 
            var startTriggers = startMarkers?.ToHashSet(); 
            writer.Write(recType.TypeInt); 
            writer.Write(len); 
            if (!alignmentRules.Alignments.TryGetValue(recType, out var alignments)) 
            { 
                inputStream.WriteTo(writer.BaseStream, inputStream.MetaData.Constants.MajorConstants.LengthAfterLength + len); 
                continue; 
            } 
            inputStream.WriteTo(writer.BaseStream, inputStream.MetaData.Constants.MajorConstants.LengthAfterLength); 
            var writerEndPos = writer.Position + len; 
            var endPos = inputStream.Position + len; 
            var dataDict = new Dictionary<RecordType, ReadOnlyMemorySlice<byte>>(); 
            ReadOnlyMemorySlice<byte>? rest = null; 
            while (inputStream.Position < endPos) 
            { 
                var subType = inputStream.GetSubrecord(); 
                if (stopMarkers?.Contains(subType.RecordType) ?? false) 
                { 
                    rest = inputStream.ReadMemory((int)(endPos - inputStream.Position), readSafe: true); 
                    break; 
                } 
                     
                if (!started && (startTriggers?.Contains(subType.RecordType) ?? false)) 
                { 
                    started = true; 
                } 
 
                if (!started) 
                { 
                    inputStream.WriteTo(writer.BaseStream, subType.TotalLength); 
                    continue; 
                } 
                     
                if (!alignments.TryGetValue(subType.RecordType, out var rule)) 
                { 
                    throw new ArgumentException($"Encountered an unknown record: {subType}"); 
                } 
                dataDict.Add(subType.RecordType, rule.GetBytes(inputStream)); 
            } 
            foreach (var alignment in alignmentRules.Alignments[recType]) 
            { 
                if (dataDict.TryGetValue( 
                        alignment.Key, 
                        out var data)) 
                { 
                    writer.Write(data); 
                    dataDict.Remove(alignment.Key); 
                } 
            } 
            if (dataDict.Count > 0) 
            { 
                throw new ArgumentException($"Encountered an unknown record: {dataDict.First().Key}"); 
            } 
            if (rest != null) 
            { 
                writer.Write(rest.Value); 
            } 
            if (writer.Position != writerEndPos) 
            { 
                throw new ArgumentException("Record alignment changed length"); 
            } 
        } 
    } 
 
    private static void AlignGroupsByRules( 
        MutagenBinaryReadStream inputStream, 
        MutagenWriter writer, 
        AlignmentRules alignmentRules, 
        RecordLocatorResults fileLocs) 
    { 
        while (!inputStream.Complete) 
        { 
            // Import until next listed major record 
            long noRecordLength; 
            if (fileLocs.GrupLocations.TryGetInDirection( 
                    inputStream.Position, 
                    higher: true, 
                    result: out var nextRec)) 
            { 
                noRecordLength = nextRec.Value.Location.Min - inputStream.Position; 
            } 
            else 
            { 
                noRecordLength = inputStream.Remaining; 
            } 
            inputStream.WriteTo(writer.BaseStream, (int)noRecordLength); 
 
            // If complete overall, return 
            if (inputStream.Complete) break; 
            var groupMeta = inputStream.GetGroupHeader(); 
            if (!groupMeta.IsGroup) 
            { 
                throw new ArgumentException(); 
            } 
            writer.Write(inputStream.ReadSpan(groupMeta.HeaderLength)); 
 
            if (!alignmentRules.GroupAlignment.TryGetValue(groupMeta.GroupType, out var groupRules)) continue; 
 
            var storage = new Dictionary<RecordType, List<ReadOnlyMemorySlice<byte>>>(); 
            var rest = new List<ReadOnlyMemorySlice<byte>>(); 
            using (var frame = MutagenFrame.ByLength(inputStream, groupMeta.ContentLength))
            {
                RecordType? lastType = null;
                while (!frame.Complete)
                {
                    var variable = inputStream.GetVariableHeader();
                    if (variable.IsGroup && lastType.HasValue)
                    {
                        var bytes = inputStream.ReadMemory(checked((int)variable.TotalLength));
                        storage.GetOrAdd(lastType.Value).Add(bytes); 
                    }
                    else
                    {
                        var majorMeta = inputStream.GetMajorRecordHeader(); 
                        var bytes = inputStream.ReadMemory(checked((int)majorMeta.TotalLength)); 
                        var type = majorMeta.RecordType;
                        if (groupRules.Contains(type)) 
                        { 
                            storage.GetOrAdd(type).Add(bytes); 
                        } 
                        else
                        { 
                            rest.Add(bytes); 
                        } 
                        lastType = type;
                    }
                } 
            } 
            foreach (var rule in groupRules) 
            { 
                if (storage.TryGetValue(rule, out var storageBytes)) 
                { 
                    foreach (var item in storageBytes) 
                    { 
                        writer.Write(item); 
                    } 
                } 
            } 
            foreach (var item in rest) 
            { 
                writer.Write(item); 
            } 
        } 
    } 
 
    private static void AlignCellChildren( 
        IMutagenReadStream mutaReader, 
        MutagenWriter writer) 
    {  
        var storage = new Dictionary<int, ReadOnlyMemorySlice<byte>>(); 
        for (int i = 0; i < 3; i++) 
        {
            if (!mutaReader.TryGetGroupHeader(out var grupHeader)) break;
            var subGroupType = grupHeader.GroupType;
            if (!writer.MetaData.Constants.GroupConstants.Cell.SubTypes.Contains(subGroupType)) 
            {
                break;
            } 
            storage[subGroupType] = mutaReader.ReadMemory(checked((int)grupHeader.TotalLength), readSafe: true); 
        } 
        foreach (var item in writer.MetaData.Constants.GroupConstants.Cell.SubTypes) 
        { 
            if (storage.TryGetValue(item, out var content)) 
            { 
                writer.Write(content); 
            } 
        } 
    } 
 
    private static void AlignWorldChildren( 
        IMutagenReadStream reader, 
        MutagenWriter writer) 
    { 
        reader.WriteTo(writer.BaseStream, 4); 
        ReadOnlyMemorySlice<byte>? roadStorage = null; 
        ReadOnlyMemorySlice<byte>? cellStorage = null; 
        List<ReadOnlyMemorySlice<byte>> grupBytes = new List<ReadOnlyMemorySlice<byte>>(); 
        for (int i = 0; i < 3; i++) 
        { 
            RecordType type = HeaderTranslation.GetNextRecordType(reader); 
            switch (type.TypeInt) 
            { 
                case RecordTypeInts.ROAD: 
                    roadStorage = reader.ReadMemory(checked((int)reader.GetMajorRecordHeader().TotalLength)); 
                    break; 
                case RecordTypeInts.CELL: 
                    if (cellStorage != null) 
                    { 
                        throw new ArgumentException(); 
                    } 
                    var cellMajorMeta = reader.GetMajorRecordHeader(); 
                    var startPos = reader.Position; 
                    reader.Position += cellMajorMeta.HeaderLength; 
                    long cellGroupLen = 0; 
                    if (reader.TryGetGroupHeader(out var cellSubGroupMeta) 
                        && cellSubGroupMeta.GroupType == writer.MetaData.Constants.GroupConstants.Cell.TopGroupType) 
                    { 
                        cellGroupLen = cellSubGroupMeta.TotalLength; 
                    } 
                    reader.Position = startPos; 
                    cellStorage = reader.ReadMemory(checked((int)(cellMajorMeta.TotalLength + cellGroupLen))); 
                    break; 
                case RecordTypeInts.GRUP: 
                    if (roadStorage != null 
                        && cellStorage != null) 
                    { 
                        i = 3; // end loop 
                        continue; 
                    } 
                    grupBytes.Add(reader.ReadMemory(checked((int)reader.GetGroupHeader().TotalLength))); 
                    break; 
                case RecordTypeInts.WRLD: 
                    i = 3; // end loop 
                    continue; 
                default: 
                    throw new NotImplementedException(); 
            } 
        } 
        if (roadStorage != null) 
        { 
            writer.Write(roadStorage.Value); 
        } 
        if (cellStorage != null) 
        { 
            writer.Write(cellStorage.Value); 
        } 
        foreach (var item in grupBytes) 
        { 
            writer.Write(item); 
        } 
    } 
}