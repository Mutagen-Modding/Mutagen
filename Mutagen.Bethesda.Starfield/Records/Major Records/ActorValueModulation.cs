namespace Mutagen.Bethesda.Starfield;

public partial class ActorValueModulation
{
    public enum TypeEnum
    {
        SimpleGroup,
        ComplexGroup,
        Modulation
    }

    public enum TextureTypeEnum
    {
        None = 0,
        Rough = 1,
        Opacity = 2,
        Normal = 3,
        AO = 4,
        Metal = 5,
        Conditions = 6,
        Emissive = 7,
    }
}
