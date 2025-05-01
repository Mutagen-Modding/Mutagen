namespace Mutagen.Bethesda.Starfield;

public partial class AcousticSpace
{
    [Flags]
    public enum DisableFlag
    {
        EnvironmentType = 0x00001,
        MusicType = 0x00002,
        SoundDetectionLevel = 0x00004,
        LoopingSound = 0x00008,
        ExteriorWeatherAttenuation = 0x00010,
        IsInterior = 0x00020,
        CrowdWalls = 0x00040,
        AffectsParentedSoundMarkers = 0x00080,
    }
}