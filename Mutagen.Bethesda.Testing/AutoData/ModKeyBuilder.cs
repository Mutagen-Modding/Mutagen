using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModKeyBuilder : ISpecimenBuilder
{
    private int _nextNum = 0;
    
    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }
        if (request is MultipleRequest mult)
        {
            request = mult.Request;
            if (request is SeededRequest multSeed)
            {
                request = multSeed.Request;
            }
            if (request is Type t)
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
            if (t == typeof(Func<ModKey>))
            {
                return () => context.Create<ModKey>();
            }
        }

        return new NoSpecimen();
    }

    public ModKey GetRandomModKey(ModType type)
    {
        return new ModKey($"Mod{_nextNum++}", ModType.Plugin);
    }
}