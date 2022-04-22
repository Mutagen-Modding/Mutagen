namespace Mutagen.Bethesda.Generation;

public static class GameCategoryExt
{
    public static string ModInterface(this GameCategory gameCategory, bool getter)
    {
        return $"I{gameCategory}Mod{(getter ? "Getter" : null)}";
    }
}