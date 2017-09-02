using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen;
using Noggog.Utility;
using Xunit;

namespace Mutagen.Tests
{
    public class Passthrough_Tests
    {
        [Fact]
        public void OblivionESM()
        {
            var mod = OblivionMod.Create_OblivionBinary(Properties.Settings.Default.OblivionESM);
            using (var tmp = new TempFolder())
            {
                var oblivionPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                mod.Write_OblivionBinary(oblivionPath);
            }
        }
    }
}
