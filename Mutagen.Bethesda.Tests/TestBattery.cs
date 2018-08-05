using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class TestBattery
    {
        public static Task RunTests(
            TestingSettings settings,
            bool reuseCaches = true,
            bool deleteCaches = false)
        {
            return Task.WhenAll(
                GetTests(
                    settings: settings,
                    reuseCaches: reuseCaches,
                    deleteCaches: deleteCaches)
                .Select((t) => Task.Run(() => t)));
        }

        public static IEnumerable<Task> GetTests(
            TestingSettings settings,
            bool reuseCaches = true,
            bool deleteCaches = false)
        {
            if (settings.KnightsESP?.Do ?? false)
            {
                yield return new Knights_Passthrough_Tests(settings).BinaryPassthroughTest(
                    settings: settings);
            }

            var obliv = new OblivionESM_Passthrough_Tests(settings);
            if (settings.OblivionESM?.Do ?? false)
            {
                yield return obliv.BinaryPassthroughTest(
                    settings: settings);
            }
            if (settings.TestGroupMasks)
            {
                yield return obliv.OblivionESM_GroupMask_Import();
                yield return obliv.OblivionESM_GroupMask_Export();
            }
            if (settings.TestFolder)
            {
                yield return obliv.OblivionESM_Folder_Reimport();
            }
        }
    }
}
