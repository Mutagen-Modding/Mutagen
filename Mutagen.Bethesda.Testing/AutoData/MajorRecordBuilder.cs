using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;
using Noggog.Testing.AutoFixture.Testing;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MajorRecordBuilder : ISpecimenBuilder
{
    private readonly GameRelease _release;
    private readonly ModConcreteBuilder _modBuilder;
    private readonly bool _configureMembers;

    public MajorRecordBuilder(
        GameRelease release, 
        ModConcreteBuilder modBuilder,
        bool configureMembers)
    {
        _release = release;
        _modBuilder = modBuilder;
        _configureMembers = configureMembers;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }
        
        if (request is not Type t) return new NoSpecimen();
        if (!t.IsAbstract && !t.IsInterface
            && t.InheritsFrom(typeof(IMajorRecord)))
        {
            var ret = GetMajorRecord(t, context);
            if (ret != null) return ret;
        }
            
        return new NoSpecimen();
    }

    private IMajorRecord? GetMajorRecord(Type t, ISpecimenContext context)
    {
        if (_modBuilder.LastCreatedConcreteMod == null) return null;
        var ret = MajorRecordInstantiator.Activator(_modBuilder.LastCreatedConcreteMod.GetNextFormKey(), _release, t);

        if (_configureMembers)
        {
            context.FillAllProperties(ret);
        }
        
        var group = _modBuilder.LastCreatedConcreteMod.TryGetTopLevelGroup(t);
        group?.AddUntyped(ret);
        return ret;
    }
}