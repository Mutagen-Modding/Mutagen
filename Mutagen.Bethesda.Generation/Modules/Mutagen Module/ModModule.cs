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
            fg.AppendLine($"private NotifyingDictionary<FormID, MajorRecord> _majorRecords = new NotifyingDictionary<FormID, MajorRecord>();");
            fg.AppendLine($"public INotifyingDictionaryGetter<FormID, MajorRecord> MajorRecords => _majorRecords;");
            fg.AppendLine($"public MajorRecord this[FormID id]");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("get => _majorRecords[id];");
                fg.AppendLine("set => SetMajorRecord(id, value);");
            }

            using (var args = new FunctionWrapper(fg,
                "protected void SetMajorRecord"))
            {
                args.Add("FormID id");
                args.Add("MajorRecord record");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("switch (record)");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui)) continue;
                        if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                        if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                        {
                            throw new ArgumentException();
                        }
                        fg.AppendLine($"case {subObj.Name} {field.Name.ToLower()}:");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"{loqui.ProtectedName}.Items.Set({field.Name.ToLower()});");
                            fg.AppendLine($"break;");
                        }
                    }
                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine($"throw new ArgumentException(\"Unknown Major Record type: {{record?.GetType()}}\");");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public INotifyingKeyedCollection<FormID, T> GetGroup<T>",
                wheres: "where T : IMajorRecord"))
            {

            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var t = typeof(T);");
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                    if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                    {
                        throw new ArgumentException();
                    }
                    fg.AppendLine($"if (t.Equals(typeof({subObj.Name})))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return (INotifyingKeyedCollection<FormID, T>){field.Name}.Items;");
                    }
                }
                fg.AppendLine("throw new ArgumentException($\"Unkown group type: {t}\");");
            }

            await base.GenerateInClass(obj, fg);
            fg.AppendLine();
        }

        public override Task GenerateInCtor(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields())
            {
                if (!(field is LoquiType loqui)) continue;
                if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                fg.AppendLine($"{field.ProtectedName}.Items.Subscribe_Enumerable_Single((change) => _majorRecords.Modify(change.Item.Key, change.Item.Value, change.AddRem));");
            }
            return base.GenerateInCtor(obj, fg);
        }

        public override async Task GenerateInVoid(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            fg.AppendLine("public class GroupMask");
            using (new BraceWrapper(fg))
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration == null) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    fg.AppendLine($"public bool {loqui.Name};");
                }
            }
        }
    }
}
