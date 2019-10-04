using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Generation
{
    public class MajorRecordEnumerationModule : GenerationModule
    {
        public enum Case { No, Yes, Maybe }

        public override async Task PostLoad(ObjectGeneration obj)
        {
            if (await HasMajorRecordsInTree(obj, false) == Case.No) return;
            obj.Interfaces.Add(LoquiInterfaceType.IGetter, nameof(IMajorRecordGetterEnumerable));
            obj.Interfaces.Add(LoquiInterfaceType.ISetter, nameof(IMajorRecordEnumerable));
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (await HasMajorRecordsInTree(obj, false) == Case.No) return;
            GenerateClassImplementation(fg);
        }

        public static void GenerateClassImplementation(FileGeneration fg, bool onlyGetter = false)
        {
            fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommonGetter)}> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
            if (!onlyGetter)
            {
                fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommon)}> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
            }
        }

        public static async Task<Case> HasMajorRecords(LoquiType loqui, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                return await HasMajorRecordsInTree(loqui.TargetObjectGeneration, includeBaseClass, loqui.GenericSpecification);
            }
            else if (specifications != null)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    if (await specObj.IsMajorRecord()) return Case.Yes;
                    return await HasMajorRecordsInTree(specObj, includeBaseClass);
                }
            }
            else if (loqui.RefType == LoquiType.LoquiRefType.Interface)
            {
                // ToDo
                // Quick hack.  Real solution should use reflection to investigate the interface
                return Case.Yes;
            }
            return Case.Maybe;
        }

        public static async Task<Case> HasMajorRecords(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (obj.Name == "ListGroup") return Case.Yes;
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    if (loqui.TargetObjectGeneration != null
                        && await loqui.TargetObjectGeneration.IsMajorRecord())
                    {
                        return Case.Yes;
                    }
                    if (await HasMajorRecords(loqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType contLoqui)
                    {
                        if (await HasMajorRecords(contLoqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        if (await HasMajorRecords(valLoqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        if (await HasMajorRecords(keyLoqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                    }
                }
            }
            return Case.No;
        }

        public static async Task<Case> HasMajorRecordsInTree(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (await HasMajorRecords(obj, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
            // If no, check subclasses
            foreach (var inheritingObject in await obj.InheritingObjects())
            {
                if (await HasMajorRecordsInTree(inheritingObject, includeBaseClass: false, specifications: specifications) == Case.Yes) return Case.Yes;
            }

            return Case.No;
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            if (await HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IMajorRecordCommonGetter)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
                {
                    args.AddPassArg("obj");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IMajorRecordCommon)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: false)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
                {
                    args.AddPassArg("obj");
                }
            }
            fg.AppendLine();
        }

        private async Task LoquiTypeHandler(FileGeneration fg, Accessor loquiAccessor, LoquiType loquiType)
        {
            // ToDo
            // Quick hack.  Real solution should use reflection to investigate the interface
            if (loquiType.RefType == LoquiType.LoquiRefType.Interface)
            {
                fg.AppendLine($"yield return {loquiAccessor};");
                return;
            }

            if (loquiType.TargetObjectGeneration != null
                && await loquiType.TargetObjectGeneration.IsMajorRecord())
            {
                fg.AppendLine($"yield return {loquiAccessor};");
            }
            if (await HasMajorRecords(loquiType, includeBaseClass: true) == Case.No)
            {
                return;
            }
            fg.AppendLine($"foreach (var item in {loquiAccessor}.EnumerateMajorRecords())");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"yield return item;");
            }
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            bool getter = maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class);
            bool setter = maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class);
            if (!getter && !setter) return;
            var accessor = new Accessor("obj");
            if (await HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
            var overrideStr = await obj.FunctionOverride(async c => await HasMajorRecords(c, includeBaseClass: false) != Case.No);
            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}IEnumerable<{nameof(IMajorRecordCommon)}{(getter ? "Getter" : null)}> EnumerateMajorRecords"))
            {
                args.Add($"{obj.Interface(getter: getter)} obj");
            }
            using (new BraceWrapper(fg))
            {
                var fgCount = fg.Count;
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasMajorRecords(baseClass, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine("foreach (var item in base.EnumerateMajorRecords(obj))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return item;");
                        }
                        break;
                    }
                }
                foreach (var field in obj.IterateFields())
                {
                    if (field is LoquiType loqui)
                    {
                        var subFg = new FileGeneration();
                        await LoquiTypeHandler(subFg, $"{accessor}.{loqui.Name}", loqui);
                        if (subFg.Count == 0) continue;
                        var doBrace = true;
                        if (loqui.SingletonType == SingletonLevel.None)
                        {
                            fg.AppendLine($"if ({accessor}.{loqui.Name} != null)");
                        }
                        else
                        {
                            doBrace = false;
                        }
                        using (new BraceWrapper(fg, doIt: doBrace))
                        {
                            fg.AppendLines(subFg);
                        }
                    }
                    else if (field is ContainerType cont)
                    {
                        if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
                        var isMajorRecord = contLoqui.TargetObjectGeneration != null && await contLoqui.TargetObjectGeneration.IsMajorRecord();
                        if (isMajorRecord
                            || await HasMajorRecords(contLoqui, includeBaseClass: true) != Case.No)
                        {
                            switch (await HasMajorRecords(contLoqui, includeBaseClass: true))
                            {
                                case Case.Yes:
                                    fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name})");
                                    using (new BraceWrapper(fg))
                                    {
                                        await LoquiTypeHandler(fg, $"subItem", contLoqui);
                                    }
                                    break;
                                case Case.Maybe:
                                    fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.WhereCastable<{contLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                    using (new BraceWrapper(fg))
                                    {
                                        await LoquiTypeHandler(fg, $"subItem", contLoqui);
                                    }
                                    break;
                                case Case.No:
                                default:
                                    break;
                            }
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.Mode != DictMode.KeyedValue) continue;
                        if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
                        var isMajorRecord = dictLoqui.TargetObjectGeneration != null && await dictLoqui.TargetObjectGeneration.IsMajorRecord();
                        if (isMajorRecord
                            || await HasMajorRecords(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            switch (await HasMajorRecords(dictLoqui, includeBaseClass: true))
                            {
                                case Case.Yes:
                                    fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                                    using (new BraceWrapper(fg))
                                    {
                                        await LoquiTypeHandler(fg, $"subItem", dictLoqui);
                                    }
                                    break;
                                case Case.Maybe:
                                    fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                    using (new BraceWrapper(fg))
                                    {
                                        await LoquiTypeHandler(fg, $"subItem", dictLoqui);
                                    }
                                    break;
                                case Case.No:
                                default:
                                    break;
                            }
                        }
                    }
                }
                if (fgCount == fg.Count)
                {
                    fg.AppendLine("yield break;");
                }
            }

            // Generate base overrides
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasMajorRecords(baseClass, includeBaseClass: true) != Case.No)
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public override IEnumerable<{nameof(IMajorRecordCommon)}{(getter ? "Getter" : null)}> EnumerateMajorRecords"))
                    {
                        args.Add($"{baseClass.Interface(getter: getter)} obj");
                    }
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            "EnumerateMajorRecords"))
                        {
                            args.Add($"({obj.Interface(getter: getter)})obj");
                        }
                    }
                }
            }
        }
    }
}
