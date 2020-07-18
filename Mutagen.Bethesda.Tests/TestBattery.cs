using Loqui;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            await foreach (var test in GetTests(settings: settings))
            {
                using var sub = test.AllOutput.Subscribe(msg =>
                {
                    System.Console.WriteLine(msg);
                });
                try
                {
                    await test.Start();
                    passed += 1 + test.ChildCount;
                }
                catch (Exception ex)
                {
                    failed += 1 + test.ChildCount;
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
                await test.Output.LastOrDefaultAsync();
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

        public static Test RunTest(string name, Func<Subject<string>, Task> toDo, bool parallel = true)
        {
            return new Test(
                name,
                parallel: parallel,
                toDo: toDo);
        }

        public static Test RunTest(string name, GameRelease release, Target target, Func<Subject<string>, Task> toDo, bool parallel = true)
        {
            return RunTest($"{release} => {target.Path}\n" +
                $"{name}",
                parallel: parallel,
                toDo: toDo);
        }

        public static async IAsyncEnumerable<Test> GetTests(TestingSettings settings)
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
                        yield return passthroughTest.BinaryPassthroughTest();
                    }
                    if (settings.PassthroughSettings?.TestImport ?? false)
                    {
                        yield return RunTest("Test Import", targetGroup.GameRelease, target, passthroughTest.TestImport);
                    }
                }
            }

            if (settings.TestGroupMasks)
            {
                yield return RunTest("GroupMask Import", (o) => OtherTests.OblivionESM_GroupMask_Import(settings, oblivPassthrough));
                yield return RunTest("GroupMask Export", (o) => OtherTests.OblivionESM_GroupMask_Export(settings, oblivPassthrough));
            }
            if (settings.TestFlattenedMod)
            {
                yield return RunTest("Flatten Mod", (o) => FlattenedMod_Tests.Oblivion_FlattenMod(settings));
            }
            if (settings.TestBenchmarks)
            {
                Mutagen.Bethesda.Tests.Benchmarks.Benchmarks.Run();
            }
            if (settings.TestRecordEnumerables)
            {
                yield return RunTest("Record Enumerations", (o) => OtherTests.RecordEnumerations(settings, oblivPassthrough));
            }
            //if (settings.TestLocators)
            //{
            //    yield return await RunTest("Data Folder Locator", target, () => OtherTests.BaseGroupIterator(target, settings.DataFolderLocations));
            //}
        }
    }
}
