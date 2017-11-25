using Loqui;
using Mutagen;

namespace Loqui
{
    public class ProtocolDefinition_Mutagen : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Mutagen");
        public void Register()
        {
            LoquiRegistration.Register(Mutagen.Internals.Header_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.TES4_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.OblivionMod_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.MasterReference_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.MajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSetting_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSettingInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSettingFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GameSettingString_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Group_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Global_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GlobalInt_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GlobalShort_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GlobalFloat_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Class_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.NamedMajorRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.ClassTraining_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Model_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Hair_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Faction_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Relation_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Rank_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Race_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.SkillBoost_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Spell_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Eye_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.RaceStatsGendered_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.RaceStats_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.RaceVoices_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.RaceHair_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.FacePart_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.GenderedBodyData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.BodyData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.BodyPart_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.FaceGenData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.Sound_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.SoundData_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.SoundDataExtended_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.SkillRecord_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Internals.MagicEffect_Registration.Instance);
        }
    }
}
