using FluentAssertions;
using Noggog.Utility;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class CreationClubListings_Test
    {
        [Fact]
        public void FromCreationClubPathMissing()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(FromCreationClubPathMissing)));
            var missingPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            Action a = () =>
                CreationClubListings.ListingsFromPath(
                    cccFilePath: missingPath,
                    dataPath: default);
            a.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void FromCreationClubPath()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(FromCreationClubPath)));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.PluginModKey.FileName,
                    Utility.PluginModKey2.FileName,
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            var results = CreationClubListings.ListingsFromPath(
                    cccFilePath: cccPath,
                    dataPath: dataPath)
                .ToList();
            results.Should().HaveCount(1);
            results[0].Should().BeEquivalentTo(new LoadOrderListing(Utility.PluginModKey, enabled: true));
        }
    }
}
