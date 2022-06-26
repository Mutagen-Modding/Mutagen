using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModKeyMultipleBuilder : ISpecimenBuilder
{
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
                    return new ModKey[]
                    {
                        ModKeyBuilder.GetRandomModKey(ModType.Plugin),
                        ModKeyBuilder.GetRandomModKey(ModType.Plugin),
                        ModKeyBuilder.GetRandomModKey(ModType.Plugin),
                    };
                }
            }
        }
            
        return new NoSpecimen();
    }
}