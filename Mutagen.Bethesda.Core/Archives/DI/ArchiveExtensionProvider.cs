using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Archives.DI;

public interface IArchiveExtensionProvider
{
    /// <summary>
    /// Returns the preferred extension (.bsa/.ba2) depending on the Game Release context
    /// </summary>
    /// <returns>Archive extension used by the game release context, with period delimiter.</returns>
    string Get();
}

public sealed class ArchiveExtensionProvider : IArchiveExtensionProvider
{
    private readonly IGameReleaseContext _gameReleaseContext;

    public ArchiveExtensionProvider(IGameReleaseContext gameReleaseContext)
    {
        _gameReleaseContext = gameReleaseContext;
    }
        
    public string Get()
    {
        switch (_gameReleaseContext.Release.ToCategory())
        {
            case GameCategory.Oblivion:
            case GameCategory.Skyrim:
                return ".bsa";
            case GameCategory.Fallout4:
            case GameCategory.Starfield:
                return ".ba2";
            default:
                throw new NotImplementedException();
        }
    }
}