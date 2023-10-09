using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModKeyBuilder : ISpecimenBuilder
{
    private static int _nextNum = 0;
    
    public object Create(object request, ISpecimenContext context)
    {
        if (request is MultipleRequest mult)
        {
            var req = mult.Request;
            if (req is SeededRequest seed)
            {
                req = seed.Request;
            }
            if (req is Type t)
            {
                if (t == typeof(ModKey))
                {
                    return new object[]
                    {
                        GetRandomModKey(ModType.Master),
                        GetRandomModKey(ModType.Light),
                        GetRandomModKey(ModType.Plugin),
                    };
                }
            }
        }
        else if (request is Type t)
        {
            if (t == typeof(ModKey))
            {
                return GetRandomModKey(ModType.Plugin);
            }
        }

        return new NoSpecimen();
    }

    public ModKey GetRandomModKey(ModType type)
    {
        return new ModKey($"Mod{_nextNum++}", ModType.Plugin);
    }
}