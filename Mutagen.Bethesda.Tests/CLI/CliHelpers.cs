#nullable enable
using System.Diagnostics;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Tests.CLI;

static class CliHelpers
{
    public static long ParsePosition(string pos) =>
        pos.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? Convert.ToInt64(pos[2..], 16)
            : long.Parse(pos);

    public static async Task<int> RunTests(TestingSettings settings)
    {
        try
        {
            var dropoff = new WorkDropoff();
            using var consumer = new WorkConsumer(
                new NumWorkThreadsConstant(null),
                dropoff, dropoff);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await TestBattery.RunTests(settings, dropoff);
            sw.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex);
            return -1;
        }
        return 0;
    }
}
