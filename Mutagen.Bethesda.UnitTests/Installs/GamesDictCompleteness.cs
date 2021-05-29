using FluentAssertions;
using Mutagen.Bethesda.Installs;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Installs
{
    public class GamesDictCompleteness
    {
        [Fact]
        public void GameDictCompleteness()
        {
            foreach (var rel in EnumExt.GetValues<GameRelease>())
            {
                GameLocations.Games.ContainsKey(rel)
                    .Should().BeTrue();
            }
        }
    }
}