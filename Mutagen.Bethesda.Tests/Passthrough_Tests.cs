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
        public static (Exception Exception, IEnumerable<(RangeInt64 Source, RangeInt64? Output)> Sections) AssertFilesEqual(
            IBinaryReadStream stream,
            string path2,
            RangeCollection ignoreList = null,
            RangeCollection sourceSkips = null,
            RangeCollection targetSkips = null,
            ushort amountToReport = 5)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var reader2 = new BinaryReadStream(path2))
            {
                var errs = GetDifferences(
                    stream,
                    reader2,
                    ignoreList,
                    sourceSkips.Empty ? null : sourceSkips,
                    targetSkips.Empty ? null : targetSkips).First(amountToReport).ToArray();
                if (errs.Length > 0)
                {
                    var posStr = string.Join(" ", errs.Select((r) =>
                    {
                        if (r.Output == null)
                        {
                            return r.Source.ToString("X");
                        }
                        else
                        {
                            return $"[{r.Source.ToString("X")} --- {r.Output.Value.ToString("X")}]";
                        }
                    }));
                    return (new ArgumentException($"{path2} Bytes did not match at positions: {posStr}"), errs);
                }
                if (stream.Position != stream.Length)
                {
                    return (new ArgumentException($"{path2} Stream had more data past position 0x{stream.Position} than {path2}"), errs);
                }
                if (reader2.Position != reader2.Length)
                {
                    return (new ArgumentException($"{path2} Stream {path2} had more data past position 0x{reader2.Position} than source stream."), errs);
                }
                return (null, errs);
            }
        }

        public static bool TestSub(
            RangeInt64 range,
            IEnumerable<byte[]> subs,
            IBinaryReadStream stream)
        {
            var curPos = stream.Position;
            stream.Position = range.Min;
            var bytes = new byte[range.Width];
            stream.Read(bytes);
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

        public static IEnumerable<(RangeInt64 Source, RangeInt64? Output)> GetDifferences(
            IBinaryReadStream reader1,
            IBinaryReadStream reader2,
            RangeCollection ignoreList,
            RangeCollection reader1Skips,
            RangeCollection reader2Skips)
        {
            bool inRange = false;
            long startRange = 0;
            long outputStartRange = 0;
            var reader1Len = reader1.Length;
            var reader2Len = reader2.Length;
            long pos1 = 0, pos2 = 0;
            while (pos1 < reader1Len
                && pos2 < reader2Len)
            {
                if (reader1Skips != null
                    && reader1Skips.TryGetCurrentRange(pos1, out var range1))
                {
                    pos1 = range1.Max + 1;
                    reader1.Position = pos1;
                    continue;
                }
                if (reader2Skips != null
                    && reader2Skips.TryGetCurrentRange(pos2, out var range2))
                {
                    pos2 = range2.Max + 1;
                    reader2.Position = pos2;
                    continue;
                }
                var b1 = reader1.ReadUInt8();
                var b2 = reader2.ReadUInt8();
                pos1++;
                pos2++;
                var pos = pos1 - 1;
                var pos22 = pos2 - 1;
                var same = b1 == b2 || (ignoreList?.IsEncapsulated(pos) ?? false);
                if (!same)
                {
                    if (!inRange)
                    {
                        startRange = pos1;
                        outputStartRange = pos2;
                        inRange = true;
                    }
                }
                else
                {
                    if (inRange)
                    {
                        var sourceRange = new RangeInt64(startRange, pos);
                        RangeInt64? outputRange = null;
                        if (startRange != outputStartRange
                            || pos1 != pos2)
                        {
                            outputRange = new RangeInt64(outputStartRange, pos22);
                        }
                        yield return (sourceRange, outputRange);
                        inRange = false;
                    }
                }
            }
        }
    }
}
