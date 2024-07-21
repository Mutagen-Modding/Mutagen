using Loqui;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda;

public static class GameCategoryExt
{
    public static bool HasFormVersion(this GameCategory release)
    {
        return release switch
        {
            GameCategory.Oblivion => false,
            GameCategory.Skyrim => true,
            GameCategory.Fallout4 => true,
            GameCategory.Starfield => true,
        };
    }

    public static GameRelease DefaultRelease(this GameCategory gameCategory)
    {
        return gameCategory switch
        {
            GameCategory.Oblivion => GameRelease.Oblivion,
            GameCategory.Skyrim => GameRelease.SkyrimSE,
            GameCategory.Fallout4 => GameRelease.Fallout4,
            GameCategory.Starfield => GameRelease.Starfield,
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
                yield return GameRelease.SkyrimSEGog;
                yield return GameRelease.SkyrimVR;
                yield return GameRelease.EnderalLE;
                yield return GameRelease.EnderalSE;
                yield break;
            case GameCategory.Fallout4:
                yield return GameRelease.Fallout4;
                yield return GameRelease.Fallout4VR;
                yield break;
            case GameCategory.Starfield:
                yield return GameRelease.Starfield;
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
            case GameCategory.Starfield:
            default:
                return true;
        }
    }
    
    public static bool IncludesMasterReferenceDataSubrecords(this GameCategory release)
    {
        return release switch
        {
            GameCategory.Oblivion => true,
            GameCategory.Skyrim => true,
            GameCategory.Fallout4 => true,
            GameCategory.Starfield => false,
        };
    }

    public static ILoquiRegistration ToModRegistration(this GameCategory category)
    {
        var ret = TryGetModRegistration(category);
        if (ret == null)
        {
            throw new MissingGameLibsException(category);
        }
        return ret;
    }

    public static ILoquiRegistration? TryGetModRegistration(this GameCategory category)
    {
        return ToModRegistrationHelper.Get(category);
    }
}

internal static class ToModRegistrationHelper
{
    private static readonly Dictionary<GameCategory, ILoquiRegistration?> _registrations = new();

    static ToModRegistrationHelper()
    {
        foreach (var category in Enums<GameCategory>.Values)
        {
            var modType = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}Mod, Mutagen.Bethesda.{category}");
            if (modType == null) continue;
            var regisProp = modType.GetProperty("StaticRegistration", 
                System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public);
            if (regisProp == null) continue;
            var regis = regisProp.GetValue(null) as ILoquiRegistration;
            if (regis == null) continue;
            _registrations[category] = regis;
        }
    }

    public static ILoquiRegistration? Get(GameCategory category)
    {
        return _registrations.GetOrDefault(category);
    }
}