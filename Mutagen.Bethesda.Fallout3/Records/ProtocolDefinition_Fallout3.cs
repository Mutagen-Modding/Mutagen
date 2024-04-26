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
            Fallout3Group_Registration.Instance,
            Fallout3ListGroup_Registration.Instance,
            Fallout3MajorRecord_Registration.Instance,
            Fallout3Mod_Registration.Instance,
            Fallout3ModHeader_Registration.Instance,
            Global_Registration.Instance,
            GlobalFloat_Registration.Instance,
            GlobalInt_Registration.Instance,
            GlobalShort_Registration.Instance,
            GlobalUnknown_Registration.Instance,
            ModStats_Registration.Instance,
            Npc_Registration.Instance
        );
    }
}
