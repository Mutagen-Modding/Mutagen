using Loqui;
using Mutagen;

namespace Loqui
{
    public class ProtocolDefinition_Mutagen : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Mutagen");
        public void Register()
        {
            LoquiRegistration.Register(Mutagen.Internals.Header_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.TES4_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.OblivionMod_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.UnknownData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.MasterReference_Registration.Instance);
        }
    }
}
