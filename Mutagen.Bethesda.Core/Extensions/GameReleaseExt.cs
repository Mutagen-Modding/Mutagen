using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda
{
    public static class GameReleaseExt
    {
        public static GameCategory ToCategory(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => GameCategory.Oblivion,
                GameRelease.SkyrimLE => GameCategory.Skyrim,
                GameRelease.SkyrimSE => GameCategory.Skyrim,
                GameRelease.SkyrimVR => GameCategory.Skyrim,
                GameRelease.EnderalLE => GameCategory.Skyrim,
                GameRelease.EnderalSE => GameCategory.Skyrim,
                GameRelease.Fallout4 => GameCategory.Fallout4,
                _ => throw new NotImplementedException(),
            };
        }

        public static ushort? GetDefaultFormVersion(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => default,
                GameRelease.SkyrimLE => 43,
                GameRelease.EnderalLE => 43,
                GameRelease.SkyrimSE => 44,
                GameRelease.EnderalSE => 44,
                GameRelease.SkyrimVR => 44,
                GameRelease.Fallout4 => 131,
                _ => throw new NotImplementedException(),
            };
        }

        public static StringsLanguageFormat GetLanguageFormat(this GameRelease release)
        {
            switch (release)
            {
                case GameRelease.Oblivion:
                    throw new ArgumentException($"Tried to get language format for an unsupported game: {release}", nameof(release));
                case GameRelease.SkyrimLE:
                case GameRelease.SkyrimSE:
                case GameRelease.SkyrimVR:
                case GameRelease.EnderalLE:
                case GameRelease.EnderalSE:
                    return StringsLanguageFormat.FullName;
                case GameRelease.Fallout4:
                    return StringsLanguageFormat.Iso;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
