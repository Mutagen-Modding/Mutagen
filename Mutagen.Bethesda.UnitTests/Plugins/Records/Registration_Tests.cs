using Loqui;
using Mutagen.Bethesda.Oblivion.Internals;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class Registration_Tests
    {
        [Fact]
        public void RegistrationTest()
        {
            Assert.True(LoquiRegistration.TryLocateRegistration(typeof(Mutagen.Bethesda.Oblivion.INpcGetter), out var regis));
            Assert.Same(Npc_Registration.Instance, regis);
        }
    }
}
