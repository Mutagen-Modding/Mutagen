using Mutagen.Bethesda.Pex;

namespace Loqui;

internal class ProtocolDefinition_Pex : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Pex");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            DebugInfo_Registration.Instance,
            DebugFunction_Registration.Instance,
            DebugPropertyGroup_Registration.Instance,
            DebugStructOrder_Registration.Instance,
            PexFile_Registration.Instance,
            PexObject_Registration.Instance,
            PexObjectStructInfo_Registration.Instance,
            PexObjectStructInfoMember_Registration.Instance,
            PexObjectVariable_Registration.Instance,
            PexObjectVariableData_Registration.Instance,
            PexObjectProperty_Registration.Instance,
            PexObjectState_Registration.Instance,
            PexObjectNamedFunction_Registration.Instance,
            PexObjectFunction_Registration.Instance,
            PexObjectFunctionVariable_Registration.Instance,
            PexObjectFunctionInstruction_Registration.Instance
        );
    }
}
