using CommandLine;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("run-config")]
public class RunConfigCommand
{
    [Option('p', "Path")]
    public string PathToConfig { get; set; }
}