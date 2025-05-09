using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing.AutoData;

public class FormKeyBuilder : ISpecimenBuilder
{
    private ModKey? _modKey;
    private uint _nextNum = 0x800;
    
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
                if (_modKey == null)
                {
                    _modKey = context.Create<ModKey>();
                }
                return new FormKey(_modKey.Value, _nextNum++);
            }
            if (t == typeof(Func<FormKey>))
            {
                return () => context.Create<FormKey>();
            }
        }

        return new NoSpecimen();
    }
}