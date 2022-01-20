using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Processing;

public class ModGroupMerger
{
    public static void MergeGroups(
        Func<IMutagenReadStream> streamCreator,
        Stream outputStream,
        RecordInterest? interest = null)
    {
        using var inputStream = streamCreator();
        using var inputStreamJumpback = streamCreator();
        using var writer = new System.IO.BinaryWriter(outputStream, Encoding.Default, leaveOpen: true);

        long runningDiff = 0;
        var fileLocs = RecordLocator.GetLocations(
            inputStream,
            interest: interest,
            additionalCriteria: (stream, recType, len) =>
            {
                return stream.GetMajorRecord().IsCompressed;
            });
        
        inputStream.Position = 0;

        var dict = new Dictionary<RecordType, List<GroupLocationMarker>>();
        foreach (var loc in fileLocs.GrupLocations)
        {
            inputStream.Position = loc.Key;
            var group = inputStream.ReadGroup();
            if (!group.IsTopLevel) continue;
            dict.GetOrAdd(loc.Value.ContainedRecordType).Add(loc.Value);
        }

        foreach (var val in dict.ToList())
        {
            if (val.Value.Count <= 1)
            {
                dict.Remove(val.Key);
            }
        }

        if (dict.Count == 0)
        {
            inputStream.BaseStream.Position = 0;
            inputStream.BaseStream.CopyTo(outputStream);
            return;
        }
        
        inputStream.Position = 0;

        var passedSet = new HashSet<RecordType>();
        while (!inputStream.Complete)
        {
            // Import until next listed group
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

            if (inputStream.Complete) break;
            
            var groupMeta = inputStream.GetGroup();

            if (!dict.TryGetValue(groupMeta.ContainedRecordType, out var groupLocations))
            {
                inputStream.WriteTo(writer.BaseStream, checked((int)groupMeta.TotalLength));
                continue;
            }

            if (!passedSet.Add(groupMeta.ContainedRecordType))
            {
                inputStream.Position += groupMeta.TotalLength;
                continue;
            }
            
            // Write last group header
            var readPos = inputStream.Position;
            var writePos = writer.BaseStream.Position;
            long totalLen = groupMeta.HeaderLength;

            inputStream.Position = groupLocations.Last().Location.Min;
            inputStream.WriteTo(writer.BaseStream, groupMeta.HeaderLength);

            // Write all group contents
            foreach (var groupLoc in groupLocations)
            {
                inputStream.Position = groupLoc.Location.Min;
                var targetGroupMeta = inputStream.GetGroupFrame(readSafe: false);
                totalLen += targetGroupMeta.Content.Length;
                writer.BaseStream.Write(targetGroupMeta.Content);
            }

            // Update group length
            writer.BaseStream.Position = writePos + 4;
            writer.Write(checked((uint)totalLen));
            
            // reset for next
            writer.BaseStream.Position = writePos + totalLen;
            inputStream.Position = readPos + groupMeta.TotalLength;
        }
    }

    private static void CopyOverHeader(RecordLocatorResults fileLocs, IMutagenReadStream inputStream, BinaryWriter writer)
    {
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
    }
}