using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Newtonsoft.Json;
using Noggog;
using Noggog.Utility;

namespace Mutagen.Bethesda.Tests.Benchmarks;

[MemoryDiagnoser]
public class OblivionXmlTranslation
{
    public static TestingSettings Settings;
    public static OblivionMod Mod;
    public static TempFolder TempFolder;
    public static ModKey ModKey;
    public static string DataPath;
    public static string BinaryPath;
    public static DirectoryPath XmlFolder;
    public static DirectoryPath OneTimeXmlFolder;

    [GlobalSetup]
    public async Task Setup()
    {
        // Load Settings
        Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
        FilePath settingsPath = "../../../../TestingSettings.xml";
        Console.WriteLine("Settings path: " + settingsPath);
        Settings = JsonConvert.DeserializeObject<TestingSettings>(settingsPath.Path);
        Console.WriteLine("Target settings: " + Settings.ToString());

        // Setup folders and paths
        ModKey = new ModKey("Oblivion", ModType.Master);
        TempFolder = TempFolder.Factory(deleteAfter: true);
        DataPath = Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm");
        XmlFolder = new DirectoryPath(Path.Combine(TempFolder.Dir.Path, "Folder"));
        XmlFolder.Create();

        // Setup
        Mod = OblivionMod.CreateFromBinary(
            new ModPath(ModKey, DataPath));
        //await Mod.WriteToXmlFolder(XmlFolder, doMasks: false);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        TempFolder.Dispose();
    }

    //[Benchmark]
    //public Task CreateFromXmlFolder()
    //{
    //    return OblivionMod.CreateFromXmlFolder(
    //        XmlFolder,
    //        ModKey);
    //}

    //[Benchmark]
    //public async Task WriteXmlFolderExisting()
    //{
    //    await Mod.WriteToXmlFolder(
    //        XmlFolder);
    //}
}

[MemoryDiagnoser]
public class OblivionXmlCleanWriteTranslation
{
    public static TestingSettings Settings;
    public static OblivionMod Mod;
    public static TempFolder TempFolder;
    public static ModKey ModKey;
    public static string DataPath;
    public static string BinaryPath;
    public static DirectoryPath XmlFolder;
    public static DirectoryPath OneTimeXmlFolder;

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
        ModKey = new ModKey("Oblivion", ModType.Master);
        TempFolder = TempFolder.Factory(deleteAfter: true);
        DataPath = Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm");
        BinaryPath = Path.Combine(TempFolder.Dir.Path, "Oblivion.esm");
        OneTimeXmlFolder = new DirectoryPath(Path.Combine(TempFolder.Dir.Path, "OneTimeFolder"));
        OneTimeXmlFolder.Create();

        // Setup
        Mod = OblivionMod.CreateFromBinary(new ModPath(ModKey, DataPath));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        TempFolder.Dispose();
    }

    [IterationCleanup]
    public void OneTimeCleanups()
    {
        OneTimeXmlFolder.DeleteEntireFolder();
    }

    //[Benchmark]
    //public async Task WriteXmlFolder()
    //{
    //    await Mod.WriteToXmlFolder(
    //        OneTimeXmlFolder);
    //}
}