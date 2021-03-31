using Loqui;
using Mutagen.Bethesda.Pex;

namespace Loqui
{
    public class ProtocolDefinition_Pex : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Pex");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.DebugInfo_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.DebugFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.DebugPropertyGroup_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.DebugStructOrder_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexFile_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObject_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectStructInfo_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectStructInfoMember_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectProperty_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectState_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectNamedFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectFunctionVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.Internals.PexObjectFunctionInstruction_Registration.Instance);
        }
    }
}
