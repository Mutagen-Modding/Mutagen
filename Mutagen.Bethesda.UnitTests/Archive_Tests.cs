using FluentAssertions;
using Mutagen.Bethesda.Archives;
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
            using var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim, Enumerable.Empty<string>())
                .Should().BeEmpty();
        }

        [Fact]
        public void GetApplicableArchivePaths_Typical()
        {
            using var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            File.WriteAllText(Path.Combine(temp.Dir.Path, "Skyrim.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "Skyrim - Textures.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "MyMod.bsa"), string.Empty);
            var applicable = Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim, Enumerable.Empty<string>())
                .ToArray();
            applicable.Should().BeEquivalentTo(new string[]
            {
                Path.Combine(temp.Dir.Path, "Skyrim.bsa"),
                Path.Combine(temp.Dir.Path, "Skyrim - Textures.bsa"),
                // Eventually this might not show?  Need to iron out what's applicable, still
                Path.Combine(temp.Dir.Path, "MyMod.bsa")
            });
        }
        #endregion
    }
}
