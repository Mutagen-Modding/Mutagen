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
        public static TempFolder ProcessedFilesFolder;
        public static byte[] PathGridBytes;
        public static MutagenMemoryReadStream PathGridReader;

        [GlobalSetup]
        public async Task Setup()
        {
            // Load Settings
            System.Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
            FilePath settingsPath = "../../../../TestingSettings.xml";
            System.Console.WriteLine("Settings path: " + settingsPath);
            Settings = TestingSettings.Create_Xml(settingsPath.Path);
            System.Console.WriteLine("Target settings: " + Settings.ToString());

            // Setup folders and paths
            ModKey = new ModKey("Oblivion", true);
            TempFolder = new TempFolder(deleteAfter: true);
            DataPath = Path.Combine(Settings.DataFolderLocations.Oblivion, "Oblivion.esm");
            BinaryPath = Path.Combine(TempFolder.Dir.Path, "Oblivion.esm");

            // Setup
            Mod = await OblivionMod.Create_Binary(
                DataPath,
                ModKey);

            var passthrough = new Oblivion_Passthrough_Test(Settings, new Target()
            {
                Path = BinaryPath,
                Do = true,
                GameMode = GameMode.Oblivion,
            });
            ProcessedFilesFolder = await passthrough.SetupProcessedFiles();
            using (var stream = new BinaryReadStream(passthrough.ProcessedPath(ProcessedFilesFolder)))
            {
                stream.Position = 0xCF614B;
                PathGridBytes = stream.ReadBytes(0x14C7);
            }
            PathGridReader = new MutagenMemoryReadStream(PathGridBytes);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            TempFolder.Dispose();
            ProcessedFilesFolder.Dispose();
        }

        [Benchmark]
        public void CreateBinary()
        {
            OblivionMod.Create_Binary(
                DataPath,
                ModKey).Wait();
        }

        [Benchmark]
        public void WriteBinary()
        {
            Mod.Write_Binary(
                BinaryPath,
                ModKey);
        }

        [Benchmark]
        public PathGrid PathGridImporting()
        {
            PathGridReader.Position = 0;
            var pathGrid = new PathGrid(FormKey.NULL);
            PathGridBinaryTranslation.FillBinary_PointToPointConnections_Custom_Public(
                new Binary.MutagenFrame(PathGridReader),
                pathGrid,
                masterReferences: null,
                errorMask: null);
            return pathGrid;
        }
    }
}
