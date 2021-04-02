using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class FieldsAspect : AspectInterfaceDefinition
    {
        public (string FieldName, string TypeName)[] Fields;

        public FieldsAspect(string interfaceType, params (string FieldName, string TypeName)[] fields)
            : base(interfaceType, null!)
        {
            Fields = fields;
            Test = ApplicabilityTest;
            Interfaces = (o) => new List<(LoquiInterfaceDefinitionType Type, string Interface)>()
            {
                (LoquiInterfaceDefinitionType.IGetter, $"{interfaceType}Getter"),
                (LoquiInterfaceDefinitionType.ISetter, interfaceType),
            };
            IdentifyFields = (o) =>
                from f in Fields
                join field in o.IterateFields(includeBaseClass: true)
                    on f.FieldName equals field.Name
                where field.TypeName(getter: true) != f.TypeName
                select field;
        }

        public bool ApplicabilityTest(ObjectGeneration o)
        {
            foreach (var f in Fields)
            {
                var field = o.IterateFields(includeBaseClass: true).FirstOrDefault(x => x.Name == f.FieldName);
                if (field == null) return false;
                if (field.TypeName(getter: true) != f.TypeName) return false;
            }
            return true;
        }
    }
}
