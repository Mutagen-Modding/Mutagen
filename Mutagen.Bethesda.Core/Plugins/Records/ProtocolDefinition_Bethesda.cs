using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Loqui
{
    public class ProtocolDefinition_Bethesda : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Bethesda");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Plugins.Records.Internals.MajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Plugins.Records.Internals.MasterReference_Registration.Instance);
        }
    }
}
