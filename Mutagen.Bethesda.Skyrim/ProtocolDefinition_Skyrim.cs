using Loqui;
using Mutagen.Bethesda.Skyrim;

namespace Loqui
{
    public class ProtocolDefinition_Skyrim : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Skyrim");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.SkyrimMod_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ModHeader_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ModStats_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.SkyrimMajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GameSetting_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GameSettingInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GameSettingFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GameSettingString_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Global_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GlobalInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GlobalShort_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GlobalFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Group_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GameSettingBool_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Keyword_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.LocationReferenceType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Action_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ObjectBounds_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.TextureSet_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Textures_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Decal_Registration.Instance);
        }
    }
}
