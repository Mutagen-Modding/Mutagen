using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Plugins;

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
            FormKey.TryFactory(OblivionFormKey, out var formKey);
            return formKey;
        }

        [Benchmark]
        public ModKey ParseModKey()
        {
            ModKey.TryFromNameAndExtension(OblivionMaster, out var modKey);
            return modKey;
        }

        [Benchmark]
        public FormID ParseFormID()
        {
            return FormID.Factory(IDString);
        }

        [Benchmark]
        public FormID ParseFormID0x()
        {
            return FormID.Factory(IDString0x);
        }
    }
}
