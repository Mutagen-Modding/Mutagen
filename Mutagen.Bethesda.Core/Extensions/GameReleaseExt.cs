using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda;

public static class GameReleaseExt
{
    public static StringsLanguageFormat GetLanguageFormat(this GameRelease release)
    {
        switch (release)
        {
            case GameRelease.Oblivion:
                throw new ArgumentException($"Tried to get language format for an unsupported game: {release}", nameof(release));
            case GameRelease.SkyrimLE:
            case GameRelease.SkyrimSE:
            case GameRelease.SkyrimSEGog:
            case GameRelease.SkyrimVR:
            case GameRelease.EnderalLE:
            case GameRelease.EnderalSE:
                return StringsLanguageFormat.FullName;
            case GameRelease.Fallout4:
            case GameRelease.Starfield:
                return StringsLanguageFormat.Iso;
            default:
                throw new NotImplementedException();
        }
    }
}