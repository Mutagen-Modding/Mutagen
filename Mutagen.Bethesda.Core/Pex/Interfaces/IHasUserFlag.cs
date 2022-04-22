using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Pex;

public interface IHasUserFlags : IHasUserFlagsGetter
{
    new uint RawUserFlags { get; set; }
}

public interface IHasUserFlagsGetter
{
    uint RawUserFlags { get; }
}

public static class IHasUserFlagExtensions
{
    public static IEnumerable<NullableUserFlag> GetActiveFlags(this IHasUserFlags hasFlags, IPexFileGetter pexFile)
    {
        var flags = (int)hasFlags.RawUserFlags;
        int index = 1;
        for (byte i = 0; i < 32; i++)
        {
            if (EnumExt.HasFlag(flags, index))
            {
                yield return new NullableUserFlag(pexFile.UserFlags[i], i);
            }
            index <<= 1;
        }
    }

    public static void SetFlag(this IHasUserFlags hasFlags, IPexFileGetter pexFile, UserFlag flag, bool on)
    {
        if (pexFile.UserFlags[flag.Index] != flag.Name)
        {
            throw new ArgumentException("Tried to set a flag that was not registered in the reference pex file.");
        }
        hasFlags.RawUserFlags = EnumExt.SetFlag(hasFlags.RawUserFlags, flag.Index, on);
    }

    public static bool HasFlag(this IHasUserFlags hasFlags, IPexFileGetter pexFile, UserFlag flag)
    {
        if (pexFile.UserFlags[flag.Index] != flag.Name)
        {
            throw new ArgumentException("Tried to set a flag that was not registered in the reference pex file.");
        }
        return EnumExt.HasFlag(hasFlags.RawUserFlags, flag.Index);
    }
}