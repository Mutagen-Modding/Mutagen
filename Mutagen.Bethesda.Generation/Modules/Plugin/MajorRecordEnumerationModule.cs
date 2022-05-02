using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Aspects;
using Noggog;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MajorRecordEnumerationModule : GenerationModule
{
    public override async Task PostLoad(ObjectGeneration obj)
    {
        if (await MajorRecordModule.HasMajorRecordsInTree(obj, false) == Case.No) return;
        obj.Interfaces.Add(LoquiInterfaceDefinitionType.IGetter, nameof(IMajorRecordGetterEnumerable));
        obj.Interfaces.Add(LoquiInterfaceDefinitionType.ISetter, nameof(IMajorRecordEnumerable));
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (await MajorRecordModule.HasMajorRecordsInTree(obj, false) == Case.No) return;
        GenerateClassImplementation(obj, sb);
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        yield return "Mutagen.Bethesda.Plugins.Records.Mapping";
    }

    public static void GenerateClassImplementation(ObjectGeneration obj, StructuredStringBuilder sb, bool onlyGetter = false)
    {
        sb.AppendLine("[DebuggerStepThrough]");
        sb.AppendLine($"IEnumerable<{nameof(IMajorRecordGetter)}> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
        sb.AppendLine("[DebuggerStepThrough]");
        sb.AppendLine($"IEnumerable<TMajor> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords<TMajor>(bool throwIfUnknown) => this.EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}(throwIfUnknown: throwIfUnknown);");
        sb.AppendLine("[DebuggerStepThrough]");
        sb.AppendLine($"IEnumerable<{nameof(IMajorRecordGetter)}> {nameof(IMajorRecordGetterEnumerable)}.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);");
        if (!onlyGetter)
        {
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<{nameof(IMajorRecord)}> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords() => this.EnumerateMajorRecords();");
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<TMajor> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords<TMajor>(bool throwIfUnknown) => this.EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}(throwIfUnknown: throwIfUnknown);");
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<{nameof(IMajorRecord)}> {nameof(IMajorRecordEnumerable)}.EnumerateMajorRecords(Type? type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);");
        }
    }

    public override async Task GenerateInCommonMixin(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
        var needsCatch = obj.GetObjectType() == ObjectType.Mod;
        string catchLine = needsCatch ? ".Catch(e => throw RecordException.Enrich(e, obj.ModKey))" : string.Empty;
        string enderSemi = needsCatch ? string.Empty : ";";
        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<{nameof(IMajorRecordGetter)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords",
                       suffixLine: catchLine))
            {
                args.AddPassArg("obj");
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<TMajor> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            args.Wheres.Add($"where TMajor : class, IMajorRecordQueryableGetter");
            args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
            args.Add($"bool throwIfUnknown = true");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
            {
                args.AddPassArg("obj");
                args.Add("type: typeof(TMajor)");
                args.AddPassArg("throwIfUnknown");
            }
            using (sb.IncreaseDepth())
            {
                sb.AppendLine($".Select(m => (TMajor)m){enderSemi}");
                if (needsCatch)
                {
                    sb.AppendLine($"{catchLine};");
                }
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<{nameof(IMajorRecordGetter)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
            args.Add($"Type type");
            args.Add($"bool throwIfUnknown = true");
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
            {
                args.AddPassArg("obj");
                args.AddPassArg("type");
                args.AddPassArg("throwIfUnknown");
            }
            using (sb.IncreaseDepth())
            {
                sb.AppendLine($".Select(m => ({nameof(IMajorRecordGetter)})m){enderSemi}");
                if (needsCatch)
                {
                    sb.AppendLine($"{catchLine};");
                }
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<{nameof(IMajorRecord)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
            args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumerateMajorRecords",
                       suffixLine: catchLine))
            {
                args.AddPassArg("obj");
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<TMajor> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
            args.Wheres.Add($"where TMajor : class, IMajorRecordQueryable");
            args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumerateMajorRecords"))
            {
                args.AddPassArg("obj");
                args.Add("type: typeof(TMajor)");
                args.Add("throwIfUnknown: true");
            }
            using (sb.IncreaseDepth())
            {
                sb.AppendLine($".Select(m => (TMajor)m){enderSemi}");
                if (needsCatch)
                {
                    sb.AppendLine($"{catchLine};");
                }
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<{nameof(IMajorRecord)}> EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Add($"this {obj.Interface(getter: false, internalInterface: true)} obj");
            args.Add($"Type? type");
            args.Add($"bool throwIfUnknown = true");
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.ISetter, obj.Generics));
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.EnumeratePotentiallyTypedMajorRecords"))
            {
                args.AddPassArg("obj");
                args.AddPassArg("type");
                args.AddPassArg("throwIfUnknown");
            }
            using (sb.IncreaseDepth())
            {
                sb.AppendLine($".Select(m => ({nameof(IMajorRecord)})m){enderSemi}");
                if (needsCatch)
                {
                    sb.AppendLine($"{catchLine};");
                }
            }
        }
        sb.AppendLine();
    }

    private async Task LoquiTypeHandler(
        StructuredStringBuilder sb,
        Accessor loquiAccessor,
        LoquiType loquiType,
        string generic,
        bool checkType,
        ObjectGeneration targetObj = null)
    {
        // ToDo  
        // Quick hack.  Real solution should use reflection to investigate the interface  
        if (loquiType.RefType == LoquiType.LoquiRefType.Interface)
        {
            if (checkType)
            {
                sb.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
            }
            using (sb.CurlyBrace(doIt: checkType))
            {
                sb.AppendLine($"yield return {loquiAccessor};");
            }
            return;
        }

        if (loquiType.TargetObjectGeneration != null
            && await loquiType.TargetObjectGeneration.IsMajorRecord()
            && (targetObj == null || targetObj == loquiType.TargetObjectGeneration))
        {
            if (checkType)
            {
                sb.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
            }
            using (sb.CurlyBrace(doIt: checkType))
            {
                sb.AppendLine($"yield return {loquiAccessor};");
            }
        }
        if (await MajorRecordModule.HasMajorRecords(loquiType, includeBaseClass: true) == Case.No)
        {
            return;
        }
        sb.AppendLine($"foreach (var item in {loquiAccessor}.EnumerateMajorRecords({(generic == null ? null : "type, throwIfUnknown: false")}))");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"yield return item;");
        }
    }

    public override async Task GenerateInCommon(ObjectGeneration obj, StructuredStringBuilder sb, MaskTypeSet maskTypes)
    {
        bool getter = maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class);
        bool setter = maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class);
        if (!getter && !setter) return;
        var accessor = new Accessor("obj");
        if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
        var overrideStr = await obj.FunctionOverride(async c => await MajorRecordModule.HasMajorRecords(c, includeBaseClass: false, includeSelf: true) != Case.No);

        using (var args = sb.Function(
                   $"public{overrideStr}IEnumerable<{nameof(IMajorRecord)}{(getter ? "Getter" : null)}> EnumerateMajorRecords"))
        {
            args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
        }
        using (sb.CurlyBrace())
        {
            if (setter)
            {
                sb.AppendLine($"foreach (var item in {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecords(obj))");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"yield return (item as {nameof(IMajorRecord)})!;");
                }
            }
            else
            {

                var sbCount = sb.Count;
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await MajorRecordModule.HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
                    {
                        sb.AppendLine("foreach (var item in base.EnumerateMajorRecords(obj))");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("yield return item;");
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

                    StructuredStringBuilder fieldFg = new StructuredStringBuilder();
                    if (field is LoquiType loqui)
                    {
                        var isMajorRecord = loqui.TargetObjectGeneration != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                        if (isMajorRecord
                            || await MajorRecordModule.HasMajorRecords(loqui, includeBaseClass: true) != Case.No)
                        {
                            var subFg = new StructuredStringBuilder();
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
                                fieldFg.AppendLine($"if ({accessor}.{loqui.Name} is {{}} {loqui.Name}item)");
                                using (fieldFg.CurlyBrace())
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
                            || await MajorRecordModule.HasMajorRecords(contLoqui, includeBaseClass: true) != Case.No)
                        {
                            switch (await MajorRecordModule.HasMajorRecords(contLoqui, includeBaseClass: true))
                            {
                                case Case.Yes:
                                    fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".EmptyIfNull()" : null)})");
                                    using (fieldFg.CurlyBrace())
                                    {
                                        await LoquiTypeHandler(fieldFg, $"subItem", contLoqui, generic: null, checkType: false);
                                    }
                                    break;
                                case Case.Maybe:
                                    fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".EmptyIfNull()" : null)}.WhereCastable<{contLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                    using (fieldFg.CurlyBrace())
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
                            || await MajorRecordModule.HasMajorRecords(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            switch (await MajorRecordModule.HasMajorRecords(dictLoqui, includeBaseClass: true))
                            {
                                case Case.Yes:
                                    fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                                    using (fieldFg.CurlyBrace())
                                    {
                                        await LoquiTypeHandler(fieldFg, $"subItem", dictLoqui, generic: null, checkType: false);
                                    }
                                    break;
                                case Case.Maybe:
                                    fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                                    using (fieldFg.CurlyBrace())
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
                            sb.AppendLine($"if ({field.NullableAccessor(getter: true, Accessor.FromType(field, accessor.ToString()))})");
                        }
                        using (sb.CurlyBrace(doIt: field.Nullable))
                        {
                            sb.AppendLines(fieldFg);
                        }
                    }
                }
                if (sbCount == sb.Count)
                {
                    sb.AppendLine("yield break;");
                }
            }
        }
        sb.AppendLine();

        // Generate base overrides  
        foreach (var baseClass in obj.BaseClassTrail())
        {
            if (await MajorRecordModule.HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
            {
                using (var args = sb.Function(
                           $"public override IEnumerable<{nameof(IMajorRecord)}{(getter ? "Getter" : null)}> EnumerateMajorRecords"))
                {
                    args.Add($"{baseClass.Interface(getter: getter)} obj");
                }
                using (sb.CurlyBrace())
                {
                    using (var args = sb.Call(
                               "EnumerateMajorRecords"))
                    {
                        args.Add($"({obj.Interface(getter: getter)})obj");
                    }
                }
                sb.AppendLine();
            }
        }

        using (var args = sb.Function(
                   $"public{overrideStr}IEnumerable<{nameof(IMajorRecordGetter)}> EnumeratePotentiallyTypedMajorRecords"))
        {
            args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
            args.Add($"Type? type");
            args.Add($"bool throwIfUnknown");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("if (type == null) return EnumerateMajorRecords(obj);");
            sb.AppendLine("return EnumerateMajorRecords(obj, type, throwIfUnknown);");
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public{overrideStr}IEnumerable<{nameof(IMajorRecordGetter)}> EnumerateMajorRecords"))
        {
            args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
            args.Add($"Type type");
            args.Add($"bool throwIfUnknown");
        }
        using (sb.CurlyBrace())
        {
            if (setter)
            {
                sb.AppendLine($"foreach (var item in {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecords(obj, type, throwIfUnknown))");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("yield return item;");
                }
            }
            else
            {

                var sbCount = sb.Count;
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await MajorRecordModule.HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
                    {
                        sb.AppendLine("foreach (var item in base.EnumerateMajorRecords<TMajor>(obj))");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("yield return item;");
                        }
                        break;
                    }
                }

                sb.AppendLine("switch (type.Name)");
                using (sb.CurlyBrace())
                {
                    var gameCategory = obj.GetObjectData().GameCategory;
                    sb.AppendLine($"case \"{nameof(IMajorRecord)}\":");
                    sb.AppendLine($"case \"{nameof(MajorRecord)}\":");
                    if (gameCategory != null)
                    {
                        sb.AppendLine($"case \"I{gameCategory}MajorRecord\":");
                        sb.AppendLine($"case \"{gameCategory}MajorRecord\":");
                    }
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine($"if (!{obj.RegistrationName}.SetterType.IsAssignableFrom(obj.GetType())) yield break;");
                        sb.AppendLine("foreach (var item in this.EnumerateMajorRecords(obj))");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("yield return item;");
                        }
                        sb.AppendLine("yield break;");
                    }
                    sb.AppendLine($"case \"{nameof(IMajorRecordGetter)}\":");
                    if (gameCategory != null)
                    {
                        sb.AppendLine($"case \"I{gameCategory}MajorRecordGetter\":");
                    }
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine("foreach (var item in this.EnumerateMajorRecords(obj))");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("yield return item;");
                        }
                        sb.AppendLine("yield break;");
                    }

                    Dictionary<object, StructuredStringBuilder> generationDict = new Dictionary<object, StructuredStringBuilder>();
                    foreach (var field in obj.IterateFields())
                    {
                        StructuredStringBuilder fieldGen;
                        if (field is LoquiType loqui)
                        {
                            if (loqui.TargetObjectGeneration.IsListGroup()) continue;
                            var isMajorRecord = loqui.TargetObjectGeneration != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                            if (!isMajorRecord
                                && await MajorRecordModule.HasMajorRecords(loqui, includeBaseClass: true) == Case.No)
                            {
                                continue;
                            }

                            if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                            {
                                fieldGen = generationDict.GetOrAdd(loqui.GetGroupTarget());
                            }
                            else
                            {
                                fieldGen = generationDict.GetOrAdd(((object)loqui?.TargetObjectGeneration) ?? loqui);
                            }
                        }
                        else if (field is ContainerType cont)
                        {
                            if (cont.SubTypeGeneration is not LoquiType contLoqui) continue;
                            if (contLoqui.RefType == LoquiType.LoquiRefType.Generic)
                            {
                                fieldGen = generationDict.GetOrAdd("default:");
                            }
                            else
                            {
                                fieldGen = generationDict.GetOrAdd(((object)contLoqui?.TargetObjectGeneration) ?? contLoqui);
                            }
                        }
                        else if (field is DictType dict)
                        {
                            if (dict.Mode != DictMode.KeyedValue) continue;
                            if (dict.ValueTypeGen is not LoquiType dictLoqui) continue;
                            if (dictLoqui.RefType == LoquiType.LoquiRefType.Generic)
                            {
                                fieldGen = generationDict.GetOrAdd("default:");
                            }
                            else
                            {
                                fieldGen = generationDict.GetOrAdd(((object)dictLoqui?.TargetObjectGeneration) ?? dictLoqui);
                            }
                        }
                        else
                        {
                            continue;
                        }
                        await ApplyIterationLines(field, fieldGen, accessor, getter);
                    }

                    bool doAdditionlDeepLogic = !obj.Name.EndsWith("ListGroup");
                    var blackList = new HashSet<string>();
                    var deepRecordMapping = new Dictionary<ObjectGeneration, HashSet<TypeGeneration>>();

                    if (doAdditionlDeepLogic)
                    {
                        deepRecordMapping = await MajorRecordModule.FindDeepRecords(obj);
                        foreach (var deepRec in deepRecordMapping)
                        {
                            StructuredStringBuilder deepFg = generationDict.GetOrAdd(deepRec.Key);
                            foreach (var field in deepRec.Value)
                            {
                                await ApplyIterationLines(field, deepFg, accessor, getter, targetObj: deepRec.Key);
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
                                        sb.AppendLine($"case \"{loqui.Interface(getter: true)}\":");
                                        sb.AppendLine($"case \"{loqui.Interface(getter: false)}\":");
                                        if (loqui.HasInternalGetInterface)
                                        {
                                            sb.AppendLine($"case \"{loqui.Interface(getter: true, internalInterface: true)}\":");
                                        }
                                        if (loqui.HasInternalSetInterface)
                                        {
                                            sb.AppendLine($"case \"{loqui.Interface(getter: false, internalInterface: true)}\":");
                                        }
                                        if (loqui.RefType == LoquiType.LoquiRefType.Interface)
                                        {
                                            blackList.Add(loqui.SetterInterface);
                                        }
                                    }
                                    break;
                                case ObjectGeneration targetObj:
                                    targetObj.AppendSwitchCases(sb);
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
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLines(kv.Value);
                                sb.AppendLine("yield break;");
                            }
                        }
                    }

                    sb.AppendLine("default:");
                    using (sb.IncreaseDepth())
                    {
                        // Generate for major record marker interfaces 
                        if (LinkInterfaceModule.ObjectMappings.TryGetValue(obj.ProtoGen.Protocol, out _)
                            || AspectInterfaceModule.ObjectMappings.TryGetValue(obj.ProtoGen.Protocol, out _))
                        {
                            sb.AppendLine($"if ({nameof(InterfaceEnumerationHelper)}.TryEnumerateInterfaceRecordsFor(GameCategory.{obj.ProtoGen.Protocol.Namespace}, {accessor}, type, out var linkInterfaces))");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine($"foreach (var item in linkInterfaces)");
                                using (sb.CurlyBrace())
                                {
                                    sb.AppendLine("yield return item;");
                                }
                                sb.AppendLine("yield break;");
                            }
                        }
                        if (generationDict.TryGetValue("default:", out var gen))
                        {
                            sb.AppendLines(gen);
                            sb.AppendLine("yield break;");
                        }
                        else
                        {
                            sb.AppendLine("if (throwIfUnknown)");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine("throw new ArgumentException($\"Unknown major record type: {type}\");");
                            }
                            sb.AppendLine($"else");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine("yield break;");
                            }
                        }
                    }
                }
            }
        }
        sb.AppendLine();

        // Generate base overrides  
        foreach (var baseClass in obj.BaseClassTrail())
        {
            if (await MajorRecordModule.HasMajorRecords(baseClass, includeBaseClass: true, includeSelf: true) != Case.No)
            {
                using (var args = sb.Function(
                           $"public override IEnumerable<TMajor> EnumerateMajorRecords<TMajor>"))
                {
                    args.Add($"{baseClass.Interface(getter: getter)} obj");
                    args.Wheres.Add($"where TMajor : IMajorRecordQueryable{(getter ? "Getter" : null)}");
                }
                using (sb.CurlyBrace())
                {
                    using (var args = sb.Call(
                               "EnumerateMajorRecords<TMajor>"))
                    {
                        args.Add($"({obj.Interface(getter: getter)})obj");
                    }
                }
                sb.AppendLine();
            }
        }
    }

    async Task ApplyIterationLines(
        TypeGeneration field,
        StructuredStringBuilder fieldGen,
        Accessor accessor,
        bool getter,
        ObjectGeneration targetObj = null,
        HashSet<ObjectGeneration> blackList = null)
    {
        if (field is GroupType group)
        {
            if (blackList?.Contains(group.GetGroupTarget()) ?? false) return;
            fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
            using (fieldGen.CurlyBrace())
            {
                fieldGen.AppendLine("yield return item;");
            }
        }
        else if (field is LoquiType loqui)
        {
            if (blackList?.Contains(loqui.TargetObjectGeneration) ?? false) return;
            var fieldAccessor = loqui.Nullable ? $"{targetObj?.ObjectName}{loqui.Name}item" : $"{accessor}.{loqui.Name}";
            if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
            { // List groups 
                fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                using (fieldGen.CurlyBrace())
                {
                    fieldGen.AppendLine("yield return item;");
                }
                return;
            }
            var subFg = new StructuredStringBuilder();
            await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: "TMajor", checkType: false, targetObj: targetObj);
            if (subFg.Count == 0) return;
            if (loqui.Singleton
                || !loqui.Nullable)
            {
                fieldGen.AppendLines(subFg);
            }
            else
            {
                using (fieldGen.CurlyBrace())
                {
                    fieldGen.AppendLine($"if ({accessor}.{loqui.Name} is {{}} {fieldAccessor})");
                    using (fieldGen.CurlyBrace())
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
                using (fieldGen.CurlyBrace())
                {
                    if (await contLoqui.TargetObjectGeneration.IsMajorRecord())
                    {
                        fieldGen.AppendLine($"if (type.IsAssignableFrom(typeof({contLoqui.GenericDef.Name})))");
                        using (fieldGen.CurlyBrace())
                        {
                            fieldGen.AppendLine($"yield return ({nameof(IMajorRecordGetter)})item;");
                        }
                    }
                    fieldGen.AppendLine($"foreach (var subItem in item.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                    using (fieldGen.CurlyBrace())
                    {
                        fieldGen.AppendLine($"yield return subItem;");
                    }
                }
            }
            else
            {
                var isMajorRecord = contLoqui.TargetObjectGeneration != null && await contLoqui.TargetObjectGeneration.IsMajorRecord();
                if (isMajorRecord
                    || await MajorRecordModule.HasMajorRecords(contLoqui, includeBaseClass: true) != Case.No)
                {
                    switch (await MajorRecordModule.HasMajorRecords(contLoqui, includeBaseClass: true))
                    {
                        case Case.Yes:
                            fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".EmptyIfNull()" : null)})");
                            using (fieldGen.CurlyBrace())
                            {
                                await LoquiTypeHandler(fieldGen, $"subItem", contLoqui, generic: "TMajor", checkType: true);
                            }
                            break;
                        case Case.Maybe:
                            fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".EmptyIfNull()" : null)}.Where(i => i.GetType() == type))");
                            using (fieldGen.CurlyBrace())
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
                fieldGen.AppendLine($"var assignable = type.IsAssignableFrom(typeof({dictLoqui.GenericDef.Name}));");
                fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.Items)");
                using (fieldGen.CurlyBrace())
                {
                    fieldGen.AppendLine($"if (assignable)");
                    using (fieldGen.CurlyBrace())
                    {
                        fieldGen.AppendLine($"yield return item;");
                    }
                    fieldGen.AppendLine($"foreach (var subItem in item.EnumerateMajorRecords(type, throwIfUnknown: false))");
                    using (fieldGen.CurlyBrace())
                    {
                        fieldGen.AppendLine($"yield return subItem;");
                    }
                }
            }
            else
            {
                var isMajorRecord = dictLoqui.TargetObjectGeneration != null && await dictLoqui.TargetObjectGeneration.IsMajorRecord();
                if (isMajorRecord
                    || await MajorRecordModule.HasMajorRecords(dictLoqui, includeBaseClass: true) != Case.No)
                {
                    switch (await MajorRecordModule.HasMajorRecords(dictLoqui, includeBaseClass: true))
                    {
                        case Case.Yes:
                            fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                            using (fieldGen.CurlyBrace())
                            {
                                await LoquiTypeHandler(fieldGen, $"subItem", dictLoqui, generic: "TMajor", checkType: false);
                            }
                            break;
                        case Case.Maybe:
                            fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: false)}, {(getter ? nameof(IMajorRecordGetterEnumerable) : nameof(IMajorRecordEnumerable))}>())");
                            using (fieldGen.CurlyBrace())
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