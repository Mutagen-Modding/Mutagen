using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class TestBattery
    {
        public static async Task RunTests(TestingSettings settings)
        {
            foreach (var t in GetTests(settings: settings))
            {
                t.Wait();
                GC.Collect();
            }
        }

        public static IEnumerable<Task> GetTests(TestingSettings settings)
        {
            var oblivPassthrough = new Passthrough()
            {
                Do = true,
                GameMode = GameMode.Oblivion,
                Path = "Oblivion.esm"
            };
            var oblivPassthroughTest = new Oblivion_Passthrough_Test(settings, oblivPassthrough);
            var passthroughTests = (settings.PassthroughSettings?.TestNormal ?? false)
                || (settings.PassthroughSettings?.TestObservable ?? false);
            foreach (var passthroughGroup in settings.PassthroughGroups)
            {
                if (!passthroughGroup.Do) continue;
                foreach (var passthrough in passthroughGroup.Passthroughs)
                {
                    if (!passthrough.Do) continue;
                    PassthroughTest passthroughTest = PassthroughTest.Factory(settings, passthrough);
                    if (passthroughTests)
                    {
                        yield return passthroughTest.BinaryPassthroughTest();
                    }
                    if (settings.PassthroughSettings?.TestImport ?? false)
                    {
                        yield return passthroughTest.TestImport();
                    }
                }
            }

            if (settings.TestGroupMasks)
            {
                yield return OtherTests.OblivionESM_GroupMask_Import(settings, oblivPassthrough);
                yield return OtherTests.OblivionESM_GroupMask_Export(settings, oblivPassthrough);
            }
            if (settings.PassthroughSettings?.TestFolder ?? false)
            {
                yield return OtherTests.OblivionESM_Folder_Reimport(settings.PassthroughSettings, oblivPassthrough, oblivPassthroughTest);
            }
            if (settings.TestModList)
            {
                yield return ModList_Tests.Oblivion_Modlist(settings);
            }
            if (settings.TestFlattenedMod)
            {
                yield return FlattenedMod_Tests.Oblivion_FlattenMod(settings);
            }
            if (settings.TestBenchmarks)
            {
                Mutagen.Bethesda.Tests.Benchmarks.Benchmarks.Run();
            }
        }
    }
}
