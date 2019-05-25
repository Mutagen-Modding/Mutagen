using Mutagen.Bethesda.Binary;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public abstract class PassthroughTest
    {
        public string Nickname { get; }
        public FilePath FilePath { get; set; }
        public byte NumMasters { get; }
        public PassthroughSettings Settings { get; }
        public string ExportFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport");
        public string ObservableExportFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_ObservableExport");
        public string UncompressedFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Uncompressed");
        public string AlignedFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Aligned");
        public string OrderedFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered");
        public string ProcessedPath(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Processed");


        public PassthroughTest(TestingSettings settings, Passthrough passthrough)
        {
            this.FilePath = Path.Combine(settings.DataFolderLocations.Get(passthrough.GameMode), passthrough.Path);
            this.Nickname = passthrough.Path;
            this.NumMasters = passthrough.NumMasters;
            this.Settings = settings.PassthroughSettings;
        }

        public abstract ModRecordAligner.AlignmentRules GetAlignmentRules();

        protected virtual BinaryFileProcessor.Config GetInstructions(
            Dictionary<long, uint> lengthTracker,
            RecordLocator.FileLocations fileLocs)
        {
            return new BinaryFileProcessor.Config();
        }

        protected virtual void AddDynamicProcessorInstructions(
            IMutagenReadStream stream,
            byte numMasters,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
        }

        protected virtual void PreProcessorJobs(
            IMutagenReadStream stream,
            RecordLocator.FileLocations fileLocs,
            BinaryFileProcessor.Config instructions,
            RecordLocator.FileLocations alignedFileLocs)
        {
        }

        public async Task<TempFolder> SetupProcessedFiles()
        {
            var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"Mutagen_Binary_Tests/{Nickname}")), deleteAfter: Settings.DeleteCachesAfter);

            var outputPath = ExportFileName(tmp);
            var observableOutputPath = ObservableExportFileName(tmp);
            var uncompressedPath = UncompressedFileName(tmp);
            var alignedPath = AlignedFileName(tmp);
            var orderedPath = OrderedFileName(tmp);
            var preprocessedPath = alignedPath;
            var processedPath = ProcessedPath(tmp);

            if (!Settings.ReuseCaches || !File.Exists(uncompressedPath))
            {
                using (var outStream = new FileStream(uncompressedPath, FileMode.Create, FileAccess.Write))
                {
                    ModDecompressor.Decompress(
                        streamCreator: () => File.OpenRead(this.FilePath.Path),
                        outputStream: outStream);
                }
            }

            if (!Settings.ReuseCaches || !File.Exists(orderedPath))
            {
                using (var outStream = new FileStream(orderedPath, FileMode.Create))
                {
                    ModRecordSorter.Sort(
                        streamCreator: () => File.OpenRead(uncompressedPath),
                        outputStream: outStream);
                }
            }

            if (!Settings.ReuseCaches || !File.Exists(alignedPath))
            {
                ModRecordAligner.Align(
                    inputPath: orderedPath,
                    outputPath: alignedPath,
                    alignmentRules: GetAlignmentRules(),
                    temp: tmp);
            }

            BinaryFileProcessor.Config instructions;
            if (!Settings.ReuseCaches || !File.Exists(processedPath))
            {
                var alignedFileLocs = RecordLocator.GetFileLocations(preprocessedPath);

                Dictionary<long, uint> lengthTracker = new Dictionary<long, uint>();

                using (var reader = new MutagenBinaryReadStream(preprocessedPath))
                {
                    foreach (var grup in alignedFileLocs.GrupLocations.And(alignedFileLocs.ListedRecords.Keys))
                    {
                        reader.Position = grup + 4;
                        lengthTracker[grup] = reader.ReadUInt32();
                    }
                }

                instructions = GetInstructions(
                    lengthTracker,
                    alignedFileLocs);

                using (var stream = new MutagenBinaryReadStream(preprocessedPath))
                {
                    var fileLocs = RecordLocator.GetFileLocations(this.FilePath.Path);
                    PreProcessorJobs(
                        stream: stream,
                        fileLocs: fileLocs,
                        instructions: instructions,
                        alignedFileLocs: alignedFileLocs);
                    foreach (var rec in fileLocs.ListedRecords)
                    {
                        AddDynamicProcessorInstructions(
                            stream: stream,
                            formID: rec.Value.FormID,
                            recType: rec.Value.Record,
                            instr: instructions,
                            loc: alignedFileLocs[rec.Value.FormID],
                            fileLocs: alignedFileLocs,
                            lengthTracker: lengthTracker,
                            numMasters: this.NumMasters);
                    }
                }

                using (var reader = new MutagenBinaryReadStream(preprocessedPath))
                {
                    foreach (var grup in lengthTracker)
                    {
                        reader.Position = grup.Key + 4;
                        if (grup.Value == reader.ReadUInt32()) continue;
                        instructions.SetSubstitution(
                            loc: grup.Key + 4,
                            sub: BitConverter.GetBytes(grup.Value));
                    }
                }

                using (var processor = new BinaryFileProcessor(
                    new FileStream(preprocessedPath, FileMode.Open, FileAccess.Read),
                    instructions))
                {
                    using (var outStream = new FileStream(processedPath, FileMode.Create, FileAccess.Write))
                    {
                        processor.CopyTo(outStream);
                    }
                }
            }

            return tmp;
        }

        protected abstract Task<IMod> ImportBinary(FilePath path, ModKey modKey);

        public async Task BinaryPassthroughTest()
        {
            using (var tmp = await SetupProcessedFiles())
            {
                var outputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport");
                var processedPath = ProcessedPath(tmp);
                var orderedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered");
                var observableOutputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_ObservableExport");

                // Do normal
                if (Settings.TestNormal)
                {
                    ModKey modKey = ModKey.Factory(this.FilePath.Name);
                    var mod = await ImportBinary(orderedPath, modKey);

                    foreach (var record in mod.MajorRecords.Items)
                    {
                        record.IsCompressed = false;
                    }
                    mod.Write_Binary(
                        outputPath,
                        Mutagen.Bethesda.Oblivion.Constants.Oblivion);
                    GC.Collect();

                    using (var stream = new MutagenBinaryReadStream(processedPath))
                    {
                        var ret = Passthrough_Tests.AssertFilesEqual(
                            stream,
                            outputPath,
                            amountToReport: 15);
                        if (ret.Exception != null)
                        {
                            throw ret.Exception;
                        }
                    }
                }

                if (Settings.TestObservable)
                {
                    using (var stream = new MutagenBinaryReadStream(processedPath))
                    {
                        var ret = Passthrough_Tests.AssertFilesEqual(
                            stream,
                            observableOutputPath,
                            amountToReport: 15);
                        if (ret.Exception != null)
                        {
                            throw ret.Exception;
                        }
                    }
                }
            }
        }

        public async Task TestImport()
        {
            ModKey modKey = ModKey.Factory(this.FilePath.Name);
            await ImportBinary(this.FilePath.Path, modKey);
        }

        public static PassthroughTest Factory(TestingSettings settings, Passthrough passthrough)
        {
            switch (passthrough.GameMode)
            {
                case GameMode.Oblivion:
                    return new Oblivion_Passthrough_Test(settings, passthrough);
                case GameMode.Skyrim:
                    return new Skyrim_Passthrough_Test(settings, passthrough);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
