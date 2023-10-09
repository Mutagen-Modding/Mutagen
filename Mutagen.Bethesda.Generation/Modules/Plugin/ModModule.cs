using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.IO.Abstractions;
using System.Xml.Linq;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records.Loqui;
using Mutagen.Bethesda.Strings.DI;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

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
            yield return "Mutagen.Bethesda.Plugins.Records.Loqui";
            yield return "Mutagen.Bethesda.Strings.DI";
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

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectData().ObjectType != ObjectType.Mod) return;
        var objData = obj.GetObjectData();

        // Game release member
        if (objData.GameReleaseOptions != null)
        {
            sb.AppendLine($"public {ReleaseEnumName(obj)} {ReleaseEnumName(obj)} {{ get; }}");
            sb.AppendLine($"public override {nameof(GameRelease)} GameRelease => {ReleaseEnumName(obj)}.ToGameRelease();");
        }
        else
        {
            sb.AppendLine($"public override {nameof(GameRelease)} GameRelease => {nameof(GameRelease)}.{obj.GetObjectData().GameCategory};");
        }

        // Interfaces
        sb.AppendLine($"IGroupGetter<T>? {nameof(IModGetter)}.{nameof(IModGetter.TryGetTopLevelGroup)}<T>() => this.{nameof(IModGetter.TryGetTopLevelGroup)}<T>();");
        sb.AppendLine($"IGroupGetter? {nameof(IModGetter)}.{nameof(IModGetter.TryGetTopLevelGroup)}(Type type) => this.{nameof(IModGetter.TryGetTopLevelGroup)}(type);");
        sb.AppendLine($"IGroup<T>? {nameof(IMod)}.{nameof(IMod.TryGetTopLevelGroup)}<T>() => this.{nameof(IMod.TryGetTopLevelGroup)}<T>();");
        sb.AppendLine($"IGroup? {nameof(IMod)}.{nameof(IMod.TryGetTopLevelGroup)}(Type type) => this.{nameof(IMod.TryGetTopLevelGroup)}(type);");
        sb.AppendLine($"void IModGetter.WriteToBinary({nameof(FilePath)} path, {nameof(BinaryWriteParameters)}? param, IFileSystem? fileSystem) => this.WriteToBinary(path, importMask: null, param: param, fileSystem: fileSystem);");
        sb.AppendLine($"void IModGetter.WriteToBinaryParallel({nameof(FilePath)} path, {nameof(BinaryWriteParameters)}? param, IFileSystem? fileSystem, {nameof(ParallelWriteParameters)}? parallelWriteParams) => this.WriteToBinaryParallel(path, param, fileSystem: fileSystem, parallelParam: parallelWriteParams);");
        sb.AppendLine($"void IModGetter.WriteToBinary({nameof(Stream)} stream, {nameof(BinaryWriteParameters)}? param) => this.WriteToBinary(stream, importMask: null, param: param);");
        sb.AppendLine($"void IModGetter.WriteToBinaryParallel({nameof(Stream)} stream, {nameof(BinaryWriteParameters)}? param, {nameof(ParallelWriteParameters)}? parallelWriteParams) => this.WriteToBinaryParallel(stream, param, parallelParam: parallelWriteParams);");
        sb.AppendLine($"IMask<bool> {nameof(IEqualsMask)}.{nameof(IEqualsMask.GetEqualsMask)}(object rhs, EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures) => {obj.MixInClassName}.GetEqualsMask(this, ({obj.Interface(getter: true, internalInterface: true)})rhs, include);");

        // Localization enabled member
        if (obj.GetObjectData().UsesStringFiles)
        {
            sb.AppendLine($"public override bool CanUseLocalization => true;");
            sb.AppendLine($"public override bool UsingLocalization");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"get => this.ModHeader.Flags.HasFlag({obj.GetObjectData().GameCategory}ModHeader.HeaderFlag.Localized);");
                sb.AppendLine($"set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag({obj.GetObjectData().GameCategory}ModHeader.HeaderFlag.Localized, value);");
            }
        }
        else
        {
            sb.AppendLine($"public override bool CanUseLocalization => false;");
            sb.AppendLine($"public override bool UsingLocalization");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("get => false;");
                sb.AppendLine("set => throw new ArgumentException(\"Tried to set localization flag on unsupported mod type\");");
            }
        }

        // Master references member
        sb.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
        sb.AppendLine($"IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;");
        sb.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
        sb.AppendLine($"IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;");

        // NextObjectID member
        sb.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
        sb.AppendLine($"uint IMod.NextFormID");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"get => this.ModHeader.Stats.NextFormID;");
            sb.AppendLine($"set => this.ModHeader.Stats.NextFormID = value;");
        }
        sb.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
        sb.AppendLine($"uint IModGetter.NextFormID => this.ModHeader.Stats.NextFormID;");

        using (var args = sb.Function(
                   $"public {obj.Name}"))
        {
            args.Add($"{nameof(ModKey)} modKey");
            if (objData.GameReleaseOptions != null)
            {
                args.Add($"{ReleaseEnumName(obj)} release");
            }
        }
        using (sb.IncreaseDepth())
        {
            sb.AppendLine(": base(modKey)");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("this.ModHeader.Stats.NextFormID = GetDefaultInitialNextFormID();");
            if (objData.GameReleaseOptions != null)
            {
                sb.AppendLine($"this.{ReleaseEnumName(obj)} = release;");
            }
            await obj.GenerateInitializer(sb);
            sb.AppendLine($"CustomCtor();");
        }

        using (var args = sb.Function(
                   "public void AddRecords"))
        {
            args.Add($"{obj.Name} rhsMod");
            args.Add($"GroupMask? mask = null");
        }
        using (sb.CurlyBrace())
        {
            foreach (var field in obj.IterateFields())
            {
                if (!(field is LoquiType loqui)) continue;
                if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                sb.AppendLine($"if (mask?.{field.Name} ?? true)");
                using (sb.CurlyBrace())
                {
                    if (loqui.TargetObjectGeneration.Name == $"{obj.ProtoGen.Protocol.Namespace}Group")
                    {
                        sb.AppendLine($"this.{field.Name}.RecordCache.Set(rhsMod.{field.Name}.RecordCache.Items);");
                    }
                    else
                    {
                        sb.AppendLine($"if (rhsMod.{field.Name}.Records.Count > 0)");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("throw new NotImplementedException(\"Cell additions need implementing\");");
                        }
                    }
                }
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   "public override void SyncRecordCount"))
        {
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("this.ModHeader.Stats.NumRecords = GetRecordCount();");
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   "public uint GetRecordCount"))
        {
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("uint count = (uint)this.EnumerateMajorRecords().Count();");
            foreach (var field in obj.IterateFields())
            {
                if (field is not LoquiType loqui) continue;
                if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                if (loqui.TargetObjectGeneration.Name.EndsWith("ListGroup"))
                {
                    sb.AppendLine($"count += {field.Name}.Records.Count > 0 ? 1 : default(uint);");
                }
                else
                {
                    sb.AppendLine($"count += {field.Name}.RecordCache.Count > 0 ? 1 : default(uint);");
                }
            }
            sb.AppendLine("GetCustomRecordCount((customCount) => count += customCount);");
            sb.AppendLine("return count;");
        }
        sb.AppendLine();

        sb.AppendLine("partial void GetCustomRecordCount(Action<uint> setter);");
        sb.AppendLine();

        await base.GenerateInClass(obj, sb);
    }

    public override async Task GenerateInCtor(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectType() != ObjectType.Mod) return;
        await base.GenerateInCtor(obj, sb);
    }

    public static string ReleaseEnumName(ObjectGeneration obj)
    {
        return $"{ModName(obj)}Release";
    }

    public static string ModName(ObjectGeneration obj)
    {
        return obj.Name.TrimEnd("Mod");
    }

    public override async Task GenerateInVoid(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectType() != ObjectType.Mod) return;
        using (sb.Namespace(obj.Namespace, fileScoped: false))
        {
            var objData = obj.GetObjectData();
            sb.AppendLine("public class GroupMask");
            using (sb.CurlyBrace())
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration == null) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    sb.AppendLine($"public bool {loqui.Name};");
                }

                sb.AppendLine("public GroupMask()");
                using (sb.CurlyBrace())
                {
                }

                sb.AppendLine("public GroupMask(bool defaultValue)");
                using (sb.CurlyBrace())
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui)) continue;
                        if (loqui.TargetObjectGeneration == null) continue;
                        if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                        sb.AppendLine($"{loqui.Name} = defaultValue;");
                    }
                }
            }
            sb.AppendLine();

            sb.AppendLine($"public interface I{obj.Name}DisposableGetter : {obj.Interface(getter: true, internalInterface: true)}, IModDisposeGetter");
            using (sb.CurlyBrace())
            {
            }
            sb.AppendLine();

            if (objData.GameReleaseOptions != null)
            {
                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Different game release versions a {ModName(obj)} mod can have");
                }
                sb.AppendLine($"public enum {ReleaseEnumName(obj)}");
                using (sb.CurlyBrace())
                {
                    using (var comma = sb.CommaCollection())
                    {
                        foreach (var opt in objData.GameReleaseOptions)
                        {
                            comma.Add($"{opt} = {(int)opt}");
                        }
                    }
                }
                sb.AppendLine();

                using (var c = sb.Class($"{ReleaseEnumName(obj)}Ext"))
                {
                    c.Static = true;
                }
                using (sb.CurlyBrace())
                {
                    using (var args = sb.Function(
                               $"public static {nameof(GameRelease)} ToGameRelease"))
                    {
                        args.Add($"this {ReleaseEnumName(obj)} release");
                    }
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine("return release switch");
                        using (sb.CurlyBrace(appendSemiColon: true))
                        {
                            using (var comma = sb.CommaCollection())
                            {
                                foreach (var item in objData.GameReleaseOptions)
                                {
                                    comma.Add($"{ReleaseEnumName(obj)}.{item} => {nameof(GameRelease)}.{item}");
                                }
                                comma.Add("_ => throw new ArgumentException()");
                            }
                        }
                    }
                    sb.AppendLine();

                    using (var args = sb.Function(
                               $"public static {ReleaseEnumName(obj)} To{ReleaseEnumName(obj)}"))
                    {
                        args.Add($"this {nameof(GameRelease)} release");
                    }
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine("return release switch");
                        using (sb.CurlyBrace(appendSemiColon: true))
                        {
                            using (var comma = sb.CommaCollection())
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
            
        GenerateModGameCategoryRegistration(obj, sb);
    }

    public void GenerateModGameCategoryRegistration(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        using (var ns = sb.Namespace(obj.ProtoGen.DefaultNamespace, fileScoped: false))
        {
            using (var c = sb.Class($"{obj.Name}_Registration"))
            {
                c.AccessModifier = AccessModifier.Internal;
                c.Partial = true;
                c.Interfaces.Add(nameof(IModRegistration));
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine($"public {nameof(GameCategory)} GameCategory => {nameof(GameCategory)}.{obj.GetObjectData().GameCategory};");
            }
            sb.AppendLine();
        }
    }

    public override async Task GenerateInCommonMixin(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInCommonMixin(obj, sb);
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
        using (var args = sb.Function(
                   $"public static IGroupGetter<T>? {nameof(IModGetter.TryGetTopLevelGroup)}<T>"))
        {
            args.Wheres.Add($"where T : {nameof(IMajorRecordGetter)}");
            args.Add($"this {obj.Interface(getter: true)} obj");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return (IGroupGetter<T>?){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
            {
                args.AddPassArg("obj");
                args.Add("type: typeof(T)");
            }
        }
        sb.AppendLine();
            
        using (var args = sb.Function(
                   $"public static IGroupGetter? {nameof(IModGetter.TryGetTopLevelGroup)}"))
        {
            args.Add($"this {obj.Interface(getter: true)} obj");
            args.Add("Type type");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return (IGroupGetter?){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
            {
                args.AddPassArg("obj");
                args.AddPassArg("type");
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public static IGroup<T>? {nameof(IMod.TryGetTopLevelGroup)}<T>"))
        {
            args.Wheres.Add($"where T : {nameof(IMajorRecord)}");
            args.Add($"this {obj.Interface(getter: false)} obj");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return (IGroup<T>?){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
            {
                args.AddPassArg("obj");
                args.Add("type: typeof(T)");
            }
        }
        sb.AppendLine();
            
        using (var args = sb.Function(
                   $"public static IGroup? {nameof(IModGetter.TryGetTopLevelGroup)}"))
        {
            args.Add($"this {obj.Interface(getter: false)} obj");
            args.Add("Type type");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return (IGroup?){obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.GetGroup"))
            {
                args.AddPassArg("obj");
                args.AddPassArg("type");
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public static void WriteToBinaryParallel"))
        {
            args.Add($"this {obj.Interface(getter: true, internalInterface: false)} item");
            args.Add($"Stream stream");
            args.Add($"{nameof(BinaryWriteParameters)}? param = null");
            args.Add($"{nameof(ParallelWriteParameters)}? parallelParam = null");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"{obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.WriteParallel"))
            {
                args.AddPassArg("item");
                args.AddPassArg("stream");
                args.Add($"parallelParam: parallelParam ?? {nameof(ParallelWriteParameters)}.{nameof(ParallelWriteParameters.Default)}");
                args.Add($"param: param ?? {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)}");
                args.Add("modKey: item.ModKey");
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public static void WriteToBinaryParallel"))
        {
            args.Add($"this {obj.Interface(getter: true, internalInterface: false)} item");
            args.Add($"string path");
            args.Add($"{nameof(BinaryWriteParameters)}? param = null");
            args.Add($"{nameof(ParallelWriteParameters)}? parallelParam = null");
            args.Add($"{nameof(IFileSystem)}? fileSystem = null");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"param ??= {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)};");
            sb.AppendLine($"parallelParam ??= {nameof(ParallelWriteParameters)}.{nameof(ParallelWriteParameters.Default)};");
            using (var args = sb.Call(
                       $"var modKey = param.{nameof(BinaryWriteParameters.RunMasterMatch)}"))
            {
                args.Add("mod: item");
                args.AddPassArg("path");
            }
            if (obj.GetObjectData().UsesStringFiles)
            {
                sb.AppendLine($"param.StringsWriter ??= Enums.HasFlag((int)item.ModHeader.Flags, item.GameRelease.ToCategory().GetLocalizedFlagIndex()!.Value) ? new StringsWriter({gameReleaseStr}, modKey, Path.Combine(Path.GetDirectoryName(path)!, \"Strings\"), {nameof(MutagenEncodingProvider)}.{nameof(MutagenEncodingProvider.Instance)}) : null;");
                sb.AppendLine("bool disposeStrings = param.StringsWriter != null;");
            }
            sb.AppendLine("using (var stream = fileSystem.GetOrDefault().FileStream.New(path, FileMode.Create, FileAccess.Write))");
            using (sb.CurlyBrace())
            {
                using (var args = sb.Call(
                           $"{obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.WriteParallel"))
                {
                    args.AddPassArg("item");
                    args.AddPassArg("stream");
                    args.AddPassArg("parallelParam");
                    args.Add($"param: param");
                    args.AddPassArg("modKey");
                }
            }
            if (obj.GetObjectData().UsesStringFiles)
            {
                sb.AppendLine("if (disposeStrings)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("param.StringsWriter?.Dispose();");
                }
            }
        }
        sb.AppendLine();
    }

    public override async Task GenerateInCommon(ObjectGeneration obj, StructuredStringBuilder sb, MaskTypeSet maskTypes)
    {
        await base.GenerateInCommon(obj, sb, maskTypes);
        if (obj.GetObjectType() != ObjectType.Mod) return;
        if (!maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)) return;

        GenerateGetGroup(obj, sb);
        GenerateWriteParallel(obj, sb);
    }

    private void GenerateGetGroup(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        using (var args = sb.Function(
                   "public object? GetGroup"))
        {
            args.Add($"{obj.Interface(getter: true)} obj");
            args.Add("Type type");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("switch (type.Name)");
            using (sb.CurlyBrace())
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                    if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                    {
                        throw new ArgumentException();
                    }
                    sb.AppendLine($"case \"{subObj.Name}\":");
                    sb.AppendLine($"case \"{subObj.Interface(getter: true)}\":");
                    sb.AppendLine($"case \"{subObj.Interface(getter: false)}\":");
                    if (subObj.HasInternalGetInterface)
                    {
                        sb.AppendLine($"case \"{subObj.Interface(getter: true, internalInterface: true)}\":");
                    }
                    if (subObj.HasInternalSetInterface)
                    {
                        sb.AppendLine($"case \"{subObj.Interface(getter: false, internalInterface: true)}\":");
                    }
                    using (sb.IncreaseDepth())
                    {
                        if (loqui.TargetObjectGeneration.Name.EndsWith("ListGroup"))
                        {
                            sb.AppendLine($"return obj.{field.Name}.Records;");
                        }
                        else
                        {
                            sb.AppendLine($"return obj.{field.Name};");
                        }
                    }
                }
                sb.AppendLine("default:");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("return null;");
                }
            }
        }
        sb.AppendLine();
    }

    private void GenerateWriteParallel(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        LoquiType groupInstance = null;
        LoquiType listGroupInstance = null;
        var objData = obj.GetObjectData();
        using (var args = sb.Function(
                   "public static void WriteParallel"))
        {
            args.Add($"{obj.Interface(getter: true, internalInterface: false)} item");
            args.Add($"Stream stream");
            args.Add($"{nameof(BinaryWriteParameters)} param");
            args.Add($"{nameof(ParallelWriteParameters)} parallelParam");
            args.Add($"ModKey modKey");
        }
        using (sb.CurlyBrace())
        {
            string gameConstantsStr;
            if (objData.GameReleaseOptions == null)
            {
                gameConstantsStr = $"{nameof(GameConstants)}.{obj.GetObjectData().GameCategory}";
            }
            else
            {
                sb.AppendLine($"var gameConstants = {nameof(GameConstants)}.Get(item.{ReleaseEnumName(obj)}.ToGameRelease());");
                gameConstantsStr = $"gameConstants";
            }
            sb.AppendLine($"var bundle = new {nameof(WritingBundle)}({gameConstantsStr})");
            using (sb.CurlyBrace(appendSemiColon: true))
            {
                sb.AppendLine("StringsWriter = param.StringsWriter,");
                sb.AppendLine("TargetLanguageOverride = param.TargetLanguageOverride,");
                sb.AppendLine($"Encodings = param.Encodings ?? {gameConstantsStr}.Encodings,");
            }

            sb.AppendLine($"var writer = new MutagenWriter(stream, bundle);");
            using (var args = sb.Call(
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

            sb.AppendLine($"Stream[] outputStreams = new Stream[{groupCount}];");
            sb.AppendLine($"List<Action> toDo = new List<Action>();");
            int i = 0;
            foreach (var field in obj.IterateFields())
            {
                if (!(field is LoquiType loqui)) continue;
                if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                if (loqui.TargetObjectGeneration.Name.EndsWith("ListGroup"))
                {
                    listGroupInstance = loqui;
                }
                else
                {
                    groupInstance = loqui;
                }

                var groupTarget = loqui.GetGroupTarget().GetObjectData();
                if (!loqui.TargetObjectGeneration.IsListGroup()
                    && groupTarget.CustomBinaryEnd == CustomEnd.Off 
                    && groupTarget.Subgroups.Count == 0)
                {
                    sb.AppendLine($"toDo.Add(() => WriteGroupParallel(item.{field.Name}, {i}, outputStreams, bundle, parallelParam));");
                }
                else
                {
                    sb.AppendLine($"toDo.Add(() => Write{field.Name}Parallel(item.{field.Name}, {i}, outputStreams, bundle, parallelParam));");
                }
                i++;
            }
            sb.AppendLine("Parallel.Invoke(parallelParam.ParallelOptions, toDo.ToArray());");
            using (var args = sb.Call(
                       $"{nameof(PluginUtilityTranslation)}.{nameof(PluginUtilityTranslation.CompileStreamsInto)}"))
            {
                args.Add("outputStreams.NotNull()");
                args.Add("stream");
            }
        }
        sb.AppendLine();

        if (groupInstance != null)
        {
            using (var args = sb.Function(
                       $"public static void WriteGroupParallel<T>"))
            {
                args.Add($"I{obj.ProtoGen.Protocol.Namespace}GroupGetter<T> group");
                args.Add("int targetIndex");
                args.Add("Stream[] streamDepositArray");
                args.Add($"{nameof(WritingBundle)} bundle");
                args.Add($"{nameof(ParallelWriteParameters)} parallelParam");
                args.Wheres.AddRange(groupInstance.TargetObjectGeneration.GenerateWhereClauses(LoquiInterfaceType.IGetter, groupInstance.TargetObjectGeneration.Generics));
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("if (group.RecordCache.Count == 0) return;");
                sb.AppendLine($"var cuts = group.Cut(parallelParam.CutCount).ToArray();");
                sb.AppendLine($"Stream[] subStreams = new Stream[cuts.Length + 1];");
                sb.AppendLine($"byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];");
                sb.AppendLine($"BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);");
                sb.AppendLine($"var groupByteStream = new MemoryStream(groupBytes);");
                sb.AppendLine($"using (var stream = new MutagenWriter(groupByteStream, bundle.Constants, dispose: false))");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"stream.Position += 8;");
                    sb.AppendLine($"{obj.ProtoGen.Protocol.Namespace}GroupBinaryWriteTranslation.WriteEmbedded<T>(group, stream);");
                }
                sb.AppendLine($"subStreams[0] = groupByteStream;");
                sb.AppendLine($"Parallel.ForEach(cuts, parallelParam.ParallelOptions, (cutItems, state, counter) =>");
                using (sb.CurlyBrace(appendSemiColon: true, appendParenthesis: true))
                {
                    sb.AppendLine($"{nameof(MemoryTributary)} trib = new {nameof(MemoryTributary)}();");
                    sb.AppendLine($"using (var stream = new MutagenWriter(trib, bundle with {{}}, dispose: false))");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"foreach (var item in cutItems)");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine($"item.WriteToBinary(stream);");
                        }
                    }
                    sb.AppendLine($"subStreams[(int)counter + 1] = trib;");
                }
                sb.AppendLine($"{nameof(PluginUtilityTranslation)}.CompileSetGroupLength(subStreams, groupBytes);");
                sb.AppendLine($"streamDepositArray[targetIndex] = new CompositeReadStream(subStreams, resetPositions: true);");
            }
            sb.AppendLine();
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

    public override async Task GenerateInInterface(ObjectGeneration obj, StructuredStringBuilder sb, bool internalInterface, bool getter)
    {
        await base.GenerateInInterface(obj, sb, internalInterface, getter);
        if (obj.GetObjectType() != ObjectType.Mod) return;
        if (!getter) return;
        if (obj.GetObjectData().GameReleaseOptions == null) return;
        sb.AppendLine($"{ReleaseEnumName(obj)} {ReleaseEnumName(obj)} {{ get; }}");
    }
}