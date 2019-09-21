using Loqui;
using Mutagen.Bethesda.Examples;

namespace Loqui
{
    public class ProtocolDefinition_Examples : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Examples");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Examples.Internals.MainVM_Registration.Instance);
        }
    }
}
