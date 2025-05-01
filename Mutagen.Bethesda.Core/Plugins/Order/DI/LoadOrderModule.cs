using StrongInject;

namespace Mutagen.Bethesda.Plugins.Order.DI;

[Register(typeof(LoadOrderImporter<>), typeof(LoadOrderImporter<>), typeof(ILoadOrderImporter<>))]
[Register<LiveLoadOrderTimings, ILiveLoadOrderTimings>]
[Register<MasterFlagsLookupProvider, IMasterFlagsLookupProvider>]
[Register<PluginLiveLoadOrderProvider, IPluginLiveLoadOrderProvider>]
[Register<CreationClubLiveLoadOrderFolderWatcher, ICreationClubLiveLoadOrderFolderWatcher>]
[Register<CreationClubLiveListingsFileReader, ICreationClubLiveListingsFileReader>]
[Register<CreationClubLiveLoadOrderProvider, ICreationClubLiveLoadOrderProvider>]
[Register<LiveLoadOrderProvider, ILiveLoadOrderProvider>]
[Register<CreationClubRawListingsReader, ICreationClubRawListingsReader>]
[Register<CreationClubEnabledProvider, ICreationClubEnabledProvider>]
[Register<CreationClubListingsPathProvider, ICreationClubListingsPathProvider>]
[Register<PluginListingCommentTrimmer, IPluginListingCommentTrimmer>]
[Register<EnabledPluginListingsProvider, IEnabledPluginListingsProvider>]
[Register<CreationClubListingsProvider, ICreationClubListingsProvider>]
[Register<HasEnabledMarkersProvider, IHasEnabledMarkersProvider>]
[Register<LoadOrderListingParser, ILoadOrderListingParser>]
[Register<PluginListingsParser, IPluginListingsParser>]
[Register<PluginListingsPathProvider, IPluginListingsPathProvider>]
[Register<PluginRawListingsReader, IPluginRawListingsReader>]
[Register<PluginListingsPathContext, IPluginListingsPathContext>]
[Register<TimestampAligner, ITimestampAligner>]
[Register<TimestampedPluginListingsProvider, ITimestampedPluginListingsProvider>]
[Register<PluginListingsProvider, IPluginListingsProvider>]
[Register<ImplicitListingsProvider, IImplicitListingsProvider>]
[Register<OrderListings, IOrderListings>]
[Register<TimestampedPluginListingsPreferences, ITimestampedPluginListingsPreferences>]
[Register<LoadOrderListingsProvider, ILoadOrderListingsProvider>]
internal class LoadOrderModule
{
}