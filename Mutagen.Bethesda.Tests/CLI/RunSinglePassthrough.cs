#nullable enable
using CommandLine;
using Mutagen.Bethesda.Installs.DI;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("run-passthrough")]
public class RunSinglePassthrough
{
    [Option('m', "ModPath")]
    public string PathToMod { get; set; } = string.Empty;

    [Option('r', "Release")]
    public GameRelease Release { get; set; }

    [Option('c', "CacheReuse")]
    public bool ReuseCaches { get; set; }

    [Option('n', "NicknameSuffix")]
    public string NicknameSuffix { get; set; } = string.Empty;

    [Option('d', "DataFolder")]
    public string? DataFolder { get; set; }

    public async Task<int> Run()
    {
        try
        {
            var locator = GameLocatorLookupCache.Instance;
            var dataDir = locator.GetDataDirectory(Release);
            var settings = new TestingSettings()
            {
                PassthroughSettings = new PassthroughSettings()
                {
                    CacheReuse = new CacheReuse(ReuseCaches),
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

            return await CliHelpers.RunTests(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
            return -1;
        }
    }
}
