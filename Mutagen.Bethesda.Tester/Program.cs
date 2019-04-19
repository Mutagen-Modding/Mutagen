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

namespace Mutagen.Bethesda.Tester
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
                    settingsFile = new FilePath("TestingSettings.xml");
                }
                if (!settingsFile.Exists)
                {
                    throw new ArgumentException($"Coult not find settings file at: {settingsFile}");
                }

                var settings = TestingSettings.Create_Xml(settingsFile.Path);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                await TestBattery.RunTests(settings);
                sw.Stop();
                System.Console.WriteLine($"Tests took: {sw.ElapsedMilliseconds * 1.0 / 1000}s");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception occurred:");
                System.Console.WriteLine(ex);
            }
            System.Console.ReadKey();
        }
    }
}
