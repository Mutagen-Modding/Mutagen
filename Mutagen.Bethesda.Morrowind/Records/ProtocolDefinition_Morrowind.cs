using Mutagen.Bethesda.Morrowind;

namespace Loqui;

internal class ProtocolDefinition_Morrowind : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Morrowind");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            Cell_Registration.Instance,
            CellBlock_Registration.Instance,
            CellSubBlock_Registration.Instance,
            GameSetting_Registration.Instance,
            GameSettingFloat_Registration.Instance,
            GameSettingInt_Registration.Instance,
            GameSettingString_Registration.Instance,
            ModStats_Registration.Instance,
            MorrowindGroup_Registration.Instance,
            MorrowindListGroup_Registration.Instance,
            MorrowindMajorRecord_Registration.Instance,
            MorrowindMod_Registration.Instance,
            MorrowindModHeader_Registration.Instance
        );
    }
}
