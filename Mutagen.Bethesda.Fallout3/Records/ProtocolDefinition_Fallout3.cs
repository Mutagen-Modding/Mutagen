using Mutagen.Bethesda.Fallout3;

namespace Loqui;

internal class ProtocolDefinition_Fallout3 : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Fallout3");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            Cell_Registration.Instance,
            CellBlock_Registration.Instance,
            CellSubBlock_Registration.Instance,
            Decal_Registration.Instance,
            Fallout3Group_Registration.Instance,
            Fallout3ListGroup_Registration.Instance,
            Fallout3MajorRecord_Registration.Instance,
            Fallout3Mod_Registration.Instance,
            Fallout3ModHeader_Registration.Instance,
            GameSetting_Registration.Instance,
            GameSettingFloat_Registration.Instance,
            GameSettingInt_Registration.Instance,
            GameSettingString_Registration.Instance,
            Global_Registration.Instance,
            GlobalFloat_Registration.Instance,
            GlobalInt_Registration.Instance,
            GlobalShort_Registration.Instance,
            GlobalUnknown_Registration.Instance,
            Icons_Registration.Instance,
            MenuIcon_Registration.Instance,
            ModStats_Registration.Instance,
            Npc_Registration.Instance,
            ObjectBounds_Registration.Instance,
            PlacedObject_Registration.Instance,
            Race_Registration.Instance,
            TextureSet_Registration.Instance
        );
    }
}
