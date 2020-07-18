using Loqui;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class TestBattery
    {
        public static async Task RunTests(TestingSettings settings)
        {
            int passed = 0;
            int failed = 0;
            await foreach (var (TestName, ex) in GetTests(settings: settings))
            {
                System.Console.WriteLine("========================================\\");
                System.Console.WriteLine(TestName);
                if (ex != null)
                {
                    failed++;
                    if (ex is IPrintable printable)
                    {
                        FileGeneration fg = new FileGeneration();
                        printable.ToString(fg);
                        var str = fg.ToString();
                        System.Console.Write(fg);
                        System.Console.WriteLine();
                    }
                    else
                    {
                        System.Console.WriteLine(ex);
                    }
                }
                else
                {
                    passed++;
                    System.Console.WriteLine("Passed");
                }
                System.Console.WriteLine("========================================/");
                System.Console.WriteLine();
                GC.Collect();
            }
            if (failed == 0)
            {
                System.Console.WriteLine("All passed");
            }
            else
            {
                System.Console.WriteLine($"{failed} / {(passed + failed)} failed");
            }
        }

        public static async Task<(string TestName, Exception ex)> RunTest(string name, Func<Task> toDo)
        {
            try
            {
                await toDo().ConfigureAwait(false);
                return (name, null);
            }
            catch (Exception ex)
            {
                return (name, ex);
            }
        }

        public static async Task<(string TestName, Exception ex)> RunTest(string name, GameRelease release, Target target, Func<Task> toDo)
        {
            return await RunTest($"{release} => {target.Path}\n" +
                $"{name}",
                toDo);
        }

        public static async IAsyncEnumerable<(string TestName, Exception ex)> GetTests(TestingSettings settings)
        {
            var oblivPassthrough = new Target()
            {
                Do = true,
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
                    PassthroughTest passthroughTest = PassthroughTest.Factory(settings, targetGroup, target);
                    if (passthroughTests)
                    {
                        await foreach (var result in passthroughTest.BinaryPassthroughTest())
                        {
                            yield return result;
                        }
                    }
                    if (settings.PassthroughSettings?.TestImport ?? false)
                    {
                        yield return await RunTest("Test Import", targetGroup.GameRelease, target, passthroughTest.TestImport);
                    }
                }
            }

            if (settings.TestGroupMasks)
            {
                yield return await RunTest("GroupMask Import", () => OtherTests.OblivionESM_GroupMask_Import(settings, oblivPassthrough));
                yield return await RunTest("GroupMask Export", () => OtherTests.OblivionESM_GroupMask_Export(settings, oblivPassthrough));
            }
            if (settings.TestFlattenedMod)
            {
                yield return await RunTest("Flatten Mod", () => FlattenedMod_Tests.Oblivion_FlattenMod(settings));
            }
            if (settings.TestBenchmarks)
            {
                Mutagen.Bethesda.Tests.Benchmarks.Benchmarks.Run();
            }
            if (settings.TestRecordEnumerables)
            {
                yield return await RunTest("Record Enumerations", () => OtherTests.RecordEnumerations(settings, oblivPassthrough));
            }
            //if (settings.TestLocators)
            //{
            //    yield return await RunTest("Data Folder Locator", target, () => OtherTests.BaseGroupIterator(target, settings.DataFolderLocations));
            //}
        }
    }
}
