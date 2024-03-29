using Loqui;
using Mutagen.Bethesda.Oblivion;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class RegistrationTests
{
    [Fact]
    public void RegistrationTest()
    {
        Assert.True(LoquiRegistration.TryLocateRegistration(typeof(Mutagen.Bethesda.Oblivion.INpcGetter), out var regis));
        Assert.Same(Npc.StaticRegistration, regis);
    }
}