using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing.AutoData;

public class FormKeyBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }
        
        if (request is Type t)
        {
            if (t == typeof(FormKey))
            {
                return new FormKey(context.Create<ModKey>(), 0x800);
            }
            if (t == typeof(Func<FormKey>))
            {
                return () => context.Create<FormKey>();
            }
        }

        return new NoSpecimen();
    }
}