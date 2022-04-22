using Loqui.Generation;
using Loqui;
using Mutagen.Bethesda.Generation.Fields;
using Noggog;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public enum BinaryGenerationType
{
    Normal,
    NoGeneration,
    Custom,
}

public abstract class BinaryTranslationModule : TranslationModule<BinaryTranslationGeneration>
{
    public override string Namespace => "Mutagen.Bethesda.Plugins.Binary.Translations.";
    public override string ModuleNickname => "Binary";
    public override bool GenerateAbstractCreates => false;
    public CustomLogicTranslationGeneration CustomLogic;

    public override async Task<bool> AsyncImport(ObjectGeneration obj)
    {
        if (obj.GetObjectData().CustomBinaryEnd == CustomEnd.Async) return true;
        return await base.AsyncImport(obj);
    }

    public string BinaryOverlayClassName(ObjectGeneration obj) => $"{obj.Name}BinaryOverlay";
    public string BinaryOverlayClassName(LoquiType loqui) => $"{loqui.TargetObjectGeneration.Name}BinaryOverlay{loqui.GenericTypes(getter: true)}";
    public string BinaryOverlayClass(ObjectGeneration obj) => $"{BinaryOverlayClassName(obj)}{obj.GetGenericTypes(MaskType.Normal)}";

    public abstract string WriterClass { get; }
    public abstract string WriterMemberName { get; }
    public abstract string ReaderClass { get; }
    public abstract string ReaderMemberName { get; }

    public const string ItemKey = "Item";
    public const string OutItemKey = "OutItem";
    public const string NextRecordTypeKey = "NextRecordType";
    public const string ContentLengthKey = "ContentLength";

    public BinaryTranslationModule(LoquiGenerator gen)
        : base(gen)
    {
    }

    public override async Task LoadWrapup(ObjectGeneration obj)
    {
        await base.LoadWrapup(obj);
        lock (_typeGenerations)
        {
            foreach (var gen in _typeGenerations.Values)
            {
                gen.Module = this;
                gen.MaskModule = this.Gen.MaskModule;
            }
        }
        obj.RequiredNamespaces.Add("System.Reactive.Disposables");
        obj.RequiredNamespaces.Add("System.Reactive.Linq");
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        await foreach (var item in base.RequiredUsingStatements(obj))
        {
            yield return item;
        }
        yield return "Mutagen.Bethesda.Binary";
        yield return "System.Buffers.Binary";
        yield return "Mutagen.Bethesda.Translations.Binary";
        if (obj.GetObjectType() == ObjectType.Mod)
        {
            yield return "Mutagen.Bethesda.Plugins.Binary.Parameters";
        }
    }

    public override async Task GenerateInTranslationWriteClass(ObjectGeneration obj, FileGeneration fg)
    {
        GenerateCustomWritePartials(obj, fg);
        GenerateCustomBinaryEndWritePartial(obj, fg);
        await base.GenerateInTranslationWriteClass(obj, fg);
    }

    public override async Task GenerateInTranslationCreateClass(ObjectGeneration obj, FileGeneration fg)
    {
        GenerateCustomCreatePartials(obj, fg);
        GenerateCustomBinaryEndCreatePartial(obj, fg);
        await base.GenerateInTranslationCreateClass(obj, fg);
    }

    public virtual bool WantsTryCreateFromBinary(ObjectGeneration obj) => !obj.Abstract;

    public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
    {
        await base.GenerateInClass(obj, fg);
        if (WantsTryCreateFromBinary(obj))
        {
            using (var args = new FunctionWrapper(fg,
                       "public static bool TryCreateFromBinary"))
            {
                foreach (var (API, Public) in this.MainAPI.ReaderAPI.IterateAPI(
                             obj,
                             TranslationDirection.Reader,
                             new APILine(OutItemKey, $"out {obj.ObjectName} item")))
                {
                    if (Public)
                    {
                        args.Add(API.Result);
                    }
                }
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var startPos = {ReaderMemberName}.Position;");
                using (var args = new ArgsWrapper(fg,
                           $"item = CreateFromBinary"))
                {
                    args.Add(this.MainAPI.PassArgs(obj, TranslationDirection.Reader));
                }
                fg.AppendLine($"return startPos != {ReaderMemberName}.Position;");
            }
        }
        await GenerateBinaryOverlayCreates(obj, fg);
    }

    private void GenerateCustomWritePartials(ObjectGeneration obj, FileGeneration fg)
    {
        foreach (var field in obj.IterateFields(nonIntegrated: true))
        {
            if (field.GetFieldData().Binary != BinaryGenerationType.Custom && !(field is CustomLogic)) continue;
            CustomLogicTranslationGeneration.GenerateWritePartialMethods(
                fg: fg,
                obj: obj,
                field: field,
                isAsync: false);
        }
    }

    private void GenerateCustomCreatePartials(ObjectGeneration obj, FileGeneration fg)
    {
        foreach (var field in obj.IterateFields(nonIntegrated: true))
        {
            if (field.GetFieldData().Binary != BinaryGenerationType.Custom && !(field is CustomLogic)) continue;
            CustomLogicTranslationGeneration.GenerateCreatePartialMethods(
                fg: fg,
                obj: obj,
                field: field,
                isAsync: false,
                useReturnValue: field is CustomLogic);
        }
    }

    protected bool HasRecordTypeFields(ObjectGeneration obj)
    {
        return GetRecordTypeFields(obj).Any();
    }

    protected IEnumerable<TypeGeneration> GetRecordTypeFields(ObjectGeneration obj)
    {
        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
        {
            if (field.GetFieldData().HasTrigger)
            {
                yield return field;
            }
        }
    }

    protected IEnumerable<TypeGeneration> GetEmbeddedFields(ObjectGeneration obj)
    {
        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
        {
            if (!field.GetFieldData().HasTrigger)
            {
                yield return field;
            }
        }
    }

    protected bool HasEmbeddedFields(ObjectGeneration obj)
    {
        return GetEmbeddedFields(obj).Any();
    }

    public bool HasAsyncStructs(ObjectGeneration obj, bool self)
    {
        IEnumerable<ObjectGeneration> enumer = obj.BaseClassTrail();
        if (self)
        {
            enumer = enumer.And(obj);
        }
        return enumer
            .SelectMany(o => GetEmbeddedFields(o))
            .Any(t =>
            {
                if (this.TryGetTypeGeneration(t.GetType(), out var gen))
                {
                    return gen.IsAsync(t, read: true);
                }
                return false;
            });
    }

    public bool HasAsyncRecords(ObjectGeneration obj, bool self)
    {
        IEnumerable<ObjectGeneration> enumer = obj.BaseClassTrail();
        if (self)
        {
            enumer = enumer.And(obj);
        }
        return enumer
            .SelectMany(o => GetRecordTypeFields(o))
            .Any(t =>
            {
                if (this.TryGetTypeGeneration(t.GetType(), out var gen))
                {
                    return gen.IsAsync(t, read: true);
                }
                return false;
            });
    }

    public bool HasAsync(ObjectGeneration obj, bool self)
    {
        return HasAsyncStructs(obj, self)
               || HasAsyncRecords(obj, self);
    }

    protected virtual async Task GenerateBinaryOverlayCreates(ObjectGeneration obj, FileGeneration fg)
    {
    }

    private void GenerateCustomBinaryEndWritePartial(ObjectGeneration obj, FileGeneration fg)
    {
        var data = obj.GetObjectData();
        if (data.CustomBinaryEnd == CustomEnd.Off) return;
        using (var args = new ArgsWrapper(fg,
                   $"public static partial void CustomBinaryEndExport"))
        {
            args.Add($"{WriterClass} {WriterMemberName}");
            args.Add($"{obj.Interface(internalInterface: true, getter: true)} obj");
        }
        using (var args = new FunctionWrapper(fg,
                   $"public static void CustomBinaryEndExportInternal"))
        {
            args.Add($"{WriterClass} {WriterMemberName}");
            args.Add($"{obj.Interface(internalInterface: true, getter: true)} obj");
        }
        using (new BraceWrapper(fg))
        {
            using (var args = new ArgsWrapper(fg,
                       $"CustomBinaryEndExport"))
            {
                args.AddPassArg(WriterMemberName);
                args.AddPassArg($"obj");
            }
        }
    }

    private void GenerateCustomBinaryEndCreatePartial(ObjectGeneration obj, FileGeneration fg)
    {
        var data = obj.GetObjectData();
        if (data.CustomBinaryEnd == CustomEnd.Off) return;
        if (data.CustomBinaryEnd == CustomEnd.Normal)
        {
            using (var args = new ArgsWrapper(fg,
                       $"public static partial void CustomBinaryEndImport"))
            {
                args.Add($"{ReaderClass} {ReaderMemberName}");
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} obj");
            }
            using (var args = new FunctionWrapper(fg,
                       $"public static void CustomBinaryEndImportPublic"))
            {
                args.Add($"{ReaderClass} {ReaderMemberName}");
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                           $"CustomBinaryEndImport"))
                {
                    args.AddPassArg(ReaderMemberName);
                    args.AddPassArg($"obj");
                }
            }
        }
    }

    protected override bool GenerateMainCreate(ObjectGeneration obj)
    {
        var data = obj.GetObjectData();
        return !data.CustomBinary && obj.GetObjectType() != ObjectType.Mod;
    }

    protected override async Task GenerateNewSnippet(ObjectGeneration obj, FileGeneration fg)
    {
        if (await obj.IsMajorRecord())
        {
            fg.AppendLine($"var ret = new {obj.Name}();");
        }
        else
        {
            if (obj.TryGetCustomRecordTypeTriggers(out var customLogicTriggers))
            {
                using (var args = new ArgsWrapper(fg,
                           $"var nextRecord = HeaderTranslation.GetNext{(obj.GetObjectType() == ObjectType.Subrecord ? "Subrecord" : "Record")}Type"))
                {
                    args.Add($"reader: {ReaderMemberName}.Reader");
                    args.Add("contentLength: out var customLen");
                }
                fg.AppendLine("nextRecord = translationParams.ConvertToCustom(nextRecord);");
                fg.AppendLine("switch (nextRecord.TypeInt)");
                using (new BraceWrapper(fg))
                {
                    foreach (var item in customLogicTriggers)
                    {
                        fg.AppendLine($"case {item.TypeInt}: // {item.Type}");
                    }
                    using (new DepthWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                                   "return CustomRecordTypeTrigger"))
                        {
                            args.Add($"{ReaderMemberName}: {ReaderMemberName}.SpawnWithLength(customLen + {ReaderMemberName}.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)})");
                            args.Add("recordType: nextRecord");
                            args.AddPassArg("translationParams");
                        }
                    }
                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine("break;");
                    }
                }
            }
            using (var args = new ArgsWrapper(fg,
                       $"var ret = new {obj.Name}{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    args.Add("modKey: frame.MetaData.ModKey");
                }
                if (obj.GetObjectData().GameReleaseOptions != null)
                {
                    args.AddPassArg("release");
                }
            }
        }
    }

    protected string GetRecordTypeString(
        ObjectGeneration obj, 
        Accessor converterAccessor,
        Accessor gameReleaseAccessor, 
        Accessor versionAccessor)
    {
        var data = obj.GetObjectData();
        if (data.GameReleaseConverters != null)
        {
            return $"{converterAccessor}.Combine({obj.RegistrationName}.Get({gameReleaseAccessor})).ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})";
        }
        if (data.VersionConverters != null)
        {
            return $"{converterAccessor}.Combine({obj.RegistrationName}.Get({versionAccessor})).ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})";
        }
        return $"{converterAccessor}.ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})";
    }

    protected override async Task GenerateCopyInSnippet(ObjectGeneration obj, FileGeneration fg, Accessor accessor)
    {
        if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
        {
            using (var args = new ArgsWrapper(fg,
                       $"base.{CopyInFromPrefix}{ModuleNickname}"))
            {
                args.AddPassArg("item");
                args.AddPassArg(ReaderMemberName);
            }
        }
        int breakIndex = 0;
        foreach (var field in obj.IterateFields(
                     nonIntegrated: true,
                     expandSets: SetMarkerType.ExpandSets.False))
        {
            if (field is SetMarkerType) continue;
            if (field is CustomLogic logic && logic.IsRecordType) continue;
            var fieldData = field.GetFieldData();
            if (fieldData.HasTrigger) continue;
            if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
            if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
            if (!field.Enabled) continue;
            if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
            {
                if (!field.IntegrateField) continue;
                throw new ArgumentException("Unsupported type generator: " + field);
            }
            if (field.Nullable)
            {
                fg.AppendLine($"if (frame.Complete) return;");
            }

            if (field is BreakType)
            {
                fg.AppendLine("if (frame.Complete)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"item.{VersioningModule.VersioningFieldName} |= {obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                    fg.AppendLine("return;");
                }
                continue;
            }
            await GenerateFillSnippet(obj, fg, field, generator, "frame");
        }
    }

    protected virtual async Task GenerateFillSnippet(ObjectGeneration obj, FileGeneration fg, TypeGeneration field, BinaryTranslationGeneration generator, string frameAccessor)
    {
        await generator.GenerateCopyIn(
            fg: fg,
            objGen: obj,
            typeGen: field,
            readerAccessor: frameAccessor,
            itemAccessor: Accessor.FromType(field, "item"),
            translationAccessor: null,
            errorMaskAccessor: null);
    }

    protected override async Task GenerateWriteSnippet(ObjectGeneration obj, FileGeneration fg)
    {
        var data = obj.GetObjectData();

        if (obj.HasLoquiBaseObject)
        {
            var firstBase = obj.BaseClassTrail().FirstOrDefault();
            if (firstBase != null)
            {
                using (var args = new ArgsWrapper(fg,
                           $"{TranslationWriteClass(firstBase)}.Instance.Write"))
                {
                    args.AddPassArg($"item");
                    args.AddPassArg(WriterMemberName);
                }
            }
        }

        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
        {
            var fieldData = field.GetFieldData();
            if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
            switch (fieldData.Binary)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.NoGeneration:
                    continue;
                case BinaryGenerationType.Custom:
                    CustomLogic.GenerateWrite(
                        fg: fg,
                        obj: obj,
                        field: field,
                        writerAccessor: WriterMemberName);
                    continue;
                default:
                    throw new NotImplementedException();
            }
            if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
            {
                throw new ArgumentException("Unsupported type generator: " + field);
            }

            if (!generator.ShouldGenerateWrite(field)) return;
            if (fieldData.Binary == BinaryGenerationType.NoGeneration) return;

            await generator.GenerateWrite(
                fg: fg,
                objGen: obj,
                typeGen: field,
                writerAccessor: WriterMemberName,
                itemAccessor: Accessor.FromType(field, "item"),
                translationAccessor: null,
                errorMaskAccessor: null,
                converterAccessor: null);
        }
    }

    public override void ReplaceTypeAssociation<Target, Replacement>()
    {
    }

    public struct PassedLengths
    {
        public TypeGeneration Field;
        public string PassedAccessor;
        public int? PassedLength;
        public PassedType PassedType;
        public string CurAccessor;
        public int? CurLength;
        public PassedType CurType;
        public int? FieldLength;
    }

    public enum PassedType
    {
        Direct,
        Relative,
    }

    public async IAsyncEnumerable<PassedLengths> IteratePassedLengths(
        ObjectGeneration obj,
        bool forOverlay,
        string passedLenPrefix = null,
        bool includeBaseClass = false,
        SetMarkerType.ExpandSets expand = SetMarkerType.ExpandSets.FalseAndInclude)
    {
        await foreach (var item in IteratePassedLengths(
                           obj,
                           obj.IterateFields(
                               expandSets: expand,
                               nonIntegrated: true,
                               includeBaseClass: true),
                           forOverlay: forOverlay,
                           passedLenPrefix: passedLenPrefix))
        {
            if (includeBaseClass
                || obj.Fields.Contains(item.Field)
                || ((expand == SetMarkerType.ExpandSets.True || expand == SetMarkerType.ExpandSets.TrueAndInclude)
                    && obj.Fields.WhereCastable<TypeGeneration, DataType>().Any(d => d.SubFields.Contains(item.Field))))
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<PassedLengths> IteratePassedLengths(
        ObjectGeneration obj,
        IEnumerable<TypeGeneration> fields,
        bool forOverlay,
        string passedLenPrefix = null)
    {
        var lengths = new PassedLengths()
        {
            PassedLength = 0,
            CurLength = 0
        };
        TypeGeneration lastUnknownField = null;
        TypeGeneration lastVersionedField = null;
        foreach (var field in fields)
        {
            lengths.Field = field;
            lengths.FieldLength = null;
            if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen))
            {
                if (!field.IntegrateField) continue;
                throw new NotImplementedException();
            }
            void processLen(int? expectedLen)
            {
                lengths.PassedLength = lengths.CurLength;
                lengths.PassedAccessor = lengths.CurAccessor;
                lengths.PassedType = lengths.CurType;
                if (expectedLen == null)
                {
                    lengths.CurLength = null;
                    lastUnknownField = field;
                    lengths.CurAccessor = $"{passedLenPrefix}{lastUnknownField.Name}EndingPos";
                    lastVersionedField = null;
                    lengths.CurType = PassedType.Relative;
                }
                else
                {
                    if (lengths.CurLength == null)
                    {
                        lengths.CurLength = expectedLen.Value;
                    }
                    else
                    {
                        lengths.CurLength += expectedLen.Value;
                    }
                    if (field.GetFieldData().HasVersioning)
                    {
                        lastVersionedField = field;
                    }
                    lengths.CurType = PassedType.Direct;
                    lengths.CurAccessor = $"0x{lengths.CurLength:X}";
                    if (lastVersionedField != null)
                    {
                        lengths.CurAccessor = $"{passedLenPrefix}{lastVersionedField.Name}VersioningOffset + {lengths.CurAccessor}";
                    }
                    if (lastUnknownField != null)
                    {
                        lengths.CurAccessor = $"{passedLenPrefix}{lastUnknownField.Name}EndingPos + {lengths.CurAccessor}";
                        lengths.CurType = PassedType.Relative;
                    }
                }
                lengths.FieldLength = expectedLen;
            }

            var data = field.GetFieldData();
            var customType = forOverlay ? data.BinaryOverlayFallback : data.Binary;
            switch (customType)
            {
                case BinaryGenerationType.Custom:
                    processLen(await CustomLogic.ExpectedLength(obj, field));
                    yield return lengths;
                    break;
                case BinaryGenerationType.NoGeneration:
                    yield return lengths;
                    break;
                default:
                    if (!data.HasTrigger)
                    {
                        if (data.Length.HasValue)
                        {
                            processLen(data.Length.Value);
                        }
                        else
                        {
                            processLen(await typeGen.GetPassedAmount(obj, field));
                        }
                    }
                    yield return lengths;
                    break;
            }
        }
    }
}