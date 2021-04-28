using Loqui;

namespace Loqui
{
    public class ProtocolDefinition_All : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("All");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
        }
    }
}
