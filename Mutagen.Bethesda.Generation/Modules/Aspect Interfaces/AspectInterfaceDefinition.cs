using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Generation
{
    public class AspectInterfaceDefinition
    {
        public List<(LoquiInterfaceDefinitionType Type, string Interface)> Interfaces = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
        public List<(LoquiInterfaceType Type, string Name, Action<FileGeneration> Actions)> FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<FileGeneration> Actions)>();
        public Func<ObjectGeneration, bool> Test { get; set; }

        public AspectInterfaceDefinition(Func<ObjectGeneration, bool> test)
        {
            Test = test;
        }
    }
}
