using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModPathMultipleBuilder : ISpecimenBuilder
{
    private ModKeyBuilder _modKeyBuilder = new();

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
                if (t == typeof(ModPath))
                {
                    var dataDir = context.Create<IDataDirectoryProvider>();
                    return new ModKey[]
                    {
                        _modKeyBuilder.GetRandomModKey(ModType.Plugin),
                        _modKeyBuilder.GetRandomModKey(ModType.Plugin),
                        _modKeyBuilder.GetRandomModKey(ModType.Plugin),
                    }.Select(mk =>
                    {
                        return new ModPath(mk, Path.Combine(dataDir.Path, mk.FileName));
                    });
                }
            }
        }
            
        return new NoSpecimen();
    }
}