using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class WarmupTests
{
    [Fact]
    public void WarmupTest()
    {
        var init = Warmup.Init();

        foreach (var category in Enums<GameCategory>.Values)
        {
            init.Should().Contain(category);
        }
    }
}