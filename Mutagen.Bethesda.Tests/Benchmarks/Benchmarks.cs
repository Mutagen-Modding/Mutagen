using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    public static class Benchmarks
    {
        public static void Run()
        {
            //BenchmarkRunner.Run<FormKeyParsing>();
            BenchmarkRunner.Run<OblivionBinaryTranslation>();
            //BenchmarkRunner.Run<StringParsing>();
            //BenchmarkRunner.Run<MiscOverhead>();
            //BenchmarkRunner.Run<Locators>();
            //BenchmarkRunner.Run<HeaderParsing>();
            //BenchmarkRunner.Run<CustomBinarySnippets>();
            //BenchmarkRunner.Run<LoquiTranslationReflection>();
        }
    }
}
