using Mutagen.Bethesda.Plugins.Records;

namespace Loqui;

internal class ProtocolDefinition_Bethesda : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Bethesda");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            MajorRecord_Registration.Instance,
            MasterReference_Registration.Instance
        );
    }
}
