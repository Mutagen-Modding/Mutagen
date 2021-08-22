using System.Reflection;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class ModKeyParameterBuilder : ISpecimenBuilder
    {
        public IMakeModExist MakeModExist { get; }

        public ModKeyParameterBuilder(IMakeModExist makeModExist)
        {
            MakeModExist = makeModExist;
        }
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not ParameterInfo p) return new NoSpecimen();
            if (p.Name == null) return new NoSpecimen();
            if (p.ParameterType != typeof(ModKey)) return new NoSpecimen();

            ModType type;
            if (p.Name.ContainsInsensitive("light"))
            {
                type = ModType.LightMaster;
            }
            else if (p.Name.ContainsInsensitive("master"))
            {
                type = ModType.Master;
            }
            else
            {
                type = ModType.Plugin;
            }
                
            var mk = new ModKey(p.Name, type);
            if (p.Name.ContainsInsensitive("existing"))
            {
                MakeModExist.MakeExist(mk, context);
            }

            return mk;
        }
    }
}