using Mutagen.Bethesda.Inis;
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
                PluginListings.HasEnabledMarkers(release);
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

        #region MyDocumentsString
        [Fact]
        public void MyDocumentsString()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                Ini.ToMyDocumentsString(release);
            }
        }
        #endregion

        #region ToIniName
        [Fact]
        public void ToIniName()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                Ini.ToIniName(release);
            }
        }
        #endregion
    }
}
