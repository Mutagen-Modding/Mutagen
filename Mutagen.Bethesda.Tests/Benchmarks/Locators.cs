using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Newtonsoft.Json;
using Noggog;
using Mutagen.Bethesda.Plugins.Analysis;

namespace Mutagen.Bethesda.Tests.Benchmarks;

public class Locators
{
    byte[] data;
    MutagenMemoryReadStream stream;

    [GlobalSetup]
    public void Setup()
    {
        // Load Settings
        Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
        FilePath settingsPath = "../../../../TestingSettings.xml";
        Console.WriteLine("Settings path: " + settingsPath);
        var settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsPath.Path));
        Console.WriteLine("Target settings: " + settings.ToString());
        var dataPath = Path.Combine(settings.DataFolderLocations.Oblivion, "Oblivion.esm");
        data = File.ReadAllBytes(dataPath);
        stream = new MutagenMemoryReadStream(data, new ParsingMeta(GameConstants.Oblivion, ModKey.Null, masterReferences: null!));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        data = null;
    }

    [Benchmark]
    public object GetFileLocations()
    {
        stream.Position = 0;
        return RecordLocator.GetLocations(stream);
    }
}