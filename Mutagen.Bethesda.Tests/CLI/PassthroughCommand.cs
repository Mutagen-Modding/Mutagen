#nullable enable
using CommandLine;

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

        // If any individual flag is set, use those; otherwise default to all off
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
}
