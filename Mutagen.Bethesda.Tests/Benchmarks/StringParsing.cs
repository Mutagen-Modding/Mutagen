using BenchmarkDotNet.Attributes;
using Noggog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    [MemoryDiagnoser]
    public class StringParsing
    {
        public static byte[] data = Enumerable.Range(1, 15).Select(i => (byte)i).ToArray();

        [Benchmark]
        public string ArrayRenting()
        {
            var span = data.AsSpan();
            var chars = ArrayPool<char>.Shared.Rent(span.Length);
            BinaryStringUtility.ToZStringBuffer(span, chars);
            var ret = new string(chars, 0, span.Length);
            ArrayPool<char>.Shared.Return(chars);
            return ret;
        }

        [Benchmark]
        public string ArrayAllocation()
        {
            var span = data.AsSpan();
            char[] chars = new char[span.Length];
            BinaryStringUtility.ToZStringBuffer(span, chars);
            var charSpan = chars.AsSpan();
            return charSpan.ToString();
        }

        [Benchmark]
        public unsafe string UnsafeAlloc()
        {
            var span = data.AsSpan();
            Span<char> chars = stackalloc char[span.Length];
            BinaryStringUtility.ToZStringBuffer(span, chars);
            return chars.ToString();
        }
    }
}
