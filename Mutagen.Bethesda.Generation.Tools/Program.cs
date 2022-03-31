using CommandLine;
using Mutagen.Bethesda.Generation.Tools.XEdit.Enum;

try
{
    var parser = new Parser();
    return await parser.ParseArguments(
            args,
            typeof(RunXEditEnumConverter))
        .MapResult(
            async (RunXEditEnumConverter xEditEnum) =>
            {
                await xEditEnum.Execute();
                return 0;
            },
            async _ =>
            {
                Console.Error.WriteLine(
                    $"Could not parse arguments into an executable command: {string.Join(' ', args)}");
                return -1;
            });
}
catch (Exception ex)
{
    System.Console.Error.WriteLine(ex);
    return -1;
}