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
            await OblivionMod.CreateFromBinary(
                DataPath,
                ModKey);
        }

        [Benchmark]
        public void CreateAndWriteBinaryWrapperToDisk()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryWrapper.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            mod.WriteToBinary(
                BinaryPath,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void CreateAndWriteBinaryWrapperToMemory()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryWrapper.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            DataOutput.Position = 0;
            mod.WriteToBinary(
                DataOutput,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void CreateAndWriteBinaryWrapperParallelToDisk()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryWrapper.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            mod.WriteToBinaryParallel(
                BinaryPath,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void CreateAndWriteBinaryWrapperParallelToMemory()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryWrapper.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            DataOutput.Position = 0;
            mod.WriteToBinaryParallel(
                DataOutput,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public async Task CreateAndWriteBinaryWrapperAsyncToDisk()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryWrapper.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            await mod.WriteToBinaryAsync(
                BinaryPath,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public async Task CreateAndWriteBinaryWrapperAsyncToMemory()
        {
            var bytes = File.ReadAllBytes(DataPath);
            var mod = OblivionModBinaryWrapper.OblivionModFactory(
                new MemorySlice<byte>(bytes),
                ModKey);
            DataOutput.Position = 0;
            await mod.WriteToBinaryAsync(
                DataOutput,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void WriteBinaryToDisk()
        {
            Mod.WriteToBinary(
                BinaryPath,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void WriteBinaryToMemory()
        {
            DataOutput.Position = 0;
            Mod.WriteToBinary(
                DataOutput,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void WriteBinaryParallelToDisk()
        {
            Mod.WriteToBinaryParallel(
                BinaryPath,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public void WriteBinaryParallelToMemory()
        {
            DataOutput.Position = 0;
            Mod.WriteToBinaryParallel(
                DataOutput,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public async Task WriteBinaryAsyncToDisk()
        {
            await Mod.WriteToBinaryAsync(
                BinaryPath,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }

        [Benchmark]
        public async Task WriteBinaryAsyncToMemory()
        {
            DataOutput.Position = 0;
            await Mod.WriteToBinaryAsync(
                DataOutput,
                Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        }
    }
}
