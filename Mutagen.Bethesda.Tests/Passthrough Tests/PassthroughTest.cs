using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Preprocessing;
using Noggog;
using Noggog.Extensions;
using Noggog.Streams.Binary;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public class PassthroughTestParams
    {
        public string NicknameSuffix { get; set; } = string.Empty;
        public PassthroughSettings PassthroughSettings { get; set; } = new PassthroughSettings();
        public GameRelease GameRelease { get; set; }
        public Target Target { get; set; } = new Target();
    }

    public abstract class PassthroughTest
    {
        public string Nickname { get; }
        public FilePath FilePath { get; set; }
        public PassthroughSettings Settings { get; }
        public Target Target { get; }
        public string ExportFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport");
        public string ObservableExportFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_ObservableExport");
        public string UncompressedFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Uncompressed");
        public string AlignedFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Aligned");
        public string OrderedFileName(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered");
        public string ProcessedPath(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Processed");
        public ModKey ModKey => ModKey.Factory(this.FilePath.Name);
        public abstract GameRelease GameRelease { get; }
        public readonly GameConstants Meta;
        protected abstract Processor ProcessorFactory();

        public PassthroughTest(PassthroughTestParams param)
        {
            var path = param.Target.Path;
            this.FilePath = path;
            this.Nickname = $"{Path.GetFileName(param.Target.Path)}{param.NicknameSuffix}";
            this.Settings = param.PassthroughSettings;
            this.Target = param.Target;
            this.Meta = GameConstants.Get(this.GameRelease);
        }

        public abstract ModRecordAligner.AlignmentRules GetAlignmentRules();

        public (TempFolder TempFolder, Test Test) SetupProcessedFiles()
        {
            var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"Mutagen_Binary_Tests/{Nickname}")), deleteAfter: Settings.DeleteCachesAfter);

            var test = new Test(
                $"Setup Processed Files",
                parallel: Settings.Parallel,
                toDo: async (o) =>
                {
                    o.OnNext(this.Nickname);
                    var outputPath = ExportFileName(tmp);
                    var observableOutputPath = ObservableExportFileName(tmp);
                    var decompressedPath = UncompressedFileName(tmp);
                    var alignedPath = AlignedFileName(tmp);
                    var orderedPath = OrderedFileName(tmp);
                    var preprocessedPath = alignedPath;
                    var processedPath = ProcessedPath(tmp);

                    if (!Settings.CacheReuse.ReuseDecompression
                        || !File.Exists(decompressedPath))
                    {
                        try
                        {
                            using var outStream = new FileStream(decompressedPath, FileMode.Create, FileAccess.Write);
                            ModDecompressor.Decompress(
                                streamCreator: () => File.OpenRead(this.FilePath.Path),
                                release: this.GameRelease,
                                outputStream: outStream);
                        }
                        catch (Exception)
                        {
                            if (File.Exists(decompressedPath))
                            {
                                File.Delete(decompressedPath);
                            }
                            throw;
                        }
                    }

                    if (!Settings.CacheReuse.ReuseAlignment
                        || !File.Exists(alignedPath))
                    {
                        ModRecordAligner.Align(
                            inputPath: decompressedPath,
                            outputPath: alignedPath,
                            gameMode: this.GameRelease,
                            alignmentRules: GetAlignmentRules(),
                            temp: tmp);
                    }

                    BinaryFileProcessor.Config instructions;
                    if (!Settings.CacheReuse.ReuseProcessing
                        || !File.Exists(processedPath))
                    {
                        instructions = new BinaryFileProcessor.Config();

                        var processor = this.ProcessorFactory();
                        if (processor != null)
                        {
                            await processor.Process(
                                tmpFolder: tmp,
                                sourcePath: this.FilePath.Path,
                                preprocessedPath: alignedPath,
                                outputPath: processedPath);
                        }
                    }
                });
            test.AddDisposeAction(tmp);
            return (tmp, test);
        }

        protected abstract Task<IMod> ImportBinary(FilePath path);
        protected abstract Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path);
        protected abstract Task<IMod> ImportCopyIn(FilePath file);

        public Test BinaryPassthroughTest()
        {
            (TempFolder tmp, Test processedTest) = SetupProcessedFiles();

            var outputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport");
            var processedPath = ProcessedPath(tmp);
            var orderedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered");
            var binaryOverlayPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_BinaryOverlay");
            var copyInPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_CopyIn");
            var strsProcessedPath = Path.Combine(tmp.Dir.Path, "Strings/Processed");

            var writeParams = new BinaryWriteParameters()
            {
                ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
            };

            // Do normal
            if (Settings.TestNormal)
            {
                var strsWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{this.Nickname}_Normal");
                bool doStrings = false;
                var passthrough = TestBattery.RunTest(
                    "Binary Normal Passthrough",
                    this.GameRelease,
                    this.Target,
                    parallel: Settings.Parallel,
                    toDo: async (o) =>
                    {
                        o.OnNext(FilePath.ToString());
                        var mod = await ImportBinary(this.FilePath.Path);
                        doStrings = mod.CanUseLocalization;

                        foreach (var record in mod.EnumerateMajorRecords())
                        {
                            record.IsCompressed = false;
                        }

                        using var stringsWriter = new StringsWriter(mod.ModKey, strsWriteDir);
                        writeParams.StringsWriter = stringsWriter;
                        mod.WriteToBinary(outputPath, writeParams);
                        GC.Collect();

                        using var stream = new MutagenBinaryReadStream(processedPath, this.GameRelease);

                        AssertFilesEqual(
                            stream,
                            outputPath,
                            amountToReport: 15);
                    });
                processedTest.AddAsChild(passthrough);
                if (doStrings)
                {
                    foreach(var item in AssertStringsEqual(
                        "Binary Normal",
                        strsProcessedPath,
                        strsWriteDir))
                    {
                        passthrough.AddAsChild(item);
                    }
                }
            }

            if (Settings.TestBinaryOverlay)
            {
                var strsWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{this.Nickname}_Overlay");
                bool doStrings = false;
                var passthrough = TestBattery.RunTest(
                    "Binary Overlay Passthrough",
                    this.GameRelease,
                    this.Target,
                    parallel: Settings.Parallel,
                    toDo: async (o) =>
                    {
                        o.OnNext(FilePath.ToString());
                        using (var wrapper = await ImportBinaryOverlay(this.FilePath.Path))
                        {
                            doStrings = wrapper.CanUseLocalization;
                            using var stringsWriter = new StringsWriter(wrapper.ModKey, strsWriteDir);
                            writeParams.StringsWriter = stringsWriter;
                            wrapper.WriteToBinary(binaryOverlayPath, writeParams);
                        }

                        using var stream = new MutagenBinaryReadStream(processedPath, this.GameRelease);

                        PassthroughTest.AssertFilesEqual(
                            stream,
                            binaryOverlayPath,
                            amountToReport: 15);
                    });
                processedTest.AddAsChild(passthrough);
                if (doStrings)
                {
                    foreach (var item in AssertStringsEqual(
                        "Binary Overlay",
                        strsProcessedPath,
                        strsWriteDir))
                    {
                        passthrough.AddAsChild(item);
                    }
                }
            }

            if (Settings.TestCopyIn)
            {
                var strsWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{this.Nickname}_CopyIn");
                bool doStrings = false;
                var passthrough = TestBattery.RunTest(
                    "Copy In Passthrough",
                    this.GameRelease,
                    this.Target,
                    parallel: Settings.Parallel,
                    toDo: async (o) =>
                    {
                        o.OnNext(FilePath.ToString());
                        var copyIn = await ImportCopyIn(this.FilePath.Path);
                        using var stringsWriter = new StringsWriter(copyIn.ModKey, strsWriteDir);
                        writeParams.StringsWriter = stringsWriter;
                        copyIn.WriteToBinary(copyInPath, writeParams);

                        using var stream = new MutagenBinaryReadStream(processedPath, this.GameRelease);

                        PassthroughTest.AssertFilesEqual(
                            stream,
                            copyInPath,
                            amountToReport: 15);
                    });
                processedTest.AddAsChild(passthrough);
                if (doStrings)
                {
                    foreach (var item in AssertStringsEqual(
                        "Copy In",
                        strsProcessedPath,
                        strsWriteDir))
                    {
                        passthrough.AddAsChild(item);
                    }
                }
            }
            return processedTest;
        }

        public async Task TestImport(Subject<string> output)
        {
            await ImportBinary(this.FilePath.Path);
        }

        public static PassthroughTest Factory(TestingSettings settings, TargetGroup group, Target target)
        {
            return Factory(new PassthroughTestParams()
            {
                NicknameSuffix = group.NicknameSuffix,
                PassthroughSettings = settings.PassthroughSettings,
                Target = target,
                GameRelease = group.GameRelease,
            });
        }

        public static PassthroughTest Factory(PassthroughTestParams passthroughSettings)
        {
            return passthroughSettings.GameRelease switch
            {
                GameRelease.Oblivion => new OblivionPassthroughTest(passthroughSettings),
                GameRelease.SkyrimLE => new SkyrimPassthroughTest(passthroughSettings, GameRelease.SkyrimLE),
                GameRelease.SkyrimSE => new SkyrimPassthroughTest(passthroughSettings, GameRelease.SkyrimSE),
                _ => throw new NotImplementedException(),
            };
        }

        public static void AssertFilesEqual(
            Stream stream,
            string path2,
            RangeCollection ignoreList = null,
            ushort amountToReport = 5)
        {
            using var reader2 = new BinaryReadStream(path2);
            Stream compareStream = new ComparisonStream(
                stream,
                reader2);

            if (ignoreList != null)
            {
                compareStream = new BasicSubstitutionRangeStream(
                    compareStream,
                    ignoreList,
                    toSubstitute: 0);
            }

            var errs = GetDifferences(compareStream)
                .First(amountToReport)
                .ToArray();
            if (errs.Length > 0)
            {
                throw new DidNotMatchException(path2, errs, stream);
            }
            if (stream.Position != stream.Length)
            {
                throw new MoreDataException(path2, stream.Position);
            }
            if (reader2.Position != reader2.Length)
            {
                throw new UnexpectedlyMoreData(path2, reader2.Position);
            }
        }

        public IEnumerable<Test> AssertStringsEqual(
            string nickname,
            DirectoryPath processedDir, 
            DirectoryPath writeDir)
        {
            foreach (var source in EnumExt.GetValues<StringsSource>())
            {
                var stringsFileName = StringsUtility.GetFileName(this.ModKey, Language.English, source);
                var sourcePath = Path.Combine(processedDir.Path, stringsFileName);
                var pathToTest = Path.Combine(writeDir.Path, stringsFileName);
                bool sourceExists = File.Exists(sourcePath);
                bool targetExists = File.Exists(pathToTest);
                yield return TestBattery.RunTest($"{nickname} {source} Strings Passthrough",
                    async (o) =>
                    {
                        if (sourceExists != targetExists)
                        {
                            throw new ArgumentException($"Strings file presence did not match for source: {source}");
                        }
                        if (!sourceExists) return;
                        AssertFilesEqual(
                            new FileStream(sourcePath, FileMode.Open),
                            pathToTest);
                    },
                    parallel: Settings.Parallel);
            }
        }

        public static IEnumerable<RangeInt64> GetDifferences(Stream reader)
        {
            byte[] buf = new byte[4096];
            bool inRange = false;
            long startRange = 0;
            var len = reader.Length;
            long pos = 0;
            while (pos < len)
            {
                var read = reader.Read(buf, 0, buf.Length);
                for (int i = 0; i < read; i++)
                {
                    if (buf[i] != 0)
                    {
                        if (!inRange)
                        {
                            startRange = pos + i;
                            inRange = true;
                        }
                    }
                    else
                    {
                        if (inRange)
                        {
                            var sourceRange = new RangeInt64(startRange, pos + i);
                            yield return sourceRange;
                            inRange = false;
                        }
                    }
                }
                pos += read;
            }
        }
    }
}
