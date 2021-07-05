using System;
using System.Linq;
using Noggog;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Mutagen.Bethesda.StructureMap
{
    public class Convention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed).ForEach(type =>
            {
                if (type.Name.EndsWith("Injection")) return;
                Enumerable.Where(type.GetInterfaces(), i => i.Name == $"I{type.Name}")
                    .ForEach(i => registry.For(i).Use(type));
            });
        }
    }
}