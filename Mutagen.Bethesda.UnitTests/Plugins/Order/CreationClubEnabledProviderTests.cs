using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubEnabledProviderTests
    {
        [Fact]
        public void AllEnumsCovered()
        {
            foreach (var category in EnumExt.GetValues<GameCategory>())
            {
                new CreationClubEnabledProvider(
                        new GameCategoryInjection(category)).Used
                    .ToString();
            }
        }
    }
}