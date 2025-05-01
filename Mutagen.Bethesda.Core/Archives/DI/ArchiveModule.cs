using StrongInject;

namespace Mutagen.Bethesda.Archives.DI;

[Register<ArchiveNameFromModKeyProvider, IArchiveNameFromModKeyProvider>]
[Register<GetArchiveIniListings, IGetArchiveIniListings>]
[Register<CachedArchiveListingDetailsProvider, IArchiveListingDetailsProvider>]
[Register<ArchiveExtensionProvider, IArchiveExtensionProvider>]
[Register<CheckArchiveApplicability, ICheckArchiveApplicability>]
[Register<GetApplicableArchivePaths, IGetApplicableArchivePaths>]
internal class ArchiveModule
{
}