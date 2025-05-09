using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Newtonsoft.Json;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Noggog.IO;

namespace Mutagen.Bethesda.Tests.Benchmarks;

[MemoryDiagnoser]
public class OblivionBinaryTranslation
{
    public static TestingSettings Settings;
    public static OblivionMod Mod;
    public static TempFolder TempFolder;
    public static ModPath DataPath;
    public static string BinaryPath;
    public static MemoryStream DataOutput;
    public static BinaryWriteParameters WriteParametersNoCheck = new()
    {
        MastersListContent = MastersListContentOption.NoCheck,
        Parallel = new ParallelWriteParameters()
        {
            MaxDegreeOfParallelism = 1
        },
    };
    public static BinaryWriteParameters WriteParametersNoCheckParallel = new()
    {
        MastersListContent = MastersListContentOption.NoCheck,
    };

    [GlobalSetup]
    public async Task Setup()
    {
        // Load Settings
        Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
        FilePath settingsPath = "../../../../TestingSettings.xml";
        Console.WriteLine("Settings path: " + settingsPath);
        Settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsPath.Path));
        Console.WriteLine("Target settings: " + Settings.ToString());

        // Setup folders and paths
        DataPath = new ModPath(
            new ModKey("Oblivion", ModType.Master),
            Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm"));
        TempFolder = TempFolder.Factory();
        BinaryPath = Path.Combine(TempFolder.Dir.Path, "Oblivion.esm");

        // Setup
        Mod = OblivionMod.CreateFromBinary(DataPath,
            OblivionRelease.Oblivion);

        DataOutput = new MemoryStream(new byte[new FileInfo(DataPath.Path).Length]);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        TempFolder.Dispose();
    }

    [Benchmark]
    public async Task CreateBinary()
    {
        OblivionMod.CreateFromBinary(DataPath,
            OblivionRelease.Oblivion);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayToDisk()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath,
            OblivionRelease.Oblivion, null);
        mod.WriteToBinary(BinaryPath, WriteParametersNoCheck);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayToMemory()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath,
            OblivionRelease.Oblivion, null);
        DataOutput.Position = 0;
        mod.WriteToBinary(DataOutput, WriteParametersNoCheck);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayParallelToDisk()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath,
            OblivionRelease.Oblivion, null);
        mod.WriteToBinary(BinaryPath, WriteParametersNoCheckParallel);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayParallelToMemory()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath,
            OblivionRelease.Oblivion, null);
        DataOutput.Position = 0;
        mod.WriteToBinary(DataOutput, WriteParametersNoCheckParallel);
    }

    [Benchmark]
    public void WriteBinaryToDisk()
    {
        Mod.WriteToBinary(BinaryPath, WriteParametersNoCheck);
    }

    [Benchmark]
    public void WriteBinaryToMemory()
    {
        DataOutput.Position = 0;
        Mod.WriteToBinary(DataOutput, WriteParametersNoCheck);
    }

    [Benchmark]
    public void WriteBinaryParallelToDisk()
    {
        Mod.WriteToBinary(BinaryPath, WriteParametersNoCheckParallel);
    }

    [Benchmark]
    public void WriteBinaryParallelToMemory()
    {
        DataOutput.Position = 0;
        Mod.WriteToBinary(DataOutput, WriteParametersNoCheckParallel);
    }
}