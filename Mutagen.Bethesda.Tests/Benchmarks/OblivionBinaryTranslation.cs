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
        public static MemoryStream DataOutput;
        public static BinaryWriteParameters WriteParametersNoCheck = new BinaryWriteParameters()
        {
            MasterFlagSync = BinaryWriteParameters.MasterFlagSyncOption.NoCheck,
            MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
        };

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
            Mod = OblivionMod.CreateFromBinary(
                DataPath,
                ModKey);

            DataOutput = new MemoryStream(new byte[new FileInfo(DataPath).Length]);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            TempFolder.Dispose();
        }

        [Benchmark]
        public async Task CreateBinary()
        {
            OblivionMod.CreateFromBinary(
                DataPath,
                ModKey);
        }

        [Benchmark]
        public void CreateAndWriteBinaryOverlayToDisk()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryOverlay.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            mod.WriteToBinary(BinaryPath, WriteParametersNoCheck);
        }

        [Benchmark]
        public void CreateAndWriteBinaryOverlayToMemory()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryOverlay.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            DataOutput.Position = 0;
            mod.WriteToBinary(DataOutput, WriteParametersNoCheck);
        }

        [Benchmark]
        public void CreateAndWriteBinaryOverlayParallelToDisk()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryOverlay.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            mod.WriteToBinaryParallel(BinaryPath, WriteParametersNoCheck);
        }

        [Benchmark]
        public void CreateAndWriteBinaryOverlayParallelToMemory()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryOverlay.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
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
}
