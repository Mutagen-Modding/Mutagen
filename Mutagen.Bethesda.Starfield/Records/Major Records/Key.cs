namespace Mutagen.Bethesda.Starfield;

partial class Key
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0002,
        CalcValueFromComponents = 0x0000_0800,
        PackInUseOnly = 0x0000_2000
    }
    
    [Flags]
    public enum Flag
    {
        AllowQuestItemCrafting,
        NonInstancedKey,
    }
}