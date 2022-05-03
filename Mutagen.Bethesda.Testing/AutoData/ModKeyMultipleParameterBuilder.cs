using System.Reflection;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class ModKeyMultipleParameterBuilder : ISpecimenBuilder
{
    public ISplitEnumerableIntoSubtypes SplitEnumerableIntoSubtypes { get; }
    public IMakeModExist MakeModExist { get; }

    public ModKeyMultipleParameterBuilder(
        ISplitEnumerableIntoSubtypes splitEnumerableIntoSubtypes,
        IMakeModExist makeModExist)
    {
        SplitEnumerableIntoSubtypes = splitEnumerableIntoSubtypes;
        MakeModExist = makeModExist;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo p) return new NoSpecimen();
        if (p.Name == null) return new NoSpecimen();
        if (!typeof(IEnumerable<ModKey>).IsAssignableFrom(p.ParameterType)) return new NoSpecimen();

        if (p.Name.ContainsInsensitive("exist"))
        {
            var ret = SplitEnumerableIntoSubtypes.Split<ModKey>(context, p.ParameterType);
            if (ret is IEnumerable<ModKey> enumer)
            {
                foreach (var mk in enumer)
                {
                    MakeModExist.MakeExist(mk, context);
                }
            }

            return ret;
        }

        return new NoSpecimen();
    }
}