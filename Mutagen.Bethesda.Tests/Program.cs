#nullable enable
using Newtonsoft.Json;
using Noggog;
using System.Diagnostics;
using System.Text.Json;
using CommandLine;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Tests.CLI;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Tests;

class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments(args, typeof(RunConfigCommand), typeof(RunSinglePassthrough), typeof(PassthroughCommand))
            .MapResult(
                (RunConfigCommand runConfig) => RunConfig(runConfig),
                (RunSinglePassthrough singlePassthrough) => RunSingle(singlePassthrough),
                (PassthroughCommand passthrough) => RunPassthrough(passthrough),
                async _ => -1);
    }

    private static async Task<int> RunConfig(RunConfigCommand cmd)
    {
        try
        {
            FilePath settingsFile = cmd.PathToConfig;
            if (!settingsFile.Exists)
            {
                throw new ArgumentException($"Could not find settings file at: {settingsFile}");
            }

            Console.WriteLine($"Using settings: {settingsFile.Path}");
            var settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsFile.Path))
                ?? throw new ArgumentException("Failed to deserialize settings");

            return await RunTests(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
            return -1;
        }
    }

    private static async Task<int> RunSingle(RunSinglePassthrough cmd)
    {
        try
        {
            var locator = GameLocatorLookupCache.Instance;
            var dataDir = locator.GetDataDirectory(cmd.Release);
            var settings = new TestingSettings()
            {
                PassthroughSettings = new PassthroughSettings()
                {
                    CacheReuse = new CacheReuse(cmd.ReuseCaches),
                    TestNormal = true,
                    TestBinaryOverlay = true,
                    DeleteCachesAfter = false,
                    TestImport = false,
                    ParallelModTranslations = false,
                    TestCopyIn = false,
                    Trimming = new TrimmingSettings()
                    {
                        Enabled = false
                    }
                },
                TargetGroups = new List<TargetGroup>()
                {
                    new TargetGroup()
                    {
                        GameRelease = cmd.Release,
                        NicknameSuffix = cmd.NicknameSuffix,
                        Do = true,
                        Targets = new List<Target>()
                        {
                            new Target()
                            {
                                Do = true,
                                Path = cmd.PathToMod
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
                DataFolderLocations = new DataFolderLocations(cmd.Release, dataDir)
            };

            return await RunTests(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
            return -1;
        }
    }

    private static async Task<int> RunPassthrough(PassthroughCommand cmd)
    {
        var overallSw = new Stopwatch();
        overallSw.Start();

        try
        {
            // Resolve data folder
            string dataDir;
            if (!string.IsNullOrEmpty(cmd.DataFolder))
            {
                dataDir = cmd.DataFolder;
            }
            else
            {
                var locator = GameLocatorLookupCache.Instance;
                dataDir = locator.GetDataDirectory(cmd.Release);
            }

            var settings = new TestingSettings()
            {
                PassthroughSettings = new PassthroughSettings()
                {
                    CacheReuse = cmd.BuildCacheReuse(),
                    TestNormal = cmd.TestNormal,
                    TestBinaryOverlay = cmd.TestOverlay,
                    TestCopyIn = cmd.TestCopyIn,
                    DeleteCachesAfter = false,
                    TestImport = false,
                    ParallelModTranslations = false,
                    Trimming = cmd.BuildTrimmingSettings()
                },
                TargetGroups = new List<TargetGroup>()
                {
                    new TargetGroup()
                    {
                        GameRelease = cmd.Release,
                        NicknameSuffix = cmd.NicknameSuffix,
                        Do = true,
                        Targets = new List<Target>()
                        {
                            new Target()
                            {
                                Do = true,
                                Path = cmd.PathToMod
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
                DataFolderLocations = new DataFolderLocations(cmd.Release, dataDir)
            };

            // Compute cache folder path for output
            var nickname = $"{Path.GetFileName(cmd.PathToMod)}{cmd.NicknameSuffix}";
            var cacheFolder = PassthroughTest.GetTestFolderPath(nickname, cmd.Release).Path;

            if (cmd.Json)
            {
                return await RunPassthroughJson(settings, cmd, cacheFolder, overallSw);
            }
            else
            {
                return await RunPassthroughText(settings, overallSw);
            }
        }
        catch (Exception ex)
        {
            overallSw.Stop();
            if (cmd.Json)
            {
                var errorResult = new PassthroughResult
                {
                    Status = "error",
                    ElapsedMs = overallSw.ElapsedMilliseconds,
                    Tests = new TestResults()
                };
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(errorResult, PassthroughResult.JsonOptions));
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

    private static async Task<int> RunPassthroughJson(
        TestingSettings settings,
        PassthroughCommand cmd,
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

        // Map results to per-test entries
        // The parent test is "Setup Processed Files" — if it passed, all sub-tests passed.
        // If it failed, the exception tells us which sub-test failed.
        TestEntry? normalEntry = null;
        TestEntry? overlayEntry = null;
        TestEntry? copyInEntry = null;

        if (cmd.TestNormal)
            normalEntry = new TestEntry { Status = "pass" };
        if (cmd.TestOverlay)
            overlayEntry = new TestEntry { Status = "pass" };
        if (cmd.TestCopyIn)
            copyInEntry = new TestEntry { Status = "pass" };

        foreach (var result in results)
        {
            if (result.Passed) continue;

            // Determine which sub-test failed based on the exception
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
                    // Normal passed if it was enabled (overlay runs after normal)
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
                    // Subsequent tests didn't run
                    if (cmd.TestOverlay)
                        overlayEntry = new TestEntry { Status = "skip" };
                    if (cmd.TestCopyIn)
                        copyInEntry = new TestEntry { Status = "skip" };
                }
                else
                {
                    // Processing phase failure — all tests failed
                    var allFailed = new TestEntry
                    {
                        Status = "fail",
                        ElapsedMs = result.ElapsedMs,
                        Error = errorInfo
                    };
                    if (cmd.TestNormal)
                        normalEntry = allFailed;
                    if (cmd.TestOverlay)
                        overlayEntry = allFailed;
                    if (cmd.TestCopyIn)
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

        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(passthroughResult, PassthroughResult.JsonOptions));
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

    private static async Task<int> RunPassthroughText(TestingSettings settings, Stopwatch overallSw)
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

    private static async Task<int> RunTests(TestingSettings settings)
    {
        try
        {
            var dropoff = new WorkDropoff();
            using var consumer = new WorkConsumer(
                new NumWorkThreadsConstant(null),
                dropoff, dropoff);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await TestBattery.RunTests(settings, dropoff);
            sw.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
            return -1;
        }
        return 0;
    }
}
