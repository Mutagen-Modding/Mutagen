using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class GameRelease_Tests
    {
        [Fact]
        public void ToCategoryCoverage()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                release.ToCategory();
            }
        }

        [Fact]
        public void HasEnabledMarkers()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                LoadOrder.HasEnabledMarkers(release);
            }
        }

        [Fact]
        public void GameConstants()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                Mutagen.Bethesda.Binary.GameConstants.Get(release);
            }
        }
    }
}
