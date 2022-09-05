using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Newtonsoft.Json;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Tests.Benchmarks;

public class CustomBinarySnippets
{
    public static TestingSettings Settings;
    public static TempFolder ProcessedFilesFolder;
    public static byte[] PathGridBytes;
    public static MutagenMemoryReadStream PathGridReader;

    [GlobalSetup]
    public async Task Setup()
    {
        // Load Settings
        Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
        FilePath settingsPath = "../../../../TestingSettings.xml";
        Console.WriteLine("Settings path: " + settingsPath);
        Settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsPath.Path));
        Console.WriteLine("Target settings: " + Settings.ToString());

        var passthroughSettings = new PassthroughTestParams()
        {
            NicknameSuffix = null,
            PassthroughSettings = Settings.PassthroughSettings,
            GameRelease = GameRelease.Oblivion,
            Target = new Target()
            {
                Path = $"Oblivion.esm",
                Do = true,
            },
        };

        var passthrough = new OblivionPassthroughTest(passthroughSettings);

        (TempFolder TempFolder, Test Test) = passthrough.SetupProcessedFiles();
        using var tmp = TempFolder;
        await Test.Start();
        using (var stream = new BinaryReadStream(passthrough.ProcessedPath(ProcessedFilesFolder.Dir)))
        {
            stream.Position = 0xCF614B;
            PathGridBytes = stream.ReadBytes(0x14C7);
        }
        PathGridReader = new MutagenMemoryReadStream(PathGridBytes, new ParsingBundle(GameRelease.Oblivion, masterReferences: null));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        ProcessedFilesFolder.Dispose();
    }

    [Benchmark]
    public PathGrid PathGridImporting()
    {
        PathGridReader.Position = 0;
        var pathGrid = new PathGrid(FormKey.Null);
        PathGridBinaryCreateTranslation.FillBinaryPointToPointConnections(
            new MutagenFrame(PathGridReader),
            pathGrid);
        return pathGrid;
    }
}