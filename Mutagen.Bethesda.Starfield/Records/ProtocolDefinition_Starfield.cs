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
            FFKWRecord_Registration.Instance,
            BFCBRecord_Registration.Instance,
            ActionRecord_Registration.Instance,
            Transform_Registration.Instance,
            ObjectBounds_Registration.Instance,
            TextureSet_Registration.Instance,
            Global_Registration.Instance,
            DamageTypeItem_Registration.Instance,
            DamageType_Registration.Instance,
            Spell_Registration.Instance,
            ObjectProperty_Registration.Instance,
            ActorValueInformation_Registration.Instance,
            Class_Registration.Instance,
            CurveTable_Registration.Instance,
            LocationTargetRadius_Registration.Instance,
            ALocationTarget_Registration.Instance,
            LocationTarget_Registration.Instance,
            LocationCell_Registration.Instance,
            LocationObjectId_Registration.Instance,
            LocationObjectType_Registration.Instance,
            LocationKeyword_Registration.Instance,
            LocationFallback_Registration.Instance,
            Relation_Registration.Instance,
            Cell_Registration.Instance,
            Faction_Registration.Instance,
            CrimeValues_Registration.Instance,
            VendorValues_Registration.Instance,
            FormList_Registration.Instance,
            PlacedObject_Registration.Instance,
            VoiceType_Registration.Instance,
            AFFERecord_Registration.Instance,
            AFFESubrecord_Registration.Instance,
            AOPFRecord_Registration.Instance
        );
    }
}
