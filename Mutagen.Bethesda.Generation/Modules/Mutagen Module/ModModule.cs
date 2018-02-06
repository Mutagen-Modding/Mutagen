using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class ModModule : GenerationModule
    {
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectData().ObjectType != ObjectType.Mod) return;
            fg.AppendLine($"public IEnumerable<MajorRecord> MajorRecords => GetMajorRecords();");
            fg.AppendLine("private IEnumerable<MajorRecord> GetMajorRecords()");
            using (new BraceWrapper(fg))
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                    fg.AppendLine($"foreach (var rec in {field.Name}.Items.Values)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("yield return rec;");
                    }
                }
            }
            await base.GenerateInClass(obj, fg);
        }
    }
}
