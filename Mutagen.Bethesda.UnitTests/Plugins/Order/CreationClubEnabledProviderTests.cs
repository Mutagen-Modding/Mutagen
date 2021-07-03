using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
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