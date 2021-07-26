using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
    public class ModModule : GenerationModule
    {
        public const string GameReleaseOptions = "GameReleaseOptions";

        public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
        {
            if (obj.GetObjectData().ObjectType == ObjectType.Mod)
            {
                yield return "System.Collections.Concurrent";
                yield return "System.Threading.Tasks";
                yield return "System.IO";
                yield return "System.IO.Abstractions";
                yield return "Mutagen.Bethesda.Plugins.Masters";
            }
        }

        public override async IAsyncEnumerable<(LoquiInterfaceType Location, string Interface)> Interfaces(ObjectGeneration obj)
        {
            if (obj.GetObjectType() != ObjectType.Mod) yield break;
            yield return (LoquiInterfaceType.IGetter, nameof(IModGetter));
            yield return (LoquiInterfaceType.ISetter, nameof(IMod));
            yield return (LoquiInterfaceType.IGetter, $"IContextGetterMod<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>");
            yield return (LoquiInterfaceType.ISetter, $"IContextMod<{obj.Interface(getter: false)}, {obj.Interface(getter: true)}>");
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectData().ObjectType != ObjectType.Mod) return;
            var objData = obj.GetObjectData();

            // Game release member
            if (objData.GameReleaseOptions != null)
            {
                fg.AppendLine($"public {ReleaseEnumName(obj)} {ReleaseEnumName(obj)} {{ get; }}");
                fg.AppendLine($"public override {nameof(GameRelease)} GameRelease => {ReleaseEnumName(obj)}.ToGameRelease();");
            }
            else
            {
                fg.AppendLine($"public override {nameof(GameRelease)} GameRelease => {nameof(GameRelease)}.{obj.GetObjectData().GameCategory};");
            }

            // Interfaces
            fg.AppendLine($"IGroupCommonGetter<T> {nameof(IModGetter)}.{nameof(IModGetter.GetTopLevelGroup)}<T>() => this.{nameof(IModGetter.GetTopLevelGroup)}<T>();");
            fg.AppendLine($"IGroupCommonGetter<IMajorRecordCommonGetter> {nameof(IModGetter)}.{nameof(IModGetter.GetTopLevelGroup)}(Type type) => this.{nameof(IModGetter.GetTopLevelGroup)}(type);");
            fg.AppendLine($"IGroupCommon<T> {nameof(IMod)}.{nameof(IMod.GetTopLevelGroup)}<T>() => this.{nameof(IMod.GetTopLevelGroup)}<T>();");
            fg.AppendLine($"void IModGetter.WriteToBinary({nameof(FilePath)} path, {nameof(BinaryWriteParameters)}? param, IFileSystem? fileSystem) => this.WriteToBinary(path, importMask: null, param: param, fileSystem: fileSystem);");
            fg.AppendLine($"void IModGetter.WriteToBinaryParallel({nameof(FilePath)} path, {nameof(BinaryWriteParameters)}? param, IFileSystem? fileSystem) => this.WriteToBinaryParallel(path, param, fileSystem: fileSystem);");
            fg.AppendLine($"IMask<bool> {nameof(IEqualsMask)}.{nameof(IEqualsMask.GetEqualsMask)}(object rhs, EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures) => {obj.MixInClassName}.GetEqualsMask(this, ({obj.Interface(getter: true, internalInterface: true)})rhs, include);");

            // Localization enabled member
            if (obj.GetObjectData().UsesStringFiles)
            {
                fg.AppendLine($"public override bool CanUseLocalization => true;");
                fg.AppendLine($"public override bool UsingLocalization");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"get => this.ModHeader.Flags.HasFlag({obj.GetObjectData().GameCategory}ModHeader.HeaderFlag.Localized);");
                    fg.AppendLine($"set => this.ModHeader.Flags.SetFlag({obj.GetObjectData().GameCategory}ModHeader.HeaderFlag.Localized, value);");
                }
            }
            else
            {
                fg.AppendLine($"public override bool CanUseLocalization => false;");
                fg.AppendLine($"public override bool UsingLocalization");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("get => false;");
                    fg.AppendLine("set => throw new ArgumentException(\"Tried to set localization flag on unsupported mod type\");");
                }
            }

            // Master references member
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;");
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;");

            // NextObjectID member
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"uint IMod.NextFormID");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"get => this.ModHeader.Stats.NextFormID;");
                fg.AppendLine($"set => this.ModHeader.Stats.NextFormID = value;");
            }
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"uint IModGetter.NextFormID => this.ModHeader.Stats.NextFormID;");

            using (var args = new FunctionWrapper(fg,
                $"public {obj.Name}"))
            {
                args.Add($"{nameof(ModKey)} modKey");
                if (objData.GameReleaseOptions != null)
                {
                    args.Add($"{ReleaseEnumName(obj)} release");
                }
            }
            using (new DepthWrapper(fg))
            {
                fg.AppendLine(": base(modKey)");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.ModHeader.Stats.NextFormID = GetDefaultInitialNextFormID();");
                if (objData.GameReleaseOptions != null)
                {
                    fg.AppendLine($"this.{ReleaseEnumName(obj)} = release;");
                }
                await obj.GenerateInitializer(fg);
                fg.AppendLine($"CustomCtor();");
            }

            using (var args = new FunctionWrapper(fg,
                "public void AddRecords"))
            {
                args.Add($"{obj.Name} rhsMod");
                args.Add($"GroupMask? mask = null");
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
                            fg.AppendLine($"this.{field.Name}.RecordCache.Set(rhsMod.{field.Name}.RecordCache.Items);");
                        }
                        else
                        {
                            fg.AppendLine($"if (rhsMod.{field.Name}.Records.Count > 0)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("throw new NotImplementedException(\"Cell additions need implementing\");");
                            }
                        }
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public override void SyncRecordCount"))
            {
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.ModHeader.Stats.NumRecords = GetRecordCount();");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public uint GetRecordCount"))
            {
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("uint count = (uint)this.EnumerateMajorRecords().Count();");
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    if (loqui.TargetObjectGeneration.Name == "ListGroup")
                    {
                        fg.AppendLine($"count += {field.Name}.Records.Count > 0 ? 1 : default(uint);");
                    }
                    else
                    {
                        fg.AppendLine($"count += {field.Name}.RecordCache.Count > 0 ? 1 : default(uint);");
                    }
                }
                fg.AppendLine("GetCustomRecordCount((customCount) => count += customCount);");
                fg.AppendLine("return count;");
            }
            fg.AppendLine();

            fg.AppendLine("partial void GetCustomRecordCount(Action<uint> setter);");
            fg.AppendLine();

            await base.GenerateInClass(obj, fg);
        }

        public override async Task GenerateInCtor(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            await base.GenerateInCtor(obj, fg);
        }

        public static string ReleaseEnumName(ObjectGeneration obj)
        {
            return $"{ModName(obj)}Release";
        }

        public static string ModName(ObjectGeneration obj)
        {
            return obj.Name.TrimEnd("Mod");
        }

        public override async Task GenerateInVoid(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            using (new NamespaceWrapper(fg, obj.Namespace))
            {
                var objData = obj.GetObjectData();
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
                fg.AppendLine();

                fg.AppendLine($"public interface I{obj.Name}DisposableGetter : {obj.Interface(getter: true, internalInterface: true)}, IModDisposeGetter");
                using (new BraceWrapper(fg))
                {
                }
                fg.AppendLine();

                if (objData.GameReleaseOptions != null)
                {
                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Different game release versions a {ModName(obj)} mod can have");
                    }
                    fg.AppendLine($"public enum {ReleaseEnumName(obj)}");
                    using (new BraceWrapper(fg))
                    {
                        using (var comma = new CommaWrapper(fg))
                        {
                            foreach (var opt in objData.GameReleaseOptions)
                            {
                                comma.Add($"{opt} = {(int)opt}");
                            }
                        }
                    }
                    fg.AppendLine();

                    using (var c = new ClassWrapper(fg, $"{ReleaseEnumName(obj)}Ext"))
                    {
                        c.Static = true;
                    }
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new FunctionWrapper(fg,
                            $"public static {nameof(GameRelease)} ToGameRelease"))
                        {
                            args.Add($"this {ReleaseEnumName(obj)} release");
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("return release switch");
                            using (new BraceWrapper(fg) { AppendSemicolon = true })
                            {
                                using (var comma = new CommaWrapper(fg))
                                {
                                    foreach (var item in objData.GameReleaseOptions)
                                    {
                                        comma.Add($"{ReleaseEnumName(obj)}.{item} => {nameof(GameRelease)}.{item}");
                                    }
                                    comma.Add("_ => throw new ArgumentException()");
                                }
                            }
                        }
                        fg.AppendLine();

                        using (var args = new FunctionWrapper(fg,
                            $"public static {ReleaseEnumName(obj)} To{ReleaseEnumName(obj)}"))
                        {
                            args.Add($"this {nameof(GameRelease)} release");
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("return release switch");
                            using (new BraceWrapper(fg) { AppendSemicolon = true })
                            {
                                using (var comma = new CommaWrapper(fg))
                                {
                                    foreach (var item in objData.GameReleaseOptions)
                                    {
                                        comma.Add($"{nameof(GameRelease)}.{item} => {ReleaseEnumName(obj)}.{item}");
                                    }
                                    comma.Add("_ => throw new ArgumentException()");
                                }
                            }
                        }
                    }
                }
            }
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInCommonMixin(obj, fg);
            var objData = obj.GetObjectData();
            string gameReleaseStr;
            if (objData.GameReleaseOptions == null)
            {
                gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
            }
            else
            {
                gameReleaseStr = $"item.GameRelease";
            }

            if (obj.GetObjectType() != ObjectType.Mod) return;
            using (var args = new FunctionWrapper(fg,
                $"public static IGroupCommonGetter<T> {nameof(IModGetter.GetTopLevelGroup)}<T>"))
            {
                args.Wheres.Add($"where T : {nameof(IMajorRecordCommonGetter)}");
                args.Add($"this {obj.Interface(getter: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return (IGroupCommonGetter<T>){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
                {
                    args.AddPassArg("obj");
                    args.Add("type: typeof(T)");
                }
            }
            fg.AppendLine();
            
            using (var args = new FunctionWrapper(fg,
                $"public static IGroupCommonGetter<{nameof(IMajorRecordCommonGetter)}> {nameof(IModGetter.GetTopLevelGroup)}"))
            {
                args.Add($"this {obj.Interface(getter: true)} obj");
                args.Add("Type type");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return (IGroupCommonGetter<{nameof(IMajorRecordCommonGetter)}>){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
                {
                    args.AddPassArg("obj");
                    args.AddPassArg("type");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static IGroupCommon<T> {nameof(IMod.GetTopLevelGroup)}<T>"))
            {
                args.Wheres.Add($"where T : {nameof(IMajorRecordCommon)}");
                args.Add($"this {obj.Interface(getter: false)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return (IGroupCommon<T>){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
                {
                    args.AddPassArg("obj");
                    args.Add("type: typeof(T)");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static void WriteToBinaryParallel"))
            {
                args.Add($"this {obj.Interface(getter: true, internalInterface: false)} item");
                args.Add($"Stream stream");
                args.Add($"{nameof(BinaryWriteParameters)}? param = null");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.WriteParallel"))
                {
                    args.AddPassArg("item");
                    args.AddPassArg("stream");
                    args.Add($"param: param ?? {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)}");
                    args.Add("modKey: item.ModKey");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static void WriteToBinaryParallel"))
            {
                args.Add($"this {obj.Interface(getter: true, internalInterface: false)} item");
                args.Add($"string path");
                args.Add($"{nameof(BinaryWriteParameters)}? param = null");
                args.Add($"{nameof(IFileSystem)}? fileSystem = null");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"param ??= {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)};");
                using (var args = new ArgsWrapper(fg,
                    $"var modKey = param.{nameof(BinaryWriteParameters.RunMasterMatch)}"))
                {
                    args.Add("mod: item");
                    args.AddPassArg("path");
                }
                if (obj.GetObjectData().UsesStringFiles)
                {
                    fg.AppendLine("bool disposeStrings = param.StringsWriter == null;");
                    fg.AppendLine($"param.StringsWriter ??= EnumExt.HasFlag((int)item.ModHeader.Flags, (int)ModHeaderCommonFlag.Localized) ? new StringsWriter({gameReleaseStr}, modKey, Path.Combine(Path.GetDirectoryName(path)!, \"Strings\")) : null;");
                }
                fg.AppendLine("using (var stream = fileSystem.GetOrDefault().FileStream.Create(path, FileMode.Create, FileAccess.Write))");
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.WriteParallel"))
                    {
                        args.AddPassArg("item");
                        args.AddPassArg("stream");
                        args.Add($"param: param");
                        args.AddPassArg("modKey");
                    }
                }
                if (obj.GetObjectData().UsesStringFiles)
                {
                    fg.AppendLine("if (disposeStrings)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("param.StringsWriter?.Dispose();");
                    }
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            await base.GenerateInCommon(obj, fg, maskTypes);
            if (obj.GetObjectType() != ObjectType.Mod) return;
            if (!maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)) return;

            GenerateGetGroup(obj, fg);
            GenerateWriteParallel(obj, fg);
        }

        private void GenerateGetGroup(ObjectGeneration obj, FileGeneration fg)
        {
            using (var args = new FunctionWrapper(fg,
                "public object GetGroup"))
            {
                args.Add($"{obj.Interface(getter: true)} obj");
                args.Add("Type type");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("switch (type.Name)");
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
                        fg.AppendLine($"case \"{subObj.Name}\":");
                        fg.AppendLine($"case \"{subObj.Interface(getter: true)}\":");
                        fg.AppendLine($"case \"{subObj.Interface(getter: false)}\":");
                        if (subObj.HasInternalGetInterface)
                        {
                            fg.AppendLine($"case \"{subObj.Interface(getter: true, internalInterface: true)}\":");
                        }
                        if (subObj.HasInternalSetInterface)
                        {
                            fg.AppendLine($"case \"{subObj.Interface(getter: false, internalInterface: true)}\":");
                        }
                        using (new DepthWrapper(fg))
                        {
                            if (loqui.TargetObjectGeneration.Name == "ListGroup")
                            {
                                fg.AppendLine($"return obj.{field.Name}.Records;");
                            }
                            else
                            {
                                fg.AppendLine($"return obj.{field.Name};");
                            }
                        }
                    }
                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine("throw new ArgumentException($\"Unknown major record type: {type}\");");
                    }
                }
            }
            fg.AppendLine();
        }

        private void GenerateWriteParallel(ObjectGeneration obj, FileGeneration fg)
        {
            LoquiType groupInstance = null;
            LoquiType listGroupInstance = null;
            var objData = obj.GetObjectData();
            fg.AppendLine("const int CutCount = 100;");
            using (var args = new FunctionWrapper(fg,
                "public static void WriteParallel"))
            {
                args.Add($"{obj.Interface(getter: true, internalInterface: false)} item");
                args.Add($"Stream stream");
                args.Add($"{nameof(BinaryWriteParameters)} param");
                args.Add($"ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                string gameConstantsStr;
                if (objData.GameReleaseOptions == null)
                {
                    gameConstantsStr = $"{nameof(GameConstants)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    fg.AppendLine($"var gameConstants = {nameof(GameConstants)}.Get(item.{ReleaseEnumName(obj)}.ToGameRelease());");
                    gameConstantsStr = $"gameConstants";
                }
                fg.AppendLine($"var bundle = new {nameof(WritingBundle)}({gameConstantsStr});");
                fg.AppendLine($"var writer = new MutagenWriter(stream, bundle);");
                using (var args = new ArgsWrapper(fg,
                    $"{nameof(ModHeaderWriteLogic)}.{nameof(ModHeaderWriteLogic.WriteHeader)}"))
                {
                    args.AddPassArg("param");
                    args.AddPassArg("writer");
                    args.Add("mod: item");
                    args.Add("modHeader: item.ModHeader.DeepCopy()");
                    args.AddPassArg("modKey");
                }

                int groupCount = obj.IterateFields()
                    .Select(f => f as LoquiType)
                    .Where(l => l != null)
                    .Where(l => l.TargetObjectGeneration?.GetObjectData().ObjectType == ObjectType.Group)
                    .Count();

                fg.AppendLine($"Stream[] outputStreams = new Stream[{groupCount}];");
                fg.AppendLine($"List<Action> toDo = new List<Action>();");
                int i = 0;
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                    if (loqui.TargetObjectGeneration.Name == "ListGroup")
                    {
                        listGroupInstance = loqui;
                    }
                    else
                    {
                        groupInstance = loqui;
                    }
                    if (loqui.GetGroupTarget().GetObjectData().CustomBinaryEnd == CustomEnd.Off
                        && loqui.TargetObjectGeneration.Name != "ListGroup")
                    {
                        fg.AppendLine($"toDo.Add(() => WriteGroupParallel(item.{field.Name}, writer.MetaData.MasterReferences!, {i}{(objData.GameReleaseOptions == null ? null : ", gameConstants")}, outputStreams{(objData.UsesStringFiles ? ", param.StringsWriter" : null)}));");
                    }
                    else
                    {
                        fg.AppendLine($"toDo.Add(() => Write{field.Name}Parallel(item.{field.Name}, writer.MetaData.MasterReferences!, {i}{(objData.GameReleaseOptions == null ? null : ", gameConstants")}, outputStreams));");
                    }
                    i++;
                }
                fg.AppendLine("Parallel.Invoke(toDo.ToArray());");
                using (var args = new ArgsWrapper(fg,
                    $"{nameof(PluginUtilityTranslation)}.{nameof(PluginUtilityTranslation.CompileStreamsInto)}"))
                {
                    args.Add("outputStreams.NotNull()");
                    args.Add("stream");
                }
            }
            fg.AppendLine();

            if (groupInstance != null)
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void WriteGroupParallel<T>"))
                {
                    args.Add("IGroupGetter<T> group");
                    args.Add($"{nameof(IMasterReferenceReader)} masters");
                    args.Add("int targetIndex");
                    if (objData.GameReleaseOptions != null)
                    {
                        args.Add($"{nameof(GameConstants)} gameConstants");
                    }
                    args.Add("Stream[] streamDepositArray");
                    if (objData.UsesStringFiles)
                    {
                        args.Add($"{nameof(StringsWriter)}? stringsWriter");
                    }
                    args.Wheres.AddRange(groupInstance.TargetObjectGeneration.GenerateWhereClauses(LoquiInterfaceType.IGetter, groupInstance.TargetObjectGeneration.Generics));
                }
                using (new BraceWrapper(fg))
                {
                    string gameConstantsStr;
                    if (objData.GameReleaseOptions == null)
                    {
                        gameConstantsStr = $"{nameof(GameConstants)}.{obj.GetObjectData().GameCategory}";
                    }
                    else
                    {
                        gameConstantsStr = "gameConstants";
                    }
                    fg.AppendLine("if (group.RecordCache.Count == 0) return;");
                    fg.AppendLine($"var cuts = group.Cut(CutCount).ToArray();");
                    fg.AppendLine($"Stream[] subStreams = new Stream[cuts.Length + 1];");
                    fg.AppendLine($"byte[] groupBytes = new byte[{gameConstantsStr}.GroupConstants.HeaderLength];");
                    fg.AppendLine($"BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);");
                    fg.AppendLine($"var groupByteStream = new MemoryStream(groupBytes);");
                    fg.AppendLine($"using (var stream = new MutagenWriter(groupByteStream, {gameConstantsStr}, dispose: false))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"stream.Position += 8;");
                        fg.AppendLine($"GroupBinaryWriteTranslation.WriteEmbedded<T>(group, stream);");
                    }
                    fg.AppendLine($"subStreams[0] = groupByteStream;");
                    fg.AppendLine($"Parallel.ForEach(cuts, (cutItems, state, counter) =>");
                    using (new BraceWrapper(fg) { AppendSemicolon = true, AppendParenthesis = true })
                    {
                        fg.AppendLine($"{nameof(MemoryTributary)} trib = new {nameof(MemoryTributary)}();");
                        fg.AppendLine($"var bundle = new {nameof(WritingBundle)}({gameConstantsStr})");
                        using (var prop = new PropertyCtorWrapper(fg))
                        {
                            prop.Add($"{nameof(WritingBundle.MasterReferences)} = masters");
                            if (objData.UsesStringFiles)
                            {
                                prop.Add($"{nameof(WritingBundle.StringsWriter)} = stringsWriter");
                            }
                        }
                        fg.AppendLine($"using (var stream = new MutagenWriter(trib, bundle, dispose: false))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"foreach (var item in cutItems)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"item.WriteToBinary(stream);");
                            }
                        }
                        fg.AppendLine($"subStreams[(int)counter + 1] = trib;");
                    }
                    fg.AppendLine($"{nameof(PluginUtilityTranslation)}.CompileSetGroupLength(subStreams, groupBytes);");
                    fg.AppendLine($"streamDepositArray[targetIndex] = new CompositeReadStream(subStreams, resetPositions: true);");
                }
                fg.AppendLine();
            }
        }

        public override async Task PreLoad(ObjectGeneration obj)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            var elems = obj.Node.Elements(XName.Get(GameReleaseOptions, LoquiGenerator.Namespace));
            if (!elems.Any()) return;
            var objData = obj.GetObjectData();
            objData.GameReleaseOptions = elems.Select(el => Enum.Parse<GameRelease>(el.Value)).ToHashSet();
            obj.Interfaces.Add(LoquiInterfaceDefinitionType.IGetter, $"IMajorRecordContextEnumerable<{obj.Interface(getter: false, internalInterface: true)}, {obj.Interface(getter: true, internalInterface: true)}>");
        }

        public override async Task GenerateInInterface(ObjectGeneration obj, FileGeneration fg, bool internalInterface, bool getter)
        {
            await base.GenerateInInterface(obj, fg, internalInterface, getter);
            if (obj.GetObjectType() != ObjectType.Mod) return;
            if (!getter) return;
            if (obj.GetObjectData().GameReleaseOptions == null) return;
            fg.AppendLine($"{ReleaseEnumName(obj)} {ReleaseEnumName(obj)} {{ get; }}");
        }
    }
}
