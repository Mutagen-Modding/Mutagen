using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class ToModRegistrationTests
{
    [Fact]
    public void Found()
    {
        var regis = GameCategory.Skyrim.ToModRegistration();
        regis.ShouldNotBeNull();
        regis.FullName.ShouldContain("Skyrim");
    }
}
