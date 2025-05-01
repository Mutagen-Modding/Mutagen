namespace Mutagen.Bethesda.Starfield;

public partial class Biome
{
    public enum TypeEnum
    {
        Default,
        Ocean,
        Polar,
        Mountain,
        Swamp,
        Archipelago,
        GasGiant
    }

    public enum TerrainMask
    {
        Base,
        Solid,
        FlatOuter,
        FlatInner,
        Talus,
        Flow,
        Path
    }
}