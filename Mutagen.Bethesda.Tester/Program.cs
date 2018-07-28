using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Tests;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
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
            LoquiRegistrationSettings.AutomaticRegistration = false;

            FilePath settingsFile = new FilePath("../../TestingSettings.xml");
            if (!settingsFile.Exists)
            {
                throw new ArgumentException($"Coult not find settings file at: {settingsFile}");
            }

            var settings = TestingSettings.Create_Xml(settingsFile.Path);

            TestBattery.RunTests(
                settings,
                reuseCaches: settings.ReuseCaches,
                deleteCaches: settings.DeleteCachesAfter).Wait();
        }
    }
}
