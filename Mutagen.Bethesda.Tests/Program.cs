using Newtonsoft.Json;
using Noggog;
using System.Diagnostics;
using CommandLine;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Tests.CLI;

namespace Mutagen.Bethesda.Tests;

class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments(args, typeof(RunConfigCommand), typeof(RunSinglePassthrough))
            .MapResult(
                (RunConfigCommand runConfig) => RunConfig(runConfig),
                (RunSinglePassthrough singlePassthrough) => RunSingle(singlePassthrough),
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
            var settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsFile.Path));

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
            var locator = new GameLocator();
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
                    ParallelWriting = false,
                    TestCopyIn = false,
                    ParallelProcessingSteps = false,
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

    private static async Task<int> RunTests(TestingSettings settings)
    {
        try
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await TestBattery.RunTests(settings);
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