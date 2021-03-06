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
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ADamageType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.DamageType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.DamageTypeIndexed_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Properties_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Class_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Condition_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ConditionGlobal_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ConditionFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ConditionData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationTargetRadius_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ALocationTarget_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationTarget_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationCell_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationObjectId_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationObjectType_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationKeyword_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LocationFallback_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Relation_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Cell_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Faction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.CrimeValues_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.VendorValues_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Rank_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.FormList_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Outfit_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.PlacedObject_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Door_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ColorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.HeadPart_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Part_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.MaterialSwap_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.SimpleModel_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Model_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.BipedBodyTemplate_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.AnimationSoundTagSet_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Armor_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Race_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Debris_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.Explosion_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.ImpactDataSet_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Fallout4.Internals.LeveledSpell_Registration.Instance);
        }
    }
}
