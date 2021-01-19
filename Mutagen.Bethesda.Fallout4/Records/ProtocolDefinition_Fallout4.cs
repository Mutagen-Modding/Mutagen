using Loqui;
using Mutagen.Bethesda.Fallout4;

namespace Loqui
{
    public class ProtocolDefinition_Fallout4 : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Fallout4");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Fallout4MajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Fallout4Mod_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Fallout4ModHeader_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ModStats_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Group_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GameSetting_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GameSettingInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GameSettingFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GameSettingString_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GameSettingBool_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Keyword_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.TransientType_Registration.Instance);
        }
    }
}
