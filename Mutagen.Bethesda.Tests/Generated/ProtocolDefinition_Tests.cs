using Loqui;
using Mutagen.Bethesda.Tests;

namespace Loqui
{
    public class ProtocolDefinition_Tests : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Tests");
        public void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.BinaryProcessorInstructions_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.Move_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.DataTarget_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.Instruction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.RecordInstruction_Registration.Instance);
        }
    }
}
