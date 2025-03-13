using StrongInject;

namespace Mutagen.Bethesda.Plugins.Order.DI;

[Register<LoadOrderListingsProvider, ILoadOrderListingsProvider>]
[Register<OrderListings, IOrderListings>]
[Register<ImplicitListingsProvider, IImplicitListingsProvider>]
[Register<EnabledPluginListingsProvider, IEnabledPluginListingsProvider>]
[Register<TimestampedPluginListingsProvider, ITimestampedPluginListingsProvider>]
[Register<PluginListingsProvider, IPluginListingsProvider>]
[Register<PluginRawListingsReader, IPluginRawListingsReader>]
[Register<PluginListingsPathContext, IPluginListingsPathContext>]
[Register<TimestampAligner, ITimestampAligner>]
[Register<PluginListingsParser, IPluginListingsParser>]
[Register<PluginListingsPathProvider, IPluginListingsPathProvider>]
[Register<CreationClubListingsProvider, ICreationClubListingsProvider>]
[Register<LoadOrderListingParser, ILoadOrderListingParser>]
[Register<HasEnabledMarkersProvider, IHasEnabledMarkersProvider>]
[Register<CreationClubEnabledProvider, ICreationClubEnabledProvider>]
[Register<PluginListingCommentTrimmer, IPluginListingCommentTrimmer>]
[Register<CreationClubListingsPathProvider, ICreationClubListingsPathProvider>]
[Register<CreationClubRawListingsReader, ICreationClubRawListingsReader>]
[Register<TimestampedPluginListingsPreferences, ITimestampedPluginListingsPreferences>]
internal class OrderModule
{
}