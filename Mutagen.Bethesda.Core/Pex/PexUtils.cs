
namespace Mutagen.Bethesda.Pex
{
    internal static class PexUtils
    {
        internal static bool IsBigEndian(this GameCategory gameCategory)
        {
            return gameCategory != GameCategory.Fallout4;
        }
    }
}