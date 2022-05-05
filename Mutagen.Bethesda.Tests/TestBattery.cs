using System.Reactive.Linq;
using System.Reactive.Subjects;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Tests;

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
                Console.WriteLine(msg);
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
                    Console.Write(printable.Print());
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(ex);
                }
            }
            await test.Output.LastOrDefaultAsync();
            GC.Collect();
        }
        if (failed == 0)
        {
            Console.WriteLine("All passed");
        }
        else
        {
            Console.WriteLine($"{failed} / {(passed + failed)} failed");
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
        return new Test(name,
            parallel: parallel,
            toDo: toDo,
            release: release,
            filePath: target.Path);
    }

    public static async IAsyncEnumerable<Test> GetTests(TestingSettings settings)
    {
        var oblivPassthrough = new Target()
        {
            Do = true,
            Path = "Oblivion.esm"
        };
        var passthroughTests = settings.PassthroughSettings?.HasAnyToRun ?? false;
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
                    yield return passthroughTest.TestImport();
                }
                if (settings.TestEquality)
                {
                    yield return passthroughTest.TestEquality();
                }
                if (settings.TestPex)
                {
                    yield return passthroughTest.TestPex();
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
            Benchmarks.Benchmarks.Run();
        }
        if (settings.TestRecordEnumerables)
        {
            yield return RunTest("Record Enumerations", (o) => OtherTests.RecordEnumerations(settings, oblivPassthrough));
        }
    }
}