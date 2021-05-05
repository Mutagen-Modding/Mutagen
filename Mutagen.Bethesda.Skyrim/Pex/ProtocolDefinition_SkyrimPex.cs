using Loqui;
using Mutagen.Bethesda.Skyrim.Pex;

namespace Loqui
{
    public class ProtocolDefinition_SkyrimPex : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("SkyrimPex");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.DebugInfo_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.DebugFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.PexFile_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.Object_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.ObjectVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.ObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.Property_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.State_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.NamedFunction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.Function_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.FunctionVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.StringObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.IntegerObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.FloatObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.BoolObjectVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.Instruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.NothingInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.ProcessIntsInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.ProcessFloatsInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.IntRemainderInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.FlipBoolInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.NegateIntInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.AssignInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.CastInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.UnconditionalBranchInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.ConditionalBranchInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.CallMethodInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.CallParentInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.CallStaticInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.ReturnInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.StringConcatInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.PropertyGetInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.PropertySetInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.CreateArrayInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.GetArrayLengthInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.GetArrayElementInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.SetArrayElementInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.FindArrayElementInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.AMathInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.CompareInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.NegateFloatInstruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.AVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.IdentifierVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.StringVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.IntVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.FloatVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Pex.Internals.BoolVariable_Registration.Instance);
        }
    }
}
