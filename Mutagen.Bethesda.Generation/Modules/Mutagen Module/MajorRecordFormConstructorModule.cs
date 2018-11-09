using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MajorRecordFormConstructorModule : GenerationModule
    {
        public override Task PostLoad(ObjectGeneration obj)
        {
            if (obj.IsMajorRecord())
            {
                obj.BasicCtorPermission = PermissionLevel.@protected;
            }
            return base.PostLoad(obj);
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (!obj.IsMajorRecord()) return;
            fg.AppendLine($"public {obj.Name}(FormKey formKey)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.FormKey = formKey;");
            }
        }
    }
}
