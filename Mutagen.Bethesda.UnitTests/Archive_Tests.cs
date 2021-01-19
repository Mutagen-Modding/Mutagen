using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Archive_Tests
    {
        #region GetApplicableArchivePaths
        [Fact]
        public void GetApplicableArchivePaths_Empty()
        {
            var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim)
                .Should().BeEmpty();
        }

        [Fact]
        public void GetApplicableArchivePaths_Typical()
        {
            var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            File.WriteAllText(Path.Combine(temp.Dir.Path, "Skyrim.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "Skyrim - Textures.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "MyMod.bsa"), string.Empty);
            var applicable = Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim)
                .ToArray();
            applicable.Should().HaveCount(2);
            applicable.Should().BeEquivalentTo(new string[]
            {
                Path.Combine(temp.Dir.Path, "Skyrim.bsa"),
                Path.Combine(temp.Dir.Path, "Skyrim - Textures.bsa")
            });
        }
        #endregion
    }
}
