using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod_Observable : ObservableModBase
    {
        public IObservable<TES4> TES4 { get; private set; }
        public IObservable<GroupObservable<GameSetting>> GameSettings { get; private set; }
        public IObservable<GroupObservable<Global>> Globals { get; private set; }
        public IObservable<GroupObservable<Class>> Classes { get; private set; }
        public IObservable<GroupObservable<Faction>> Factions { get; private set; }
        public IObservable<GroupObservable<Hair>> Hairs { get; private set; }
        public IObservable<GroupObservable<Eye>> Eyes { get; private set; }
        public IObservable<GroupObservable<Race>> Races { get; private set; }
        public IObservable<GroupObservable<Sound>> Sounds { get; private set; }
        public IObservable<GroupObservable<SkillRecord>> Skills { get; private set; }
        public IObservable<GroupObservable<MagicEffect>> MagicEffects { get; private set; }
        public IObservable<GroupObservable<Script>> Scripts { get; private set; }
        public IObservable<GroupObservable<LandTexture>> LandTextures { get; private set; }
        public IObservable<GroupObservable<Enchantment>> Enchantments { get; private set; }
        public IObservable<GroupObservable<SpellUnleveled>> Spells { get; private set; }
        public IObservable<GroupObservable<Birthsign>> Birthsigns { get; private set; }
        public IObservable<GroupObservable<Activator>> Activators { get; private set; }
        public IObservable<GroupObservable<AlchemicalApparatus>> AlchemicalApparatus { get; private set; }
        public IObservable<GroupObservable<Armor>> Armors { get; private set; }
        public IObservable<GroupObservable<Book>> Books { get; private set; }
        public IObservable<GroupObservable<Clothing>> Clothes { get; private set; }
        public IObservable<GroupObservable<Container>> Containers { get; private set; }
        public IObservable<GroupObservable<Door>> Doors { get; private set; }
        public IObservable<GroupObservable<Ingredient>> Ingredients { get; private set; }
        public IObservable<GroupObservable<Light>> Lights { get; private set; }
        public IObservable<GroupObservable<Miscellaneous>> Miscellaneous { get; private set; }
        public IObservable<GroupObservable<Static>> Statics { get; private set; }
        public IObservable<GroupObservable<Grass>> Grasses { get; private set; }
        public IObservable<GroupObservable<Tree>> Trees { get; private set; }
        public IObservable<GroupObservable<Flora>> Flora { get; private set; }
        public IObservable<GroupObservable<Furnature>> Furnature { get; private set; }
        public IObservable<GroupObservable<Weapon>> Weapons { get; private set; }
        public IObservable<GroupObservable<Ammo>> Ammo { get; private set; }
        public IObservable<GroupObservable<NPC>> NPCs { get; private set; }
        public IObservable<GroupObservable<Creature>> Creatures { get; private set; }
        public IObservable<GroupObservable<LeveledCreature>> LeveledCreatures { get; private set; }
        public IObservable<GroupObservable<SoulGem>> SoulGems { get; private set; }
        public IObservable<GroupObservable<Key>> Keys { get; private set; }
        public IObservable<GroupObservable<Potion>> Potions { get; private set; }
        public IObservable<GroupObservable<Subspace>> Subspaces { get; private set; }
        public IObservable<GroupObservable<SigilStone>> SigilStones { get; private set; }
        public IObservable<GroupObservable<LeveledItem>> LeveledItems { get; private set; }
        public IObservable<GroupObservable<Weather>> Weathers { get; private set; }
        public IObservable<GroupObservable<Climate>> Climates { get; private set; }
        public IObservable<GroupObservable<Region>> Regions { get; private set; }
        public IObservable<ListGroup<CellBlock>> Cells { get; private set; }
        public IObservable<GroupObservable<Worldspace>> Worldspaces { get; private set; }
        public IObservable<GroupObservable<DialogTopic>> DialogTopics { get; private set; }
        public IObservable<GroupObservable<Quest>> Quests { get; private set; }
        public IObservable<GroupObservable<IdleAnimation>> IdleAnimations { get; private set; }
        public IObservable<GroupObservable<AIPackage>> AIPackages { get; private set; }
        public IObservable<GroupObservable<CombatStyle>> CombatStyles { get; private set; }
        public IObservable<GroupObservable<LoadScreen>> LoadScreens { get; private set; }
        public IObservable<GroupObservable<LeveledSpell>> LeveledSpells { get; private set; }
        public IObservable<GroupObservable<AnimatedObject>> AnimatedObjects { get; private set; }
        public IObservable<GroupObservable<Water>> Waters { get; private set; }
        public IObservable<GroupObservable<EffectShader>> EffectShaders { get; private set; }

        public OblivionMod_Observable(IObservable<string> streamSource)
            : base(streamSource)
        {
        }

        public static OblivionMod_Observable FromPath(IObservable<string> streamSource)
        {
            OblivionMod_Observable ret = new OblivionMod_Observable(streamSource);
            ret.GameSettings = ret.GetGroupObservable<GameSetting>(GameSetting_Registration.TRIGGERING_RECORD_TYPE);
            ret.Globals = ret.GetGroupObservable<Global>(Global_Registration.TRIGGERING_RECORD_TYPE);
            ret.Classes = ret.GetGroupObservable<Class>(Class_Registration.TRIGGERING_RECORD_TYPE);
            ret.Factions = ret.GetGroupObservable<Faction>(Faction_Registration.TRIGGERING_RECORD_TYPE);
            ret.Hairs = ret.GetGroupObservable<Hair>(Hair_Registration.TRIGGERING_RECORD_TYPE);
            ret.Eyes = ret.GetGroupObservable<Eye>(Eye_Registration.TRIGGERING_RECORD_TYPE);
            ret.Races = ret.GetGroupObservable<Race>(Race_Registration.TRIGGERING_RECORD_TYPE);
            ret.Sounds = ret.GetGroupObservable<Sound>(Sound_Registration.TRIGGERING_RECORD_TYPE);
            ret.Skills = ret.GetGroupObservable<SkillRecord>(SkillRecord_Registration.TRIGGERING_RECORD_TYPE);
            ret.MagicEffects = ret.GetGroupObservable<MagicEffect>(MagicEffect_Registration.TRIGGERING_RECORD_TYPE);
            ret.Scripts = ret.GetGroupObservable<Script>(Script_Registration.TRIGGERING_RECORD_TYPE);
            ret.LandTextures = ret.GetGroupObservable<LandTexture>(LandTexture_Registration.TRIGGERING_RECORD_TYPE);
            ret.Enchantments = ret.GetGroupObservable<Enchantment>(Enchantment_Registration.TRIGGERING_RECORD_TYPE);
            ret.Spells = ret.GetGroupObservable<SpellUnleveled>(SpellUnleveled_Registration.TRIGGERING_RECORD_TYPE);
            ret.Birthsigns = ret.GetGroupObservable<Birthsign>(Birthsign_Registration.TRIGGERING_RECORD_TYPE);
            ret.Activators = ret.GetGroupObservable<Activator>(Activator_Registration.TRIGGERING_RECORD_TYPE);
            ret.AlchemicalApparatus = ret.GetGroupObservable<AlchemicalApparatus>(AlchemicalApparatus_Registration.TRIGGERING_RECORD_TYPE);
            ret.Armors = ret.GetGroupObservable<Armor>(Armor_Registration.TRIGGERING_RECORD_TYPE);
            ret.Books = ret.GetGroupObservable<Book>(Book_Registration.TRIGGERING_RECORD_TYPE);
            ret.Clothes = ret.GetGroupObservable<Clothing>(Clothing_Registration.TRIGGERING_RECORD_TYPE);
            ret.Containers = ret.GetGroupObservable<Container>(Container_Registration.TRIGGERING_RECORD_TYPE);
            ret.Doors = ret.GetGroupObservable<Door>(Door_Registration.TRIGGERING_RECORD_TYPE);
            ret.Ingredients = ret.GetGroupObservable<Ingredient>(Ingredient_Registration.TRIGGERING_RECORD_TYPE);
            ret.Lights = ret.GetGroupObservable<Light>(Light_Registration.TRIGGERING_RECORD_TYPE);
            ret.Miscellaneous = ret.GetGroupObservable<Miscellaneous>(Miscellaneous_Registration.TRIGGERING_RECORD_TYPE);
            ret.Statics = ret.GetGroupObservable<Static>(Static_Registration.TRIGGERING_RECORD_TYPE);
            ret.Grasses = ret.GetGroupObservable<Grass>(Grass_Registration.TRIGGERING_RECORD_TYPE);
            ret.Trees = ret.GetGroupObservable<Tree>(Tree_Registration.TRIGGERING_RECORD_TYPE);
            ret.Flora = ret.GetGroupObservable<Flora>(Flora_Registration.TRIGGERING_RECORD_TYPE);
            ret.Furnature = ret.GetGroupObservable<Furnature>(Furnature_Registration.TRIGGERING_RECORD_TYPE);
            ret.Weapons = ret.GetGroupObservable<Weapon>(Weapon_Registration.TRIGGERING_RECORD_TYPE);
            ret.Ammo = ret.GetGroupObservable<Ammo>(Ammo_Registration.TRIGGERING_RECORD_TYPE);
            ret.NPCs = ret.GetGroupObservable<NPC>(NPC_Registration.TRIGGERING_RECORD_TYPE);
            ret.Creatures = ret.GetGroupObservable<Creature>(Creature_Registration.TRIGGERING_RECORD_TYPE);
            ret.LeveledCreatures = ret.GetGroupObservable<LeveledCreature>(LeveledCreature_Registration.TRIGGERING_RECORD_TYPE);
            ret.SoulGems = ret.GetGroupObservable<SoulGem>(SoulGem_Registration.TRIGGERING_RECORD_TYPE);
            ret.Keys = ret.GetGroupObservable<Key>(Key_Registration.TRIGGERING_RECORD_TYPE);
            ret.Potions = ret.GetGroupObservable<Potion>(Potion_Registration.TRIGGERING_RECORD_TYPE);
            ret.Subspaces = ret.GetGroupObservable<Subspace>(Subspace_Registration.TRIGGERING_RECORD_TYPE);
            ret.SigilStones = ret.GetGroupObservable<SigilStone>(SigilStone_Registration.TRIGGERING_RECORD_TYPE);
            ret.LeveledItems = ret.GetGroupObservable<LeveledItem>(LeveledItem_Registration.TRIGGERING_RECORD_TYPE);
            ret.Weathers = ret.GetGroupObservable<Weather>(Weather_Registration.TRIGGERING_RECORD_TYPE);
            ret.Climates = ret.GetGroupObservable<Climate>(Climate_Registration.TRIGGERING_RECORD_TYPE);
            ret.Regions = ret.GetGroupObservable<Region>(Region_Registration.TRIGGERING_RECORD_TYPE);
            ret.Worldspaces = ret.GetGroupObservable<Worldspace>(Worldspace_Registration.TRIGGERING_RECORD_TYPE);
            ret.DialogTopics = ret.GetGroupObservable<DialogTopic>(DialogTopic_Registration.TRIGGERING_RECORD_TYPE);
            ret.Quests = ret.GetGroupObservable<Quest>(Quest_Registration.TRIGGERING_RECORD_TYPE);
            ret.IdleAnimations = ret.GetGroupObservable<IdleAnimation>(IdleAnimation_Registration.TRIGGERING_RECORD_TYPE);
            ret.AIPackages = ret.GetGroupObservable<AIPackage>(AIPackage_Registration.TRIGGERING_RECORD_TYPE);
            ret.CombatStyles = ret.GetGroupObservable<CombatStyle>(CombatStyle_Registration.TRIGGERING_RECORD_TYPE);
            ret.LoadScreens = ret.GetGroupObservable<LoadScreen>(LoadScreen_Registration.TRIGGERING_RECORD_TYPE);
            ret.LeveledSpells = ret.GetGroupObservable<LeveledSpell>(LeveledSpell_Registration.TRIGGERING_RECORD_TYPE);
            ret.AnimatedObjects = ret.GetGroupObservable<AnimatedObject>(AnimatedObject_Registration.TRIGGERING_RECORD_TYPE);
            ret.Waters = ret.GetGroupObservable<Water>(Water_Registration.TRIGGERING_RECORD_TYPE);
            ret.EffectShaders = ret.GetGroupObservable<EffectShader>(EffectShader_Registration.TRIGGERING_RECORD_TYPE);
            return ret;
        }

    }
}
