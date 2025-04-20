using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Aspects;
using Noggog;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MajorRecordContextEnumerationModule : GenerationModule
{
    public override async Task PostLoad(ObjectGeneration obj)
    {
        if (obj.GetObjectType() == ObjectType.Mod)
        {
            obj.Interfaces.Add(LoquiInterfaceDefinitionType.IGetter, $"IMajorRecordContextEnumerable<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>");
        }
        if (await MajorRecordModule.HasMajorRecordsInTree(obj, false) == Case.No) return;
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectType() != ObjectType.Mod) return;
        GenerateClassImplementation(obj, sb);
    }

    public static void GenerateClassImplementation(ObjectGeneration obj, StructuredStringBuilder sb, bool onlyGetter = false)
    {
        if (obj.GetObjectType() == ObjectType.Mod)
        {
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<IModContext<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}, TSetter, TGetter>> IMajorRecordContextEnumerable<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>.EnumerateMajorRecordContexts<TSetter, TGetter>({nameof(ILinkCache)} linkCache, bool throwIfUnknown) => this.EnumerateMajorRecordContexts{obj.GetGenericTypes(MaskType.Normal, "TSetter".AsEnumerable().And("TGetter").ToArray())}(linkCache, throwIfUnknown: throwIfUnknown);");
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<IModContext<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>.EnumerateMajorRecordContexts({nameof(ILinkCache)} linkCache, Type type, bool throwIfUnknown) => this.EnumerateMajorRecordContexts(linkCache, type: type, throwIfUnknown: throwIfUnknown);");

            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<IModContext<IMajorRecordGetter>> IMajorRecordSimpleContextEnumerable.EnumerateMajorRecordSimpleContexts() => this.EnumerateMajorRecordContexts();");
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<IModContext<TMajor>> IMajorRecordSimpleContextEnumerable.EnumerateMajorRecordSimpleContexts<TMajor>(bool throwIfUnknown) => this.EnumerateMajorRecordContexts(linkCache: null!, typeof(TMajor), throwIfUnknown: throwIfUnknown).Select(x => x.AsType<{typeof(IMajorRecordQueryableGetter)}, TMajor>());");
            sb.AppendLine("[DebuggerStepThrough]");
            sb.AppendLine($"IEnumerable<IModContext<IMajorRecordGetter>> IMajorRecordSimpleContextEnumerable.EnumerateMajorRecordSimpleContexts(Type type, bool throwIfUnknown) => this.EnumerateMajorRecordContexts(linkCache: null!, type: type, throwIfUnknown: throwIfUnknown);");
        }
    }

    public override async Task GenerateInCommonMixin(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectType() != ObjectType.Mod) return;
        var needsCatch = obj.GetObjectType() == ObjectType.Mod;
        string catchLine = needsCatch ? ".Catch(e => RecordException.EnrichAndThrow(e, obj.ModKey))" : string.Empty;
        string enderSemi = needsCatch ? string.Empty : ";";
        var modGetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: true);
        var modSetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: false);

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<IModContext<{modSetter}, {modGetter}, TSetter, TGetter>> EnumerateMajorRecordContexts{obj.GetGenericTypes(MaskType.Normal, new string[] { "TSetter", "TGetter" })}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            args.Wheres.Add($"where TSetter : class, IMajorRecordQueryable, TGetter");
            args.Wheres.Add($"where TGetter : class, IMajorRecordQueryableGetter");
            args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
            args.Add($"{nameof(ILinkCache)} linkCache");
            args.Add($"bool throwIfUnknown = true");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts"))
            {
                args.AddPassArg("obj");
                args.AddPassArg("linkCache");
                args.Add("type: typeof(TGetter)");
                args.AddPassArg("throwIfUnknown");
            }
            using (sb.IncreaseDepth())
            {
                sb.AppendLine($".Select(m => m.AsType<{modSetter}, {modGetter}, {nameof(IMajorRecordQueryable)}, {nameof(IMajorRecordQueryableGetter)}, TSetter, TGetter>()){enderSemi}");
                if (needsCatch)
                {
                    sb.AppendLine($"{catchLine};");
                }
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordContexts"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts",
                       semiColon: !needsCatch))
            {
                args.AddPassArg("obj");
                args.Add("linkCache: null!");
            }
            using (sb.IncreaseDepth())
            {
                if (needsCatch)
                {
                    sb.AppendLine($"{catchLine};");
                }
            }
        }
        sb.AppendLine();

        sb.AppendLine("[DebuggerStepThrough]");
        using (var args = sb.Function(
                   $"public static IEnumerable<IModContext<{modSetter}, {modGetter}, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
            args.Add($"{nameof(ILinkCache)} linkCache");
            args.Add($"Type type");
            args.Add($"bool throwIfUnknown = true");
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts",
                       semiColon: !needsCatch))
            {
                args.AddPassArg("obj");
                args.AddPassArg("linkCache");
                args.AddPassArg("type");
                args.AddPassArg("throwIfUnknown");
            }
            using (sb.IncreaseDepth())
            {
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
        ObjectGeneration obj,
        Accessor loquiAccessor,
        LoquiType loquiType,
        Action<Call> addGetOrAddArg,
        Action<Call> duplicateInArg,
        string generic,
        bool includeType,
        bool checkType,
        bool includeSelf = true)
    {
        var modGetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: true);
        var modSetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: false);

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
                using (var args = sb.Call(
                           $"yield return new ModContext<{modSetter}, {modGetter}, {loquiType.Interface(getter: false)}, {loquiType.Interface(getter: true)}>"))
                {
                    args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add($"record: {loquiAccessor}");
                    args.Add($"parent: curContext");
                    addGetOrAddArg(args);
                    duplicateInArg(args);
                }
            }
            return;
        }

        if (includeSelf
            && loquiType.TargetObjectGeneration != null
            && await loquiType.TargetObjectGeneration.IsMajorRecord())
        {
            if (checkType)
            {
                sb.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
            }
            using (sb.CurlyBrace(doIt: checkType))
            {
                using (var args = sb.Call(
                           $"yield return new ModContext<{modSetter}, {modGetter}, {loquiType.Interface(getter: false, internalInterface: true)}, {loquiType.Interface(getter: true, internalInterface: true)}>"))
                {
                    args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add($"record: {loquiAccessor}");
                    args.Add($"parent: curContext");
                    addGetOrAddArg(args);
                    duplicateInArg(args);
                }
            }
        }
        if (await MajorRecordModule.HasMajorRecords(loquiType, includeBaseClass: true, includeSelf: false) == Case.No)
        {
            return;
        }
        if (obj.IsListGroup()) return;

        if (obj.IsTopLevelGroup())
        {
            sb.AppendLine($"foreach (var item in {loquiAccessor}.EnumerateMajorRecords({(generic == null ? null : "type, throwIfUnknown: false")}))");
            using (sb.CurlyBrace())
            {
                using (var args = sb.Call(
                           $"yield return new ModContext<{modSetter}, {modGetter}, {loquiType.Interface(getter: false, internalInterface: true)}, {loquiType.Interface(getter: true, internalInterface: true)}>"))
                {
                    args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add("record: item");
                    args.Add($"getOrAddAsOverride: (m, r) => m.{loquiType.Name}.GetOrAddAsOverride(r)");
                    args.Add($"duplicateInArg: (m, r, e) => m.{loquiType.Name}.DuplicateInAsNewRecord(r, e)");
                }
            }
        }
        else
        {
            using (var args = sb.Call(
                       $"foreach (var item in {loquiType.TargetObjectGeneration.CommonClassInstance(loquiAccessor, LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts",
                       suffixLine: ")",
                       semiColon: false))
            {
                args.Add($"obj: {loquiAccessor}");
                args.AddPassArg("linkCache");
                if (includeType)
                {
                    args.AddPassArg("type");
                }
                args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                args.Add($"parent: {(obj.GetObjectType() == ObjectType.Mod ? "null" : "curContext")}");
                if (includeType)
                {
                    args.Add("throwIfUnknown: false");
                }
                addGetOrAddArg(args);
                duplicateInArg(args);
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("yield return item;");
            }
        }
    }

    public override async Task GenerateInCommon(ObjectGeneration obj, StructuredStringBuilder sb, MaskTypeSet maskTypes)
    {
        bool getter = maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class);
        if (!getter) return;
        var accessor = new Accessor("obj");
        if (obj.Abstract) return;
        if (obj.GetObjectType() == ObjectType.Group) return;
        if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) == Case.No) return;
        var overrideStr = await obj.FunctionOverride(async c => await MajorRecordModule.HasMajorRecords(c, includeBaseClass: false, includeSelf: true) != Case.No);
        var modGetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: true);
        var modSetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: false);

        using (var args = sb.Function(
                   $"public{overrideStr}IEnumerable<IModContext<{modSetter}, {modGetter}, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts"))
        {
            args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
            args.Add($"{nameof(ILinkCache)} linkCache");
            if (obj.GetObjectType() != ObjectType.Mod)
            {
                args.Add($"{nameof(ModKey)} modKey");
                args.Add($"IModContext? parent");
            }
            if (obj.GetObjectType() == ObjectType.Record)
            {
                args.Add($"Func<{modSetter}, {obj.Interface(getter: true)}, {obj.Interface(getter: false)}> getOrAddAsOverride");
                args.Add($"Func<{modSetter}, {obj.Interface(getter: true)}, string?, FormKey?, {obj.Interface(getter: false)}> duplicateInto");
            }
        }
        using (sb.CurlyBrace())
        {
            if (obj.GetObjectType() == ObjectType.Record)
            {
                using (var args = sb.Call(
                           $"var curContext = new ModContext<{modSetter}, {modGetter}, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                {
                    args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add("record: obj");
                    args.AddPassArg("getOrAddAsOverride");
                    args.AddPassArg("duplicateInto");
                    args.AddPassArg("parent");
                }
            }
            var sbCount = sb.Count;
            var gameCategory = obj.GetObjectData().GameCategory;
            Dictionary<object, StructuredStringBuilder> generationDict = new Dictionary<object, StructuredStringBuilder>();

            bool doAdditionlDeepLogic = !obj.Name.EndsWith("ListGroup");
            var typesWithDeepTargets = new List<TypeGeneration>();
            if (doAdditionlDeepLogic)
            {
                var deepRecordMapping = await MajorRecordModule.FindDeepRecords(obj);
                foreach (var deepRec in deepRecordMapping)
                {
                    typesWithDeepTargets.AddRange(deepRec.Value);
                }
            }

            foreach (var field in obj.IterateFields())
            {
                StructuredStringBuilder fieldGen;
                if (field is LoquiType loqui)
                {
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
                    if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
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
                    if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
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
                await ApplyIterationLines(
                    obj: obj,
                    field: field,
                    fieldGen: fieldGen,
                    accessor: accessor,
                    getter: getter,
                    includeType: false,
                    hasTarget: typesWithDeepTargets.Any(f => f.Name == field.Name));
            }

            foreach (var kv in generationDict)
            {
                sb.AppendLines(kv.Value);
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public{overrideStr}IEnumerable<IModContext<{modSetter}, {modGetter}, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts"))
        {
            args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
            args.Add($"{nameof(ILinkCache)} linkCache");
            args.Add($"Type type");
            if (obj.GetObjectType() != ObjectType.Mod)
            {
                args.Add($"{nameof(ModKey)} modKey");
                args.Add($"IModContext? parent");
            }
            args.Add($"bool throwIfUnknown");
            if (obj.GetObjectType() == ObjectType.Record)
            {
                args.Add($"Func<{modSetter}, {obj.Interface(getter: true)}, {obj.Interface(getter: false)}> getOrAddAsOverride");
                args.Add($"Func<{modSetter}, {obj.Interface(getter: true)}, string?, FormKey?, {obj.Interface(getter: false)}> duplicateInto");
            }
        }
        using (sb.CurlyBrace())
        {
            if (obj.GetObjectType() == ObjectType.Record)
            {
                using (var args = sb.Call(
                           $"var curContext = new ModContext<{modSetter}, {modGetter}, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                {
                    args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add("record: obj");
                    args.AddPassArg("getOrAddAsOverride");
                    args.AddPassArg("duplicateInto");
                    args.AddPassArg("parent");
                }
            }
            var sbCount = sb.Count;
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
                    using (var args = sb.Call(
                               $"foreach (var item in this.EnumerateMajorRecordContexts",
                               suffixLine: ")",
                               semiColon: false))
                    {
                        args.Add("obj");
                        args.AddPassArg("linkCache");
                        if (obj.GetObjectType() != ObjectType.Mod)
                        {
                            args.AddPassArg("modKey");
                            args.AddPassArg("parent");
                        }
                        if (obj.GetObjectType() == ObjectType.Record)
                        {
                            args.AddPassArg("getOrAddAsOverride");
                            args.AddPassArg("duplicateInto");
                        }
                    }
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
                    using (var args = sb.Call(
                               $"foreach (var item in this.EnumerateMajorRecordContexts",
                               suffixLine: ")",
                               semiColon: false))
                    {
                        args.Add("obj");
                        args.AddPassArg("linkCache");
                        if (obj.GetObjectType() != ObjectType.Mod)
                        {
                            args.AddPassArg("modKey");
                            args.AddPassArg("parent");
                        }
                        if (obj.GetObjectType() == ObjectType.Record)
                        {
                            args.AddPassArg("getOrAddAsOverride");
                            args.AddPassArg("duplicateInto");
                        }
                    }
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
                        if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
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
                        if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
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
                    await ApplyIterationLines(
                        obj: obj,
                        field: field,
                        fieldGen: fieldGen,
                        accessor: accessor,
                        getter: getter,
                        includeType: true);
                }

                bool doAdditionlDeepLogic = !obj.Name.EndsWith("ListGroup");

                if (doAdditionlDeepLogic)
                {
                    var deepRecordMapping = await MajorRecordModule.FindDeepRecords(obj);
                    foreach (var deepRec in deepRecordMapping)
                    {
                        StructuredStringBuilder deepFg = generationDict.GetOrAdd(deepRec.Key);
                        foreach (var field in deepRec.Value)
                        {
                            await ApplyIterationLines(
                                obj: obj,
                                field: field,
                                fieldGen: deepFg,
                                accessor: accessor,
                                getter: getter,
                                hasTarget: true,
                                includeSelf: false,
                                includeType: true);
                        }
                    }

                    HashSet<string> blackList = new HashSet<string>();
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
                                sb.AppendLine($"case \"{targetObj.ObjectName}\":");
                                sb.AppendLine($"case \"{targetObj.Interface(getter: true)}\":");
                                sb.AppendLine($"case \"{targetObj.Interface(getter: false)}\":");
                                if (targetObj.HasInternalGetInterface)
                                {
                                    sb.AppendLine($"case \"{targetObj.Interface(getter: true, internalInterface: true)}\":");
                                }
                                if (targetObj.HasInternalSetInterface)
                                {
                                    sb.AppendLine($"case \"{targetObj.Interface(getter: false, internalInterface: true)}\":");
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
                        using (var args = sb.Call(
                                   $"if ({nameof(InterfaceEnumerationHelper)}.TryEnumerateInterfaceContextsFor<{obj.Interface(getter: getter, internalInterface: true)}, I{obj.ProtoGen.Protocol.Namespace}Mod, I{obj.ProtoGen.Protocol.Namespace}ModGetter>",
                                   suffixLine: ")",
                                   semiColon: false))
                        {
                            args.Add($"GameCategory.{obj.ProtoGen.Protocol.Namespace}");
                            args.Add($"{accessor}");
                            args.Add("type");
                            args.Add("linkCache");
                            if (obj.GetObjectType() != ObjectType.Mod)
                            {
                                args.Add($"(lk, t, b) => this.EnumerateMajorRecordContexts(obj, lk, t, modKey, parent, b, getOrAddAsOverride, duplicateInto)");
                            }
                            args.Add("out var linkInterfaces");
                        }
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
        sb.AppendLine();
    }

    async Task ApplyIterationLines(
        ObjectGeneration obj,
        TypeGeneration field,
        StructuredStringBuilder fieldGen,
        Accessor accessor,
        bool getter,
        bool includeType,
        bool hasTarget = false,
        HashSet<ObjectGeneration> blackList = null,
        bool includeSelf = true)
    {
        if (field is GroupType group)
        {
            if (blackList?.Contains(group.GetGroupTarget()) ?? false) return;

            var groupTargetGetter = group.GetGroupTarget().Interface(getter: true, internalInterface: true);
            var groupTargetSetter = group.GetGroupTarget().GetTypeName(LoquiInterfaceType.Direct);
            if (includeSelf)
            {
                using (var args = fieldGen.Call(
                           $"foreach (var item in InterfaceEnumerationHelper.EnumerateGroupContexts<{obj.GetModName(getter: false)}, {obj.GetModName(getter: true)}, {groupTargetSetter}, {groupTargetGetter}>",
                           suffixLine: ")",
                           semiColon: false))
                {
                    args.Add($"srcGroup: obj.{field.Name}");
                    args.Add($"type: {(includeType ? "type" : $"typeof({group.GetGroupTarget().Interface(getter: true)})")}");
                    args.Add("modKey: obj.ModKey");
                    args.Add($"group: (m) => m.{group.Name}");
                    args.Add($"groupGetter: (m) => m.{group.Name}");
                }

                using (fieldGen.CurlyBrace())
                {
                    fieldGen.AppendLine($"yield return item;");
                }
            }
            if (hasTarget)
            {
                fieldGen.AppendLine($"foreach (var groupItem in obj.{field.Name})");
                using (fieldGen.CurlyBrace())
                {
                    using (var args = fieldGen.Call(
                               $"foreach (var item in {group.GetGroupTarget().CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecordContexts",
                               suffixLine: ")",
                               semiColon: false))
                    {
                        args.Add("groupItem");
                        args.AddPassArg("linkCache");
                        if (includeType)
                        {
                            args.AddPassArg("type");
                            args.AddPassArg("throwIfUnknown");
                        }
                        args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                        args.Add($"parent: {(obj.GetObjectType() == ObjectType.Mod ? "null" : "curContext")}");
                        args.Add($"getOrAddAsOverride: (m, r) => m.{field.Name}.GetOrAddAsOverride(linkCache.Resolve<{groupTargetGetter}>(r.FormKey))");
                        args.Add($"duplicateInto: (m, r, e, f) => m.{field.Name}.DuplicateInAsNewRecord(linkCache.Resolve<{groupTargetGetter}>(r.FormKey), e, f)");
                    }
                    using (fieldGen.CurlyBrace())
                    {
                        fieldGen.AppendLine("yield return item;");
                    }
                }
            }
        }
        else if (field is LoquiType loqui)
        {
            if (blackList?.Contains(loqui.TargetObjectGeneration) ?? false) return;
            var fieldAccessor = loqui.Nullable ? $"{obj.ObjectName}{loqui.Name}item" : $"{accessor}.{loqui.Name}";
            if (loqui.TargetObjectGeneration.IsListGroup())
            { // List groups 
                using (var args = fieldGen.Call(
                           $"foreach (var item in obj.{field.Name}.EnumerateMajorRecordContexts",
                           suffixLine: ")",
                           semiColon: false))
                {
                    args.AddPassArg("linkCache");
                    if (includeType)
                    {
                        args.AddPassArg("type");
                        args.AddPassArg("throwIfUnknown");
                    }
                    args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add($"parent: {(obj.GetObjectType() == ObjectType.Mod ? "null" : "curContext")}");
                }
                using (fieldGen.CurlyBrace())
                {
                    fieldGen.AppendLine("yield return item;");
                }
                return;
            }
            var subFg = new StructuredStringBuilder();
            await LoquiTypeHandler(
                subFg,
                obj,
                fieldAccessor,
                loqui,
                generic: "TMajor",
                includeType: includeType,
                checkType: false,
                includeSelf: includeSelf,
                addGetOrAddArg: (args) =>
                {
                    args.Add(subFg =>
                    {
                        subFg.AppendLine($"getOrAddAsOverride: (m, r) =>");
                        using (subFg.CurlyBrace())
                        {
                            subFg.AppendLine($"var baseRec = getOrAddAsOverride(m, linkCache.Resolve<{obj.Interface(getter: true)}>(obj.FormKey));");
                            subFg.AppendLine($"if (baseRec.{loqui.Name} != null) return baseRec.{loqui.Name};");
                            subFg.AppendLine($"var copy = r.DeepCopy(ModContextExt.{loqui.TargetObjectGeneration.Name}CopyMask);");
                            subFg.AppendLine($"baseRec.{loqui.Name} = copy;");
                            subFg.AppendLine($"return copy;");
                        }
                    });
                },
                duplicateInArg: (args) =>
                {
                    args.Add(subFg =>
                    {
                        subFg.AppendLine($"duplicateInto: (m, r, e, f) =>");
                        using (subFg.CurlyBrace())
                        {
                            subFg.AppendLine($"var baseRec = getOrAddAsOverride(m, linkCache.Resolve<{obj.Interface(getter: true)}>(obj.FormKey));");
                            subFg.AppendLine($"var dupRec = r.Duplicate(f ?? m.GetNextFormKey(e), ModContextExt.{loqui.TargetObjectGeneration.Name}CopyMask);");
                            subFg.AppendLine($"baseRec.{loqui.Name} = dupRec;");
                            subFg.AppendLine($"return dupRec;");
                        }
                    });
                });
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
                        if (includeType)
                        {
                            fieldGen.AppendLine($"if (type.IsAssignableFrom(typeof({contLoqui.GenericDef.Name})))");
                        }
                        using (fieldGen.CurlyBrace(doIt: includeType))
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
            else if (contLoqui.TargetObjectGeneration?.IsListGroup() ?? false)
            {
                using (var args = fieldGen.Call(
                           $"foreach (var item in {accessor}.{field.Name}.EnumerateMajorRecordContexts",
                           suffixLine: ")",
                           semiColon: false))
                {
                    if (includeType)
                    {
                        args.AddPassArg("type");
                    }
                    else
                    {
                        args.Add($"type: typeof({nameof(IMajorRecordGetter)})");
                    }
                    args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                    args.Add($"parent: {(obj.GetObjectType() == ObjectType.Mod ? "null" : "curContext")}");
                    args.AddPassArg("linkCache");
                    args.Add("throwIfUnknown: false");
                    args.Add("worldspace: obj");
                    args.AddPassArg("getOrAddAsOverride");
                }
                using (fieldGen.CurlyBrace())
                {
                    fieldGen.AppendLine("yield return item;");
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
                                await LoquiTypeHandler(
                                    fieldGen,
                                    obj,
                                    $"subItem",
                                    contLoqui,
                                    generic: "TMajor",
                                    includeType: includeType,
                                    checkType: includeType,
                                    addGetOrAddArg: (args) =>
                                    {
                                        args.Add(subFg =>
                                        {
                                            subFg.AppendLine($"getOrAddAsOverride: (m, r) =>");
                                            using (subFg.CurlyBrace())
                                            {
                                                subFg.AppendLine($"var parent = getOrAddAsOverride(m, linkCache.Resolve<{obj.Interface(getter: true)}>(obj.FormKey));");
                                                subFg.AppendLine($"var ret = parent.{cont.Name}.FirstOrDefault(x => x.FormKey == r.FormKey);");
                                                subFg.AppendLine("if (ret != null) return ret;");
                                                subFg.AppendLine($"ret = ({contLoqui.TypeName()})(({contLoqui.Interface(getter: true)})r).DeepCopy();");
                                                subFg.AppendLine($"parent.{cont.Name}.Add(ret);");
                                                subFg.AppendLine($"return ret;");
                                            }
                                        });
                                    },
                                    duplicateInArg: (args) =>
                                    {
                                        args.Add(subFg =>
                                        {
                                            subFg.AppendLine($"duplicateInto: (m, r, e, f) =>");
                                            using (subFg.CurlyBrace())
                                            {
                                                subFg.AppendLine($"var dup = ({contLoqui.TypeName()})(({contLoqui.Interface(getter: true)})r).Duplicate(f ?? m.GetNextFormKey(e));");
                                                subFg.AppendLine($"getOrAddAsOverride(m, linkCache.Resolve<{obj.Interface(getter: true)}>(obj.FormKey)).{cont.Name}.Add(dup);");
                                                subFg.AppendLine($"return dup;");
                                            }
                                        });
                                    });
                            }
                            break;
                        case Case.Maybe:
                            throw new NotImplementedException();
                        case Case.No:
                        default:
                            break;
                    }
                }
            }
        }
        else if (field is DictType dict)
        {
            throw new NotImplementedException();
        }
    }
}