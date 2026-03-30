#nullable enable
using System.Diagnostics;
using System.Text.Json;
using CommandLine;
using Mutagen.Bethesda.Installs.DI;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("passthrough")]
public class PassthroughCommand
{
    [Option('m', "mod", Required = true, HelpText = "Path to mod file")]
    public string PathToMod { get; set; } = string.Empty;

    [Option('r', "release", Required = true, HelpText = "GameRelease enum value")]
    public GameRelease Release { get; set; }

    [Option('d', "data-folder", HelpText = "Override data folder path")]
    public string? DataFolder { get; set; }

    [Option('n', "nickname", HelpText = "Nickname suffix for cache folder")]
    public string NicknameSuffix { get; set; } = string.Empty;

    // Test mode flags
    [Option("test-normal", Default = true, HelpText = "Run normal import/export test")]
    public bool TestNormal { get; set; }

    [Option("test-overlay", Default = true, HelpText = "Run binary overlay test")]
    public bool TestOverlay { get; set; }

    [Option("test-copyin", Default = false, HelpText = "Run copy-in test")]
    public bool TestCopyIn { get; set; }

    // Cache control flags
    [Option("cache-all", Default = false, HelpText = "Enable all 5 cache reuse flags")]
    public bool CacheAll { get; set; }

    [Option("no-cache", Default = false, HelpText = "Disable all 5 cache reuse flags")]
    public bool NoCache { get; set; }

    [Option("cache-decompress", Default = false, HelpText = "Reuse decompression cache")]
    public bool CacheDecompress { get; set; }

    [Option("cache-merge", Default = false, HelpText = "Reuse merge cache")]
    public bool CacheMerge { get; set; }

    [Option("cache-align", Default = false, HelpText = "Reuse alignment cache")]
    public bool CacheAlign { get; set; }

    [Option("cache-process", Default = false, HelpText = "Reuse processing cache")]
    public bool CacheProcess { get; set; }

    [Option("cache-trim", Default = false, HelpText = "Reuse trimming cache")]
    public bool CacheTrim { get; set; }

    // Trimming
    [Option("include", Separator = ',', HelpText = "Comma-separated record types to include (enables trimming)")]
    public IEnumerable<string> Include { get; set; } = Array.Empty<string>();

    [Option("exclude", Separator = ',', HelpText = "Comma-separated record types to exclude (enables trimming)")]
    public IEnumerable<string> Exclude { get; set; } = Array.Empty<string>();

    // Output format
    [Option("json", Default = false, HelpText = "Emit JSON output")]
    public bool Json { get; set; }

    public CacheReuse BuildCacheReuse()
    {
        if (CacheAll)
            return new CacheReuse(true);
        if (NoCache)
            return new CacheReuse(false);

        return new CacheReuse
        {
            ReuseDecompression = CacheDecompress,
            ReuseMerge = CacheMerge,
            ReuseAlignment = CacheAlign,
            ReuseProcessing = CacheProcess,
            ReuseTrimming = CacheTrim
        };
    }

    public TrimmingSettings BuildTrimmingSettings()
    {
        var includeList = Include.ToList();
        var excludeList = Exclude.ToList();
        var enabled = includeList.Count > 0 || excludeList.Count > 0;

        return new TrimmingSettings
        {
            Enabled = enabled,
            TypesToInclude = includeList,
            TypesToTrim = excludeList
        };
    }

    public async Task<int> Run()
    {
        var overallSw = new Stopwatch();
        overallSw.Start();

        try
        {
            string dataDir;
            if (!string.IsNullOrEmpty(DataFolder))
            {
                dataDir = DataFolder;
            }
            else
            {
                var locator = GameLocatorLookupCache.Instance;
                dataDir = locator.GetDataDirectory(Release);
            }

            var settings = new TestingSettings()
            {
                PassthroughSettings = new PassthroughSettings()
                {
                    CacheReuse = BuildCacheReuse(),
                    TestNormal = TestNormal,
                    TestBinaryOverlay = TestOverlay,
                    TestCopyIn = TestCopyIn,
                    DeleteCachesAfter = false,
                    TestImport = false,
                    ParallelModTranslations = false,
                    Trimming = BuildTrimmingSettings()
                },
                TargetGroups = new List<TargetGroup>()
                {
                    new TargetGroup()
                    {
                        GameRelease = Release,
                        NicknameSuffix = NicknameSuffix,
                        Do = true,
                        Targets = new List<Target>()
                        {
                            new Target()
                            {
                                Do = true,
                                Path = PathToMod
                            }
                        }
                    }
                },
                TestFlattenedMod = false,
                TestBenchmarks = false,
                TestEquality = false,
                TestPex = false,
                TestGroupMasks = false,
                TestRecordEnumerables = false,
                DataFolderLocations = new DataFolderLocations(Release, dataDir)
            };

            var nickname = $"{Path.GetFileName(PathToMod)}{NicknameSuffix}";
            var cacheFolder = PassthroughTest.GetTestFolderPath(nickname, Release).Path;

            if (Json)
            {
                return await RunJson(settings, cacheFolder, overallSw);
            }
            else
            {
                return await RunText(settings, overallSw);
            }
        }
        catch (Exception ex)
        {
            overallSw.Stop();
            if (Json)
            {
                var errorResult = new PassthroughResult
                {
                    Status = "error",
                    ElapsedMs = overallSw.ElapsedMilliseconds,
                    Tests = new TestResults()
                };
                Console.WriteLine(JsonSerializer.Serialize(errorResult, PassthroughResult.JsonOptions));
                return 0;
            }
            else
            {
                Console.WriteLine("Exception occurred:");
                Console.WriteLine(ex);
                return -1;
            }
        }
    }

    private async Task<int> RunJson(
        TestingSettings settings,
        string cacheFolder,
        Stopwatch overallSw)
    {
        var dropoff = new WorkDropoff();
        using var consumer = new WorkConsumer(
            new NumWorkThreadsConstant(null),
            dropoff, dropoff);
        consumer.Start();

        var results = await TestBattery.RunTestsStructured(settings, dropoff);
        overallSw.Stop();

        TestEntry? normalEntry = null;
        TestEntry? overlayEntry = null;
        TestEntry? copyInEntry = null;

        if (TestNormal)
            normalEntry = new TestEntry { Status = "pass" };
        if (TestOverlay)
            overlayEntry = new TestEntry { Status = "pass" };
        if (TestCopyIn)
            copyInEntry = new TestEntry { Status = "pass" };

        foreach (var result in results)
        {
            if (result.Passed) continue;

            var error = result.Error;
            if (error != null)
            {
                var errorInfo = ErrorInfo.FromException(error);
                var errorFile = GetErrorFile(error);

                if (errorFile != null && errorFile.Contains("_BinaryOverlay"))
                {
                    overlayEntry = new TestEntry
                    {
                        Status = "fail",
                        ElapsedMs = result.ElapsedMs,
                        Error = errorInfo
                    };
                }
                else if (errorFile != null && errorFile.Contains("_CopyIn"))
                {
                    copyInEntry = new TestEntry
                    {
                        Status = "fail",
                        ElapsedMs = result.ElapsedMs,
                        Error = errorInfo
                    };
                }
                else if (errorFile != null && errorFile.Contains("_NormalImport"))
                {
                    normalEntry = new TestEntry
                    {
                        Status = "fail",
                        ElapsedMs = result.ElapsedMs,
                        Error = errorInfo
                    };
                    if (TestOverlay)
                        overlayEntry = new TestEntry { Status = "skip" };
                    if (TestCopyIn)
                        copyInEntry = new TestEntry { Status = "skip" };
                }
                else
                {
                    var allFailed = new TestEntry
                    {
                        Status = "fail",
                        ElapsedMs = result.ElapsedMs,
                        Error = errorInfo
                    };
                    if (TestNormal)
                        normalEntry = allFailed;
                    if (TestOverlay)
                        overlayEntry = allFailed;
                    if (TestCopyIn)
                        copyInEntry = allFailed;
                }
            }
        }

        var anyFailed = (normalEntry?.Status == "fail")
                        || (overlayEntry?.Status == "fail")
                        || (copyInEntry?.Status == "fail");

        var passthroughResult = new PassthroughResult
        {
            Status = anyFailed ? "fail" : "pass",
            ElapsedMs = overallSw.ElapsedMilliseconds,
            CacheFolder = cacheFolder,
            Tests = new TestResults
            {
                Normal = normalEntry,
                Overlay = overlayEntry,
                CopyIn = copyInEntry
            }
        };

        Console.WriteLine(JsonSerializer.Serialize(passthroughResult, PassthroughResult.JsonOptions));
        return 0;
    }

    private static string? GetErrorFile(Exception ex)
    {
        return ex switch
        {
            DidNotMatchException d => d.Path,
            MoreDataException m => m.Path,
            UnexpectedlyMoreData u => u.Path,
            _ => ex.Message
        };
    }

    private static async Task<int> RunText(TestingSettings settings, Stopwatch overallSw)
    {
        try
        {
            var dropoff = new WorkDropoff();
            using var consumer = new WorkConsumer(
                new NumWorkThreadsConstant(null),
                dropoff, dropoff);
            consumer.Start();
            await TestBattery.RunTests(settings, dropoff);
            overallSw.Stop();
        }
        catch (Exception ex)
        {
            overallSw.Stop();
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
            return -1;
        }
        return 0;
    }
}
