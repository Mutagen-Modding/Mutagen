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
            LoquiRegistration.Register(Mutagen.Internals.MasterReference_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.MajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSetting_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSettingInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSettingFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSettingString_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Group_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Global_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GlobalInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GlobalShort_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GlobalFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Class_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.NamedMajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.ClassData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.ClassTraining_Registration.Instance);
        }
    }
}
