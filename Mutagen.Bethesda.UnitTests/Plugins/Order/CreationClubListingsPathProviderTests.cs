using System.IO;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubListingsPathProviderTests
    {
        [Theory, MutagenAutoData]
        public void NotUsed(
            [Frozen]ICreationClubEnabledProvider enabledProvider,
            CreationClubListingsPathProvider sut)
        {
            A.CallTo(() => enabledProvider.Used).Returns(false);
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
            A.CallTo(() => enabledProvider.Used).Returns(true);
            foreach (var category in EnumExt.GetValues<GameCategory>())
            {
                A.CallTo(() => gameCategoryContext.Category).Returns(category);
                sut.Path
                    .Should().Be(new FilePath(Path.Combine(gameDir.Path, $"{category}.ccc")));
            }
        }
    }
}