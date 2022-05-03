using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModPathBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type t) return new NoSpecimen();
        if (t != typeof(ModPath)) return new NoSpecimen();
            
        var modKey = context.Create<ModKey>();
        var dataDir = context.Create<IDataDirectoryProvider>();
        return new ModPath(modKey, Path.Combine(dataDir.Path, modKey.FileName));
    }
}