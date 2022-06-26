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

        foreach (var category in EnumExt.GetValues<GameCategory>())
        {
            init.Should().Contain(category);
        }
    }
}