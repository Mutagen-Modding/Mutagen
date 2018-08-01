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
    public class OblivionMod_Observable
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
    }
}
