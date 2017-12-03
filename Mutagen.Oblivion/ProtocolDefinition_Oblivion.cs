using Loqui;
using Mutagen.Oblivion;

namespace Loqui
{
    public class ProtocolDefinition_Oblivion : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Oblivion");
        public void Register()
        {
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Header_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.TES4_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.OblivionMod_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.MasterReference_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.MajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GameSetting_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GameSettingInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GameSettingFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GameSettingString_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Group_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Global_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GlobalInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GlobalShort_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GlobalFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Class_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.NamedMajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.ClassTraining_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Model_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Hair_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Faction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Relation_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Rank_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Race_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.SkillBoost_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Spell_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Eye_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.RaceStatsGendered_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.RaceStats_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.RaceVoices_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.RaceHair_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.FacePart_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.GenderedBodyData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.BodyData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.BodyPart_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.FaceGenData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Sound_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.SoundData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.SoundDataExtended_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.SkillRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.MagicEffect_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.MagicData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Script_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.ScriptMetaSummary_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.LocalVariable_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.LocalVariableData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.ScriptReference_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.LandTexture_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.HavokData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.EnchantmentEffect_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.ScriptEffect_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Oblivion.Internals.Enchantment_Registration.Instance);
        }
    }
}
