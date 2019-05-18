using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    [KeepBenchmarkFiles]
    [AsciiDocExporter]
    public class FormKeyParsing
    {
        private const string OblivionMaster = "Oblivion.esm";
        private const string OblivionFormKey = "123456Oblivion.esm";

        [Benchmark]
        public FormKey ParseStringFormKey()
        {
            Bethesda.FormKey.TryFactory(OblivionFormKey, out var formKey);
            return formKey;
        }
    }
}
