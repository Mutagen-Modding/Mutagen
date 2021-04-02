using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class AspectInterfaceDefinition
    {
        public string Name { get; }
        public Func<ObjectGeneration, IEnumerable<(LoquiInterfaceDefinitionType Type, string Interface)>>? Interfaces;
        public List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, FileGeneration> Actions)> FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, FileGeneration> Actions)>();
        public Func<ObjectGeneration, bool> Test { get; set; }
        public Func<ObjectGeneration, IEnumerable<TypeGeneration>>? IdentifyFields { get; set; } = null;


        public AspectInterfaceDefinition(
            string name,
            Func<ObjectGeneration, bool> test)
        {
            Name = name;
            Test = test;

            IdentifyFields = DefaultIdentifyFields;
        }

        protected IEnumerable<TypeGeneration> DefaultIdentifyFields(ObjectGeneration o) =>
            from field in o.Fields
            join f in FieldActions.Select(x => x.Name).Distinct()
              on field.Name equals f
            select field;
    }
}
