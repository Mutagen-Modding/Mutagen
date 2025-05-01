using StrongInject;

namespace Mutagen.Bethesda.Assets.DI;

[Register<DataDirectoryAssetProvider>]
[Register<ArchiveAssetProvider>]
[Register<GameAssetProvider, IAssetProvider>]
internal class AssetModule
{
}