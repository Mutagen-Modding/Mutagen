using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    [MemoryDiagnoser]
    public class FormKeyParsing
    {
        private const string OblivionMaster = "Oblivion.esm";
        private const string IDString = "01123456";
        private const string IDString0x = "0x01123456";
        private readonly string OblivionFormKey = $"123456Oblivion.esm";

        [Benchmark]
        public FormKey ParseFormKey()
        {
            Bethesda.FormKey.TryFactory(OblivionFormKey, out var formKey);
            return formKey;
        }

        [Benchmark]
        public ModKey ParseModKey()
        {
            Bethesda.ModKey.TryFactory(OblivionMaster, out var modKey);
            return modKey;
        }

        [Benchmark]
        public FormID ParseFormID()
        {
            return Bethesda.FormID.Factory(IDString);
        }

        [Benchmark]
        public FormID ParseFormID0x()
        {
            return Bethesda.FormID.Factory(IDString0x);
        }
    }
}
