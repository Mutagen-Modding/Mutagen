using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Newtonsoft.Json;
using Noggog;
using Noggog.Utility;
using System.IO;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins.Binary.Parameters;

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
    public static BinaryWriteParameters WriteParametersNoCheck = new BinaryWriteParameters()
    {
        MastersListContent = MastersListContentOption.NoCheck,
    };

    [GlobalSetup]
    public async Task Setup()
    {
        // Load Settings
        System.Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
        FilePath settingsPath = "../../../../TestingSettings.xml";
        System.Console.WriteLine("Settings path: " + settingsPath);
        Settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsPath.Path));
        System.Console.WriteLine("Target settings: " + Settings.ToString());

        // Setup folders and paths
        DataPath = new ModPath(
            new ModKey("Oblivion", ModType.Master),
            Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm"));
        TempFolder = TempFolder.Factory(deleteAfter: true);
        BinaryPath = Path.Combine(TempFolder.Dir.Path, "Oblivion.esm");

        // Setup
        Mod = OblivionMod.CreateFromBinary(DataPath);

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
        OblivionMod.CreateFromBinary(DataPath);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayToDisk()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath);
        mod.WriteToBinary(BinaryPath, WriteParametersNoCheck);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayToMemory()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath);
        DataOutput.Position = 0;
        mod.WriteToBinary(DataOutput, WriteParametersNoCheck);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayParallelToDisk()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath);
        mod.WriteToBinaryParallel(BinaryPath, WriteParametersNoCheck);
    }

    [Benchmark]
    public void CreateAndWriteBinaryOverlayParallelToMemory()
    {
        var mod = OblivionModBinaryOverlay.OblivionModFactory(DataPath);
        DataOutput.Position = 0;
        mod.WriteToBinaryParallel(DataOutput, WriteParametersNoCheck);
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
        Mod.WriteToBinaryParallel(BinaryPath, WriteParametersNoCheck);
    }

    [Benchmark]
    public void WriteBinaryParallelToMemory()
    {
        DataOutput.Position = 0;
        Mod.WriteToBinaryParallel(DataOutput, WriteParametersNoCheck);
    }
}