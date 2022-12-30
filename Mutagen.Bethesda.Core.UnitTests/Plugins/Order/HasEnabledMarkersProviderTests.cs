using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class HasEnabledMarkersProviderTests
{
    [Fact]
    public void CoversAllReleases()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            new HasEnabledMarkersProvider(
                    new GameReleaseInjection(release))
                .HasEnabledMarkers
                .ToString();
        }
    }
}