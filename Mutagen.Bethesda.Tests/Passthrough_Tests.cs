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
using Noggog.Streams.Binary;

namespace Mutagen.Bethesda.Tests
{
    public class Passthrough_Tests
    {
        public static (Exception Exception, IEnumerable<RangeInt64> Sections) AssertFilesEqual(
            Stream stream,
            string path2,
            RangeCollection ignoreList = null,
            ushort amountToReport = 5)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var reader2 = new BinaryReadStream(path2))
            {
                Stream compareStream = new ComparisonStream(
                    stream,
                    reader2);

                if (ignoreList != null)
                {
                    compareStream = new BasicSubstitutionRangeStream(
                        compareStream,
                        ignoreList,
                        toSubstitute: 0);
                }

                var errs = GetDifferences(compareStream)
                    .First(amountToReport)
                    .ToArray();
                if (errs.Length > 0)
                {
                    var posStr = string.Join(" ", errs.Select((r) =>
                    {
                        return r.ToString("X");
                    }));
                    return (new ArgumentException($"{path2} Bytes did not match at positions: {posStr}"), errs);
                }
                if (stream.Position != stream.Length)
                {
                    return (new ArgumentException($"{path2} Stream had more data past position 0x{stream.Position.ToString("X")} than {path2}"), errs);
                }
                if (reader2.Position != reader2.Length)
                {
                    return (new ArgumentException($"{path2} Stream {path2} had more data past position 0x{reader2.Position.ToString("X")} than source stream."), errs);
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

        public static IEnumerable<RangeInt64> GetDifferences(
            Stream reader)
        {
            byte[] buf = new byte[4096];
            bool inRange = false;
            long startRange = 0;
            var len = reader.Length;
            long pos = 0;
            while (pos < len)
            {
                var read = reader.Read(buf, 0, buf.Length);
                for (int i = 0; i < read; i++)
                {
                    if (buf[i] != 0)
                    {
                        if (!inRange)
                        {
                            startRange = pos + i;
                            inRange = true;
                        }
                    }
                    else
                    {
                        if (inRange)
                        {
                            var sourceRange = new RangeInt64(startRange, pos + i);
                            yield return sourceRange;
                            inRange = false;
                        }
                    }
                }
                pos += read;
            }
        }
    }
}
