using Mutagen.Bethesda.Starfield;

namespace Loqui;

internal class ProtocolDefinition_Starfield : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Starfield");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            ModStats_Registration.Instance,
            TransientType_Registration.Instance,
            LocationReferenceType_Registration.Instance,
            StarfieldGroup_Registration.Instance,
            StarfieldMajorRecord_Registration.Instance,
            StarfieldMod_Registration.Instance,
            Npc_Registration.Instance,
            Race_Registration.Instance,
            Weapon_Registration.Instance,
            SimpleModel_Registration.Instance,
            Model_Registration.Instance,
            StarfieldModHeader_Registration.Instance,
            GameSetting_Registration.Instance,
            GameSettingInt_Registration.Instance,
            GameSettingFloat_Registration.Instance,
            GameSettingString_Registration.Instance,
            GameSettingBool_Registration.Instance,
            GameSettingUInt_Registration.Instance,
            Keyword_Registration.Instance,
            AttractionRule_Registration.Instance,
            FFKW_Registration.Instance,
            BFCBRecord_Registration.Instance
        );
    }
}
