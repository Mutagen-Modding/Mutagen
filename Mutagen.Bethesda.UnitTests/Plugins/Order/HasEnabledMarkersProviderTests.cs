using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class HasEnabledMarkersProviderTests
    {
        [Fact]
        public void CoversAllReleases()
        {
            foreach (var release in EnumExt.GetValues<GameRelease>())
            {
                new HasEnabledMarkersProvider(
                        new GameReleaseInjection(release))
                    .HasEnabledMarkers
                    .ToString();
            }
        }
    }
}