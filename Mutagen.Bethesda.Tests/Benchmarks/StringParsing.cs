using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers;
using System.Linq;

namespace Mutagen.Bethesda.Tests
{
    [MemoryDiagnoser]
    public class StringParsing
    {
        public static byte[] data = Enumerable.Range(1, 15).Select(i => (byte)i).ToArray();
        public static BinaryMemoryReadStream stream = new(data);
        [Benchmark]
        public string StringCreate()
        {
            return string.Create(data.Length, data, (chars, state) =>
            {
                for (int i = 0; i < state.Length; i++)
                {
                    chars[i] = (char)state[i];
                }
            });
        }

        [Benchmark]
        public string ReadSpan()
        {
            return BinaryStringUtility.ToZString(stream.GetSpan(data.Length));
        }

        [Benchmark]
        public string ReadMemory()
        {
            return BinaryStringUtility.ToZString(stream.GetMemory(data.Length));
        }
    }
}
