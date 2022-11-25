using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Archives;

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

        var get = new GetArchiveIniListings(
            fileSystem,
            new IniPathProvider(
                new GameReleaseInjection(GameRelease.SkyrimSE),
                new IniPathLookup()));
            
        get.Get(Ini.GetTypicalPath(GameRelease.SkyrimSE))
            .Should().Equal(new FileName[]
            {
                "Skyrim - Misc.bsa",
                "Skyrim - Shaders.bsa",
                "Skyrim - Voices_en0.bsa",
                "Skyrim - Textures0.bsa",
            });
    }
}