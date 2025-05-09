using Noggog;

namespace Mutagen.Bethesda.Starfield;

public static class Array2dCopyHelper
{
    public static Array2d<T> DeepCopy<T>(
        this IReadOnlyArray2d<T> array,
        Func<KeyValuePair<P2Int, T>, T> copyFunc)
    {
        var ret = new T[array.Width, array.Height];
        for (int y = 0; y < array.Height; y++)
        {
            for (int x = 0; x < array.Width; x++)
            {
                ret[x, y] = copyFunc(new KeyValuePair<P2Int, T>(new P2Int(x, y), array[x, y]));
            }
        }
        return new Array2d<T>(ret);
    }
}