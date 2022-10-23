namespace Mutagen.Bethesda;

/// <summary>
/// Modes that games can be installed
/// </summary>
[Flags]
public enum GameInstallMode
{
    Steam = 0x01,
    /*Nog*/Gog = 0x02,
}