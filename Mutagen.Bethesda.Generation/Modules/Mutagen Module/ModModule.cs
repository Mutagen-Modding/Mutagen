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
            fg.AppendLine($"private NotifyingDictionary<FormKey, MajorRecord> _majorRecords = new NotifyingDictionary<FormKey, MajorRecord>();");
            fg.AppendLine($"public INotifyingDictionaryGetter<FormKey, MajorRecord> MajorRecords => _majorRecords;");
            fg.AppendLine($"public MajorRecord this[FormKey id]");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("get => _majorRecords[id];");
                fg.AppendLine("set => SetMajorRecord(id, value);");
            }

            using (var args = new FunctionWrapper(fg,
                "protected void SetMajorRecord"))
            {
                args.Add("FormKey id");
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
                        if (!subObj.BaseClassTrail().Any((b) => b.Name.Equals("MajorRecord"))) continue;
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
                        fg.AppendLine($"throw new ArgumentException($\"Unknown settable MajorRecord type: {{record?.GetType()}}\");");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public INotifyingKeyedCollection<FormKey, T> GetGroup<T>",
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
                        fg.AppendLine($"return (INotifyingKeyedCollection<FormKey, T>){field.Name}.Items;");
                    }
                }
                fg.AppendLine("throw new ArgumentException($\"Unkown group type: {t}\");");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public void AddRecords"))
            {
                args.Add($"{obj.Name} rhsMod");
                args.Add($"GroupMask mask = null");
            }
            using (new BraceWrapper(fg))
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    fg.AppendLine($"if (mask?.{field.Name} ?? true)");
                    using (new BraceWrapper(fg))
                    {
                        if (loqui.TargetObjectGeneration.Name == "Group")
                        {
                            fg.AppendLine($"this.{field.Name}.Items.Set(rhsMod.{field.Name}.Items.Values);");
                        }
                        else
                        {
                            fg.AppendLine("if (rhsMod.{field.Name}.Items.Count > 0)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("throw new NotImplementedException(\"Cell additions need implementing\")");
                            }
                        }
                    }
                }
            }
            fg.AppendLine();

            await base.GenerateInClass(obj, fg);
            fg.AppendLine();
        }

        public override Task GenerateInCtor(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields())
            {
                if (!(field is LoquiType loqui)) continue;
                if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                {
                    throw new ArgumentException();
                }
                if (subObj.BaseClassTrail().Any((b) => b.Name.Equals("MajorRecord")))
                {
                    fg.AppendLine($"{field.ProtectedName}.Items.Subscribe_Enumerable_Single((change) => Mutagen.Bethesda.Utility.ModifyButThrow(_majorRecords, change));");
                }
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

                fg.AppendLine("public GroupMask()");
                using (new BraceWrapper(fg))
                {
                }

                fg.AppendLine("public GroupMask(bool defaultValue)");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui)) continue;
                        if (loqui.TargetObjectGeneration == null) continue;
                        if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                        fg.AppendLine($"{loqui.Name} = defaultValue;");
                    }
                }
            }
        }
    }
}
