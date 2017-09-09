using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen;
using Noggog.Utility;
using Xunit;
using Mutagen.Internals;

namespace Mutagen.Tests
{
    public class Passthrough_Tests
    {
        [Fact]
        public void OblivionESM()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_OblivionBinary(
                Properties.Settings.Default.OblivionESM,
                out inputErrMask);
            using (var tmp = new TempFolder())
            {
                var oblivionPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                mod.Write_OblivionBinary(
                    oblivionPath,
                    out outputErrMask);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }
    }
}
