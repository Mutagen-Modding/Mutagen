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
            bool reuseCaches = true,
            bool deleteCaches = false)
        {
            return Task.WhenAll(
                GetTests(
                    reuseCaches: reuseCaches,
                    deleteCaches: deleteCaches)
                .Select((t) => Task.Run(() => t)));
        }

        public static IEnumerable<Task> GetTests(
            bool reuseCaches = true,
            bool deleteCaches = false)
        {
            yield return new Knights_Passthrough_Tests().BinaryPassthroughTest(reuseOld: reuseCaches, deleteAfter: deleteCaches);

            var obliv = new OblivionESM_Passthrough_Tests();
            yield return obliv.BinaryPassthroughTest(reuseOld: reuseCaches, deleteAfter: deleteCaches);
            yield return obliv.OblivionESM_GroupMask_Import();
            yield return obliv.OblivionESM_GroupMask_Export();
        }
    }
}
