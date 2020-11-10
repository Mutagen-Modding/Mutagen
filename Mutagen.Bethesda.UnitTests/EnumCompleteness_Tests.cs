using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class EnumCompleteness_Tests
    {
        #region GameRelease
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

        [Fact]
        public void DefaultFormVersion()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                release.GetDefaultFormVersion();
            }
        }
        #endregion

        #region GameCategory
        [Fact]
        public void HasFormVersion()
        {
            foreach (var cat in EnumExt.GetValues<GameCategory>())
            {
                cat.HasFormVersion();
            }
        }
        #endregion
    }
}
