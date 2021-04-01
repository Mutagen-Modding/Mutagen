using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class AspectInterfaceDefinition
    {
        public string Name { get; }
        public Func<ObjectGeneration, IEnumerable<(LoquiInterfaceDefinitionType Type, string Interface)>> Interfaces;
        public List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, FileGeneration> Actions)> FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, FileGeneration> Actions)>();
        public Func<ObjectGeneration, bool> Test { get; set; }
        public Func<ObjectGeneration, IEnumerable<TypeGeneration>> IdentifyFields { get; set; }


        public AspectInterfaceDefinition(
            string name,
            Func<ObjectGeneration, bool> test)
        {
            Name = name;
            Test = test;
        }
    }
}
