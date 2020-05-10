using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Tests;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
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
                    settingsFile = new FilePath("../../../TestingSettings.xml");
                }
                if (!settingsFile.Exists)
                {
                    throw new ArgumentException($"Coult not find settings file at: {settingsFile}");
                }

                System.Console.WriteLine($"Using settings: {settingsFile.Path}");
                var settings = TestingSettings.CreateFromXml(settingsFile.Path);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                await TestBattery.RunTests(settings);
                sw.Stop();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception occurred:");
                System.Console.WriteLine(ex);
            }
            System.Console.ReadLine();
        }
    }
}
