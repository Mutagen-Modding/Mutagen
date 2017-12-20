using Loqui;
using Mutagen.Bethesda;

namespace Loqui
{
    public class ProtocolDefinition_Bethesda : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Bethesda");
        public void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Internals.MajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Internals.NamedMajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Internals.MasterReference_Registration.Instance);
        }
    }
}
