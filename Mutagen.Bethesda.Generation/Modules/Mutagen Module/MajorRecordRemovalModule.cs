using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MajorRecordRemovalModule : GenerationModule
    {
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (await MajorRecordModule.HasMajorRecordsInTree(obj, false) == Case.No) return;
            GenerateClassImplementation(obj, fg);
        }

        public static void GenerateClassImplementation(ObjectGeneration obj, FileGeneration fg, bool onlyGetter = false)
        {
            if (!onlyGetter)
            {
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"void {nameof(IMajorRecordEnumerable)}.Remove({nameof(FormKey)} formKey) => this.Remove(formKey);");
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"void {nameof(IMajorRecordEnumerable)}.Remove(HashSet<{nameof(FormKey)}> formKeys) => this.Remove(formKeys);");
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"void {nameof(IMajorRecordEnumerable)}.Remove(IEnumerable<{nameof(FormKey)}> formKeys) => this.Remove(formKeys);");
            }
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static void Remove{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
                args.Add($"{nameof(FormKey)} key");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var keys = new HashSet<{nameof(FormKey)}>();");
                fg.AppendLine("keys.Add(key);");
                using (var args = new ArgsWrapper(fg,
                    $"{obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Remove"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("keys");
                }
            }
            fg.AppendLine();

            if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static void Remove{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
                args.Add($"IEnumerable<{nameof(FormKey)}> keys");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Remove"))
                {
                    args.AddPassArg("obj");
                    args.Add("keys: keys.ToHashSet()");
                }
            }
            fg.AppendLine();

            if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static void Remove{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
                args.Add($"HashSet<{nameof(FormKey)}> keys");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Remove"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("keys");
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            if (!maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class)) return;
            var accessor = new Accessor("obj");
            if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
            var overrideStr = await obj.FunctionOverride(async c => await MajorRecordModule.HasMajorRecords(c, includeBaseClass: false, includeSelf: true) != Case.No);

            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}void Remove"))
            {
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} obj");
                args.Add($"HashSet<{nameof(FormKey)}> keys");
            }
            using (new BraceWrapper(fg))
            {
                var fgCount = fg.Count;
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await MajorRecordModule.HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
                    {
                        fg.AppendLine($"base.Remove(obj, keys);");
                        break;
                    }
                }
                foreach (var field in obj.IterateFields())
                {
                    switch (field)
                    {
                        case LoquiType _:
                        case ContainerType _:
                        case DictType _:
                            break;
                        default:
                            continue;
                    }

                    FileGeneration fieldFg = new FileGeneration();

                    if (field is LoquiType loqui)
                    {
                        var fieldAccessor = $"{accessor}.{loqui.Name}";
                        if (await MajorRecordModule.HasMajorRecords(loqui.TargetObjectGeneration, includeBaseClass: true, includeSelf: false) != Case.No)
                        {
                            fg.AppendLine($"{fieldAccessor}{loqui.NullChar}.Remove(keys);");
                        }
                        var isMajorRecord = loqui.TargetObjectGeneration != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                        if (isMajorRecord)
                        {
                            fg.AppendLine($"if ({(loqui.Nullable ? $"{fieldAccessor} != null && " : string.Empty)}keys.Contains({fieldAccessor}.FormKey))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"{fieldAccessor} = null;");
                            }
                        }
                    }
                    else if (field is ContainerType cont)
                    {
                        if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
                        var isMajorRecord = await contLoqui.IsMajorRecord();
                        if (isMajorRecord)
                        {
                            fg.AppendLine($"{accessor}.{field.Name}.Remove(keys);");
                        }
                        if (await MajorRecordModule.HasMajorRecords(contLoqui, includeBaseClass: true, includeSelf: false) != Case.No)
                        {
                            fg.AppendLine($"{accessor}.{field.Name}.ForEach(i => i.Remove(keys));");
                        }
                        if (contLoqui.TargetObjectGeneration?.IsListGroup() ?? false)
                        {
                            fg.AppendLine($"{accessor}.{field.Name}.Remove(i => i.{contLoqui.TargetObjectGeneration.Fields.FirstOrDefault(f => f is ContainerType).Name}.Count == 0);");
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.Mode != DictMode.KeyedValue) continue;
                        if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
                        var isMajorRecord = dictLoqui.TargetObjectGeneration != null && await dictLoqui.TargetObjectGeneration.IsMajorRecord();
                        if (isMajorRecord
                            || await MajorRecordModule.HasMajorRecords(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            switch (await MajorRecordModule.HasMajorRecords(dictLoqui, includeBaseClass: true))
                            {
                                case Case.Yes:
                                case Case.Maybe:
                                    fg.AppendLine($"{accessor}.{field.Name}.Remove(keys);");
                                    break;
                                case Case.No:
                                default:
                                    break;
                            }
                        }
                    }

                    if (fieldFg.Count > 0)
                    {
                        if (field.Nullable)
                        {
                            fg.AppendLine($"if ({field.NullableAccessor(getter: true, Accessor.FromType(field, accessor.ToString()))})");
                        }
                        using (new BraceWrapper(fg, doIt: field.Nullable))
                        {
                            fg.AppendLines(fieldFg);
                        }
                    }
                }
            }
            fg.AppendLine();

            // Generate base overrides  
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await MajorRecordModule.HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public override void Remove"))
                    {
                        args.Add($"{baseClass.Interface(getter: false)} obj");
                        args.Add($"HashSet<{nameof(FormKey)}> keys");
                    }
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            "Remove"))
                        {
                            args.Add($"({obj.Interface(getter: false)})obj");
                            args.AddPassArg("keys");
                        }
                    }
                    fg.AppendLine();
                }
            }
        }
    }
}
