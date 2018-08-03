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

        public OblivionMod_Observable()
        {
        }

        public OblivionMod_Observable(IObservable<string> streamSource)
        {
            Init();
        }

        private void Init()
        {
            this.TES4 = this.GetObservableRecord<TES4>(TES4_Registration.TRIGGERING_RECORD_TYPE);
            this.GameSettings = this.GetGroupObservable<GameSetting>(GameSetting_Registration.TRIGGERING_RECORD_TYPE);
            this.Globals = this.GetGroupObservable<Global>(Global_Registration.TRIGGERING_RECORD_TYPE);
            this.Classes = this.GetGroupObservable<Class>(Class_Registration.TRIGGERING_RECORD_TYPE);
            this.Factions = this.GetGroupObservable<Faction>(Faction_Registration.TRIGGERING_RECORD_TYPE);
            this.Hairs = this.GetGroupObservable<Hair>(Hair_Registration.TRIGGERING_RECORD_TYPE);
            this.Eyes = this.GetGroupObservable<Eye>(Eye_Registration.TRIGGERING_RECORD_TYPE);
            this.Races = this.GetGroupObservable<Race>(Race_Registration.TRIGGERING_RECORD_TYPE);
            this.Sounds = this.GetGroupObservable<Sound>(Sound_Registration.TRIGGERING_RECORD_TYPE);
            this.Skills = this.GetGroupObservable<SkillRecord>(SkillRecord_Registration.TRIGGERING_RECORD_TYPE);
            this.MagicEffects = this.GetGroupObservable<MagicEffect>(MagicEffect_Registration.TRIGGERING_RECORD_TYPE);
            this.Scripts = this.GetGroupObservable<Script>(Script_Registration.TRIGGERING_RECORD_TYPE);
            this.LandTextures = this.GetGroupObservable<LandTexture>(LandTexture_Registration.TRIGGERING_RECORD_TYPE);
            this.Enchantments = this.GetGroupObservable<Enchantment>(Enchantment_Registration.TRIGGERING_RECORD_TYPE);
            this.Spells = this.GetGroupObservable<SpellUnleveled>(SpellUnleveled_Registration.TRIGGERING_RECORD_TYPE);
            this.Birthsigns = this.GetGroupObservable<Birthsign>(Birthsign_Registration.TRIGGERING_RECORD_TYPE);
            this.Activators = this.GetGroupObservable<Activator>(Activator_Registration.TRIGGERING_RECORD_TYPE);
            this.AlchemicalApparatus = this.GetGroupObservable<AlchemicalApparatus>(AlchemicalApparatus_Registration.TRIGGERING_RECORD_TYPE);
            this.Armors = this.GetGroupObservable<Armor>(Armor_Registration.TRIGGERING_RECORD_TYPE);
            this.Books = this.GetGroupObservable<Book>(Book_Registration.TRIGGERING_RECORD_TYPE);
            this.Clothes = this.GetGroupObservable<Clothing>(Clothing_Registration.TRIGGERING_RECORD_TYPE);
            this.Containers = this.GetGroupObservable<Container>(Container_Registration.TRIGGERING_RECORD_TYPE);
            this.Doors = this.GetGroupObservable<Door>(Door_Registration.TRIGGERING_RECORD_TYPE);
            this.Ingredients = this.GetGroupObservable<Ingredient>(Ingredient_Registration.TRIGGERING_RECORD_TYPE);
            this.Lights = this.GetGroupObservable<Light>(Light_Registration.TRIGGERING_RECORD_TYPE);
            this.Miscellaneous = this.GetGroupObservable<Miscellaneous>(Miscellaneous_Registration.TRIGGERING_RECORD_TYPE);
            this.Statics = this.GetGroupObservable<Static>(Static_Registration.TRIGGERING_RECORD_TYPE);
            this.Grasses = this.GetGroupObservable<Grass>(Grass_Registration.TRIGGERING_RECORD_TYPE);
            this.Trees = this.GetGroupObservable<Tree>(Tree_Registration.TRIGGERING_RECORD_TYPE);
            this.Flora = this.GetGroupObservable<Flora>(Flora_Registration.TRIGGERING_RECORD_TYPE);
            this.Furnature = this.GetGroupObservable<Furnature>(Furnature_Registration.TRIGGERING_RECORD_TYPE);
            this.Weapons = this.GetGroupObservable<Weapon>(Weapon_Registration.TRIGGERING_RECORD_TYPE);
            this.Ammo = this.GetGroupObservable<Ammo>(Ammo_Registration.TRIGGERING_RECORD_TYPE);
            this.NPCs = this.GetGroupObservable<NPC>(NPC_Registration.TRIGGERING_RECORD_TYPE);
            this.Creatures = this.GetGroupObservable<Creature>(Creature_Registration.TRIGGERING_RECORD_TYPE);
            this.LeveledCreatures = this.GetGroupObservable<LeveledCreature>(LeveledCreature_Registration.TRIGGERING_RECORD_TYPE);
            this.SoulGems = this.GetGroupObservable<SoulGem>(SoulGem_Registration.TRIGGERING_RECORD_TYPE);
            this.Keys = this.GetGroupObservable<Key>(Key_Registration.TRIGGERING_RECORD_TYPE);
            this.Potions = this.GetGroupObservable<Potion>(Potion_Registration.TRIGGERING_RECORD_TYPE);
            this.Subspaces = this.GetGroupObservable<Subspace>(Subspace_Registration.TRIGGERING_RECORD_TYPE);
            this.SigilStones = this.GetGroupObservable<SigilStone>(SigilStone_Registration.TRIGGERING_RECORD_TYPE);
            this.LeveledItems = this.GetGroupObservable<LeveledItem>(LeveledItem_Registration.TRIGGERING_RECORD_TYPE);
            this.Weathers = this.GetGroupObservable<Weather>(Weather_Registration.TRIGGERING_RECORD_TYPE);
            this.Climates = this.GetGroupObservable<Climate>(Climate_Registration.TRIGGERING_RECORD_TYPE);
            this.Regions = this.GetGroupObservable<Region>(Region_Registration.TRIGGERING_RECORD_TYPE);
        }

        public OblivionMod_Observable Where(Func<KeyValuePair<FormID, IObservable<MajorRecord>>, bool> selector)
        {
            return new OblivionMod_Observable()
            {
                TES4 = this.TES4,
                GameSettings = this.GameSettings.Select((g) => g.Where(selector)),
                Globals = this.Globals.Select((g) => g.Where(selector)),
                Classes = this.Classes.Select((g) => g.Where(selector)),
                Factions = this.Factions.Select((g) => g.Where(selector)),
                Hairs = this.Hairs.Select((g) => g.Where(selector)),
                Eyes = this.Eyes.Select((g) => g.Where(selector)),
                Races = this.Races.Select((g) => g.Where(selector)),
                Sounds = this.Sounds.Select((g) => g.Where(selector)),
                Skills = this.Skills.Select((g) => g.Where(selector)),
                MagicEffects = this.MagicEffects.Select((g) => g.Where(selector)),
                Scripts = this.Scripts.Select((g) => g.Where(selector)),
                LandTextures = this.LandTextures.Select((g) => g.Where(selector)),
                Enchantments = this.Enchantments.Select((g) => g.Where(selector)),
                Spells = this.Spells.Select((g) => g.Where(selector)),
                Birthsigns = this.Birthsigns.Select((g) => g.Where(selector)),
                Activators = this.Activators.Select((g) => g.Where(selector)),
                AlchemicalApparatus = this.AlchemicalApparatus.Select((g) => g.Where(selector)),
                Armors = this.Armors.Select((g) => g.Where(selector)),
                Books = this.Books.Select((g) => g.Where(selector)),
                Clothes = this.Clothes.Select((g) => g.Where(selector)),
                Containers = this.Containers.Select((g) => g.Where(selector)),
                Doors = this.Doors.Select((g) => g.Where(selector)),
                Ingredients = this.Ingredients.Select((g) => g.Where(selector)),
                Lights = this.Lights.Select((g) => g.Where(selector)),
                Miscellaneous = this.Miscellaneous.Select((g) => g.Where(selector)),
                Statics = this.Statics.Select((g) => g.Where(selector)),
                Grasses = this.Grasses.Select((g) => g.Where(selector)),
                Trees = this.Trees.Select((g) => g.Where(selector)),
                Flora = this.Flora.Select((g) => g.Where(selector)),
                Furnature = this.Furnature.Select((g) => g.Where(selector)),
                Weapons = this.Weapons.Select((g) => g.Where(selector)),
                Ammo = this.Ammo.Select((g) => g.Where(selector)),
                NPCs = this.NPCs.Select((g) => g.Where(selector)),
                Creatures = this.Creatures.Select((g) => g.Where(selector)),
                LeveledCreatures = this.LeveledCreatures.Select((g) => g.Where(selector)),
                SoulGems = this.SoulGems.Select((g) => g.Where(selector)),
                Keys = this.Keys.Select((g) => g.Where(selector)),
                Potions = this.Potions.Select((g) => g.Where(selector)),
                Subspaces = this.Subspaces.Select((g) => g.Where(selector)),
                SigilStones = this.SigilStones.Select((g) => g.Where(selector)),
                LeveledItems = this.LeveledItems.Select((g) => g.Where(selector)),
                Weathers = this.Weathers.Select((g) => g.Where(selector)),
                Climates = this.Climates.Select((g) => g.Where(selector)),
                Regions = this.Regions.Select((g) => g.Where(selector)),
            };
        }

        public OblivionMod_Observable Do(Action<MajorRecord> doAction)
        {
            return new OblivionMod_Observable()
            {
                TES4 = this.TES4,
                GameSettings = this.GameSettings.Select((g) => g.Do(doAction)),
                Globals = this.Globals.Select((g) => g.Do(doAction)),
                Classes = this.Classes.Select((g) => g.Do(doAction)),
                Factions = this.Factions.Select((g) => g.Do(doAction)),
                Hairs = this.Hairs.Select((g) => g.Do(doAction)),
                Eyes = this.Eyes.Select((g) => g.Do(doAction)),
                Races = this.Races.Select((g) => g.Do(doAction)),
                Sounds = this.Sounds.Select((g) => g.Do(doAction)),
                Skills = this.Skills.Select((g) => g.Do(doAction)),
                MagicEffects = this.MagicEffects.Select((g) => g.Do(doAction)),
                Scripts = this.Scripts.Select((g) => g.Do(doAction)),
                LandTextures = this.LandTextures.Select((g) => g.Do(doAction)),
                Enchantments = this.Enchantments.Select((g) => g.Do(doAction)),
                Spells = this.Spells.Select((g) => g.Do(doAction)),
                Birthsigns = this.Birthsigns.Select((g) => g.Do(doAction)),
                Activators = this.Activators.Select((g) => g.Do(doAction)),
                AlchemicalApparatus = this.AlchemicalApparatus.Select((g) => g.Do(doAction)),
                Armors = this.Armors.Select((g) => g.Do(doAction)),
                Books = this.Books.Select((g) => g.Do(doAction)),
                Clothes = this.Clothes.Select((g) => g.Do(doAction)),
                Containers = this.Containers.Select((g) => g.Do(doAction)),
                Doors = this.Doors.Select((g) => g.Do(doAction)),
                Ingredients = this.Ingredients.Select((g) => g.Do(doAction)),
                Lights = this.Lights.Select((g) => g.Do(doAction)),
                Miscellaneous = this.Miscellaneous.Select((g) => g.Do(doAction)),
                Statics = this.Statics.Select((g) => g.Do(doAction)),
                Grasses = this.Grasses.Select((g) => g.Do(doAction)),
                Trees = this.Trees.Select((g) => g.Do(doAction)),
                Flora = this.Flora.Select((g) => g.Do(doAction)),
                Furnature = this.Furnature.Select((g) => g.Do(doAction)),
                Weapons = this.Weapons.Select((g) => g.Do(doAction)),
                Ammo = this.Ammo.Select((g) => g.Do(doAction)),
                NPCs = this.NPCs.Select((g) => g.Do(doAction)),
                Creatures = this.Creatures.Select((g) => g.Do(doAction)),
                LeveledCreatures = this.LeveledCreatures.Select((g) => g.Do(doAction)),
                SoulGems = this.SoulGems.Select((g) => g.Do(doAction)),
                Keys = this.Keys.Select((g) => g.Do(doAction)),
                Potions = this.Potions.Select((g) => g.Do(doAction)),
                Subspaces = this.Subspaces.Select((g) => g.Do(doAction)),
                SigilStones = this.SigilStones.Select((g) => g.Do(doAction)),
                LeveledItems = this.LeveledItems.Select((g) => g.Do(doAction)),
                Weathers = this.Weathers.Select((g) => g.Do(doAction)),
                Climates = this.Climates.Select((g) => g.Do(doAction)),
                Regions = this.Regions.Select((g) => g.Do(doAction)),
            };
        }

        public async Task Write_Binary(MutagenWriter writer)
        {
            (await this.TES4.LastAsync()).Write_Binary(writer);
            await WriteGroup(writer, this.GameSettings);
            await WriteGroup(writer, this.Globals);
            await WriteGroup(writer, this.Classes);
            await WriteGroup(writer, this.Factions);
            await WriteGroup(writer, this.Hairs);
            await WriteGroup(writer, this.Eyes);
            await WriteGroup(writer, this.Races);
            await WriteGroup(writer, this.Sounds);
            await WriteGroup(writer, this.Skills);
            await WriteGroup(writer, this.MagicEffects);
            await WriteGroup(writer, this.Scripts);
            await WriteGroup(writer, this.LandTextures);
            await WriteGroup(writer, this.Enchantments);
            await WriteGroup(writer, this.Spells);
            await WriteGroup(writer, this.Birthsigns);
            await WriteGroup(writer, this.Activators);
            await WriteGroup(writer, this.AlchemicalApparatus);
            await WriteGroup(writer, this.Armors);
            await WriteGroup(writer, this.Books);
            await WriteGroup(writer, this.Clothes);
            await WriteGroup(writer, this.Containers);
            await WriteGroup(writer, this.Doors);
            await WriteGroup(writer, this.Ingredients);
            await WriteGroup(writer, this.Lights);
            await WriteGroup(writer, this.Miscellaneous);
            await WriteGroup(writer, this.Statics);
            await WriteGroup(writer, this.Grasses);
            await WriteGroup(writer, this.Trees);
            await WriteGroup(writer, this.Flora);
            await WriteGroup(writer, this.Furnature);
            await WriteGroup(writer, this.Weapons);
            await WriteGroup(writer, this.Ammo);
            await WriteGroup(writer, this.NPCs);
            await WriteGroup(writer, this.Creatures);
            await WriteGroup(writer, this.LeveledCreatures);
            await WriteGroup(writer, this.SoulGems);
            await WriteGroup(writer, this.Keys);
            await WriteGroup(writer, this.Potions);
            await WriteGroup(writer, this.Subspaces);
            await WriteGroup(writer, this.SigilStones);
            await WriteGroup(writer, this.LeveledItems);
            await WriteGroup(writer, this.Weathers);
            await WriteGroup(writer, this.Climates);
            await WriteGroup(writer, this.Regions);
        }

        public async Task Write_Binary(string path)
        {
            using (var writer = new MutagenWriter(path))
            {
                await Write_Binary(writer);
            }
        }

    }
}
