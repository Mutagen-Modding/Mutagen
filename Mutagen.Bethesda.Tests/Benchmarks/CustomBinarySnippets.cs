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
            System.Console.WriteLine("Running in directory: " + Directory.GetCurrentDirectory());
            FilePath settingsPath = "../../../../TestingSettings.xml";
            System.Console.WriteLine("Settings path: " + settingsPath);
            Settings = TestingSettings.CreateFromXml(settingsPath.Path);
            System.Console.WriteLine("Target settings: " + Settings.ToString());

            var passthrough = new OblivionPassthroughTest(Settings, new Target()
            {
                Path = $"Oblivion.esm",
                Do = true,
                GameMode = GameMode.Oblivion,
            });

            ProcessedFilesFolder = await passthrough.SetupProcessedFiles();
            using (var stream = new BinaryReadStream(passthrough.ProcessedPath(ProcessedFilesFolder)))
            {
                stream.Position = 0xCF614B;
                PathGridBytes = stream.ReadBytes(0x14C7);
            }
            PathGridReader = new MutagenMemoryReadStream(PathGridBytes, new ParsingBundle(GameMode.Oblivion));
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
            PathGridBinaryCreateTranslation.FillBinaryPointToPointConnectionsCustomPublic(
                new Binary.MutagenFrame(PathGridReader),
                pathGrid);
            return pathGrid;
        }
    }
}
