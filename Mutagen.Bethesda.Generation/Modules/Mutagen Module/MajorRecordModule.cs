using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MajorRecordModule : GenerationModule
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
                fg.AppendLine("CustomCtor();");
            }
            fg.AppendLine();

            if (obj.Abstract)
            {
                if (obj.IsTopClass)
                {
                    fg.AppendLine($"public abstract MajorRecord Duplicate(Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecords = null);");
                }
            }
            else
            {
                fg.AppendLine($"partial void PostDuplicate({obj.Name} obj, {obj.Name} rhs, Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecords);");
                fg.AppendLine();
                
                fg.AppendLine($"public override MajorRecord Duplicate(Func<FormKey> getNextFormKey, IList<(MajorRecord Record, FormKey OriginalFormKey)> duplicatedRecords)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"var ret = new {obj.Name}(getNextFormKey());");
                    fg.AppendLine("ret.CopyFieldsFrom(this);");
                    fg.AppendLine("duplicatedRecords?.Add((ret, this.FormKey));");
                    fg.AppendLine("PostDuplicate(ret, this, getNextFormKey, duplicatedRecords);");
                    fg.AppendLine("return ret;");
                }
                fg.AppendLine();
            }
        }
    }
}
