using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loqui;
using Noggog;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.Generation.Modules.Binary
{
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
        }

        public override async Task GenerateInTranslationWriteClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateCustomWritePartials(obj, fg);
            GenerateCustomBinaryEndWritePartial(obj, fg);
            await GenerateWriteExtras(obj, fg);
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
                    isAsync: false);
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
                $"static partial void CustomBinaryEndExport"))
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
                    $"static partial void CustomBinaryEndImport"))
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
                    fg.AppendLine("nextRecord = recordTypeConverter.ConvertToCustom(nextRecord);");
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
                                args.AddPassArg("recordTypeConverter");
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
                        args.AddPassArg("modKey");
                    }
                    if (obj.GetObjectData().GameReleaseOptions != null)
                    {
                        args.AddPassArg("release");
                    }
                }
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine($"{ReaderMemberName}.MetaData.ModKey = modKey;");
                }
            }
        }

        protected string GetRecordTypeString(ObjectGeneration obj, Accessor gameReleaseAccessor, Accessor versionAccessor)
        {
            var data = obj.GetObjectData();
            if (data.GameReleaseConverters != null)
            {
                return $"recordTypeConverter.Combine({obj.RegistrationName}.Get({gameReleaseAccessor})).ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})";
            }
            if (data.VersionConverters != null)
            {
                return $"recordTypeConverter.Combine({obj.RegistrationName}.Get({versionAccessor})).ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})";
            }
            return $"recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})";
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
            var isMajor = await obj.IsMajorRecord();
            var hasRecType = obj.TryGetRecordType(out var recType);
            if (hasRecType)
            {
                using (var args = new ArgsWrapper(fg,
                    $"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Header)}",
                    ")",
                    semiColon: false))
                {
                    args.AddPassArg(WriterMemberName);
                    args.Add($"record: {GetRecordTypeString(obj, "writer.MetaData.Constants.Release", "writer.MetaData.FormVersion")}");
                    args.Add($"type: {nameof(ObjectType)}.{obj.GetObjectType()}");
                }
            }
            using (new BraceWrapper(fg, doIt: hasRecType))
            {
                if (isMajor)
                {
                    fg.AppendLine("try");
                }
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{nameof(ModHeaderWriteLogic)}.{nameof(ModHeaderWriteLogic.WriteHeader)}"))
                    {
                        args.AddPassArg("param");
                        args.AddPassArg(WriterMemberName);
                        args.Add("mod: item");
                        args.Add("modHeader: item.ModHeader.DeepCopy()");
                        args.AddPassArg("modKey");
                    }
                }
                using (new BraceWrapper(fg, doIt: isMajor))
                {
                    if (HasEmbeddedFields(obj))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"WriteEmbedded"))
                        {
                            args.AddPassArg($"item");
                            args.AddPassArg(WriterMemberName);
                        }
                    }
                    else
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{this.TranslationWriteClass(firstBase)}.WriteEmbedded"))
                            {
                                args.AddPassArg($"item");
                                args.AddPassArg(WriterMemberName);
                            }
                        }
                    }
                    if (HasRecordTypeFields(obj))
                    {
                        if (await obj.IsMajorRecord())
                        {
                            fg.AppendLine($"{WriterMemberName}.{nameof(MutagenWriter.MetaData)}.{nameof(WritingBundle.FormVersion)} = item.FormVersion;");
                        }
                        using (var args = new ArgsWrapper(fg,
                            $"WriteRecordTypes"))
                        {
                            args.AddPassArg($"item");
                            args.AddPassArg(WriterMemberName);
                            if (obj.GetObjectType() == ObjectType.Mod)
                            {
                                args.AddPassArg($"importMask");
                            }
                            else
                            {
                                args.AddPassArg($"recordTypeConverter");
                            }
                        }
                        if (await obj.IsMajorRecord())
                        {
                            fg.AppendLine($"{WriterMemberName}.{nameof(MutagenWriter.MetaData)}.{nameof(WritingBundle.FormVersion)} = null;");
                        }
                    }
                    else
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasRecordTypeFields(b));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                            $"{this.TranslationWriteClass(firstBase)}.WriteRecordTypes"))
                            {
                                args.AddPassArg($"item");
                                args.AddPassArg(WriterMemberName);
                                args.AddPassArg($"recordTypeConverter");
                            }
                        }
                    }
                }
                if (isMajor)
                {
                    fg.AppendLine("catch (Exception ex)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"throw RecordException.Enrich(ex, item);");
                    }
                }
            }
            if (data.CustomBinaryEnd != CustomEnd.Off)
            {
                using (var args = new ArgsWrapper(fg,
                    $"CustomBinaryEndExportInternal"))
                {
                    args.AddPassArg(WriterMemberName);
                    args.Add("obj: item");
                }
            }
        }

        private async Task GenerateWriteExtras(ObjectGeneration obj, FileGeneration fg)
        {
            var data = obj.GetObjectData();
            if (HasEmbeddedFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void WriteEmbedded{obj.GetGenericTypes(MaskType.Normal)}"))
                {
                    args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                    args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                    args.Add($"{WriterClass} {WriterMemberName}");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasLoquiBaseObject)
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{TranslationWriteClass(firstBase)}.WriteEmbedded"))
                            {
                                args.AddPassArg("item");
                                args.AddPassArg(WriterMemberName);
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.False))
                    {
                        var fieldData = field.GetFieldData();
                        if (fieldData.HasTrigger) continue;
                        if (field is CustomLogic logic && logic.IsRecordType) continue;
                        if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                        if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                        if (field is BreakType breakType)
                        {
                            fg.AppendLine($"if (!item.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index}))");
                            fg.AppendLine("{");
                            fg.Depth++;
                        }
                        if (!field.Enabled) continue;
                        List<string> conditions = new List<string>();
                        if (conditions.Count > 0)
                        {
                            using (var args = new IfWrapper(fg, ANDs: true))
                            {
                                foreach (var item in conditions)
                                {
                                    args.Add(item);
                                }
                            }
                        }
                        using (new BraceWrapper(fg, doIt: conditions.Count > 0))
                        {
                            var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                            if (fieldData.Binary == BinaryGenerationType.Custom)
                            {
                                CustomLogic.GenerateWrite(
                                    fg: fg,
                                    obj: obj,
                                    field: field,
                                    writerAccessor: WriterMemberName);
                                continue;
                            }
                            if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                            {
                                if (!field.IntegrateField) continue;
                                throw new ArgumentException("Unsupported type generator: " + field);
                            }
                            if (fieldData.HasVersioning)
                            {
                                fg.AppendLine($"if ({VersioningModule.GetVersionIfCheck(fieldData, "writer.MetaData.FormVersion!.Value")})");
                            }
                            using (new BraceWrapper(fg, doIt: fieldData.HasVersioning))
                            {
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
                    }
                    for (int i = 0; i < obj.Fields.WhereCastable<TypeGeneration, BreakType>().Count(); i++)
                    {
                        fg.Depth--;
                        fg.AppendLine("}");
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void WriteRecordTypes{obj.GetGenericTypes(MaskType.Normal)}"))
                {
                    args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                    args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                    args.Add($"{WriterClass} {WriterMemberName}");
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        args.Add($"GroupMask? importMask");
                    }
                    args.Add($"RecordTypeConverter? recordTypeConverter{(obj.GetObjectType() == ObjectType.Mod ? " = null" : null)}");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasLoquiBaseObject)
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((f) => HasRecordTypeFields(f));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{TranslationWriteClass(firstBase)}.WriteRecordTypes"))
                            {
                                args.AddPassArg($"item");
                                args.AddPassArg(WriterMemberName);
                                if (data.BaseRecordTypeConverter?.FromConversions.Count > 0)
                                {
                                    args.Add($"recordTypeConverter: recordTypeConverter.Combine({obj.RegistrationName}.BaseConverter)");
                                }
                                else
                                {
                                    args.AddPassArg("recordTypeConverter");
                                }
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                    {
                        var fieldData = field.GetFieldData();
                        if (!fieldData.HasTrigger)
                        {
                            if (field is not CustomLogic custom)
                            {
                                continue;
                            }
                            if (!custom.IsRecordType)
                            {
                                continue;
                            }
                        }
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

                        Action generate = () =>
                        {
                            var accessor = Accessor.FromType(field, "item");
                            if (field is DataType dataType)
                            {
                                if (dataType.Nullable)
                                {
                                    fg.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Has))");
                                }
                                using (new BraceWrapper(fg, doIt: dataType.Nullable))
                                {
                                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}({WriterMemberName}, recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(fieldData.RecordType.Value)})))");
                                    using (new BraceWrapper(fg))
                                    {
                                        bool isInRange = false;
                                        foreach (var subField in dataType.IterateFieldsWithMeta())
                                        {
                                            if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var subGenerator))
                                            {
                                                throw new ArgumentException("Unsupported type generator: " + subField.Field);
                                            }

                                            var subData = subField.Field.GetFieldData();
                                            if (!subGenerator.ShouldGenerateCopyIn(subField.Field)) continue;
                                            switch (subData.Binary)
                                            {
                                                case BinaryGenerationType.Normal:
                                                    break;
                                                case BinaryGenerationType.NoGeneration:
                                                    continue;
                                                case BinaryGenerationType.Custom:
                                                    using (var args = new ArgsWrapper(fg,
                                                        $"{TranslationWriteClass(obj)}.WriteBinary{subField.Field.Name}"))
                                                    {
                                                        args.AddPassArg(WriterMemberName);
                                                        args.AddPassArg("item");
                                                    }
                                                    continue;
                                                default:
                                                    throw new NotImplementedException();
                                            }
                                            if (subField.BreakIndex != -1)
                                            {
                                                fg.AppendLine($"if (!item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Break{subField.BreakIndex}))");
                                                fg.AppendLine("{");
                                                fg.Depth++;
                                            }
                                            if (subField.Range != null && !isInRange)
                                            {
                                                isInRange = true;
                                                fg.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Range{subField.RangeIndex}))");
                                                fg.AppendLine("{");
                                                fg.Depth++;
                                            }
                                            if (subField.Range == null && isInRange)
                                            {
                                                isInRange = false;
                                                fg.Depth--;
                                                fg.AppendLine("}");
                                            }
                                            if (subData.HasVersioning)
                                            {
                                                fg.AppendLine($"if ({VersioningModule.GetVersionIfCheck(subData, $"{WriterMemberName}.MetaData.FormVersion!.Value")})");
                                            }
                                            using (new BraceWrapper(fg, doIt: subData.HasVersioning))
                                            {
                                                subGenerator.GenerateWrite(
                                                    fg: fg,
                                                    objGen: obj,
                                                    typeGen: subField.Field,
                                                    writerAccessor: WriterMemberName,
                                                        translationAccessor: null,
                                                    itemAccessor: Accessor.FromType(subField.Field, "item"),
                                                    errorMaskAccessor: null,
                                                    converterAccessor: "recordTypeConverter");
                                            }
                                        }
                                        for (int i = 0; i < dataType.BreakIndices.Count; i++)
                                        {
                                            fg.Depth--;
                                            fg.AppendLine("}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!generator.ShouldGenerateWrite(field)) return;
                                if (fieldData.Binary == BinaryGenerationType.NoGeneration) return;

                                var loqui = field as LoquiType;

                                // Skip modheader
                                if (loqui != null
                                    && loqui.Name == "ModHeader")
                                {
                                    return;
                                }

                                bool doIf = true;
                                if (loqui != null
                                    && loqui.TargetObjectGeneration?.GetObjectType() == ObjectType.Group
                                    && obj.GetObjectType() == ObjectType.Mod)
                                {
                                    fg.AppendLine($"if (importMask?.{field.Name} ?? true)");
                                }
                                else
                                {
                                    doIf = false;
                                }
                                using (new BraceWrapper(fg, doIt: doIf))
                                {
                                    generator.GenerateWrite(
                                        fg: fg,
                                        objGen: obj,
                                        typeGen: field,
                                        writerAccessor: WriterMemberName,
                                        itemAccessor: accessor,
                                        translationAccessor: null,
                                        errorMaskAccessor: null,
                                        converterAccessor: "recordTypeConverter");
                                }
                            }
                        };

                        if (fieldData.CustomVersion == null)
                        {
                            if (fieldData.HasVersioning)
                            {
                                fg.AppendLine($"if ({VersioningModule.GetVersionIfCheck(fieldData, $"{WriterMemberName}.MetaData.FormVersion!.Value")})");
                            }
                            using (new BraceWrapper(fg, doIt: fieldData.HasVersioning))
                            {
                                generate();
                            }
                        }
                        else
                        {
                            fg.AppendLine($"if (item.FormVersion <= {fieldData.CustomVersion})");
                            using (new BraceWrapper(fg))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"{field.Name}CustomVersionWrite"))
                                {
                                    args.AddPassArg($"item");
                                    args.AddPassArg(WriterMemberName);
                                    args.AddPassArg($"recordTypeConverter");
                                }
                            }
                            fg.AppendLine("else");
                            using (new BraceWrapper(fg))
                            {
                                generate();
                            }
                        }
                    }

                    if (data.EndMarkerType.HasValue)
                    {
                        fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}({WriterMemberName}, {obj.RecordTypeHeaderName(data.EndMarkerType.Value)})) {{ }} // End Marker");
                    }
                }
                fg.AppendLine();
            }

            foreach (var field in obj.Fields)
            {
                var fieldData = field.GetFieldData();
                if (fieldData.CustomVersion != null)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"static partial void {field.Name}CustomVersionWrite"))
                    {
                        args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
                        args.Add($"{WriterClass} {WriterMemberName}");
                        args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter");
                    }
                    fg.AppendLine();
                }
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
}
