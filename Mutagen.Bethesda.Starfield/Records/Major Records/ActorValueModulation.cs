namespace Mutagen.Bethesda.Starfield;

public partial class ActorValueModulation
{
    public enum TypeEnum
    {
        None,
        Simple,
        Complex,
        Modulation
    }

    [Flags]
    public enum TextureTypeEnum
    {
        Rough = 0x0001,
        Opacity = 0x0002,
        Normal = 0x0004,
        AO = 0x0008,
        Metal = 0x0010,
        Conditions = 0x0020,
        Emissive = 0x0040,
    }
}
