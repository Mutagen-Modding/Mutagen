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
            var oblivPassthrough = new Target()
            {
                Do = true,
                GameMode = GameMode.Oblivion,
                Path = "Oblivion.esm"
            };
            var passthroughTests = (settings.PassthroughSettings?.TestNormal ?? false)
                || (settings.PassthroughSettings?.TestBinaryOverlay ?? false)
                || (settings.PassthroughSettings?.TestCopyIn ?? false)
                || (settings.PassthroughSettings?.TestFolder ?? false);
            foreach (var targetGroup in settings.TargetGroups)
            {
                if (!targetGroup.Do) continue;
                foreach (var target in targetGroup.Targets)
                {
                    if (!target.Do) continue;
                    PassthroughTest passthroughTest = PassthroughTest.Factory(settings, target);
                    if (passthroughTests)
                    {
                        yield return passthroughTest.BinaryPassthroughTest();
                    }
                    if (settings.PassthroughSettings?.TestImport ?? false)
                    {
                        yield return passthroughTest.TestImport();
                    }
                    if (settings.TestLocators)
                    {
                        yield return OtherTests.BaseGroupIterator(target, settings.DataFolderLocations);
                    }
                }
            }

            if (settings.TestGroupMasks)
            {
                yield return OtherTests.OblivionESM_GroupMask_Import(settings, oblivPassthrough);
                yield return OtherTests.OblivionESM_GroupMask_Export(settings, oblivPassthrough);
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
