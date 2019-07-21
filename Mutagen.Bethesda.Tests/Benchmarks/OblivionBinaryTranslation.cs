using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    [MemoryDiagnoser]
    public class OblivionBinaryTranslation
    {
        public static TestingSettings Settings;
        public static OblivionMod Mod;
        public static TempFolder TempFolder;
        public static ModKey ModKey;
        public static string DataPath;
        public static string BinaryPath;

        [GlobalSetup]
        public async Task Setup()
        {
            // Load Settings
            System.Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
            FilePath settingsPath = "../../../../TestingSettings.xml";
            System.Console.WriteLine("Settings path: " + settingsPath);
            Settings = TestingSettings.CreateFromXml(settingsPath.Path);
            System.Console.WriteLine("Target settings: " + Settings.ToString());

            // Setup folders and paths
            ModKey = new ModKey("Oblivion", true);
            TempFolder = new TempFolder(deleteAfter: true);
            DataPath = Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm");
            BinaryPath = Path.Combine(TempFolder.Dir.Path, "Oblivion.esm");

            // Setup
            Mod = await OblivionMod.CreateFromBinary(
                DataPath,
                ModKey);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            TempFolder.Dispose();
        }

        [Benchmark]
        public void CreateBinary()
        {
            OblivionMod.CreateFromBinary(
                DataPath,
                ModKey).Wait();
        }

        [Benchmark]
        public void WriteBinary()
        {
            Mod.WriteToBinary(
                BinaryPath,
                ModKey);
        }
    }
}
