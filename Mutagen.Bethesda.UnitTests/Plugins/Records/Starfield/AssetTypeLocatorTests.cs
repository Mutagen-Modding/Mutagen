using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Starfield.Assets;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class AssetTypeLocatorTests
{
    [Fact]
    public void LocatesStarfieldAssetTypes()
    {
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Starfield, Path.Combine("Meshes", "Armor", "Helmet.nif"))
            .ShouldBeOfType<StarfieldModelAssetType>();
    }
}
