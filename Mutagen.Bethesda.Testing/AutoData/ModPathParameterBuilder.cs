using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class ModPathParameterBuilder : ISpecimenBuilder
    {
        public IMakeModExist MakeModExist { get; }

        public ModPathParameterBuilder(IMakeModExist makeModExist)
        {
            MakeModExist = makeModExist;
        }
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not ParameterInfo p) return new NoSpecimen();
            if (p.Name == null) return new NoSpecimen();
            if (p.ParameterType != typeof(ModPath)) return new NoSpecimen();
            if (p.Name.ContainsInsensitive("existing"))
            {
                var modPath = context.Create<ModPath>();
                MakeModExist.MakeExist(modPath.ModKey, context);
                return modPath;
            }
            return new NoSpecimen();
        }
    }
}