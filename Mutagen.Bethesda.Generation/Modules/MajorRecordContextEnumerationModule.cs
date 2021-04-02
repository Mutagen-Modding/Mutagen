using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation.Modules
{
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

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            GenerateClassImplementation(obj, fg);
        }

        public static void GenerateClassImplementation(ObjectGeneration obj, FileGeneration fg, bool onlyGetter = false)
        {
            if (obj.GetObjectType() == ObjectType.Mod)
            {
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<IModContext<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}, TSetter, TGetter>> IMajorRecordContextEnumerable<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>.EnumerateMajorRecordContexts<TSetter, TGetter>({nameof(ILinkCache)} linkCache, bool throwIfUnknown) => this.EnumerateMajorRecordContexts{obj.GetGenericTypes(MaskType.Normal, "TSetter".AsEnumerable().And("TGetter").ToArray())}(linkCache, throwIfUnknown: throwIfUnknown);");
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine($"IEnumerable<IModContext<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}, IMajorRecordCommon, IMajorRecordCommonGetter>> IMajorRecordContextEnumerable<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>.EnumerateMajorRecordContexts({nameof(ILinkCache)} linkCache, Type type, bool throwIfUnknown) => this.EnumerateMajorRecordContexts(linkCache, type: type, throwIfUnknown: throwIfUnknown);");
            }
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            var needsCatch = obj.GetObjectType() == ObjectType.Mod;
            string catchLine = needsCatch ? ".Catch(e => throw RecordException.Enrich(e, obj.ModKey))" : string.Empty;
            string enderSemi = needsCatch ? string.Empty : ";";
            var modGetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: true);
            var modSetter = obj.GetObjectData().GameCategory.Value.ModInterface(getter: false);

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<IModContext<{modSetter}, {modGetter}, TSetter, TGetter>> EnumerateMajorRecordContexts{obj.GetGenericTypes(MaskType.Normal, new string[] { "TSetter", "TGetter" })}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
                args.Wheres.Add($"where TSetter : class, IMajorRecordCommon, TGetter");
                args.Wheres.Add($"where TGetter : class, IMajorRecordCommonGetter");
                args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
                args.Add($"{nameof(ILinkCache)} linkCache");
                args.Add($"bool throwIfUnknown = true");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new FunctionWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("linkCache");
                    args.Add("type: typeof(TGetter)");
                    args.AddPassArg("throwIfUnknown");
                }
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine($".Select(m => m.AsType<{modSetter}, {modGetter}, {nameof(IMajorRecordCommon)}, {nameof(IMajorRecordCommonGetter)}, TSetter, TGetter>()){enderSemi}");
                    if (needsCatch)
                    {
                        fg.AppendLine($"{catchLine};");
                    }
                }
            }
            fg.AppendLine();

            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<IModContext<{modSetter}, {modGetter}, IMajorRecordCommon, IMajorRecordCommonGetter>> EnumerateMajorRecordContexts{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
                args.Add($"{nameof(ILinkCache)} linkCache");
                args.Add($"Type type");
                args.Add($"bool throwIfUnknown = true");
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("linkCache");
                    args.AddPassArg("type");
                    args.AddPassArg("throwIfUnknown");
                }
            }
            fg.AppendLine();
        }

        private async Task LoquiTypeHandler(
            FileGeneration fg,
            ObjectGeneration obj,
            Accessor loquiAccessor,
            LoquiType loquiType,
            Action<ArgsWrapper> addGetOrAddArg,
            Action<ArgsWrapper> duplicateInArg,
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
                    fg.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
                }
                using (new BraceWrapper(fg, doIt: checkType))
                {
                    using (var args = new ArgsWrapper(fg,
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
                    fg.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
                }
                using (new BraceWrapper(fg, doIt: checkType))
                {
                    using (var args = new ArgsWrapper(fg,
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
                fg.AppendLine($"foreach (var item in {loquiAccessor}.EnumerateMajorRecords({(generic == null ? null : "type, throwIfUnknown: false")}))");
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
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
                using (var args = new ArgsWrapper(fg,
                    $"foreach (var item in {loquiType.TargetObjectGeneration.CommonClassInstance(loquiAccessor, LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateMajorRecordContexts",
                    suffixLine: ")")
                {
                    SemiColon = false
                })
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
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("yield return item;");
                }
            }
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
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

            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}IEnumerable<IModContext<{modSetter}, {modGetter}, IMajorRecordCommon, IMajorRecordCommonGetter>> EnumerateMajorRecordContexts"))
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
                    args.Add($"Func<{modSetter}, {obj.Interface(getter: true)}, string?, {obj.Interface(getter: false)}> duplicateInto");
                }
            }
            using (new BraceWrapper(fg))
            {
                if (obj.GetObjectType() == ObjectType.Record)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"var curContext = new ModContext<{modSetter}, {modGetter}, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                    {
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                        args.Add("record: obj");
                        args.AddPassArg("getOrAddAsOverride");
                        args.AddPassArg("duplicateInto");
                        args.AddPassArg("parent");
                    }
                }
                var fgCount = fg.Count;
                var gameCategory = obj.GetObjectData().GameCategory;
                Dictionary<object, FileGeneration> generationDict = new Dictionary<object, FileGeneration>();

                bool doAdditionlDeepLogic = obj.Name != "ListGroup";
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
                    FileGeneration fieldGen;
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
                        includeType: false,
                        hasTarget: typesWithDeepTargets.Any(f => f.Name == field.Name));
                }

                foreach (var kv in generationDict)
                {
                    fg.AppendLines(kv.Value);
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}IEnumerable<IModContext<{modSetter}, {modGetter}, IMajorRecordCommon, IMajorRecordCommonGetter>> EnumerateMajorRecordContexts"))
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
                    args.Add($"Func<{modSetter}, {obj.Interface(getter: true)}, string?, {obj.Interface(getter: false)}> duplicateInto");
                }
            }
            using (new BraceWrapper(fg))
            {
                if (obj.GetObjectType() == ObjectType.Record)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"var curContext = new ModContext<{modSetter}, {modGetter}, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                    {
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                        args.Add("record: obj");
                        args.AddPassArg("getOrAddAsOverride");
                        args.AddPassArg("duplicateInto");
                        args.AddPassArg("parent");
                    }
                }
                var fgCount = fg.Count;
                fg.AppendLine("switch (type.Name)");
                using (new BraceWrapper(fg))
                {
                    var gameCategory = obj.GetObjectData().GameCategory;
                    fg.AppendLine($"case \"{nameof(IMajorRecordCommon)}\":");
                    fg.AppendLine($"case \"{nameof(IMajorRecord)}\":");
                    fg.AppendLine($"case \"{nameof(MajorRecord)}\":");
                    if (gameCategory != null)
                    {
                        fg.AppendLine($"case \"I{gameCategory}MajorRecord\":");
                        fg.AppendLine($"case \"{gameCategory}MajorRecord\":");
                    }
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine($"if (!{obj.RegistrationName}.SetterType.IsAssignableFrom(obj.GetType())) yield break;");
                        using (var args = new ArgsWrapper(fg,
                            $"foreach (var item in this.EnumerateMajorRecordContexts",
                            suffixLine: ")")
                        {
                            SemiColon = false
                        })
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
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return item;");
                        }
                        fg.AppendLine("yield break;");
                    }
                    fg.AppendLine($"case \"{nameof(IMajorRecordGetter)}\":");
                    fg.AppendLine($"case \"{nameof(IMajorRecordCommonGetter)}\":");
                    if (gameCategory != null)
                    {
                        fg.AppendLine($"case \"I{gameCategory}MajorRecordGetter\":");
                    }
                    using (new DepthWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"foreach (var item in this.EnumerateMajorRecordContexts",
                            suffixLine: ")")
                        {
                            SemiColon = false
                        })
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

                    bool doAdditionlDeepLogic = obj.Name != "ListGroup";

                    if (doAdditionlDeepLogic)
                    {
                        var deepRecordMapping = await MajorRecordModule.FindDeepRecords(obj);
                        foreach (var deepRec in deepRecordMapping)
                        {
                            FileGeneration deepFg = generationDict.GetOrAdd(deepRec.Key);
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
                                        if (loqui.RefType == LoquiType.LoquiRefType.Interface)
                                        {
                                            blackList.Add(loqui.SetterInterface);
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

                        // Generate for major record marker interfaces 
                        if (LinkInterfaceModule.ObjectMappings.TryGetValue(obj.ProtoGen.Protocol, out var interfs))
                        {
                            foreach (var interf in interfs)
                            {
                                if (blackList.Contains(interf.Key)) continue;
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
                                        using (var args = new ArgsWrapper(subFg,
                                            $"foreach (var item in EnumerateMajorRecordContexts",
                                            suffixLine: ")")
                                        {
                                            SemiColon = false
                                        })
                                        {
                                            args.Add(accessor.ToString());
                                            args.AddPassArg("linkCache");
                                            args.Add($"type: typeof({grup.GetGroupTarget().Interface(getter: true)})");
                                            args.AddPassArg("throwIfUnknown");
                                        }
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
                                    await ApplyIterationLines(
                                        obj: obj,
                                        field: deepObj,
                                        fieldGen: subFg,
                                        accessor: accessor,
                                        getter: getter,
                                        blackList: passedObjects,
                                        hasTarget: true,
                                        includeSelf: false,
                                        includeType: true);
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
            fg.AppendLine();
        }

        async Task ApplyIterationLines(
            ObjectGeneration obj,
            TypeGeneration field,
            FileGeneration fieldGen,
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
                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name})");
                    using (new BraceWrapper(fieldGen))
                    {
                        using (var args = new ArgsWrapper(fieldGen,
                            $"yield return new GroupModContext<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}, {groupTargetSetter}, {groupTargetGetter}>"))
                        {
                            args.Add("modKey: obj.ModKey");
                            args.Add("record: item");
                            args.Add($"group: (m) => m.{group.Name}");
                            args.Add($"groupGetter: (m) => m.{group.Name}");
                        }
                    }
                }
                if (hasTarget)
                {
                    fieldGen.AppendLine($"foreach (var groupItem in obj.{field.Name})");
                    using (new BraceWrapper(fieldGen))
                    {
                        using (var args = new ArgsWrapper(fieldGen,
                            $"foreach (var item in {group.GetGroupTarget().CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecordContexts",
                            suffixLine: ")")
                        {
                            SemiColon = false
                        })
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
                            args.Add($"duplicateInto: (m, r, e) => m.{field.Name}.DuplicateInAsNewRecord(linkCache.Resolve<{groupTargetGetter}>(r.FormKey), e)");
                        }
                        using (new BraceWrapper(fieldGen))
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
                    using (var args = new ArgsWrapper(fieldGen,
                        $"foreach (var item in obj.{field.Name}.EnumerateMajorRecordContexts",
                        suffixLine: ")")
                    {
                        SemiColon = false
                    })
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
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine("yield return item;");
                    }
                    return;
                }
                var subFg = new FileGeneration();
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
                            using (new BraceWrapper(subFg))
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
                            subFg.AppendLine($"duplicateInto: (m, r, e) =>");
                            using (new BraceWrapper(subFg))
                            {
                                subFg.AppendLine($"var baseRec = getOrAddAsOverride(m, linkCache.Resolve<{obj.Interface(getter: true)}>(obj.FormKey));");
                                subFg.AppendLine($"var dupRec = r.Duplicate(m.GetNextFormKey(e), ModContextExt.{loqui.TargetObjectGeneration.Name}CopyMask);");
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
                            if (includeType)
                            {
                                fieldGen.AppendLine($"if (type.IsAssignableFrom(typeof({contLoqui.GenericDef.Name})))");
                            }
                            using (new BraceWrapper(fieldGen, doIt: includeType))
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
                else if (contLoqui.TargetObjectGeneration?.IsListGroup() ?? false)
                {
                    using (var args = new ArgsWrapper(fieldGen,
                        $"foreach (var item in {accessor}.{field.Name}.EnumerateMajorRecordContexts",
                        suffixLine: ")")
                    {
                        SemiColon = false
                    })
                    {
                        if (includeType)
                        {
                            args.AddPassArg("type");
                        }
                        else
                        {
                            args.Add($"type: typeof({nameof(IMajorRecordCommonGetter)})");
                        }
                        args.Add($"modKey: {(obj.GetObjectType() == ObjectType.Mod ? "obj.ModKey" : "modKey")}");
                        args.Add($"parent: {(obj.GetObjectType() == ObjectType.Mod ? "null" : "curContext")}");
                        args.AddPassArg("linkCache");
                        args.Add("throwIfUnknown: false");
                        args.Add("worldspace: obj");
                        args.AddPassArg("getOrAddAsOverride");
                    }
                    using (new BraceWrapper(fieldGen))
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
                                using (new BraceWrapper(fieldGen))
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
                                                using (new BraceWrapper(subFg))
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
                                                subFg.AppendLine($"duplicateInto: (m, r, e) =>");
                                                using (new BraceWrapper(subFg))
                                                {
                                                    subFg.AppendLine($"var dup = ({contLoqui.TypeName()})(({contLoqui.Interface(getter: true)})r).Duplicate(m.GetNextFormKey(e));");
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
}
