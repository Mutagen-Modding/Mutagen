#nullable enable

namespace Mutagen.Bethesda.Tests;

public record TestingSettings
{
    public bool TestGroupMasks { get; set; }
    public bool TestFlattenedMod { get; set; }
    public bool TestBenchmarks { get; set; }
    public bool TestRecordEnumerables { get; set; }
    public bool TestEquality { get; set; }
    public bool TestPex { get; set; }
    public DataFolderLocations DataFolderLocations { get; set; } = new();
    public PassthroughSettings PassthroughSettings { get; set; } = new();
    public List<TargetGroup> TargetGroups { get; set; } = new();
}

public record DataFolderLocations
{
    public string Oblivion { get; set; } = string.Empty;
    public string Skyrim { get; set; } = string.Empty;
    public string SkyrimSpecialEdition { get; set; } = string.Empty;
    public string SkyrimVR { get; set; } = string.Empty;
    public string Fallout4 { get; set; } = string.Empty;
    public string Starfield { get; set; } = string.Empty;

    public DataFolderLocations()
    {
    }

    public DataFolderLocations(GameRelease release, string path)
    {
        Set(release, path);
    }

    public string Get(GameRelease mode)
    {
        switch (mode)
        {
            case GameRelease.Oblivion:
                return Oblivion;
            case GameRelease.SkyrimLE:
                return Skyrim;
            case GameRelease.SkyrimSE:
                return SkyrimSpecialEdition;
            case GameRelease.Fallout4:
                return Fallout4;
            case GameRelease.Starfield:
                return Starfield;
            default:
                throw new NotImplementedException();
        }
    }

    public void Set(GameRelease mode, string path)
    {
        switch (mode)
        {
            case GameRelease.Oblivion:
                Oblivion = path;
                break;
            case GameRelease.SkyrimLE:
                Skyrim = path;
                break;
            case GameRelease.SkyrimSE:
                SkyrimSpecialEdition = path;
                break;
            case GameRelease.Fallout4:
                Fallout4 = path;
                break;
            case GameRelease.Starfield:
                Starfield = path;
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

public record PassthroughSettings
{
    public CacheReuse CacheReuse { get; set; } = new();
    public TrimmingSettings Trimming { get; set; } = new();
    public bool DeleteCachesAfter { get; set; } = true;
    public bool TestNormal { get; set; }
    public bool TestBinaryOverlay { get; set; }
    public bool TestImport { get; set; }
    public bool TestCopyIn { get; set; }
    public bool ParallelWriting { get; set; }
    public bool ParallelProcessingSteps { get; set; }

    public bool HasAnyToRun => TestNormal
                               || TestBinaryOverlay
                               || TestCopyIn;
}

public record CacheReuse
{
    public bool ReuseDecompression { get; set; }
    public bool ReuseMerge { get; set; }
    public bool ReuseAlignment { get; set; }
    public bool ReuseProcessing { get; set; }
    public bool ReuseTrimming { get; set; }

    public CacheReuse()
    {
    }

    public CacheReuse(bool on)
    {
        ReuseDecompression = on;
        ReuseMerge = on;
        ReuseAlignment = on;
        ReuseProcessing = on;
        ReuseTrimming = on;
    }
}

public record TrimmingSettings
{
    public bool Enabled { get; set; }= true;
    public List<string> TypesToTrim { get; set; } = new();
    public List<string> TypesToInclude { get; set; } = new();
}

public record TargetGroup
{
    public bool Do { get; set; }
    public string NicknameSuffix { get; set; } = string.Empty;
    public GameRelease GameRelease { get; set; }
    public List<Target> Targets { get; set; } = new();
}

public record Target
{
    public bool Do { get; set; }
    public string Path { get; set; } = string.Empty;
    public byte NumMasters { get; set; }
    public byte? ExpectedBaseGroupCount { get; set; }
}