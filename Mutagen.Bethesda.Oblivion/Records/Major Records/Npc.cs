using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class Npc
{
    [Flags]
    public enum NpcFlag
    {
        Female = 0x000001,
        Essential = 0x000002,
        Respawn = 0x000008,
        AutoCalcStats = 0x000010,
        PCLevelOffset = 0x000080,
        NoLowLevelProcessing = 0x000200,
        NoRumors = 0x002000,
        Summonable = 0x004000,
        NoPersuasion = 0x008000,
        CanCorpseCheck = 0x100000,
    }

    [Flags]
    public enum BuySellServiceFlag
    {
        Weapons = 0x00001,
        Armor = 0x00002,
        Clothing = 0x00004,
        Books = 0x00008,
        Ingredients = 0x00010,
        Lights = 0x00080,
        Apparatus = 0x00100,
        Miscellaneous = 0x00400,
        Spells = 0x00800,
        MagicItems = 0x01000,
        Potions = 0x02000,
        Training = 0x04000,
        Recharge = 0x10000,
        Repair = 0x20000,
    }
}