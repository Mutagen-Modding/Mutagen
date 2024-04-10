using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModConcreteBuilder : ISpecimenBuilder
{
    private readonly GameRelease _release;
    public IMod? LastCreatedConcreteMod;

    public ModConcreteBuilder(GameRelease release)
    {
        _release = release;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }

        if (request is not Type t) return new NoSpecimen();
        if (!t.IsInterface && !t.IsAbstract && t.InheritsFrom(typeof(IMod)))
        {
            var ret = ModInstantiator.Activator(context.Create<ModKey>(), _release, forceUseLowerFormIDRanges: false);
            LastCreatedConcreteMod = ret;
            return ret;
        }

        return new NoSpecimen();
    }
}