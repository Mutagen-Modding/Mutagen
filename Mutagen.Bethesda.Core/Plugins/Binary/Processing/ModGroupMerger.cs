using System.Buffers.Binary;
using System.Text;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Plugins.Binary.Processing;

public static class ModGroupMerger
{
    private readonly record struct SiblingGroup(long Start, long TotalLength);

    public static void MergeGroups(
        Func<IMutagenReadStream> streamCreator,
        Stream outputStream,
        RecordInterest? interest = null)
    {
        using var inputStream = streamCreator();
        using var writer = new BinaryWriter(outputStream, Encoding.Default, leaveOpen: true);
        var constants = inputStream.MetaData.Constants;

        // Copy the mod header verbatim
        inputStream.Position = 0;
        var modHeader = inputStream.GetModHeaderFrame();
        var modHeaderLen = checked((int)(modHeader.HeaderLength + modHeader.ContentLength));
        inputStream.Position = 0;
        inputStream.WriteTo(writer.BaseStream, modHeaderLen);

        // Collect top-level groups, grouping by ContainedRecordType (preserving first-seen order)
        var topLevelByType = new Dictionary<uint, List<SiblingGroup>>();
        var topLevelOrder = new List<uint>();

        while (inputStream.TryGetGroupHeader(out var groupHeader))
        {
            var key = BinaryPrimitives.ReadUInt32LittleEndian(groupHeader.ContainedRecordTypeData);
            if (!topLevelByType.TryGetValue(key, out var list))
            {
                list = new List<SiblingGroup>();
                topLevelByType[key] = list;
                topLevelOrder.Add(key);
            }
            list.Add(new SiblingGroup(inputStream.Position, groupHeader.TotalLength));
            inputStream.Position += groupHeader.TotalLength;
        }

        foreach (var key in topLevelOrder)
        {
            var siblings = topLevelByType[key];
            if (interest != null)
            {
                inputStream.Position = siblings[0].Start;
                var header = inputStream.GetGroupHeader();
                if (!interest.IsInterested(header.ContainedRecordType))
                {
                    foreach (var sib in siblings)
                    {
                        inputStream.Position = sib.Start;
                        inputStream.WriteTo(writer.BaseStream, checked((int)sib.TotalLength));
                    }
                    continue;
                }
            }
            WriteMergedGroupList(inputStream, writer, constants, siblings);
        }
    }

    /// <summary>
    /// Writes a set of sibling GRUPs that share a merge key (GroupType + Label).
    /// - If there's one sibling, emits it, recursing into its sub-groups so nested duplicates are still merged.
    /// - If multiple, emits the last sibling's header (preserving its label/last-modified stamp),
    ///   then merges their contents.  For leaf groups, contents are concatenated.  For container
    ///   groups (content begins with GRUP), children from every sibling are gathered, re-grouped
    ///   by their merge key, and recursively processed — so duplicates at any depth get merged.
    /// </summary>
    private static void WriteMergedGroupList(
        IMutagenReadStream inputStream,
        BinaryWriter writer,
        GameConstants constants,
        List<SiblingGroup> siblings)
    {
        if (siblings.Count == 0) return;

        var last = siblings[^1];
        inputStream.Position = last.Start;
        var lastHeader = inputStream.GetGroupHeader();
        var headerLen = lastHeader.HeaderLength;

        long outputHeaderPos = writer.BaseStream.Position;
        inputStream.WriteTo(writer.BaseStream, headerLen);

        bool hasSubGroups = FirstSiblingContainsSubGroups(inputStream, constants, siblings[0], headerLen);

        if (!hasSubGroups)
        {
            foreach (var sib in siblings)
            {
                long contentLen = sib.TotalLength - headerLen;
                if (contentLen <= 0) continue;
                inputStream.Position = sib.Start + headerLen;
                inputStream.WriteTo(writer.BaseStream, checked((int)contentLen));
            }
        }
        else
        {
            var childrenByKey = new Dictionary<(int GroupType, uint Label), List<SiblingGroup>>();
            var childOrder = new List<(int GroupType, uint Label)>();

            foreach (var sib in siblings)
            {
                long pos = sib.Start + headerLen;
                long endPos = sib.Start + sib.TotalLength;
                while (pos < endPos)
                {
                    inputStream.Position = pos;
                    var childHeader = inputStream.GetGroupHeader();
                    var key = (childHeader.GroupType,
                        BinaryPrimitives.ReadUInt32LittleEndian(childHeader.ContainedRecordTypeData));
                    if (!childrenByKey.TryGetValue(key, out var list))
                    {
                        list = new List<SiblingGroup>();
                        childrenByKey[key] = list;
                        childOrder.Add(key);
                    }
                    list.Add(new SiblingGroup(pos, childHeader.TotalLength));
                    pos += childHeader.TotalLength;
                }
            }

            foreach (var key in childOrder)
            {
                WriteMergedGroupList(inputStream, writer, constants, childrenByKey[key]);
            }
        }

        long contentEnd = writer.BaseStream.Position;
        long totalLen = contentEnd - outputHeaderPos;
        writer.BaseStream.Position = outputHeaderPos + 4;
        writer.Write(checked((uint)totalLen));
        writer.BaseStream.Position = contentEnd;
    }

    private static bool FirstSiblingContainsSubGroups(
        IMutagenReadStream inputStream,
        GameConstants constants,
        SiblingGroup sibling,
        int headerLen)
    {
        long contentLen = sibling.TotalLength - headerLen;
        if (contentLen < constants.GroupConstants.HeaderLength) return false;
        inputStream.Position = sibling.Start + headerLen;
        return inputStream.TryGetGroupHeader(out _);
    }
}
