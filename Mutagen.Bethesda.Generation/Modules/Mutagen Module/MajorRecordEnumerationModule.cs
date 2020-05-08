using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class MajorRecordEnumerationModule : GenerationModule
    {
        public enum Case { No, Yes, Maybe }

        public override async Task PostLoad(ObjectGeneration obj)
        {
            if (await HasMajorRecordsInTree(obj, false) == Case.No) return;
            obj.Interfaces.Add(LoquiInterfaceDefinitionType.IGetter, nameof(IMajorRecordGetterEnumerable));
            obj.Interfaces.Add(LoquiInterfaceDefinitionType.ISetter, nameof(IMajorRecordEnumerable));
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (await HasMajorRecordsInTree(obj, false) == Case.No) return;
            GenerateClassImplementation(obj, fg);
        }

        public static void GenerateClassImplementation(ObjectGeneration obj, FileGeneration fg, bool onlyGetter = false)
        {
            fg.AppendLine("[DebuggerStepThrough]");
            fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommonGetter)}> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
            fg.AppendLine("[DebuggerStepThrough]");
            fg.AppendLine($"IEnumerable<TMajor> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords<TMajor>() => this.EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}();");
            if (!onlyGetter)
            {
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommon)}> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<TMajor> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords<TMajor>() => this.EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}();");
            }
        }

        public static async Task<Case> HasMajorRecords(LoquiType loqui, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                if (await loqui.TargetObjectGeneration.IsMajorRecord()) return Case.Yes;
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
            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IMajorRecordCommonGetter)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
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

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<TMajor> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
                args.Wheres.Add($"where TMajor : class, IMajorRecordCommonGetter");
                args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords<TMajor>"))
                {
                    args.AddPassArg("obj");
                }
            }
            fg.AppendLine();

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IMajorRecordCommon)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
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

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<TMajor> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
                args.Wheres.Add($"where TMajor : class, IMajorRecordCommon");
                args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumerateMajorRecords<TMajor>"))
                {
                    args.AddPassArg("obj");
                }
            }
            fg.AppendLine();
        }

        private async Task LoquiTypeHandler(FileGeneration fg, Accessor loquiAccessor, LoquiType loquiType, string generic)
        {
            // ToDo
            // Quick hack.  Real solution should use reflection to investigate the interface
            if (loquiType.RefType == LoquiType.LoquiRefType.Interface)
            {
                if (generic == null)
                {
                    fg.AppendLine($"yield return {loquiAccessor};");
                }
                else
                {
                    fg.AppendLine($"yield return ({loquiAccessor} as TMajor)!;");
                }
                return;
            }

            if (loquiType.TargetObjectGeneration != null
                && await loquiType.TargetObjectGeneration.IsMajorRecord())
            {
                if (generic == null)
                {
                    fg.AppendLine($"yield return {loquiAccessor};");
                }
                else
                {
                    fg.AppendLine($"yield return ({loquiAccessor} as TMajor)!;");
                }
            }
            if (await HasMajorRecords(loquiType, includeBaseClass: true) == Case.No)
            {
                return;
            }
            fg.AppendLine($"foreach (var item in {loquiAccessor}.EnumerateMajorRecords{(generic == null ? null : $"<{generic}>")}())");
            using (new BraceWrapper(fg))
            {
                if (generic == null)
                {
                    fg.AppendLine($"yield return item;");
                }
                else
                {
                    fg.AppendLine($"yield return (item as TMajor)!;");
                }
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
                args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                if (setter)
                {
                    fg.AppendLine($"foreach (var item in {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecords(obj))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"yield return (item as {nameof(IMajorRecordCommon)})!;");
                    }
                }
                else
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
                        switch (field)
                        {
                            case LoquiType loqui:
                            case ContainerType cont:
                            case DictType dict:
                                break;
                            default:
                                continue;
                        }
                        if (field.HasBeenSet)
                        {
                            fg.AppendLine($"if ({field.HasBeenSetAccessor(getter: true, Accessor.FromType(field, accessor.ToString()))})");
                        }
                        using (new BraceWrapper(fg, doIt: field.HasBeenSet))
                        {
                            if (field is LoquiType loqui)
                            {
                                var isMajorRecord = loqui.TargetObjectGeneration != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                                if (isMajorRecord
                                    || await HasMajorRecords(loqui, includeBaseClass: true) != Case.No)
                                {
                                    var subFg = new FileGeneration();
                                    var fieldAccessor = loqui.HasBeenSet ? $"{loqui.Name}item" : $"{accessor}.{loqui.Name}";
                                    await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: null);
                                    if (subFg.Count == 0) continue;
                                    switch (loqui.SingletonType)
                                    {
                                        case SingletonLevel.None:
                                            if (loqui.HasBeenSet)
                                            {
                                                fg.AppendLine($"if ({accessor}.{loqui.Name}.TryGet(out var {loqui.Name}item))");
                                                using (new BraceWrapper(fg))
                                                {
                                                    fg.AppendLines(subFg);
                                                }
                                            }
                                            else
                                            {
                                                fg.AppendLines(subFg);
                                            }
                                            break;
                                        case SingletonLevel.NotNull:
                                        case SingletonLevel.Singleton:
                                            fg.AppendLines(subFg);
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
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
                                            fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.HasBeenSet ? ".TryIterate()" : null)})");
                                            using (new BraceWrapper(fg))
                                            {
                                                await LoquiTypeHandler(fg, $"subItem", contLoqui, generic: null);
                                            }
                                            break;
                                        case Case.Maybe:
                                            fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.HasBeenSet ? ".TryIterate()" : null)}.WhereCastable<{contLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                            using (new BraceWrapper(fg))
                                            {
                                                await LoquiTypeHandler(fg, $"subItem", contLoqui, generic: null);
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
                                                await LoquiTypeHandler(fg, $"subItem", dictLoqui, generic: null);
                                            }
                                            break;
                                        case Case.Maybe:
                                            fg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                            using (new BraceWrapper(fg))
                                            {
                                                await LoquiTypeHandler(fg, $"subItem", dictLoqui, generic: null);
                                            }
                                            break;
                                        case Case.No:
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    if (fgCount == fg.Count)
                    {
                        fg.AppendLine("yield break;");
                    }
                }
            }
            fg.AppendLine();

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
                    fg.AppendLine();
                }
            }

            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}IEnumerable<TMajor> EnumerateMajorRecords<TMajor>"))
            {
                args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
                args.Wheres.Add($"where TMajor : class, {nameof(IMajorRecordCommon)}{(getter ? "Getter" : null)}");
            }
            using (new BraceWrapper(fg))
            {
                if (setter)
                {
                    fg.AppendLine($"foreach (var item in {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecords<TMajor>(obj))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("yield return (item as TMajor)!;");
                    }
                }
                else
                {

                    var fgCount = fg.Count;
                    foreach (var baseClass in obj.BaseClassTrail())
                    {
                        if (await HasMajorRecords(baseClass, includeBaseClass: true) != Case.No)
                        {
                            fg.AppendLine("foreach (var item in base.EnumerateMajorRecords<TMajor>(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }
                            break;
                        }
                    }

                    fg.AppendLine("switch (typeof(TMajor).Name)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"case \"{nameof(IMajorRecordCommon)}\":");
                        fg.AppendLine($"case \"{nameof(IMajorRecordCommonGetter)}\":");
                        fg.AppendLine($"case \"{nameof(MajorRecord)}\":");
                        var gameMode = obj.GetObjectData().GameMode;
                        if (gameMode != null)
                        {
                            fg.AppendLine($"case \"I{gameMode}MajorRecord\":");
                            fg.AppendLine($"case \"I{gameMode}MajorRecordGetter\":");
                            fg.AppendLine($"case \"{gameMode}MajorRecord\":");
                        }
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine("foreach (var item in this.EnumerateMajorRecords(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return (item as TMajor)!;");
                            }
                            fg.AppendLine("yield break;");
                        }
                        Dictionary<object, FileGeneration> generationDict = new Dictionary<object, FileGeneration>();
                        foreach (var field in obj.IterateFields())
                        {
                            FileGeneration fieldGen;
                            LoquiType targetLoqui;
                            if (field is LoquiType loqui)
                            {
                                var isMajorRecord = loqui.TargetObjectGeneration != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                                if (!isMajorRecord
                                    && await HasMajorRecords(loqui, includeBaseClass: true) == Case.No)
                                {
                                    continue;
                                }

                                if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                                {
                                    fieldGen = generationDict.TryCreateValue(loqui.GetGroupTarget());
                                }
                                else
                                {
                                    fieldGen = generationDict.TryCreateValue(loqui);
                                }

                                var fieldAccessor = loqui.HasBeenSet ? $"{loqui.Name}item" : $"{accessor}.{loqui.Name}";
                                if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                                {
                                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.EnumerateMajorRecords<TMajor>())");
                                    using (new BraceWrapper(fieldGen))
                                    {
                                        fieldGen.AppendLine("yield return item;");
                                    }
                                    continue;
                                }
                                var subFg = new FileGeneration();
                                await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: "TMajor");
                                if (subFg.Count == 0) continue;
                                switch (loqui.SingletonType)
                                {
                                    case SingletonLevel.None:
                                        if (loqui.HasBeenSet)
                                        {
                                            fieldGen.AppendLine($"if ({accessor}.{loqui.Name}.TryGet(out var {loqui.Name}item))");
                                            using (new BraceWrapper(fieldGen))
                                            {
                                                fieldGen.AppendLines(subFg);
                                            }
                                        }
                                        else
                                        {
                                            fieldGen.AppendLines(subFg);
                                        }
                                        break;
                                    case SingletonLevel.NotNull:
                                    case SingletonLevel.Singleton:
                                        fieldGen.AppendLines(subFg);
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else if (field is ContainerType cont)
                            {
                                if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
                                if (contLoqui.RefType == LoquiType.LoquiRefType.Generic)
                                {
                                    fieldGen = generationDict.TryCreateValue("default:");
                                    fieldGen.AppendLine($"if(typeof(TMajor).IsAssignableFrom(typeof({contLoqui.GenericDef.Name})))");
                                    using (new BraceWrapper(fieldGen))
                                    {
                                        fieldGen.AppendLine($"foreach (var item in obj.{field.Name})");
                                        using (new BraceWrapper(fieldGen))
                                        {
                                            fieldGen.AppendLine($"yield return (item as TMajor)!;");
                                        }
                                    }
                                }
                                else
                                {
                                    fieldGen = generationDict.TryCreateValue(contLoqui);
                                    targetLoqui = contLoqui;
                                    var isMajorRecord = contLoqui.TargetObjectGeneration != null && await contLoqui.TargetObjectGeneration.IsMajorRecord();
                                    if (isMajorRecord
                                        || await HasMajorRecords(contLoqui, includeBaseClass: true) != Case.No)
                                    {
                                        switch (await HasMajorRecords(contLoqui, includeBaseClass: true))
                                        {
                                            case Case.Yes:
                                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.HasBeenSet ? ".TryIterate()" : null)})");
                                                using (new BraceWrapper(fieldGen))
                                                {
                                                    await LoquiTypeHandler(fieldGen, $"subItem", contLoqui, generic: "TMajor");
                                                }
                                                break;
                                            case Case.Maybe:
                                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.HasBeenSet ? ".TryIterate()" : null)}.WhereCastable<{contLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                                using (new BraceWrapper(fieldGen))
                                                {
                                                    await LoquiTypeHandler(fieldGen, $"subItem", contLoqui, generic: "TMajor");
                                                }
                                                break;
                                            case Case.No:
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            else if (field is DictType dict)
                            {
                                if (dict.Mode != DictMode.KeyedValue) continue;
                                if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
                                if (dictLoqui.RefType == LoquiType.LoquiRefType.Generic)
                                {
                                    fieldGen = generationDict.TryCreateValue("default:");
                                    fieldGen.AppendLine($"if(typeof(TMajor).IsAssignableFrom(typeof({dictLoqui.GenericDef.Name})))");
                                    using (new BraceWrapper(fieldGen))
                                    {
                                        fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.Items)");
                                        using (new BraceWrapper(fieldGen))
                                        {
                                            fieldGen.AppendLine($"yield return (item as TMajor)!;");
                                        }
                                    }
                                }
                                else
                                {
                                    fieldGen = generationDict.TryCreateValue(dictLoqui);
                                    targetLoqui = dictLoqui;
                                    var isMajorRecord = dictLoqui.TargetObjectGeneration != null && await dictLoqui.TargetObjectGeneration.IsMajorRecord();
                                    if (isMajorRecord
                                        || await HasMajorRecords(dictLoqui, includeBaseClass: true) != Case.No)
                                    {
                                        switch (await HasMajorRecords(dictLoqui, includeBaseClass: true))
                                        {
                                            case Case.Yes:
                                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                                                using (new BraceWrapper(fieldGen))
                                                {
                                                    await LoquiTypeHandler(fieldGen, $"subItem", dictLoqui, generic: "TMajor");
                                                }
                                                break;
                                            case Case.Maybe:
                                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                                using (new BraceWrapper(fieldGen))
                                                {
                                                    await LoquiTypeHandler(fieldGen, $"subItem", dictLoqui, generic: "TMajor");
                                                }
                                                break;
                                            case Case.No:
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        foreach (var kv in generationDict)
                        {
                            switch (kv.Key)
                            {
                                case LoquiType loqui:
                                    if (loqui.RefType == LoquiType.LoquiRefType.Generic)
                                    {
                                        // Handled in default case
                                        continue;
                                    }
                                    else
                                    {
                                        if (loqui.TargetObjectGeneration != null)
                                        {
                                            fg.AppendLine($"case \"{loqui.TargetObjectGeneration.ObjectName}\":");
                                        }
                                        fg.AppendLine($"case \"{loqui.Interface(getter: true)}\":");
                                        fg.AppendLine($"case \"{loqui.Interface(getter: false)}\":");
                                        if (loqui.HasInternalGetInterface)
                                        {
                                            fg.AppendLine($"case \"{loqui.Interface(getter: true, internalInterface: true)}\":");
                                        }
                                        if (loqui.HasInternalSetInterface)
                                        {
                                            fg.AppendLine($"case \"{loqui.Interface(getter: false, internalInterface: true)}\":");
                                        }
                                    }
                                    break;
                                case ObjectGeneration targetObj:
                                    fg.AppendLine($"case \"{targetObj.ObjectName}\":");
                                    fg.AppendLine($"case \"{targetObj.Interface(getter: true)}\":");
                                    fg.AppendLine($"case \"{targetObj.Interface(getter: false)}\":");
                                    if (targetObj.HasInternalGetInterface)
                                    {
                                        fg.AppendLine($"case \"{targetObj.Interface(getter: true, internalInterface: true)}\":");
                                    }
                                    if (targetObj.HasInternalSetInterface)
                                    {
                                        fg.AppendLine($"case \"{targetObj.Interface(getter: false, internalInterface: true)}\":");
                                    }
                                    break;
                                case string str:
                                    if (str != "default:")
                                    {
                                        throw new NotImplementedException();
                                    }
                                    continue;
                                default:
                                    throw new NotImplementedException();
                            }
                            using (new DepthWrapper(fg))
                            {
                                fg.AppendLines(kv.Value);
                                fg.AppendLine("yield break;");
                            }
                        }

                        fg.AppendLine("default:");
                        using (new DepthWrapper(fg))
                        {
                            if (generationDict.TryGetValue("default:", out var gen))
                            {
                                fg.AppendLines(gen);
                                fg.AppendLine("yield break;");
                            }
                            else
                            {
                                fg.AppendLine("throw new ArgumentException();");
                            }
                        }
                    }
                }
            }
            fg.AppendLine();

            // Generate base overrides
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasMajorRecords(baseClass, includeBaseClass: true) != Case.No)
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public override IEnumerable<TMajor> EnumerateMajorRecords<TMajor>"))
                    {
                        args.Add($"{baseClass.Interface(getter: getter)} obj");
                        args.Wheres.Add($"where TMajor : {nameof(IMajorRecordCommon)}{(getter ? "Getter" : null)}");
                    }
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            "EnumerateMajorRecords<TMajor>"))
                        {
                            args.Add($"({obj.Interface(getter: getter)})obj");
                        }
                    }
                    fg.AppendLine();
                }
            }
        }
    }
}
