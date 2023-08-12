using CommandLine;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("run-passthrough")]
public class RunSinglePassthrough
{
    [Option('m', "ModPath")]
    public string PathToMod { get; set; }
    
    [Option('r', "Release")]
    public GameRelease Release { get; set; }
    
    [Option('c', "CacheReuse")]
    public bool ReuseCaches { get; set; }
    
    [Option('n', "NicknameSuffix")]
    public string NicknameSuffix { get; set; }
    
    [Option('d', "DataFolder")]
    public string? DataFolder { get; set; }
}