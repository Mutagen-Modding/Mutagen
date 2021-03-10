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
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GameSettingUInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.TransientType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.AttractionRule_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Keyword_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationReferenceType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ActionRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Transform_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ObjectBounds_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Component_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Global_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.MiscItem_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.SoundDescriptor_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Decal_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.TextureSet_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GlobalInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GlobalShort_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GlobalFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.GlobalBool_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ActorValueInformation_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ASpell_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ADamageType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.DamageType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.DamageTypeIndexed_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Properties_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Class_Registration.Instance);
        }
    }
}
