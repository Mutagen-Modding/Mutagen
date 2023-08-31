using Mutagen.Bethesda.Starfield;

namespace Loqui;

internal class ProtocolDefinition_Starfield : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Starfield");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            StarfieldGroup_Registration.Instance,
            StarfieldModHeader_Registration.Instance,
            ModStats_Registration.Instance,
            StarfieldMajorRecord_Registration.Instance,
            StarfieldMod_Registration.Instance,
            Npc_Registration.Instance,
            Race_Registration.Instance,
            Weapon_Registration.Instance,
            SimpleModel_Registration.Instance,
            Model_Registration.Instance
        );
    }
}
