using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Newtonsoft.Json;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    public class Locators
    {
        byte[] data;
        MutagenMemoryReadStream stream;

        [GlobalSetup]
        public void Setup()
        {
            // Load Settings
            System.Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
            FilePath settingsPath = "../../../../TestingSettings.xml";
            System.Console.WriteLine("Settings path: " + settingsPath);
            var settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsPath.Path));
            System.Console.WriteLine("Target settings: " + settings.ToString());
            var dataPath = Path.Combine(settings.DataFolderLocations.Oblivion, "Oblivion.esm");
            data = File.ReadAllBytes(dataPath);
            stream = new MutagenMemoryReadStream(data, new ParsingBundle(GameConstants.Oblivion));
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            data = null;
        }

        [Benchmark]
        public object BaseGRUPIterator()
        {
            stream.Position = 0;
            return RecordLocator.IterateBaseGroupLocations(stream);
        }

        [Benchmark]
        public object GetFileLocations()
        {
            stream.Position = 0;
            return RecordLocator.GetFileLocations(stream);
        }
    }
}
