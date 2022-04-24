using Newtonsoft.Json;
using Noggog;
using System.Diagnostics;

namespace Mutagen.Bethesda.Tests;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            FilePath settingsFile;
            if (args.Length == 1)
            {
                settingsFile = new FilePath(args[0]);
            }
            else
            {
                settingsFile = new FilePath("../../../TestingSettings.json");
            }
            if (!settingsFile.Exists)
            {
                throw new ArgumentException($"Could not find settings file at: {settingsFile}");
            }

            Console.WriteLine($"Using settings: {settingsFile.Path}");
            var settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(settingsFile.Path));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            await TestBattery.RunTests(settings);
            sw.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
        }
        Console.ReadLine();
    }
}