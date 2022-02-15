using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class GameCategoryExt
    {
        public static bool HasFormVersion(this GameCategory release)
        {
            return release switch
            {
                GameCategory.Oblivion => false,
                GameCategory.Skyrim => true,
                GameCategory.Fallout4 => true,
                _ => throw new NotImplementedException(),
            };
        }

        public static GameRelease DefaultRelease(this GameCategory gameCategory)
        {
            return gameCategory switch
            {
                GameCategory.Oblivion => GameRelease.Oblivion,
                GameCategory.Skyrim => GameRelease.SkyrimSE,
                GameCategory.Fallout4 => GameRelease.Fallout4,
                _ => throw new NotImplementedException(),
            };
        }

        public static IEnumerable<GameRelease> GetRelatedReleases(this GameCategory gameCategory)
        {
            switch (gameCategory)
            {
                case GameCategory.Oblivion:
                    yield return GameRelease.Oblivion;
                    yield break;
                case GameCategory.Skyrim:
                    yield return GameRelease.SkyrimLE;
                    yield return GameRelease.SkyrimSE;
                    yield return GameRelease.SkyrimVR;
                    yield return GameRelease.EnderalLE;
                    yield return GameRelease.EnderalSE;
                    yield break;
                case GameCategory.Fallout4:
                    yield return GameRelease.Fallout4;
                    yield break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool HasLocalization(this GameCategory category)
        {
            switch (category)
            {
                case GameCategory.Oblivion:
                    return false;
                case GameCategory.Skyrim:
                case GameCategory.Fallout4:
                default:
                    return true;
            }
        }
    }
}
