using AutoFixture.Kernel;
using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData;

public class AbstractSubclassBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t
            && t.IsAbstract
            && t.InheritsFrom(typeof(ILoquiObject)))
        {
            foreach (var regis in LoquiRegistration.StaticRegister.Registrations)
            {
                if (regis.ClassType.BaseType != t) continue;
                if (regis.ClassType.GetConstructors().All(c => c.GetParameters().Length != 0)) continue;
                return context.Resolve(regis.ClassType);
            }
        }

        return new NoSpecimen();
    }
}