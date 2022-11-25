using FluentAssertions;
using Mutagen.Bethesda.Installs.DI;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Installs;

public class GamesDictCompleteness
{
    [Fact]
    public void GameDictCompleteness()
    {
        foreach (var rel in Enums<GameRelease>.Values)
        {
            GameLocator.Games.ContainsKey(rel)
                .Should().BeTrue();
        }
    }
}