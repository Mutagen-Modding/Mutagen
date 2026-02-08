#nullable enable
using CommandLine;
using Newtonsoft.Json;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("run-config")]
public class RunConfigCommand
{
    [Option('p', "Path")]
    public string PathToConfig { get; set; } = string.Empty;

    public async Task<int> Run()
    {
        try
        {
            FilePath settingsFile = PathToConfig;
            if (!settingsFile.Exists)
            {
                throw new ArgumentException($"Could not find settings file at: {settingsFile}");
            }

            Console.WriteLine($"Using settings: {settingsFile.Path}");
            var settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsFile.Path))
                ?? throw new ArgumentException("Failed to deserialize settings");

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
