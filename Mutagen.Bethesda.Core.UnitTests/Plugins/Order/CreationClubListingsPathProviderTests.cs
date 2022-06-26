using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubListingsPathProviderTests
{
    [Theory, MutagenAutoData]
    public void NotUsed(
        [Frozen]ICreationClubEnabledProvider enabledProvider,
        CreationClubListingsPathProvider sut)
    {
        enabledProvider.Used.Returns(false);
        sut.Path
            .Should().BeNull();
    }

    [Theory, MutagenAutoData]
    public void Typical(
        [Frozen]IGameDirectoryProvider gameDir,
        [Frozen]IGameCategoryContext gameCategoryContext,
        [Frozen]ICreationClubEnabledProvider enabledProvider,
        CreationClubListingsPathProvider sut)
    {
        enabledProvider.Used.Returns(true);
        foreach (var category in EnumExt.GetValues<GameCategory>())
        {
            gameCategoryContext.Category.Returns(category);
            sut.Path
                .Should().Be(new FilePath(Path.Combine(gameDir.Path, $"{category}.ccc")));
        }
    }
}