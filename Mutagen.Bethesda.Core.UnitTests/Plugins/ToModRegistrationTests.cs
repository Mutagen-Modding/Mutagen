using FluentAssertions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class ToModRegistrationTests
{
    [Fact]
    public void NotFound()
    {
        var regis = GameCategory.Starfield.TryGetModRegistration();
        regis.Should().BeNull();
    }
}
