using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Records.Binary;
using Mutagen.Bethesda.Records.Binary.Processing;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Utility;
using Mutagen.Bethesda.Strings;
using Noggog;
using Noggog.Streams.Binary;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public ModPath FilePath { get; set; }
        public PassthroughSettings Settings { get; }
        public Target Target { get; }
        public ModPath ExportFileName(TempFolder tmp) => new ModPath(ModKey, Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport"));
        public ModPath ObservableExportFileName(TempFolder tmp) => new ModPath(ModKey, Path.Combine(tmp.Dir.Path, $"{this.Nickname}_ObservableExport"));
        public ModPath UncompressedFileName(TempFolder tmp) => new ModPath(ModKey, Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Uncompressed"));
        public ModPath AlignedFileName(TempFolder tmp) => new ModPath(ModKey, Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Aligned"));
        public ModPath OrderedFileName(TempFolder tmp) => new ModPath(ModKey, Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered"));
        public ModPath ProcessedPath(TempFolder tmp) => new ModPath(ModKey, Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Processed"));
        public ModKey ModKey => FilePath.ModKey;
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
            var tmp = TempFolder.FactoryByPath(Path.Combine(Path.GetTempPath(), $"Mutagen_Binary_Tests/{Nickname}"), deleteAfter: Settings.DeleteCachesAfter);

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
                                streamCreator: () => new MutagenBinaryReadStream(this.FilePath, this.GameRelease),
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
                            outputPath: alignedPath.Path,
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
                                logging: o,
                                sourcePath: this.FilePath,
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

            var masterRefs = MasterReferenceReader.FromPath(new ModPath(ModKey, this.FilePath.Path), GameRelease);

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
                        doStrings = mod.UsingLocalization;

                        foreach (var record in mod.EnumerateMajorRecords())
                        {
                            record.IsCompressed = false;
                        }

                        var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(this.GameRelease, mod.ModKey, strsWriteDir) : null);
                        mod.WriteToBinary(outputPath, writeParam);
                        GC.Collect();

                        using var stream = new MutagenBinaryReadStream(processedPath, this.GameRelease);
                        writeParam.StringsWriter?.Dispose();

                        AssertFilesEqual(
                            stream,
                            outputPath,
                            amountToReport: 15);
                    });
                processedTest.AddAsChild(passthrough);
                if (doStrings)
                {
                    foreach (var item in AssertStringsEqual(
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
                            doStrings = wrapper.UsingLocalization;
                            var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(this.GameRelease, wrapper.ModKey, strsWriteDir) : null);
                            wrapper.WriteToBinary(binaryOverlayPath, writeParam);
                            writeParam.StringsWriter?.Dispose();
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
                        doStrings = copyIn.UsingLocalization;
                        var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(this.GameRelease, copyIn.ModKey, strsWriteDir) : null);
                        copyIn.WriteToBinary(copyInPath, writeParam);
                        writeParam.StringsWriter?.Dispose();

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

        public Test TestImport()
        {
            return TestBattery.RunTest("Test Import", GameRelease, Target, async (output) =>
            {
                await ImportBinary(this.FilePath.Path);
            });
        }

        public Test TestEquality()
        {
            return TestBattery.RunTest("Equals", GameRelease, Target, async (output) =>
            {
                var mod = await ImportBinaryOverlay(this.FilePath.Path);
                var eqMask = mod.GetEqualsMask(mod);
                if (!eqMask.All(b => b))
                {
                    throw new Exception("Mod mask did not equal itself");
                }
                System.Console.WriteLine("Equals mask clean.");
                if (!mod.Equals(mod))
                {
                    throw new Exception("Mod did not equal itself");
                }
                System.Console.WriteLine("Direct equals matched.");
            });
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

        public BinaryWriteParameters GetWriteParam(MasterReferenceReader masterRefs, StringsWriter stringsWriter)
        {
            return new BinaryWriteParameters()
            {
                ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                MastersListContent = BinaryWriteParameters.MastersListContentOption.NoCheck,
                RecordCount = BinaryWriteParameters.RecordCountOption.NoCheck,
                NextFormID = BinaryWriteParameters.NextFormIDOption.NoCheck,
                FormIDUniqueness = BinaryWriteParameters.FormIDUniquenessOption.NoCheck,
                MasterFlag = BinaryWriteParameters.MasterFlagOption.NoCheck,
                MastersListOrdering = masterRefs,
                StringsWriter = stringsWriter,
            };
        }

        public static PassthroughTest Factory(PassthroughTestParams passthroughSettings)
        {
            return passthroughSettings.GameRelease switch
            {
                GameRelease.Oblivion => new OblivionPassthroughTest(passthroughSettings),
                GameRelease.SkyrimLE => new SkyrimPassthroughTest(passthroughSettings, GameRelease.SkyrimLE),
                GameRelease.SkyrimSE => new SkyrimPassthroughTest(passthroughSettings, GameRelease.SkyrimSE),
                GameRelease.SkyrimVR => new SkyrimPassthroughTest(passthroughSettings, GameRelease.SkyrimVR),
                GameRelease.Fallout4 => new Fallout4PassthroughTest(passthroughSettings),
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
                var stringsFileName = StringsUtility.GetFileName(this.GameRelease.GetLanguageFormat(), this.ModKey, Language.English, source);
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
