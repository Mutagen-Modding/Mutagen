using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class NumFieldsModule : GenerationModule
    {
        public override Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            fg.AppendLine($"public const int NumStructFields = {obj.IterateFields(expandSets: SetMarkerType.ExpandSets.False).Where((f) => !f.GetFieldData().HasTrigger).Count()};");
            var typedFields = obj.IterateFields().Where((f) => f.GetFieldData().HasTrigger).Sum((f) =>
            {
                if (!(f is SetMarkerType set)) return 1;
                return set.IterateFields().Count();
            });
            fg.AppendLine($"public const int NumTypedFields = {typedFields};");
            return base.GenerateInRegistration(obj, fg);
        }
    }
}
