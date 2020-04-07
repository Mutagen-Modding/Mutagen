using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Oblivion;
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
            System.Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
            FilePath settingsPath = "../../../../TestingSettings.xml";
            System.Console.WriteLine("Settings path: " + settingsPath);
            Settings = TestingSettings.CreateFromXml(settingsPath.Path);
            System.Console.WriteLine("Target settings: " + Settings.ToString());

            // Setup folders and paths
            ModKey = new ModKey("Oblivion", true);
            TempFolder = new TempFolder(deleteAfter: true);
            DataPath = Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm");
            XmlFolder = new DirectoryPath(Path.Combine(TempFolder.Dir.Path, "Folder"));
            XmlFolder.Create();

            // Setup
            Mod = OblivionMod.CreateFromBinary(
                DataPath,
                ModKey);
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
            OneTimeXmlFolder = new DirectoryPath(Path.Combine(TempFolder.Dir.Path, "OneTimeFolder"));
            OneTimeXmlFolder.Create();

            // Setup
            Mod = OblivionMod.CreateFromBinary(
                DataPath,
                ModKey);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            TempFolder.Dispose();
        }

        [IterationCleanup]
        public void OneTimeCleanups()
        {
            OneTimeXmlFolder.DeleteContainedFiles(true);
        }

        //[Benchmark]
        //public async Task WriteXmlFolder()
        //{
        //    await Mod.WriteToXmlFolder(
        //        OneTimeXmlFolder);
        //}
    }
}
