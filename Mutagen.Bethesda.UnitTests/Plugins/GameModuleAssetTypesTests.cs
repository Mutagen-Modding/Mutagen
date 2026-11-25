using Shouldly;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class GameModuleAssetTypesTests
{
    // Each game lists its asset types by hand, so a newly added asset type is easy to forget.
    [Theory]
    [InlineData(GameCategory.Oblivion)]
    [InlineData(GameCategory.Skyrim)]
    [InlineData(GameCategory.Fallout4)]
    [InlineData(GameCategory.Starfield)]
    [InlineData(GameCategory.Fallout3)]
    public void ListsEveryAssetTypeOfTheGame(GameCategory category)
    {
        var module = GameRuntime.Modules.Single(m => m.Category == category);
        var assetTypesInAssembly = module.GetType().Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IAssetType)));

        module.AssetTypes.Select(t => t.GetType()).ShouldBe(assetTypesInAssembly, ignoreOrder: true);
    }
}
