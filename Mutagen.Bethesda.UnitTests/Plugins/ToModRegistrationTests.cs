using FluentAssertions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class ToModRegistrationTests
{
    [Fact]
    public void Found()
    {
        var regis = GameCategory.Skyrim.ToModRegistration();
        regis.Should().NotBeNull();
        regis.FullName.Should().Contain("Skyrim");
    }
}
