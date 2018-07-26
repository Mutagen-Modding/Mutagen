using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Tests;
using System.Reactive.Linq;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Mutagen.Bethesda.Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            LoquiRegistrationSettings.AutomaticRegistration = false;

            //var pool = new ObjectPool<MutagenFrame>(
            //    creator: new MutagenFrame(
            //        new BinaryReadStream(@"C:\Games\Steam\steamapps\common\Oblivion\Data\Oblivion.esm")),
            //    maxInstances: 15,
            //    actions: new LifecycleActions<MutagenFrame>()
            //    {

            //    });
            var mod = OblivionMod_Observable.Create_Binary(
                () => new FileStream(
                    @"C:\Games\Steam\steamapps\common\Oblivion\Data\Oblivion.esm",
                    FileMode.Open,
                    FileAccess.Read));
            await mod.GameSettings
                .SelectMany(x => x.Items.Values)
                .Switch()
                .Do((x) =>
                {
                    System.Console.WriteLine($"Testing {x}");
                });


            //var tests = new Oblivion_Passthrough_Tests();
            //tests.OblivionESM_Folder_Reimport().Wait();

            //var mod = OblivionMod.Create_Binary(
            //    //@"C:\Games\Oblivion\Data\Oblivion.esm",
            //    @"C:\Users\Levia\AppData\Local\Temp\Mutagen_Oblivion_Binary\Oblivion.esm_Uncompressed",
            //    out var inputErrMask);
            //GC.Collect();
            //System.Console.ReadLine();
            //System.Console.WriteLine(mod.NPCs.Items.Count);
            System.Console.WriteLine("DONE");
            System.Console.ReadLine();
        }
    }
}
