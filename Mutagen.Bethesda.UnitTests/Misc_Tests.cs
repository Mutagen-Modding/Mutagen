using Loqui;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Misc_Tests
    {
        [Fact]
        public void RegistrationTest()
        {
            Assert.True(LoquiRegistration.TryLocateRegistration(typeof(Mutagen.Bethesda.Oblivion.INpcGetter), out var regis));
            Assert.Same(Npc_Registration.Instance, regis);
        }
    }
}
