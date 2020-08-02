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
            fg.AppendLine("[DebuggerStepThrough]");
            fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommonGetter)}> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type, throwIfUnknown);");
            if (!onlyGetter)
            {
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommon)}> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<TMajor> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords<TMajor>() => this.EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}();");
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<{nameof(IMajorRecordCommon)}> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type, throwIfUnknown);");
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

        public static async Task<Case> HasMajorRecords(ObjectGeneration obj, bool includeBaseClass, bool includeSelf, GenericSpecification specifications = null)
        {
            if (obj.Name == "ListGroup") return Case.Yes;
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    if (includeSelf
                        && loqui.TargetObjectGeneration != null
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

        public static async IAsyncEnumerable<ObjectGeneration> IterateMajorRecords(LoquiType loqui, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (specifications?.Specifications.Count > 0)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    if (await specObj.IsMajorRecord()) yield return specObj;
                    await foreach (var item in IterateMajorRecords(specObj, includeBaseClass, includeSelf: true, loqui.GenericSpecification))
                    {
                        yield return item;
                    }
                }
            }
            else if (loqui.TargetObjectGeneration != null)
            {
                if (await loqui.TargetObjectGeneration.IsMajorRecord()) yield return loqui.TargetObjectGeneration;
                await foreach (var item in IterateMajorRecords(loqui.TargetObjectGeneration, includeBaseClass, includeSelf: true, loqui.GenericSpecification))
                {
                    yield return item;
                }
            }
            else if (loqui.RefType == LoquiType.LoquiRefType.Interface)
            {
                // Must be a link interface 
                if (!LinkInterfaceModule.ObjectMappings[loqui.ObjectGen.ProtoGen.Protocol].TryGetValue(loqui.SetterInterface, out var mappings))
                {
                    throw new ArgumentException();
                }
                foreach (var obj in mappings)
                {
                    yield return obj;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static async IAsyncEnumerable<ObjectGeneration> IterateMajorRecords(ObjectGeneration obj, bool includeBaseClass, bool includeSelf, GenericSpecification specifications = null)
        {
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    if (includeSelf
                        && loqui.TargetObjectGeneration != null
                        && await loqui.TargetObjectGeneration.IsMajorRecord())
                    {
                        yield return loqui.TargetObjectGeneration;
                    }
                    await foreach (var item in IterateMajorRecords(loqui, includeBaseClass, specifications))
                    {
                        yield return item;
                    }
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType contLoqui)
                    {
                        await foreach (var item in IterateMajorRecords(contLoqui, includeBaseClass, specifications))
                        {
                            yield return item;
                        }
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        await foreach (var item in IterateMajorRecords(valLoqui, includeBaseClass, specifications))
                        {
                            yield return item;
                        }
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        await foreach (var item in IterateMajorRecords(keyLoqui, includeBaseClass, specifications))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public static async Task<Case> HasMajorRecordsInTree(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (await HasMajorRecords(obj, includeBaseClass: includeBaseClass, includeSelf: false, specifications: specifications) == Case.Yes) return Case.Yes;
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
                using (var args = new FunctionWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
                {
                    args.AddPassArg("obj");
                    args.Add("type: typeof(TMajor)");
                    args.Add("throwIfUnknown: true");
                }
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine(".Select(m => (TMajor)m);");
                }
            }
            fg.AppendLine();

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IMajorRecordCommonGetter)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
                args.Add($"Type type");
                args.Add($"bool throwIfUnknown = true");
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new FunctionWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("type");
                    args.AddPassArg("throwIfUnknown");
                }
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine($".Select(m => ({nameof(IMajorRecordCommonGetter)})m);");
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
                using (var args = new FunctionWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
                {
                    args.AddPassArg("obj");
                    args.Add("type: typeof(TMajor)");
                    args.Add("throwIfUnknown: true");
                }
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine(".Select(m => (TMajor)m);");
                }
            }
            fg.AppendLine();

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IMajorRecordCommon)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
                args.Add($"Type type");
                args.Add($"bool throwIfUnknown = true");
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new FunctionWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("type");
                    args.AddPassArg("throwIfUnknown");
                }
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine($".Select(m => ({nameof(IMajorRecordCommon)})m);");
                }
            }
            fg.AppendLine();
        }

        private async Task LoquiTypeHandler(
            FileGeneration fg,
            Accessor loquiAccessor,
            LoquiType loquiType,
            string generic,
            bool checkType)
        {
            // ToDo  
            // Quick hack.  Real solution should use reflection to investigate the interface  
            if (loquiType.RefType == LoquiType.LoquiRefType.Interface)
            {
                if (checkType)
                {
                    fg.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
                }
                using (new BraceWrapper(fg, doIt: checkType))
                {
                    fg.AppendLine($"yield return {loquiAccessor};");
                }
                return;
            }

            if (loquiType.TargetObjectGeneration != null
                && await loquiType.TargetObjectGeneration.IsMajorRecord())
            {
                if (checkType)
                {
                    fg.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
                }
                using (new BraceWrapper(fg, doIt: checkType))
                {
                    fg.AppendLine($"yield return {loquiAccessor};");
                }
            }
            if (await HasMajorRecords(loquiType, includeBaseClass: true) == Case.No)
            {
                return;
            }
            fg.AppendLine($"foreach (var item in {loquiAccessor}.EnumerateMajorRecords({(generic == null ? null : "type, throwIfUnknown: false")}))");
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
            var overrideStr = await obj.FunctionOverride(async c => await HasMajorRecords(c, includeBaseClass: false, includeSelf: true) != Case.No);

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
                        if (await HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
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
                            var isMajorRecord = loqui.TargetObjectGeneration != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                            if (isMajorRecord
                                || await HasMajorRecords(loqui, includeBaseClass: true) != Case.No)
                            {
                                var subFg = new FileGeneration();
                                var fieldAccessor = loqui.Nullable ? $"{loqui.Name}item" : $"{accessor}.{loqui.Name}";
                                await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: null, checkType: false);
                                if (subFg.Count == 0) continue;
                                if (loqui.Singleton
                                    || !loqui.Nullable)
                                {
                                    fieldFg.AppendLines(subFg);
                                }
                                else
                                {
                                    fieldFg.AppendLine($"if ({accessor}.{loqui.Name}.TryGet(out var {loqui.Name}item))");
                                    using (new BraceWrapper(fieldFg))
                                    {
                                        fieldFg.AppendLines(subFg);
                                    }
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
                                        fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".TryIterate()" : null)})");
                                        using (new BraceWrapper(fieldFg))
                                        {
                                            await LoquiTypeHandler(fieldFg, $"subItem", contLoqui, generic: null, checkType: false);
                                        }
                                        break;
                                    case Case.Maybe:
                                        fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".TryIterate()" : null)}.WhereCastable<{contLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                        using (new BraceWrapper(fieldFg))
                                        {
                                            await LoquiTypeHandler(fieldFg, $"subItem", contLoqui, generic: null, checkType: false);
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
                                        fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                                        using (new BraceWrapper(fieldFg))
                                        {
                                            await LoquiTypeHandler(fieldFg, $"subItem", dictLoqui, generic: null, checkType: false);
                                        }
                                        break;
                                    case Case.Maybe:
                                        fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                        using (new BraceWrapper(fieldFg))
                                        {
                                            await LoquiTypeHandler(fieldFg, $"subItem", dictLoqui, generic: null, checkType: false);
                                        }
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
                if (await HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
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
                $"public{overrideStr}IEnumerable<{nameof(IMajorRecordCommonGetter)}> EnumerateMajorRecords"))
            {
                args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
                args.Add($"Type type");
                args.Add($"bool throwIfUnknown");
            }
            using (new BraceWrapper(fg))
            {
                if (setter)
                {
                    fg.AppendLine($"foreach (var item in {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecords(obj, type, throwIfUnknown))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("yield return item;");
                    }
                }
                else
                {

                    var fgCount = fg.Count;
                    foreach (var baseClass in obj.BaseClassTrail())
                    {
                        if (await HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
                        {
                            fg.AppendLine("foreach (var item in base.EnumerateMajorRecords<TMajor>(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }
                            break;
                        }
                    }

                    fg.AppendLine("switch (type.Name)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"case \"{nameof(IMajorRecordCommon)}\":");
                        fg.AppendLine($"case \"{nameof(IMajorRecordCommonGetter)}\":");
                        fg.AppendLine($"case \"{nameof(MajorRecord)}\":");
                        var gameCategory = obj.GetObjectData().GameCategory;
                        if (gameCategory != null)
                        {
                            fg.AppendLine($"case \"I{gameCategory}MajorRecord\":");
                            fg.AppendLine($"case \"I{gameCategory}MajorRecordGetter\":");
                            fg.AppendLine($"case \"{gameCategory}MajorRecord\":");
                        }
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine("foreach (var item in this.EnumerateMajorRecords(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }
                            fg.AppendLine("yield break;");
                        }

                        Dictionary<object, FileGeneration> generationDict = new Dictionary<object, FileGeneration>();
                        foreach (var field in obj.IterateFields())
                        {
                            FileGeneration fieldGen;
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
                                    fieldGen = generationDict.TryCreateValue(((object)loqui?.TargetObjectGeneration) ?? loqui);
                                }
                            }
                            else if (field is ContainerType cont)
                            {
                                if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
                                if (contLoqui.RefType == LoquiType.LoquiRefType.Generic)
                                {
                                    fieldGen = generationDict.TryCreateValue("default:");
                                }
                                else
                                {
                                    fieldGen = generationDict.TryCreateValue(((object)contLoqui?.TargetObjectGeneration) ?? contLoqui);
                                }
                            }
                            else if (field is DictType dict)
                            {
                                if (dict.Mode != DictMode.KeyedValue) continue;
                                if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
                                if (dictLoqui.RefType == LoquiType.LoquiRefType.Generic)
                                {
                                    fieldGen = generationDict.TryCreateValue("default:");
                                }
                                else
                                {
                                    fieldGen = generationDict.TryCreateValue(((object)dictLoqui?.TargetObjectGeneration) ?? dictLoqui);
                                }
                            }
                            else
                            {
                                continue;
                            }
                            await ApplyIterationLines(field, fieldGen, accessor, getter);
                        }

                        bool doAdditionlDeepLogic = obj.Name != "ListGroup";

                        if (doAdditionlDeepLogic)
                        {
                            // Find and add "Deep" records 
                            var deepRecordMapping = new Dictionary<ObjectGeneration, HashSet<TypeGeneration>>();
                            foreach (var field in obj.IterateFields())
                            {
                                if (field is LoquiType loqui)
                                {
                                    var groupType = field as GroupType;
                                    await foreach (var deepObj in IterateMajorRecords(loqui, includeBaseClass: true))
                                    {
                                        if (groupType != null
                                            && groupType.GetGroupTarget() == deepObj)
                                        {
                                            continue;
                                        }
                                        if (loqui.TargetObjectGeneration == deepObj) continue;
                                        deepRecordMapping.TryCreateValue(deepObj).Add(field);
                                    }
                                }
                                else if (field is ContainerType cont)
                                {
                                    if (!(cont.SubTypeGeneration is LoquiType subLoqui)) continue;
                                    await foreach (var deepObj in IterateMajorRecords(subLoqui, includeBaseClass: true))
                                    {
                                        if (subLoqui.TargetObjectGeneration == deepObj) continue;
                                        deepRecordMapping.TryCreateValue(deepObj).Add(field);
                                    }
                                }
                            }
                            foreach (var deepRec in deepRecordMapping)
                            {
                                FileGeneration deepFg = generationDict.TryCreateValue(deepRec.Key);
                                foreach (var field in deepRec.Value)
                                {
                                    await ApplyIterationLines(field, deepFg, accessor, getter, nickname: deepRec.Key.ObjectName);
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

                            if ((obj.GetObjectType() == ObjectType.Mod
                                || obj.GetObjectType() == ObjectType.Group))
                            {
                                // Generate for major record marker interfaces 
                                if (LinkInterfaceModule.ObjectMappings.TryGetValue(obj.ProtoGen.Protocol, out var interfs))
                                {
                                    foreach (var interf in interfs)
                                    {
                                        FileGeneration subFg = new FileGeneration();
                                        HashSet<ObjectGeneration> passedObjects = new HashSet<ObjectGeneration>();
                                        HashSet<TypeGeneration> deepObjects = new HashSet<TypeGeneration>();
                                        foreach (var subObj in interf.Value)
                                        {
                                            var grup = obj.Fields
                                                .WhereCastable<TypeGeneration, GroupType>()
                                                .Where(g => g.GetGroupTarget() == subObj)
                                                .FirstOrDefault();

                                            if (grup != null)
                                            {
                                                subFg.AppendLine($"foreach (var item in EnumerateMajorRecords({accessor}, typeof({grup.GetGroupTarget().ObjectName}), throwIfUnknown: throwIfUnknown))");
                                                using (new BraceWrapper(subFg))
                                                {
                                                    subFg.AppendLine("yield return item;");
                                                }
                                                passedObjects.Add(grup.GetGroupTarget());
                                            }
                                            else if (deepRecordMapping.TryGetValue(subObj, out var deepRec))
                                            {
                                                foreach (var field in deepRec)
                                                {
                                                    deepObjects.Add(field);
                                                }
                                            }
                                        }
                                        foreach (var deepObj in deepObjects)
                                        {
                                            await ApplyIterationLines(deepObj, subFg, accessor, getter, blackList: passedObjects);
                                        }
                                        if (!subFg.Empty)
                                        {
                                            fg.AppendLine($"case \"{interf.Key}\":");
                                            fg.AppendLine($"case \"{interf.Key}Getter\":");
                                            using (new BraceWrapper(fg))
                                            {
                                                fg.AppendLines(subFg);
                                                fg.AppendLine("yield break;");
                                            }
                                        }
                                    }
                                }
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
                                fg.AppendLine("if (throwIfUnknown)");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine("throw new ArgumentException($\"Unknown major record type: {type}\");");
                                }
                                fg.AppendLine($"else");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine("yield break;");
                                }
                            }
                        }
                    }
                }
            }
            fg.AppendLine();

            // Generate base overrides  
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
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

        async Task ApplyIterationLines(
            TypeGeneration field,
            FileGeneration fieldGen,
            Accessor accessor,
            bool getter,
            string nickname = null,
            HashSet<ObjectGeneration> blackList = null)
        {
            if (field is GroupType group)
            {
                if (blackList?.Contains(group.GetGroupTarget()) ?? false) return;
                fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                using (new BraceWrapper(fieldGen))
                {
                    fieldGen.AppendLine("yield return item;");
                }
            }
            else if (field is LoquiType loqui)
            {
                if (blackList?.Contains(loqui.TargetObjectGeneration) ?? false) return;
                var fieldAccessor = loqui.Nullable ? $"{nickname}{loqui.Name}item" : $"{accessor}.{loqui.Name}";
                if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                { // List groups 
                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine("yield return item;");
                    }
                    return;
                }
                var subFg = new FileGeneration();
                await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: "TMajor", checkType: false);
                if (subFg.Count == 0) return;
                if (loqui.Singleton
                    || !loqui.Nullable)
                {
                    fieldGen.AppendLines(subFg);
                }
                else
                {
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine($"if ({accessor}.{loqui.Name}.TryGet(out var {fieldAccessor}))");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLines(subFg);
                        }
                    }
                }
            }
            else if (field is ContainerType cont)
            {
                if (!(cont.SubTypeGeneration is LoquiType contLoqui)) return;
                if (contLoqui.RefType == LoquiType.LoquiRefType.Generic)
                {
                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name})");
                    using (new BraceWrapper(fieldGen))
                    {
                        if (await contLoqui.TargetObjectGeneration.IsMajorRecord())
                        {
                            fieldGen.AppendLine($"if (type.IsAssignableFrom(typeof({contLoqui.GenericDef.Name})))");
                            using (new BraceWrapper(fieldGen))
                            {
                                fieldGen.AppendLine($"yield return ({nameof(IMajorRecordCommonGetter)})item;");
                            }
                        }
                        fieldGen.AppendLine($"foreach (var subItem in item.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLine($"yield return subItem;");
                        }
                    }
                }
                else
                {
                    var isMajorRecord = contLoqui.TargetObjectGeneration != null && await contLoqui.TargetObjectGeneration.IsMajorRecord();
                    if (isMajorRecord
                        || await HasMajorRecords(contLoqui, includeBaseClass: true) != Case.No)
                    {
                        switch (await HasMajorRecords(contLoqui, includeBaseClass: true))
                        {
                            case Case.Yes:
                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".TryIterate()" : null)})");
                                using (new BraceWrapper(fieldGen))
                                {
                                    await LoquiTypeHandler(fieldGen, $"subItem", contLoqui, generic: "TMajor", checkType: true);
                                }
                                break;
                            case Case.Maybe:
                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".TryIterate()" : null)}.Where(i => i.GetType() == type))");
                                using (new BraceWrapper(fieldGen))
                                {
                                    await LoquiTypeHandler(fieldGen, $"subItem", contLoqui, generic: "TMajor", checkType: true);
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
                if (dict.Mode != DictMode.KeyedValue) return;
                if (!(dict.ValueTypeGen is LoquiType dictLoqui)) return;
                if (dictLoqui.RefType == LoquiType.LoquiRefType.Generic)
                {
                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.Items)");
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine($"if (type.IsAssignableFrom(typeof({dictLoqui.GenericDef.Name})))");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLine($"yield return item;");
                        }
                        fieldGen.AppendLine($"foreach (var subItem in item.EnumerateMajorRecords(type, throwIfUnknown: false))");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLine($"yield return subItem;");
                        }
                    }
                }
                else
                {
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
                                    await LoquiTypeHandler(fieldGen, $"subItem", dictLoqui, generic: "TMajor", checkType: false);
                                }
                                break;
                            case Case.Maybe:
                                fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                using (new BraceWrapper(fieldGen))
                                {
                                    await LoquiTypeHandler(fieldGen, $"subItem", dictLoqui, generic: "TMajor", checkType: false);
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
    }
}
