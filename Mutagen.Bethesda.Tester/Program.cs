using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Tests;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            LoquiRegistrationSettings.AutomaticRegistration = false;

            TestBattery.RunTests(
                reuseCaches: true,
                deleteCaches: false).Wait();
        }
    }
}
