using Loqui;
using Mutagen.Bethesda.Tests;

namespace Loqui
{
    public class ProtocolDefinition_Tests : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Tests");
        public void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.TestingSettings_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.Passthrough_Registration.Instance);
        }
    }
}
