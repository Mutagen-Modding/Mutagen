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
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.DebugInfo_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.DebugFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.DebugPropertyGroup_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.DebugStructOrder_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexFile_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObject_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectStructInfo_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectStructInfoMember_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectProperty_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectState_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectNamedFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectFunctionVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Pex.PexObjectFunctionInstruction_Registration.Instance);
        }
    }
}
