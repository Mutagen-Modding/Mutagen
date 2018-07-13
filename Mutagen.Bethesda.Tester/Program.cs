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

            var tests = new Oblivion_Passthrough_Tests();
            tests.OblivionESM_Binary_Internal(
                reuseOld: false,
                deleteAfter: false).Wait();

            //var mod = OblivionMod.Create_Binary(
            //    //@"C:\Games\Oblivion\Data\Oblivion.esm",
            //    @"C:\Users\Levia\AppData\Local\Temp\Mutagen_Oblivion_Binary\Oblivion.esm_Uncompressed",
            //    out var inputErrMask);
            //System.Console.WriteLine("DONE");
            //GC.Collect();
            //System.Console.ReadLine();
            //System.Console.WriteLine(mod.NPCs.Items.Count);
            //System.Console.ReadLine();
        }
    }
}
