using System.IO;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubListingsPathProviderTests
    {
        [Fact]
        public void NotUsed()
        {
            new CreationClubListingsPathProvider(
                    new GameCategoryInjection(GameCategory.Skyrim),
                    new CreationClubEnabledInjection(false),
                    new GameDirectoryInjection("C:/SomeFolder"))
                .Path
                .Should().BeNull();
        }

        [Fact]
        public void Typical()
        {
            var gameDir = "C:/SomeFolder";
            foreach (var category in EnumExt.GetValues<GameCategory>())
            {
                new CreationClubListingsPathProvider(
                        new GameCategoryInjection(category),
                        new CreationClubEnabledInjection(true),
                        new GameDirectoryInjection(gameDir))
                    .Path
                    .Should().Be(new FilePath($"C:/SomeFolder/{category}.ccc"));
            }
        }
    }
}