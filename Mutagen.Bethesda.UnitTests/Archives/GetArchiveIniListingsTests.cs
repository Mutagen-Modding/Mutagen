using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Inis;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Archives
{
    public class GetArchiveIniListingsTests
    {
        [Fact]
        public void Typical()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Ini.GetTypicalPath(GameRelease.SkyrimSE).Path, new MockFileData(@"[Archive]
sResourceArchiveList=Skyrim - Misc.bsa, Skyrim - Shaders.bsa
sResourceArchiveList2=Skyrim - Voices_en0.bsa, Skyrim - Textures0.bsa") }
            });

            var get = new GetArchiveIniListings(fileSystem);
            
            get.Get(GameRelease.SkyrimSE, Ini.GetTypicalPath(GameRelease.SkyrimSE))
                .Should().BeEquivalentTo(new FileName[]
                {
                    "Skyrim - Misc.bsa",
                    "Skyrim - Shaders.bsa",
                    "Skyrim - Voices_en0.bsa",
                    "Skyrim - Textures0.bsa",
                });
        }
    }
}