using Mutagen.Bethesda.Plugins.Order.DI;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LiveLoadOrderProviderTests
    {
        [Fact]
        public void NoSub()
        {
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            var loadOrderListingsProvider = Substitute.For<ILoadOrderListingsProvider>();
            new LiveLoadOrderProvider(
                pluginLive,
                cccLive,
                loadOrderListingsProvider);
            var changed = pluginLive.DidNotReceive().Changed;
            pluginLive.DidNotReceive().Get(out _);
            changed = cccLive.DidNotReceive().Changed;
            cccLive.DidNotReceive().Get(out _);
            loadOrderListingsProvider.DidNotReceive().Get();
        }
    }
}