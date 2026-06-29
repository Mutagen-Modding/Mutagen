#nullable enable
using CommandLine;
using Mutagen.Bethesda.Tests.CLI;

namespace Mutagen.Bethesda.Tests;

class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments(args,
                typeof(RunConfigCommand),
                typeof(RunSinglePassthrough),
                typeof(PassthroughCommand),
                typeof(InspectGrupCommand),
                typeof(InspectRecordCommand),
                typeof(InspectBytesCommand),
                typeof(ListGrupsCommand),
                typeof(SearchSubrecordsCommand), 
                typeof(AnalyzeSubrecord))
            .MapResult(
                (RunConfigCommand cmd) => cmd.Run(),
                (RunSinglePassthrough cmd) => cmd.Run(),
                (PassthroughCommand cmd) => cmd.Run(),
                (InspectGrupCommand cmd) => Task.FromResult(cmd.Run()),
                (InspectRecordCommand cmd) => Task.FromResult(cmd.Run()),
                (InspectBytesCommand cmd) => Task.FromResult(cmd.Run()),
                (ListGrupsCommand cmd) => Task.FromResult(cmd.Run()),
                (SearchSubrecordsCommand cmd) => Task.FromResult(cmd.Run()),
                (AnalyzeSubrecord analyze) => Task.FromResult(analyze.Execute()),
                async _ => -1);
    }
}
