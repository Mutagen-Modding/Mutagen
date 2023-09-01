using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Pex;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;
using Noggog.Streams.Binary;
using System.Reactive.Subjects;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings.DI;
using Noggog.IO;

namespace Mutagen.Bethesda.Tests;

public class PassthroughTestParams
{
    public string NicknameSuffix { get; set; } = string.Empty;
    public PassthroughSettings PassthroughSettings { get; set; } = new();
    public GameRelease GameRelease { get; set; }
    public Target Target { get; set; } = new();
}

public abstract class PassthroughTest
{
    public string Nickname { get; }
    public ModPath FilePath { get; set; }
    public PassthroughSettings Settings { get; }
    public Target Target { get; }
    public ModPath ExportFileName(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_NormalExport"));
    public ModPath ObservableExportFileName(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_ObservableExport"));
    public ModPath TrimmedFileName(DirectoryPath path) => Settings.Trimming.Enabled ? new(ModKey, Path.Combine(path, $"{Nickname}_Trimmed")) : FilePath;
    public ModPath MergedFileName(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_Merged"));
    public ModPath UncompressedFileName(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_Uncompressed"));
    public ModPath AlignedFileName(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_Aligned"));
    public ModPath OrderedFileName(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_Ordered"));
    public ModPath ProcessedPath(DirectoryPath path) => new(ModKey, Path.Combine(path, $"{Nickname}_Processed"));
    public ModKey ModKey => FilePath.ModKey;
    public abstract GameRelease GameRelease { get; }
    public readonly GameConstants Meta;
    protected abstract Processor? ProcessorFactory();
    
    public static DirectoryPath GetTestFolderPath(string nickname) => Path.Combine(Path.GetTempPath(), $"Mutagen_Binary_Tests/{nickname}");

    public PassthroughTest(PassthroughTestParams param)
    {
        var path = param.Target.Path;
        FilePath = path;
        Nickname = $"{Path.GetFileName(param.Target.Path)}{param.NicknameSuffix}";
        Settings = param.PassthroughSettings;
        Target = param.Target;
        Meta = GameConstants.Get(GameRelease);
    }

    public abstract AlignmentRules GetAlignmentRules();

    public (TempFolder TempFolder, Test Test) SetupProcessedFiles()
    {
        var tmp = TempFolder.FactoryByPath(GetTestFolderPath(Nickname), deleteAfter: Settings.DeleteCachesAfter);

        var test = new Test(
            $"Setup Processed Files",
            parallel: Settings.ParallelProcessingSteps,
            toDo: async (o) =>
            {
                o.OnNext(Nickname);
                    
                var path = await ExecuteTrimming(tmp.Dir);

                path = await ExecuteDecompression(path, tmp.Dir);

                path = await ExecuteGroupMerging(path, tmp.Dir);

                path = ExecuteAlignment(path, tmp.Dir);

                await ExecuteProcessing(path, tmp.Dir, o);
            });
        test.AddDisposeAction(tmp);
        return (tmp, test);
    }

    private async Task ExecuteProcessing(ModPath prev, DirectoryPath tmp, Subject<string> o)
    {
        var processedPath = ProcessedPath(tmp);
        BinaryFileProcessor.Config instructions;
        if (!Settings.CacheReuse.ReuseProcessing
            || !File.Exists(processedPath))
        {
            instructions = new BinaryFileProcessor.Config();

            var processor = ProcessorFactory();
            if (processor != null)
            {
                await processor.Process(
                    tmpFolder: tmp,
                    logging: o,
                    sourcePath: FilePath,
                    preprocessedPath: prev,
                    outputPath: processedPath);
            }
        }
    }

    private ModPath ExecuteAlignment(ModPath prev, DirectoryPath temp)
    {
        var alignedPath = AlignedFileName(temp);
        
        if (!Settings.CacheReuse.ReuseAlignment
            || !File.Exists(alignedPath))
        {
            ModRecordAligner.Align(
                inputPath: prev,
                outputPath: alignedPath.Path,
                gameMode: GameRelease,
                alignmentRules: GetAlignmentRules(),
                temp: temp);
        }

        return alignedPath;
    }

    private async Task<ModPath> ExecuteDecompression(ModPath prev, DirectoryPath temp)
    {
        var decompressedPath = UncompressedFileName(temp);
        
        if (!Settings.CacheReuse.ReuseDecompression
            || !File.Exists(decompressedPath))
        {
            try
            {
                await using var outStream = new FileStream(decompressedPath, FileMode.Create, FileAccess.Write);
                ModDecompressor.Decompress(
                    streamCreator: () => new MutagenBinaryReadStream(prev, GameRelease),
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

        return decompressedPath;
    }

    private async Task<ModPath> ExecuteGroupMerging(ModPath prev, DirectoryPath temp)
    {
        var path = MergedFileName(temp);
        
        if (!Settings.CacheReuse.ReuseMerge
            || !File.Exists(path))
        {
            try
            {
                await using var outStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                ModGroupMerger.MergeGroups(
                    streamCreator: () => new MutagenBinaryReadStream(prev, GameRelease),
                    outputStream: outStream);
            }
            catch (Exception)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                throw;
            }
        }

        return path;
    }

    private async Task<ModPath> ExecuteTrimming(DirectoryPath tempFolder)
    {
        var trimmedPath = TrimmedFileName(tempFolder);
        if (!Settings.CacheReuse.ReuseTrimming
            || !File.Exists(trimmedPath))
        {
            if (Settings.Trimming.Enabled)
            {
                try
                {
                    await using var outStream = new FileStream(trimmedPath, FileMode.Create, FileAccess.Write);
                    ModTrimmer.Trim(
                        streamCreator: () => new MutagenBinaryReadStream(FilePath, GameRelease),
                        outputStream: outStream,
                        interest: new RecordInterest(
                            uninterestingTypes: Settings.Trimming.TypesToTrim.Select(x => new RecordType(x))));
                }
                catch (Exception)
                {
                    if (File.Exists(trimmedPath))
                    {
                        File.Delete(trimmedPath);
                    }

                    throw;
                }
            }
            else
            {
                trimmedPath = FilePath;
            }
        }

        return trimmedPath;
    }

    protected abstract Task<IMod> ImportBinary(FilePath path);
    protected abstract Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path);
    protected abstract Task<IMod> ImportCopyIn(FilePath file);

    public Test BinaryPassthroughTest()
    {
        (TempFolder tmp, Test processedTest) = SetupProcessedFiles();

        var processedPath = ProcessedPath(tmp.Dir);
        var binaryOverlayPath = Path.Combine(tmp.Dir.Path, $"{Nickname}_BinaryOverlay");
        var copyInPath = Path.Combine(tmp.Dir.Path, $"{Nickname}_CopyIn");
        var strsProcessedPath = Path.Combine(tmp.Dir.Path, "Strings/Processed");
        var trimmedPath = TrimmedFileName(tmp.Dir.Path);

        var masterRefs = MasterReferenceCollection.FromPath(new ModPath(ModKey, FilePath.Path), GameRelease);

        // Do normal
        if (Settings.TestNormal)
        {
            var strsWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{Nickname}_NormalImport_NormalExport");
            var strsParallelWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{Nickname}_NormalImport_ParallelExport");
            bool doStrings = false;
            var passthrough = TestBattery.RunTest(
                "Binary Normal Passthrough",
                GameRelease,
                Target,
                parallel: Settings.ParallelProcessingSteps,
                toDo: async (o) =>
                {
                    o.OnNext(FilePath.ToString());
                    var mod = await ImportBinary(trimmedPath.Path);
                    doStrings = mod.UsingLocalization;

                    foreach (var record in mod.EnumerateMajorRecords())
                    {
                        record.IsCompressed = false;
                    }

                    var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(GameRelease, mod.ModKey, strsWriteDir, MutagenEncodingProvider.Instance) : null);
                    var outputPath = Path.Combine(tmp.Dir.Path, $"{Nickname}_NormalImport_NormalExport");
                    mod.WriteToBinary(outputPath, writeParam);
                    GC.Collect();

                    using var stream = new MutagenBinaryReadStream(processedPath, GameRelease);
                    writeParam.StringsWriter?.Dispose();

                    AssertFilesEqual(
                        stream,
                        outputPath,
                        amountToReport: 15);
                });
            processedTest.AddAsChild(passthrough);
            Test passthroughParallel = null;
            if (Settings.ParallelWriting)
            {
                passthroughParallel = TestBattery.RunTest(
                    "Binary Normal Passthrough Parallel",
                    GameRelease,
                    Target,
                    parallel: Settings.ParallelProcessingSteps,
                    toDo: async (o) =>
                    {
                        o.OnNext(FilePath.ToString());
                        var mod = await ImportBinary(trimmedPath.Path);
                        doStrings = mod.UsingLocalization;

                        foreach (var record in mod.EnumerateMajorRecords())
                        {
                            record.IsCompressed = false;
                        }

                        var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(GameRelease, mod.ModKey, strsParallelWriteDir, MutagenEncodingProvider.Instance) : null);
                        var outputPath = Path.Combine(tmp.Dir.Path, $"{Nickname}_NormalImport_ParallelExport");
                        mod.WriteToBinaryParallel(outputPath, writeParam, parallelWriteParameters: new ParallelWriteParameters() { MaxDegreeOfParallelism = 1 });
                        GC.Collect();

                        using var stream = new MutagenBinaryReadStream(processedPath, GameRelease);
                        writeParam.StringsWriter?.Dispose();

                        AssertFilesEqual(
                            stream,
                            outputPath,
                            amountToReport: 15);
                    });
                processedTest.AddAsChild(passthroughParallel);
            }
            if (doStrings)
            {
                foreach (var item in AssertStringsEqual(
                             "Binary Normal Import Normal Export",
                             strsProcessedPath,
                             strsWriteDir))
                {
                    passthrough.AddAsChild(item);
                }

                if (Settings.ParallelWriting)
                {
                    foreach (var item in AssertStringsEqual(
                                 "Binary Normal Import Parallel Export",
                                 strsProcessedPath,
                                 strsParallelWriteDir))
                    {
                        passthroughParallel.AddAsChild(item);
                    }
                }
            }
        }

        if (Settings.TestBinaryOverlay)
        {
            var strsWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{Nickname}_Overlay");
            bool doStrings = false;
            var passthrough = TestBattery.RunTest(
                "Binary Overlay Passthrough",
                GameRelease,
                Target,
                parallel: Settings.ParallelProcessingSteps,
                toDo: async (o) =>
                {
                    o.OnNext(FilePath.ToString());
                    using (var wrapper = await ImportBinaryOverlay(trimmedPath))
                    {
                        doStrings = wrapper.UsingLocalization;
                        var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(GameRelease, wrapper.ModKey, strsWriteDir, MutagenEncodingProvider.Instance) : null);
                        wrapper.WriteToBinary(binaryOverlayPath, writeParam);
                        writeParam.StringsWriter?.Dispose();
                    }

                    using var stream = new MutagenBinaryReadStream(processedPath, GameRelease);

                    AssertFilesEqual(
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
            var strsWriteDir = Path.Combine(tmp.Dir.Path, "Strings", $"{Nickname}_CopyIn");
            bool doStrings = false;
            var passthrough = TestBattery.RunTest(
                "Copy In Passthrough",
                GameRelease,
                Target,
                parallel: Settings.ParallelProcessingSteps,
                toDo: async (o) =>
                {
                    o.OnNext(FilePath.ToString());
                    var copyIn = await ImportCopyIn(trimmedPath);
                    doStrings = copyIn.UsingLocalization;
                    var writeParam = GetWriteParam(masterRefs, doStrings ? new StringsWriter(GameRelease, copyIn.ModKey, strsWriteDir, MutagenEncodingProvider.Instance) : null);
                    copyIn.WriteToBinary(copyInPath, writeParam);
                    writeParam.StringsWriter?.Dispose();

                    using var stream = new MutagenBinaryReadStream(processedPath, GameRelease);

                    AssertFilesEqual(
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
            await ImportBinary(FilePath.Path);
        });
    }

    public Test TestEquality()
    {
        return TestBattery.RunTest("Equals", GameRelease, Target, async (output) =>
        {
            var mod = await ImportBinaryOverlay(FilePath.Path);
            var eqMask = mod.GetEqualsMask(mod);
            if (!eqMask.All(b => b))
            {
                throw new Exception("Mod mask did not equal itself");
            }
            Console.WriteLine("Equals mask clean.");
            if (!mod.Equals(mod))
            {
                throw new Exception("Mod did not equal itself");
            }
            Console.WriteLine("Direct equals matched.");
        });
    }

    public Test TestPex()
    {
        return TestBattery.RunTest("Pex", GameRelease, Target, async (output) =>
        {
            IEnumerable<FileName> bsas;
            if (Implicits.Get(GameRelease).BaseMasters.Contains(FilePath.ModKey))
            {
                bsas = Archive.GetIniListings(GameRelease).ToList();
            }
            else
            {
                bsas = new FileName($"{FilePath.ModKey.Name}.{Archive.GetExtension(GameRelease)}").AsEnumerable();
            }
            foreach (var bsa in bsas)
            {
                var archive = Archive.CreateReader(GameRelease, Path.Combine(Path.GetDirectoryName(FilePath)!, bsa.String));
                foreach (var file in archive.Files)
                {
                    if (!Path.GetExtension(file.Path).Equals(".pex", StringComparison.OrdinalIgnoreCase)) continue;
                    TestPex(GameRelease, file.GetMemorySlice());
                }
            }
        });
    }

    public void TestPex(GameRelease release, ReadOnlyMemorySlice<byte> bytes)
    {
        var memStream = BinaryMemoryReadStream.LittleEndian(bytes);
        PexFile.CreateFromStream(memStream, release.ToCategory());
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

    public BinaryWriteParameters GetWriteParam(IReadOnlyMasterReferenceCollection masterRefs, StringsWriter stringsWriter)
    {
        return new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck,
            MastersListContent = MastersListContentOption.NoCheck,
            RecordCount = RecordCountOption.NoCheck,
            NextFormID = NextFormIDOption.NoCheck,
            FormIDUniqueness = FormIDUniquenessOption.NoCheck,
            MasterFlag = MasterFlagOption.NoCheck,
            MastersListOrdering = AMastersListOrderingOption.ByMasters(masterRefs),
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
            GameRelease.Starfield => new Fallout4PassthroughTest(passthroughSettings),
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
        foreach (var source in Enums<StringsSource>.Values)
        {
            var stringsFileName = StringsUtility.GetFileName(GameRelease.GetLanguageFormat(), ModKey, Language.English, source);
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
                parallel: Settings.ParallelProcessingSteps);
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