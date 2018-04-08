using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen;
using Noggog.Utility;
using Xunit;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Tests
{
    public class Passthrough_Tests
    {
        public static IEnumerable<(FileSection Source, FileSection? Output)> ConstructRanges<T>(
            IEnumerable<(long Source, long Output, T Item)> items,
            Func<T, bool> eval)
        {
            bool inRange = false;
            long startRange = 0;
            long outputStartRange = 0;
            foreach (var item in items)
            {
                if (eval(item.Item))
                {
                    if (!inRange)
                    {
                        startRange = item.Source;
                        outputStartRange = item.Output;
                        inRange = true;
                    }
                }
                else
                {
                    if (inRange)
                    {
                        var sourceRange = new FileSection(startRange, item.Source - 1);
                        FileSection? outputRange = null;
                        if (startRange != outputStartRange
                            || item.Source != item.Output)
                        {
                            outputRange = new FileSection(outputStartRange, item.Output - 1);
                        }
                        yield return (sourceRange, outputRange);
                        inRange = false;
                    }
                }
            }
        }

        public static (Exception Exception, IEnumerable<(FileSection Source, FileSection? Output)> Sections) AssertFilesEqual(
            Stream stream,
            string path2,
            RangeCollection ignoreList = null,
            RangeCollection sourceSkips = null,
            RangeCollection targetSkips = null,
            ushort amountToReport = 5)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var reader2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
            {
                var errs = ConstructRanges(
                        GetDifferences(stream, reader2, ignoreList, sourceSkips, targetSkips),
                        b => !b).First(amountToReport).ToArray();
                if (errs.Length > 0)
                {
                    var posStr = string.Join(" ", errs.Select((r) =>
                    {
                        if (r.Output == null)
                        {
                            return r.Source.Range.ToString("X");
                        }
                        else
                        {
                            return $"[{r.Source.Range.ToString("X")} --- {r.Output.Value.Range.ToString("X")}]";
                        }
                    }));
                    return (new ArgumentException($"{path2} Bytes did not match at positions: {posStr}"), errs);
                }
                if (stream.Position != stream.Length)
                {
                    return (new ArgumentException($"{path2} Stream had more data past position 0x{new FileLocation(stream.Position)} than {path2}"), errs);
                }
                if (reader2.Position != reader2.Length)
                {
                    return (new ArgumentException($"{path2} Stream {path2} had more data past position 0x{new FileLocation(reader2.Position)} than source stream."), errs);
                }
                return (null, errs);
            }
        }

        public static bool TestSub(
            RangeInt64 range,
            IEnumerable<byte[]> subs,
            MutagenReader stream)
        {
            var curPos = stream.Position;
            stream.Position = new FileLocation(range.Min);
            var bytes = new byte[range.Width];
            stream.ReadInto(bytes);
            stream.Position = curPos;
            foreach (var sub in subs)
            {
                if (sub.SequenceEqual(bytes))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<(long Source, long Output, bool Equal)> GetDifferences(
            Stream reader1,
            Stream reader2,
            RangeCollection ignoreList,
            RangeCollection reader1Skips,
            RangeCollection reader2Skips)
        {
            var reader1Len = reader1.Length;
            var reader2Len = reader2.Length;
            while (reader1.Position < reader1Len
                && reader2.Position < reader2Len)
            {
                if (reader1Skips != null
                    && reader1Skips.TryGetCurrentRange(reader1.Position, out var range1))
                {
                    reader1.Position = new FileLocation(range1.Max + 1);
                    continue;
                }
                if (reader2Skips != null
                    && reader2Skips.TryGetCurrentRange(reader2.Position, out var range2))
                {
                    reader2.Position = new FileLocation(range2.Max + 1);
                    continue;
                }
                var b1 = reader1.ReadByte();
                var b2 = reader2.ReadByte();
                var pos = reader1.Position - 1;
                var same = b1 == b2 || (ignoreList?.IsEncapsulated(pos) ?? false);
                yield return (
                    pos,
                    reader2.Position - 1,
                    same);
            }
        }
    }
}
