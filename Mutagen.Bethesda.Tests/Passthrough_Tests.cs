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
        public static Exception AssertFilesEqual(
            Stream stream,
            string path2,
            RangeCollection ignoreList = null,
            RangeCollection sourceSkips = null,
            RangeCollection targetSkips = null)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var reader2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
            {
                var errs = RangeInt64.ConstructRanges(
                        GetDifferences(stream, reader2, ignoreList, sourceSkips, targetSkips),
                        b => !b).First(5).ToArray();
                if (errs.Length > 0)
                {
                    throw new ArgumentException($"{path2} Bytes did not match at positions: {string.Join(" ", errs.Select((r) => r.ToString("X")))}");
                }
                if (stream.Position != stream.Length)
                {
                    return new ArgumentException($"{path2} Stream had more data past position 0x{stream.Position} than {path2}");
                }
                if (reader2.Position != reader2.Length)
                {
                    return new ArgumentException($"{path2} Stream {path2} had more data past position 0x{reader2.Position} than source stream.");
                }
            }
            return null;
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

        public static IEnumerable<KeyValuePair<long, bool>> GetDifferences(
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
                var same = b1 == b2 || (ignoreList?.IsEncapsulated(reader1.Length) ?? false);
                yield return new KeyValuePair<long, bool>(
                    reader1.Position - 1,
                    same);
            }
        }
    }
}
