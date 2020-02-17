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
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ActionRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ObjectBounds_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.TextureSet_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Textures_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Decal_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Class_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Relation_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Faction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.FormList_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Outfit_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.PlacedObject_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Rank_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.VendorValues_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.VendorLocation_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Condition_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ConditionGlobal_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ConditionFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.FunctionConditionData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GetEventData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ConditionData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Model_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.AlternateTexture_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.HeadPart_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Part_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Hair_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Eye_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Armor_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Race_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.SpellAbstract_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.BodyTemplate_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.RaceData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.SkillBoost_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.GenderedFormLinks_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.VoiceType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.ColorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.Attack_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.AttackData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.BodyDataPair_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.BodyData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Skyrim.Internals.BodyPartData_Registration.Instance);
        }
    }
}
