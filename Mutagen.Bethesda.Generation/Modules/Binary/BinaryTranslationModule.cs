using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using System.IO;
using Noggog;
using Loqui.Internal;
using System.Xml.Linq;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Generation
{
    public enum BinaryGenerationType
    {
        Normal,
        NoGeneration,
        Custom,
    }

    public class BinaryTranslationModule : TranslationModule<BinaryTranslationGeneration>
    {
        public override string Namespace => "Mutagen.Bethesda.Binary.";
        public override string ModuleNickname => "Binary";
        public override bool GenerateAbstractCreates => false;
        public readonly CustomLogicTranslationGeneration CustomLogic;
        public override bool DoTranslationInterface(ObjectGeneration obj) => obj.GetObjectType() != ObjectType.Mod;
        public override bool DirectTranslationReference(ObjectGeneration obj) => obj.GetObjectType() == ObjectType.Mod;
        public override string TranslatorReference(ObjectGeneration obj, Accessor item)
        {
            if (obj.GetObjectType() != ObjectType.Mod)
            {
                return base.TranslatorReference(obj, item);
            }
            return $"{this.TranslationWriteClass(obj)}.Instance";
        }

        public override async Task<bool> AsyncImport(ObjectGeneration obj)
        {
            if (obj.GetObjectData().CustomBinaryEnd == CustomEnd.Async) return true;
            return await base.AsyncImport(obj);
        }

        public string BinaryOverlayClassName(ObjectGeneration obj) => $"{obj.Name}BinaryOverlay";
        public string BinaryOverlayClassName(LoquiType loqui) => $"{loqui.TargetObjectGeneration.Name}BinaryOverlay{loqui.GenericTypes(getter: true)}";
        public string BinaryOverlayClass(ObjectGeneration obj) => $"{BinaryOverlayClassName(obj)}{obj.GetGenericTypes(MaskType.Normal)}";

        public BinaryTranslationModule(LoquiGenerator gen)
            : base(gen)
        {
            this.DoErrorMasks = false;
            this.TranslationMaskParameter = false;
            this._typeGenerations[typeof(LoquiType)] = new LoquiBinaryTranslationGeneration(ModuleNickname);
            this._typeGenerations[typeof(BoolType)] = new BooleanBinaryTranslationGeneration();
            this._typeGenerations[typeof(CharType)] = new PrimitiveBinaryTranslationGeneration<char>(expectedLen: 1);
            this._typeGenerations[typeof(DateTimeType)] = new PrimitiveBinaryTranslationGeneration<DateTime>(expectedLen: null);
            this._typeGenerations[typeof(DoubleType)] = new PrimitiveBinaryTranslationGeneration<double>(expectedLen: 8);
            this._typeGenerations[typeof(EnumType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(FloatType)] = new FloatBinaryTranslationGeneration();
            this._typeGenerations[typeof(PercentType)] = new PercentBinaryTranslationGeneration();
            this._typeGenerations[typeof(Int8Type)] = new SByteBinaryTranslationGeneration();
            this._typeGenerations[typeof(Int16Type)] = new PrimitiveBinaryTranslationGeneration<short>(expectedLen: 2);
            this._typeGenerations[typeof(Int32Type)] = new PrimitiveBinaryTranslationGeneration<int>(expectedLen: 4);
            this._typeGenerations[typeof(Int64Type)] = new PrimitiveBinaryTranslationGeneration<long>(expectedLen: 8);
            this._typeGenerations[typeof(P3UInt16Type)] = new PointBinaryTranslationGeneration<P3UInt16>(expectedLen: 6);
            this._typeGenerations[typeof(P2FloatType)] = new PointBinaryTranslationGeneration<P2Float>(expectedLen: 8);
            this._typeGenerations[typeof(P3FloatType)] = new PointBinaryTranslationGeneration<P3Float>(expectedLen: 12);
            this._typeGenerations[typeof(P2Int32Type)] = new PointBinaryTranslationGeneration<P2Int>(expectedLen: 8);
            this._typeGenerations[typeof(P3IntType)] = new PointBinaryTranslationGeneration<P3Int>(expectedLen: 12);
            this._typeGenerations[typeof(P2Int16Type)] = new PointBinaryTranslationGeneration<P2Int16>(expectedLen: 4);
            this._typeGenerations[typeof(P3Int16Type)] = new PointBinaryTranslationGeneration<P3Int16>(expectedLen: 6);
            this._typeGenerations[typeof(P2FloatType)] = new PointBinaryTranslationGeneration<P2Float>(expectedLen: 8);
            this._typeGenerations[typeof(StringType)] = new StringBinaryTranslationGeneration()
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(FilePathType)] = new FilePathBinaryTranslationGeneration();
            this._typeGenerations[typeof(UInt8Type)] = new ByteBinaryTranslationGeneration();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>(expectedLen: 2);
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>(expectedLen: 4);
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>(expectedLen: 8);
            this._typeGenerations[typeof(FormIDType)] = new PrimitiveBinaryTranslationGeneration<FormID>(expectedLen: 4)
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(FormKeyType)] = new FormKeyBinaryTranslationGeneration();
            this._typeGenerations[typeof(ModKeyType)] = new ModKeyBinaryTranslationGeneration();
            this._typeGenerations[typeof(RecordTypeType)] = new RecordTypeBinaryTranslationGeneration();
            this._typeGenerations[typeof(FormLinkType)] = new FormLinkBinaryTranslationGeneration();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ArrayType)] = new ArrayBinaryTranslationGeneration();
            this._typeGenerations[typeof(DictType)] = new DictBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new ByteArrayBinaryTranslationGeneration();
            this._typeGenerations[typeof(BufferType)] = new BufferBinaryTranslationGeneration();
            this._typeGenerations[typeof(DataType)] = new DataBinaryTranslationGeneration();
            this._typeGenerations[typeof(ColorType)] = new ColorBinaryTranslationGeneration()
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(ZeroType)] = new ZeroBinaryTranslationGeneration();
            this._typeGenerations[typeof(NothingType)] = new NothingBinaryTranslationGeneration();
            this._typeGenerations[typeof(CustomLogic)] = new CustomLogicTranslationGeneration();
            this._typeGenerations[typeof(GenderedType)] = new GenderedTypeBinaryTranslationGeneration();
            this._typeGenerations[typeof(BreakType)] = new BreakBinaryTranslationGeneration();
            this._typeGenerations[typeof(MarkerType)] = new MarkerBinaryTranslationGeneration();
            APILine[] modAPILines = new APILine[]
            {
                new APILine(
                    nicknameKey: "GroupMask",
                    resolutionString: "GroupMask? importMask = null",
                    when: (obj, dir) => obj.GetObjectType() == ObjectType.Mod)
            };
            var modKey = new APILine(
                nicknameKey: "ModKey",
                resolutionString: "ModKey modKey",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var modKeyWriter = new APILine(
                nicknameKey: "ModKeyWriter",
                resolutionString: "ModKey modKey",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Reader) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var modKeyOptional = new APILine(
                nicknameKey: "ModKeyOptional",
                resolutionString: "ModKey? modKeyOverride = null",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var writeParamOptional = new APILine(
                nicknameKey: "WriteParamOptional",
                resolutionString: $"{nameof(BinaryWriteParameters)}? param = null",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Reader) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var stringsReadParamOptional = new APILine(
                nicknameKey: "StringsParamsOptional",
                resolutionString: $"{nameof(StringsReadParameters)}? stringsParam = null",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod
                        && obj.GetObjectData().UsesStringFiles;
                });
            var recordInfoCache = new APILine(
                nicknameKey: "RecordInfoCache",
                resolutionString: $"{nameof(RecordInfoCache)} infoCache",
                when: (obj, dir) =>
                {
                    if (dir != TranslationDirection.Reader) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var parallel = new APILine(
                nicknameKey: "Parallel",
                resolutionString: "bool parallel = true",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var recTypeConverter = new APILine(
                "RecordTypeConverter",
                $"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine("MutagenWriter", "MutagenWriter writer") },
                    optionalAPI: modAPILines,
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(modKeyWriter, "modKey"),
                        CustomMethodAPI.FactoryPrivate(recTypeConverter, "null"),
                        CustomMethodAPI.FactoryPublic(writeParamOptional),
                    }),
                readerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine("MutagenFrame", "MutagenFrame frame") },
                    optionalAPI: modAPILines,
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(modKeyWriter, "modKey"),
                        CustomMethodAPI.FactoryPrivate(recTypeConverter, "null"),
                        CustomMethodAPI.FactoryPublic(writeParamOptional),
                    }));
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Path", "string path") },
                        customAPI: new CustomMethodAPI[]
                        {
                        },
                        optionalAPI: modKeyOptional
                            .AndSingle(writeParamOptional)
                            .And(modAPILines)
                            .And(stringsReadParamOptional)
                            .And(parallel)
                            .ToArray()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromPathOut,
                        ConvertFromPathIn),
                    When = (o) => o.GetObjectType() == ObjectType.Mod
                });
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Stream", "Stream stream") },
                        customAPI: new CustomMethodAPI[]
                        {
                            CustomMethodAPI.FactoryPublic(modKey),
                            CustomMethodAPI.FactoryPublic(recordInfoCache),
                            CustomMethodAPI.FactoryPublic(writeParamOptional),
                        },
                        optionalAPI: modAPILines
                            .And(parallel)
                            .ToArray()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromStreamOut,
                        ConvertFromStreamIn),
                    When = (o) => o.GetObjectType() == ObjectType.Mod
                });
            this.CustomLogic = new CustomLogicTranslationGeneration() { Module = this };
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
        }

        private void ConvertFromStreamOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("var modKey = item.ModKey;");
            using (var args = new ArgsWrapper(fg,
                $"using (var writer = new MutagenWriter",
                suffixLine: ")",
                semiColon: false))
            {
                args.AddPassArg("stream");
                args.Add($"new {nameof(WritingBundle)}(item.GameMode)");
                args.Add("dispose: false");
            }
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
            }
        }

        private void ConvertFromStreamIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(stream, {nameof(GameMode)}.{obj.GetObjectData().GameMode}))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var frame = new MutagenFrame(reader);");
                fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = infoCache;");
                fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
            }
        }

        public override async Task GenerateInTranslationWriteClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateCustomWritePartials(obj, fg);
            GenerateCustomBinaryEndWritePartial(obj, fg);
            await GenerateWriteExtras(obj, fg);
            await base.GenerateInTranslationWriteClass(obj, fg);
        }

        public override async Task GenerateInVoid(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInVoid(obj, fg);
            using (new NamespaceWrapper(fg, obj.InternalNamespace))
            {
                await GenerateImportWrapper(obj, fg);
            }
        }

        public override async Task GenerateInTranslationCreateClass(ObjectGeneration obj, FileGeneration fg)
        {
            await GenerateCreateExtras(obj, fg);
            GenerateCustomCreatePartials(obj, fg);
            GenerateCustomBinaryEndCreatePartial(obj, fg);
            await base.GenerateInTranslationCreateClass(obj, fg);
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (!obj.Abstract && obj.GetObjectType() != ObjectType.Mod)
            {
                using (var args = new FunctionWrapper(fg,
                    "public static bool TryCreateFromBinary"))
                {
                    args.Add($"{nameof(MutagenFrame)} frame");
                    args.Add($"out {obj.ObjectName} item");
                    args.Add($"RecordTypeConverter? recordTypeConverter = null");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"var startPos = frame.Position;");
                    fg.AppendLine($"item = CreateFromBinary(frame, recordTypeConverter);");
                    fg.AppendLine($"return startPos != frame.Position;");
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

        private bool HasRecordTypeFields(ObjectGeneration obj)
        {
            return GetRecordTypeFields(obj).Any();
        }

        private IEnumerable<TypeGeneration> GetRecordTypeFields(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
            {
                if (field.GetFieldData().HasTrigger)
                {
                    yield return field;
                }
            }
        }

        private IEnumerable<TypeGeneration> GetEmbeddedFields(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (!field.GetFieldData().HasTrigger)
                {
                    yield return field;
                }
            }
        }

        private bool HasEmbeddedFields(ObjectGeneration obj)
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

        private async Task GenerateCreateExtras(ObjectGeneration obj, FileGeneration fg)
        {
            var data = obj.GetObjectData();
            bool typelessStruct = obj.IsTypelessStruct();

            if (await obj.IsMajorRecord())
            {
                if (data.TriggeringRecordTypes.Count == 1
                    && !obj.Abstract)
                {
                    fg.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => {obj.GetTriggeringSource()};");
                }
                else
                {
                    fg.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => throw new ArgumentException();");
                }
            }

            if ((!obj.Abstract && obj.BaseClassTrail().All((b) => b.Abstract)) || HasEmbeddedFields(obj))
            {
                var async = HasAsyncStructs(obj, self: true);
                using (var args = new FunctionWrapper(fg,
                    $"public static {Loqui.Generation.Utility.TaskReturn(async)} Fill{ModuleNickname}Structs"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add("MutagenFrame frame");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{Loqui.Generation.Utility.Await(async)}{TranslationCreateClass(obj.BaseClass)}.Fill{ModuleNickname}Structs"))
                        {
                            args.Add("item: item");
                            args.Add("frame: frame");
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
                        if (field.HasBeenSet)
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
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static {Loqui.Generation.Utility.TaskWrap(nameof(ParseResult), HasAsyncRecords(obj, self: true))} Fill{ModuleNickname}RecordTypes"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add("MutagenFrame frame");
                    if (obj.GetObjectType() == ObjectType.Subrecord)
                    {
                        args.Add($"int? lastParsed");
                    }
                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        args.Add("Dictionary<RecordType, int>? recordParseCount");
                    }
                    args.Add("RecordType nextRecordType");
                    args.Add("int contentLength");
                    if (data.ObjectType == ObjectType.Mod)
                    {
                        args.Add($"GroupMask? importMask");
                    }
                    args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                }
                using (new BraceWrapper(fg))
                {
                    var mutaObjType = obj.GetObjectType();
                    fg.AppendLine($"nextRecordType = recordTypeConverter.ConvertToStandard(nextRecordType);");
                    fg.AppendLine("switch (nextRecordType.TypeInt)");
                    using (new BraceWrapper(fg))
                    {
                        var fields = new List<(int, int, TypeGeneration Field)>();
                        foreach (var field in obj.IterateFieldIndices(
                            expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                            nonIntegrated: true))
                        {
                            var fieldData = field.Field.GetFieldData();
                            if (fieldData.GenerationTypes.Count() == 0) continue;
                            if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                            if (field.Field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }

                            if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                            fields.Add(field);
                        }

                        var doubleUsages = new Dictionary<RecordType, List<(int, int, TypeGeneration)>>();
                        foreach (var field in fields)
                        {
                            var fieldData = field.Field.GetFieldData();
                            if (fieldData.GenerationTypes.Count() > 1) continue;
                            foreach (var gen in fieldData.GenerationTypes)
                            {
                                if (gen.Key.Count() > 1) continue;
                                LoquiType loqui = gen.Value as LoquiType;
                                if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;
                                doubleUsages.TryCreateValue(gen.Key.First()).Add(field);
                            }
                        }
                        foreach (var item in doubleUsages.ToList())
                        {
                            if (item.Value.Count <= 1)
                            {
                                doubleUsages.Remove(item.Key);
                            }
                        }

                        foreach (var field in fields)
                        {
                            var fieldData = field.Field.GetFieldData();
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }
                            foreach (var gen in fieldData.GenerationTypes)
                            {
                                LoquiType loqui = gen.Value as LoquiType;
                                if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;

                                List<(int, int, TypeGeneration Field)> doubles = null;
                                if (gen.Key.Count() == 1)
                                {
                                    if (doubleUsages.TryGetValue(gen.Key.First(), out doubles))
                                    {
                                        // Means we handled earlier, break out
                                        if (doubles.Count == 0) continue;
                                    }
                                }

                                foreach (var trigger in gen.Key)
                                {
                                    fg.AppendLine($"case RecordTypeInts.{trigger.CheckedType}:");
                                }
                                using (new BraceWrapper(fg))
                                {
                                    if (doubles == null)
                                    {
                                        await GenerateLastParsedShortCircuit(
                                            obj: obj,
                                            fg: fg,
                                            field: field,
                                            doublesPotential: false,
                                            nextRecAccessor: "nextRecordType",
                                            toDo: async () =>
                                            {
                                                var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                                if (groupMask)
                                                {
                                                    fg.AppendLine($"if (importMask?.{field.Field.Name} ?? true)");
                                                }
                                                using (new BraceWrapper(fg, doIt: groupMask))
                                                {
                                                    await GenerateFillSnippet(obj, fg, gen.Value, generator, "frame");
                                                }
                                                if (groupMask)
                                                {
                                                    fg.AppendLine("else");
                                                    using (new BraceWrapper(fg))
                                                    {
                                                        fg.AppendLine("frame.Position += contentLength;");
                                                    }
                                                }
                                            });
                                    }
                                    else
                                    {
                                        fg.AppendLine($"switch (recordParseCount?.TryCreateValue(nextRecordType) ?? 0)");
                                        using (new BraceWrapper(fg))
                                        {
                                            int count = 0;
                                            foreach (var doublesField in doubles)
                                            {
                                                if (!this.TryGetTypeGeneration(doublesField.Field.GetType(), out var doubleGen))
                                                {
                                                    throw new ArgumentException("Unsupported type generator: " + doublesField.Field);
                                                }
                                                fg.AppendLine($"case {count++}:");
                                                using (new DepthWrapper(fg))
                                                {
                                                    await GenerateLastParsedShortCircuit(
                                                        obj: obj,
                                                        fg: fg,
                                                        field: doublesField,
                                                        doublesPotential: true,
                                                        nextRecAccessor: "nextRecordType",
                                                        toDo: async () =>
                                                        {
                                                            var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                                            if (groupMask)
                                                            {
                                                                fg.AppendLine($"if (importMask?.{doublesField.Field.Name} ?? true)");
                                                            }
                                                            using (new BraceWrapper(fg, doIt: groupMask))
                                                            {
                                                                await GenerateFillSnippet(obj, fg, doublesField.Field, doubleGen, "frame");
                                                            }
                                                            if (groupMask)
                                                            {
                                                                fg.AppendLine("else");
                                                                using (new BraceWrapper(fg))
                                                                {
                                                                    fg.AppendLine("frame.Position += contentLength;");
                                                                }
                                                            }
                                                        });
                                                }
                                            }
                                            fg.AppendLine($"default:");
                                            using (new DepthWrapper(fg))
                                            {
                                                fg.AppendLine($"throw new NotImplementedException();");
                                            }
                                        }
                                        doubles.Clear();
                                    }
                                }
                            }
                        }

                        if (data.EndMarkerType.HasValue)
                        {
                            fg.AppendLine($"case RecordTypeInts.{data.EndMarkerType}: // End Marker");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"frame.ReadSubrecordFrame();");
                                fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                            }
                        }
                        fg.AppendLine($"default:");
                        using (new DepthWrapper(fg))
                        {
                            bool first = true;
                            // Generic options
                            foreach (var field in obj.IterateFieldIndices())
                            {
                                var fieldData = field.Field.GetFieldData();
                                if (!fieldData.HasTrigger
                                    || fieldData.TriggeringRecordTypes.Count > 0
                                    || fieldData.GenerationTypes.Count() > 0) continue;
                                if (field.Field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                                if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + field.Field);
                                }

                                if (generator.ShouldGenerateCopyIn(field.Field))
                                {
                                    using (var args = new IfWrapper(fg, ANDs: true, first: first))
                                    {
                                        foreach (var trigger in fieldData.TriggeringRecordAccessors)
                                        {
                                            args.Add($"nextRecordType.Equals({trigger})");
                                        }
                                    }
                                    first = false;
                                    using (new BraceWrapper(fg))
                                    {
                                        await GenerateFillSnippet(obj, fg, field.Field, generator, "frame");
                                        fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                    }
                                }
                            }

                            // Default case
                            if (obj.GetObjectData().CustomRecordFallback)
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return CustomRecordFallback"))
                                {
                                    args.AddPassArg($"item");
                                    args.AddPassArg("frame");
                                    if (obj.GetObjectType() == ObjectType.Subrecord)
                                    {
                                        args.AddPassArg($"lastParsed");
                                    }
                                    args.AddPassArg("recordParseCount");
                                    args.AddPassArg("nextRecordType");
                                    args.AddPassArg("contentLength");
                                    args.AddPassArg($"recordTypeConverter");
                                }
                            }
                            else if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasRecordTypeFields(b)))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return {Loqui.Generation.Utility.Await(HasAsyncRecords(obj, self: false))}{TranslationCreateClass(obj.BaseClass)}.Fill{ModuleNickname}RecordTypes"))
                                {
                                    args.AddPassArg("item");
                                    args.AddPassArg("frame");
                                    if (obj.BaseClass.GetObjectType() == ObjectType.Subrecord)
                                    {
                                        args.AddPassArg($"lastParsed");
                                    }
                                    if (obj.GetObjectType() != ObjectType.Mod)
                                    {
                                        args.AddPassArg("recordParseCount");
                                    }
                                    args.AddPassArg("nextRecordType");
                                    args.AddPassArg("contentLength");
                                    if (data.BaseRecordTypeConverter?.FromConversions.Count > 0)
                                    {
                                        args.Add($"recordTypeConverter: {obj.RegistrationName}.BaseConverter");
                                    }
                                }
                            }
                            else
                            {
                                var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                if (mutaObjType == ObjectType.Subrecord)
                                {
                                    fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                }
                                else if (failOnUnknown)
                                {
                                    fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                                }
                                else
                                {
                                    string addString;
                                    switch (obj.GetObjectType())
                                    {
                                        case ObjectType.Mod:
                                            addString = null;
                                            break;
                                        case ObjectType.Subrecord:
                                        case ObjectType.Record:
                                            addString = $" + frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}";
                                            break;
                                        case ObjectType.Group:
                                            addString = $" + frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.MajorConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}";
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                    fg.AppendLine($"frame.Position += contentLength{addString};");
                                    fg.AppendLine($"return default(int?);");
                                }
                            }
                        }
                    }
                }
                fg.AppendLine();
            }

            foreach (var field in obj.Fields)
            {
                if (field.GetFieldData().CustomVersion != null)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"static partial void {field.Name}CustomVersionParse"))
                    {
                        args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                        args.Add($"{nameof(MutagenFrame)} frame");
                        args.Add($"{nameof(RecordType)} nextRecordType");
                        args.Add($"int contentLength");
                        args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                    }
                    fg.AppendLine();
                }
            }
        }

        private async Task GenerateBinaryOverlayCreates(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            var objData = obj.GetObjectData();
            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"ReadOnlyMemorySlice<byte> bytes");
                args.Add($"ModKey modKey");
                if (objData.UsesStringFiles)
                {
                    args.Add($"{nameof(IStringsFolderLookup)}? stringsLookup = null");
                }
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({nameof(GameMode)}.{obj.GetObjectData().GameMode});");
                fg.AppendLine($"meta.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenMemoryReadStream)}(bytes, meta));");
                if (objData.UsesStringFiles)
                {
                    fg.AppendLine($"meta.{nameof(ParsingBundle.StringsLookup)} = stringsLookup;");
                }
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    args.Add(subFg =>
                    {
                        using (var subArgs = new FunctionWrapper(subFg,
                            $"stream: new {nameof(MutagenMemoryReadStream)}"))
                        {
                            subArgs.Add("data: bytes");
                            subArgs.Add($"metaData: meta");
                        }
                    });
                    args.AddPassArg("modKey");
                    args.Add("shouldDispose: false");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"string path");
                args.Add($"ModKey? modKeyOverride = null");
                if (objData.UsesStringFiles)
                {
                    args.Add($"{nameof(StringsReadParameters)}? stringsParam = null");
                }
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    args.AddPassArg("path");
                    args.Add("modKeyOverride ?? ModKey.Factory(Path.GetFileName(path))");
                    if (objData.UsesStringFiles)
                    {
                        args.AddPassArg("stringsParam");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"{nameof(IMutagenReadStream)} stream");
                args.Add($"ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    args.AddPassArg($"stream");
                    args.AddPassArg("modKey");
                    args.Add("shouldDispose: false");
                }
            }
            fg.AppendLine();
        }

        private static async Task GenerateLastParsedShortCircuit(
            ObjectGeneration obj,
            FileGeneration fg,
            (int PublicIndex, int InternalIndex, TypeGeneration Field) field,
            bool doublesPotential,
            Accessor nextRecAccessor,
            Func<Task> toDo)
        {
            var dataSet = field.Field as DataType;
            var typelessStruct = obj.IsTypelessStruct();
            if (typelessStruct && field.Field.GetFieldData().IsTriggerForObject)
            {
                if (dataSet != null)
                {
                    fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){dataSet.SubFields.Last().IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                }
                else if (field.Field is CustomLogic)
                {
                    var objFields = obj.IterateFieldIndices(nonIntegrated: false).ToList();
                    var nextField = objFields.FirstOrDefault((i) => i.InternalIndex > field.InternalIndex);
                    var prevField = objFields.LastOrDefault((i) => i.InternalIndex < field.InternalIndex);
                    if (nextField.Field != null)
                    {
                        fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){nextField.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                    }
                    else if (prevField.Field != null)
                    {
                        fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){prevField.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                    }
                }
                else if (!(field.Field is MarkerType))
                {
                    fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){field.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                }
            }
            await toDo();
            if (dataSet != null)
            {
                fg.AppendLine($"return (int){dataSet.SubFields.Last(f => f.IntegrateField && f.Enabled).IndexEnumName};");
            }
            else if (field.Field is CustomLogic)
            {
                fg.AppendLine($"return {(typelessStruct ? "lastParsed" : "null")};");
            }
            else if (field.Field is MarkerType marker)
            {
                if (marker.EndMarker)
                {
                    fg.AppendLine($"return {nameof(ParseResult)}.{nameof(ParseResult.Stop)};");
                }
                else
                {
                    if (doublesPotential)
                    {
                        fg.AppendLine($"return new {nameof(ParseResult)}(default(int?), {nextRecAccessor});");
                    }
                    else
                    {
                        fg.AppendLine($"return default(int?);");
                    }
                }
            }
            else
            {
                if (doublesPotential)
                {
                    fg.AppendLine($"return new {nameof(ParseResult)}((int){field.Field.IndexEnumName}, {nextRecAccessor});");
                }
                else
                {
                    fg.AppendLine($"return (int){field.Field.IndexEnumName};");
                }
            }
        }

        private void GenerateCustomBinaryEndWritePartial(ObjectGeneration obj, FileGeneration fg)
        {
            var data = obj.GetObjectData();
            if (data.CustomBinaryEnd == CustomEnd.Off) return;
            using (var args = new ArgsWrapper(fg,
                $"static partial void CustomBinaryEndExport"))
            {
                args.Add("MutagenWriter writer");
                args.Add($"{obj.Interface(internalInterface: true, getter: true)} obj");
            }
            using (var args = new FunctionWrapper(fg,
                $"public static void CustomBinaryEndExportInternal"))
            {
                args.Add("MutagenWriter writer");
                args.Add($"{obj.Interface(internalInterface: true, getter: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"CustomBinaryEndExport"))
                {
                    args.Add($"writer: writer");
                    args.Add($"obj: obj");
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
                    args.Add("MutagenFrame frame");
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} obj");
                }
                using (var args = new FunctionWrapper(fg,
                    $"public static void CustomBinaryEndImportPublic"))
                {
                    args.Add("MutagenFrame frame");
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} obj");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"CustomBinaryEndImport"))
                    {
                        args.Add("frame: frame");
                        args.Add($"obj: obj");
                    }
                }
            }
        }

        private void GenerateStructStateSubscriptions(ObjectGeneration obj, FileGeneration fg)
        {
            if (!obj.StructHasBeenSet()) return;
            List<TypeGeneration> affectedFields = new List<TypeGeneration>();
            foreach (var field in obj.IterateFields())
            {
                var data = field.GetFieldData();
                if (data.HasTrigger) break;
                if (field.HasBeenSet)
                {
                    affectedFields.Add(field);
                    continue;
                }
            }
        }

        private async Task GenerateFillSnippet(ObjectGeneration obj, FileGeneration fg, TypeGeneration field, BinaryTranslationGeneration generator, string frameAccessor)
        {
            if (field is DataType set)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
                fg.AppendLine($"var dataFrame = {frameAccessor}.SpawnWithLength(contentLength);");
                if (set.HasBeenSet)
                {
                    fg.AppendLine($"if (!dataFrame.Complete)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"item.{set.StateName} = {obj.ObjectName}.{set.EnumName}.Has;");
                    }
                }
                bool isInRange = false;
                foreach (var subField in set.IterateFieldsWithMeta())
                {
                    if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var subGenerator))
                    {
                        throw new ArgumentException("Unsupported type generator: " + subField.Field);
                    }

                    if (!subGenerator.ShouldGenerateCopyIn(subField.Field)) continue;
                    if (subField.BreakIndex != -1)
                    {
                        fg.AppendLine($"if (dataFrame.Complete)");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"item.{set.StateName} |= {obj.ObjectName}.{set.EnumName}.Break{subField.BreakIndex};");
                            string enumName = null;
                            for (int i = subField.FieldIndex - 1; i >= 0; i--)
                            {
                                if (!set.SubFields.TryGet(i, out var prevField)) continue;
                                if (!prevField.IntegrateField) continue;
                                enumName = prevField.IndexEnumName;
                                break;
                            }
                            if (enumName != null)
                            {
                                enumName = $"(int){enumName}";
                            }
                            fg.AppendLine($"return {enumName ?? "default(int?)"};");
                        }
                    }
                    if (subField.Range != null && !isInRange)
                    {
                        isInRange = true;
                        fg.AppendLine($"if (dataFrame.TotalLength > {subField.Range.DataSetSizeMin})");
                        fg.AppendLine("{");
                        fg.Depth++;
                        fg.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Range{subField.RangeIndex};");
                    }
                    if (subField.Range == null && isInRange)
                    {
                        isInRange = false;
                        fg.Depth--;
                        fg.AppendLine("}");
                    }
                    await GenerateFillSnippet(obj, fg, subField.Field, subGenerator, "dataFrame");
                }
                if (isInRange)
                {
                    isInRange = false;
                    fg.AppendLine("}");
                    fg.Depth--;
                }
                return;
            }

            var data = field.GetFieldData();
            switch (data.Binary)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    CustomLogic.GenerateFill(
                        fg,
                        field,
                        frameAccessor,
                        isAsync: false);
                    return;
                default:
                    throw new NotImplementedException();
            }

            if (data.CustomVersion != null)
            {
                fg.AppendLine($"if (item.FormVersion <= {data.CustomVersion})");
                using (new BraceWrapper(fg))
                {
                    await GenerateLastParsedShortCircuit(
                        obj: obj,
                        fg: fg,
                        field: field.GetIndexData(),
                        doublesPotential: false,
                        nextRecAccessor: "nextRecordType",
                        toDo: async () =>
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{field.Name}CustomVersionParse"))
                            {
                                args.AddPassArg($"item");
                                args.AddPassArg($"frame");
                                args.AddPassArg($"nextRecordType");
                                args.AddPassArg($"contentLength");
                                args.AddPassArg($"recordTypeConverter");
                            }
                        });
                }
            }

            if (data.MarkerType != null && data.RecordType != null)
            {
                // Skip marker
                fg.AppendLine("frame.Position += frame.MetaData.SubConstants.HeaderLength + contentLength;");
                // read in target record type.
                fg.AppendLine("var nextRec = frame.MetaData.GetSubrecord(frame);");
                // Return if it's not there
                fg.AppendLine($"if (nextRec.RecordType != {obj.RecordTypeHeaderName(data.RecordType.Value)}) throw new ArgumentException(\"Marker was read but not followed by expected subrecord.\");");
                fg.AppendLine("contentLength = nextRec.RecordLength;");
            }

            if (data.OverflowRecordType.HasValue)
            {
                fg.AppendLine($"if (nextRecordType == {obj.RecordTypeHeaderName(data.OverflowRecordType.Value)})");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var overflowHeader = frame.ReadSubrecordFrame();");
                    fg.AppendLine("contentLength = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(overflowHeader.Content));");
                }
            }

            await generator.GenerateCopyIn(
                fg: fg,
                objGen: obj,
                typeGen: field,
                readerAccessor: frameAccessor,
                itemAccessor: Accessor.FromType(field, "item"),
                translationAccessor: null,
                errorMaskAccessor: null);
        }

        private void ConvertFromPathOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
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
                fg.AppendLine("var stringsWriter = param.StringsWriter ?? (EnumExt.HasFlag((int)item.ModHeader.Flags, Mutagen.Bethesda.Internals.Constants.LocalizedFlag) ? new StringsWriter(modKey, Path.Combine(Path.GetDirectoryName(path), \"Strings\")) : null);");
            }
            fg.AppendLine($"var bundle = new {nameof(WritingBundle)}(item.GameMode)");
            using (var prop = new PropertyCtorWrapper(fg))
            {
                if (obj.GetObjectData().UsesStringFiles)
                {
                    prop.Add($"{nameof(WritingBundle.StringsWriter)} = stringsWriter");
                }
            }
            fg.AppendLine("using var memStream = new MemoryTributary();");
            using (var args = new ArgsWrapper(fg,
                $"using (var writer = new MutagenWriter",
                suffixLine: ")")
            {
                SemiColon = false
            })
            {
                args.Add("memStream");
                args.Add("bundle");
                args.Add("dispose: false");
            }
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
            }
            fg.AppendLine("using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"memStream.Position = 0;");
                fg.AppendLine($"memStream.CopyTo(fs);");
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

        private void ConvertFromPathIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(path, {nameof(GameMode)}.{obj.GetObjectData().GameMode}))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var modKey = modKeyOverride ?? ModKey.Factory(Path.GetFileName(path));");
                fg.AppendLine("var frame = new MutagenFrame(reader);");
                fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenBinaryReadStream)}(path, {nameof(GameMode)}.{obj.GetObjectData().GameMode}));");
                fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                if (obj.GetObjectData().UsesStringFiles)
                {
                    fg.AppendLine("if (reader.Remaining < 12)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                    }
                    fg.AppendLine($"var flags = reader.GetInt32(offset: 8);");
                    fg.AppendLine($"if (EnumExt.HasFlag(flags, Mutagen.Bethesda.Internals.Constants.LocalizedFlag))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"frame.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.StringsLookup)} = StringsFolderLookupOverlay.TypicalFactory(path, stringsParam, modKey);");
                    }
                }
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
            }
        }

        protected override bool GenerateMainCreate(ObjectGeneration obj)
        {
            var data = obj.GetObjectData();
            return !data.CustomBinary;
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
                        args.Add("reader: frame.Reader");
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
                                args.Add($"frame: frame.SpawnWithLength(customLen + frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)})");
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
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine($"var ret = new {obj.Name}{obj.GetGenericTypes(MaskType.Normal)}(modKey);");
                }
                else
                {
                    fg.AppendLine($"var ret = new {obj.Name}{obj.GetGenericTypes(MaskType.Normal)}();");
                }
            }
        }

        protected override async Task GenerateCopyInSnippet(ObjectGeneration obj, FileGeneration fg, Accessor accessor)
        {
            var data = obj.GetObjectData();

            bool typelessStruct = obj.IsTypelessStruct();
            ObjectType objType = obj.GetObjectType();

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                fg.AppendLine($"frame.Reader.MetaData.{nameof(ParsingBundle.MasterReferences)} = new {nameof(MasterReferenceReader)}(modKey, {accessor}.ModHeader.MasterReferences);");
            }

            if (await obj.IsMajorRecord())
            {
                bool async = this.HasAsync(obj, self: true);
                using (var args = new ArgsWrapper(fg,
                    $"{Loqui.Generation.Utility.Await(async)}Utility{(async ? "Async" : null)}Translation.MajorRecordParse<{obj.Interface(getter: false, internalInterface: true)}>"))
                {
                    args.Add($"record: {accessor}");
                    args.Add($"frame: frame");
                    args.Add($"recordTypeConverter: recordTypeConverter");
                    args.Add($"fillStructs: {TranslationCreateClass(obj)}.FillBinaryStructs");
                    args.Add($"fillTyped: {TranslationCreateClass(obj)}.FillBinaryRecordTypes");
                }
                if (data.CustomBinaryEnd != CustomEnd.Off)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{Loqui.Generation.Utility.Await(data.CustomBinaryEnd == CustomEnd.Async)}{this.TranslationCreateClass(obj)}.CustomBinaryEndImport{(await this.AsyncImport(obj) ? null : "Public")}"))
                    {
                        args.Add("frame: frame");
                        args.Add($"obj: {accessor}");
                    }
                }
            }
            else
            {
                IEnumerable<RecordType> recordTypes = await obj.GetTriggeringRecordTypes();
                var frameMod = (objType != ObjectType.Subrecord || recordTypes.Any())
                    && objType != ObjectType.Mod;
                if (frameMod)
                {
                    switch (objType)
                    {
                        case ObjectType.Subrecord:
                            if (obj.TryGetRecordType(out var recType))
                            {
                                if (obj.Fields.Any(f => f.GetFieldData().HasTrigger))
                                {
                                    fg.AppendLine($"frame.Position += frame.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength;");
                                }
                                else
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"frame = frame.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseSubrecord",
                                        suffixLine: ")"))
                                    {
                                        args.Add("frame.Reader");
                                        args.Add($"recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})");
                                    }
                                }
                            }
                            break;
                        case ObjectType.Record:
                            using (var args = new ArgsWrapper(fg,
                                $"frame = frame.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseRecord",
                                suffixLine: ")"))
                            {
                                args.Add("frame.Reader");
                                args.Add($"recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})");
                            }
                            break;
                        case ObjectType.Group:
                            break;
                        case ObjectType.Mod:
                        default:
                            throw new NotImplementedException();
                    }
                }
                bool async = this.HasAsync(obj, self: true);
                var utilityTranslation = $"{Loqui.Generation.Utility.Await(async)}Utility{(async ? "Async" : null)}Translation";
                switch (objType)
                {
                    case ObjectType.Subrecord:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.SubrecordParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.Add("frame: frame");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj)
                                || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    case ObjectType.Record:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.RecordParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.Add("frame: frame");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj)
                                || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    case ObjectType.Group:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.GroupParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.Add("frame: frame");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj)
                                || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    case ObjectType.Mod:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.ModParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.Add("frame: frame");
                            args.Add("importMask: importMask");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                GenerateStructStateSubscriptions(obj, fg);
                if (data.CustomBinaryEnd != CustomEnd.Off)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{Loqui.Generation.Utility.Await(data.CustomBinaryEnd == CustomEnd.Async)}{this.TranslationCreateClass(obj)}.CustomBinaryEndImportPublic"))
                    {
                        args.Add("frame: frame");
                        args.Add($"obj: {accessor}");
                    }
                }
            }
        }

        protected override void GenerateWriteSnippet(ObjectGeneration obj, FileGeneration fg)
        {
            var data = obj.GetObjectData();
            var hasRecType = obj.TryGetRecordType(out var recType);
            if (hasRecType)
            {
                using (var args = new ArgsWrapper(fg,
                    $"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Header)}",
                    ")",
                    semiColon: false))
                {
                    args.Add("writer: writer");
                    args.Add($"record: recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})");
                    args.Add($"type: Mutagen.Bethesda.Binary.{nameof(ObjectType)}.{obj.GetObjectType()}");
                }
            }
            using (new BraceWrapper(fg, doIt: hasRecType))
            {
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine("param ??= BinaryWriteParameters.Default;");
                    fg.AppendLine($"writer.{nameof(MutagenWriter.MetaData)}.{nameof(WritingBundle.MasterReferences)} = {nameof(UtilityTranslation)}.{nameof(UtilityTranslation.ConstructWriteMasters)}(item, param);");
                }
                if (HasEmbeddedFields(obj))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"WriteEmbedded"))
                    {
                        args.AddPassArg($"item");
                        args.AddPassArg($"writer");
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
                            args.AddPassArg($"writer");
                        }
                    }
                }
                if (HasRecordTypeFields(obj))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"WriteRecordTypes"))
                    {
                        args.AddPassArg($"item");
                        args.AddPassArg($"writer");
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            args.AddPassArg($"importMask");
                            args.AddPassArg($"modKey");
                            args.AddPassArg($"param");
                        }
                        args.AddPassArg($"recordTypeConverter");
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
                            args.AddPassArg($"writer");
                            args.AddPassArg($"recordTypeConverter");
                        }
                    }
                }
            }
            if (data.CustomBinaryEnd != CustomEnd.Off)
            {
                using (var args = new ArgsWrapper(fg,
                    $"CustomBinaryEndExportInternal"))
                {
                    args.AddPassArg($"writer");
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
                    args.Add("MutagenWriter writer");
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
                                args.AddPassArg("writer");
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
                                    writerAccessor: "writer");
                                continue;
                            }
                            if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                            {
                                if (!field.IntegrateField) continue;
                                throw new ArgumentException("Unsupported type generator: " + field);
                            }
                            generator.GenerateWrite(
                                fg: fg,
                                objGen: obj,
                                typeGen: field,
                                writerAccessor: "writer",
                                itemAccessor: Accessor.FromType(field, "item"),
                                translationAccessor: null,
                                errorMaskAccessor: null,
                                converterAccessor: null);
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
                    args.Add("MutagenWriter writer");
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        args.Add($"GroupMask? importMask");
                        args.Add($"ModKey modKey");
                        args.Add($"{nameof(BinaryWriteParameters)} param");
                    }
                    args.Add("RecordTypeConverter? recordTypeConverter");
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
                                args.AddPassArg("writer");
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
                            if (!(field is CustomLogic custom))
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
                                    writerAccessor: "writer");
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
                                if (dataType.HasBeenSet)
                                {
                                    fg.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Has))");
                                }
                                using (new BraceWrapper(fg, doIt: dataType.HasBeenSet))
                                {
                                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(fieldData.RecordType.Value)})))");
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
                                                        args.Add("writer: writer");
                                                        args.Add("item: item");
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
                                            subGenerator.GenerateWrite(
                                                fg: fg,
                                                objGen: obj,
                                                typeGen: subField.Field,
                                                writerAccessor: "writer",
                                                    translationAccessor: null,
                                                itemAccessor: Accessor.FromType(subField.Field, "item"),
                                                errorMaskAccessor: null,
                                                converterAccessor: "recordTypeConverter");
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

                                // Custom Modheader insert
                                if (loqui != null
                                    && loqui.Name == "ModHeader")
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"WriteModHeader"))
                                    {
                                        args.Add("mod: item");
                                        args.AddPassArg("writer");
                                        args.AddPassArg("modKey");
                                    }
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
                                        writerAccessor: "writer",
                                        itemAccessor: accessor,
                                        translationAccessor: null,
                                        errorMaskAccessor: null,
                                        converterAccessor: "recordTypeConverter");
                                }
                            }
                        };

                        if (fieldData.CustomVersion == null)
                        {
                            generate();
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
                                    args.AddPassArg($"writer");
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
                        fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {obj.RecordTypeHeaderName(data.EndMarkerType.Value)})) {{ }} // End Marker");
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
                        args.Add($"{nameof(MutagenWriter)} writer");
                        args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter");
                    }
                    fg.AppendLine();
                }
            }
        }

        protected async Task GenerateImportWrapper(ObjectGeneration obj, FileGeneration fg)
        {
            var objData = obj.GetObjectData();
            if (objData.BinaryOverlay != BinaryGenerationType.Normal) return;

            var dataAccessor = new Accessor("_data");
            var packageAccessor = new Accessor("_package");
            var metaAccessor = new Accessor($"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}");
            var needsMasters = await obj.GetNeedsMasters();
            var anyHasRecordTypes = (await obj.EntireClassTree()).Any(c => HasRecordTypeFields(c));

            using (var args = new ClassWrapper(fg, $"{BinaryOverlayClass(obj)}"))
            {
                args.Partial = true;
                args.BaseClass = obj.HasLoquiBaseObject ? BinaryOverlayClass(obj.BaseClass) : (obj.GetObjectType() == ObjectType.Mod ? null : nameof(BinaryOverlay));
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    args.Interfaces.Add($"I{obj.Name}DisposableGetter");
                }
                else
                {
                    args.Interfaces.Add(obj.Interface(getter: true, internalInterface: true));
                }
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
            }
            using (new BraceWrapper(fg))
            {
                await obj.GenerateRouting(fg, getterOnly: true);
                obj.GenerateGetterInterfaceImplementations(fg);
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine($"public {nameof(GameMode)} GameMode => {nameof(GameMode)}.{obj.GetObjectData().GameMode};");
                    fg.AppendLine($"IReadOnlyCache<T, FormKey> {nameof(IModGetter)}.GetGroupGetter<T>() => this.GetGroupGetter<T>();");
                    fg.AppendLine($"void IModGetter.WriteToBinary(string path, {nameof(BinaryWriteParameters)}? param) => this.WriteToBinary(path, importMask: null, param: param);");
                    fg.AppendLine($"void IModGetter.WriteToBinaryParallel(string path, {nameof(BinaryWriteParameters)}? param) => this.WriteToBinaryParallel(path, param: param);");
                    fg.AppendLine($"IReadOnlyList<{nameof(IMasterReferenceGetter)}> {nameof(IModGetter)}.MasterReferences => this.ModHeader.MasterReferences;");
                    fg.AppendLine($"public bool CanUseLocalization => {(obj.GetObjectData().UsesStringFiles ? "true" : "false")};");
                }

                if (obj.GetObjectType() == ObjectType.Mod
                    || (await LinkModule.HasLinks(obj, includeBaseClass: false) != LinkModule.LinkCase.No))
                {
                    await LinkModule.GenerateInterfaceImplementation(obj, fg);
                }

                if (await MajorRecordEnumerationModule.HasMajorRecordsInTree(obj, includeBaseClass: false) != MajorRecordEnumerationModule.Case.No)
                {
                    MajorRecordEnumerationModule.GenerateClassImplementation(obj, fg, onlyGetter: true);
                }

                foreach (var transl in obj.ProtoGen.Gen.GenerationModules
                    .WhereCastable<IGenerationModule, ITranslationModule>())
                {
                    if (transl.DoTranslationInterface(obj))
                    {
                        await transl.GenerateTranslationInterfaceImplementation(obj, fg);
                    }
                }

                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine($"public {nameof(ModKey)} ModKey {{ get; }}");
                    fg.AppendLine($"private readonly {nameof(BinaryOverlayFactoryPackage)} _package;");
                    fg.AppendLine($"private readonly {nameof(IBinaryReadStream)} _data;");
                    fg.AppendLine($"private readonly bool _shouldDispose;");

                    fg.AppendLine("public void Dispose()");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("if (!_shouldDispose) return;");
                        fg.AppendLine("_data.Dispose();");
                    }
                }

                if (obj.GetObjectData().MajorRecordFlags)
                {
                    fg.AppendLine($"public {obj.ObjectName}.MajorFlag MajorFlags => ({obj.ObjectName}.MajorFlag)this.MajorRecordFlagsRaw;");
                }

                fg.AppendLine();
                if (obj.Fields.Any(f => f is BreakType))
                {
                    fg.AppendLine($"public {obj.ObjectName}.{VersioningModule.VersioningEnumName} {VersioningModule.VersioningFieldName} {{ get; private set; }}");
                }

                int? totalPassedLength = 0;
                await foreach (var lengths in IteratePassedLengths(obj, forOverlay: true))
                {
                    if (totalPassedLength != null)
                    {
                        if (lengths.CurLength == null)
                        {
                            totalPassedLength = null;
                        }
                        else
                        {
                            totalPassedLength = lengths.CurLength;
                        }
                    }
                    if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen))
                    {
                        if (!lengths.Field.IntegrateField) continue;
                        throw new NotImplementedException();
                    }
                    using (new RegionWrapper(fg, lengths.Field.Name)
                    {
                        AppendExtraLine = false,
                        SkipIfOnlyOneLine = true
                    })
                    {
                        var data = lengths.Field.GetFieldData();
                        switch (data.BinaryOverlayFallback)
                        {
                            case BinaryGenerationType.NoGeneration:
                                continue;
                            default:
                                break;
                        }
                        await typeGen.GenerateWrapperFields(
                            fg,
                            obj,
                            lengths.Field,
                            dataAccessor,
                            lengths.PassedLength,
                            lengths.PassedAccessor);
                        if (!data.HasTrigger)
                        {
                            if (lengths.CurLength == null)
                            {
                                fg.AppendLine($"protected int {lengths.Field.Name}EndingPos;");
                                if (data.BinaryOverlayFallback == BinaryGenerationType.Custom)
                                {
                                    fg.AppendLine($"partial void Custom{lengths.Field.Name}EndPos();");
                                }
                            }
                        }
                    }
                }

                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"partial void CustomFactoryEnd"))
                    {
                        args.Add($"{nameof(OverlayStream)} stream");
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                        args.Add($"int offset");
                    }
                    if (objData.CustomBinaryEnd != CustomEnd.Off)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"partial void CustomEnd"))
                        {
                            args.Add($"{nameof(OverlayStream)} stream");
                            args.Add($"int finalPos");
                            args.Add($"int offset");
                        }
                    }
                    fg.AppendLine();
                    using (var args = new ArgsWrapper(fg,
                        $"partial void CustomCtor"))
                    {
                    }
                }

                using (var args = new FunctionWrapper(fg,
                    $"protected {BinaryOverlayClassName(obj)}"))
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        args.Add($"{nameof(IMutagenReadStream)} stream");
                        args.Add("ModKey modKey");
                        args.Add($"bool shouldDispose");
                    }
                    else
                    {
                        args.Add($"ReadOnlyMemorySlice<byte> bytes");
                        args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                    }
                }
                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    using (new DepthWrapper(fg))
                    {
                        using (var args = new FunctionWrapper(fg,
                            ": base"))
                        {
                            args.AddPassArg("bytes");
                            args.AddPassArg($"package");
                        }
                    }
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        fg.AppendLine("this.ModKey = modKey;");
                        fg.AppendLine("this._data = stream;");
                        using (var args = new ArgsWrapper(fg,
                            $"this._package = new {nameof(BinaryOverlayFactoryPackage)}"))
                        {
                            args.Add($"stream.{nameof(IMutagenReadStream.MetaData)}");
                        }
                        fg.AppendLine($"this._package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)} = new {nameof(MasterReferenceReader)}(modKey);");
                        fg.AppendLine("this._shouldDispose = shouldDispose;");
                    }
                    foreach (var field in obj.IterateFields(
                        expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                        nonIntegrated: true))
                    {
                        if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen)) continue;
                        typeGen.GenerateWrapperCtor(
                            fg: fg,
                            objGen: obj,
                            typeGen: field);
                    }
                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.CustomCtor"))
                        {
                        }
                    }
                }
                fg.AppendLine();

                if (!obj.Abstract)
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        using (var args = new FunctionWrapper(fg,
                            $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                        {
                            args.Add($"ReadOnlyMemorySlice<byte> data");
                            args.Add("ModKey modKey");
                            if (objData.UsesStringFiles)
                            {
                                args.Add($"{nameof(IStringsFolderLookup)}? stringsLookup = null");
                            }
                        }
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"return {obj.Name}Factory"))
                            {
                                fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({nameof(GameMode)}.{obj.GetObjectData().GameMode});");
                                fg.AppendLine($"meta.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenMemoryReadStream)}(data, meta));");
                                if (objData.UsesStringFiles)
                                {
                                    fg.AppendLine($"meta.{nameof(ParsingBundle.StringsLookup)} = stringsLookup;");
                                }
                                args.Add(subFg =>
                                {
                                    using (var subArgs = new FunctionWrapper(subFg,
                                        $"stream: new {nameof(MutagenMemoryReadStream)}"))
                                    {
                                        subArgs.AddPassArg("data");
                                        subArgs.Add($"metaData: meta");
                                    }
                                });
                                args.AddPassArg("modKey");
                                args.Add("shouldDispose: false");
                            }
                        }
                        fg.AppendLine();

                        using (var args = new FunctionWrapper(fg,
                            $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                        {
                            args.Add($"string path");
                            args.Add("ModKey modKey");
                            if (objData.UsesStringFiles)
                            {
                                args.Add($"{nameof(StringsReadParameters)}? stringsParam = null");
                            }
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({nameof(GameMode)}.{obj.GetObjectData().GameMode})");
                            using (new BraceWrapper(fg) { AppendSemicolon = true })
                            {
                                fg.AppendLine($"{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenBinaryReadStream)}(path, {nameof(GameMode)}.{obj.GetObjectData().GameMode}))");
                            }
                            using (var args = new ArgsWrapper(fg,
                                $"var stream = new {nameof(MutagenBinaryReadStream)}"))
                            {
                                args.AddPassArg("path");
                                args.Add($"metaData: meta");
                            }
                            if (objData.UsesStringFiles)
                            {
                                fg.AppendLine("if (stream.Remaining < 12)");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                                }
                                fg.AppendLine($"var flags = stream.GetInt32(offset: 8);");
                                fg.AppendLine($"if (EnumExt.HasFlag(flags, Mutagen.Bethesda.Internals.Constants.LocalizedFlag))");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine($"meta.StringsLookup = StringsFolderLookupOverlay.TypicalFactory(path, stringsParam, modKey);");
                                }
                            }

                            using (var args = new ArgsWrapper(fg,
                                $"return {obj.Name}Factory"))
                            {
                                args.AddPassArg("stream");
                                args.AddPassArg("modKey");
                                args.Add("shouldDispose: true");
                            }
                        }
                        fg.AppendLine();
                    }

                    using (var args = new FunctionWrapper(fg,
                        $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                    {
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            args.Add($"{nameof(IMutagenReadStream)} stream");
                            args.Add("ModKey modKey");
                            args.Add("bool shouldDispose");
                        }
                        else
                        {
                            args.Add($"{nameof(OverlayStream)} stream");
                            args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                            if (obj.IsVariableLengthStruct())
                            {
                                args.Add($"int finalPos");
                            }
                            args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                        }
                    }
                    using (new BraceWrapper(fg))
                    {
                        if (await obj.IsMajorRecord())
                        {
                            if (objData.CustomBinaryEnd != CustomEnd.Off)
                            {
                                fg.AppendLine("var origStream = stream;");
                            }
                            fg.AppendLine($"stream = {nameof(UtilityTranslation)}.{nameof(UtilityTranslation.DecompressStream)}(stream);");
                        }
                        if (obj.TryGetCustomRecordTypeTriggers(out var customLogicTriggers))
                        {
                            fg.AppendLine($"var nextRecord = recordTypeConverter.ConvertToCustom(package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.Get{(obj.GetObjectType() == ObjectType.Subrecord ? "Subrecord" : "MajorRecord")}(stream).RecordType);");
                            fg.AppendLine($"switch (nextRecord.TypeInt)");
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
                                        args.AddPassArg($"stream");
                                        args.Add("recordType: nextRecord");
                                        args.AddPassArg("package");
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
                            $"var ret = new {BinaryOverlayClassName(obj)}{obj.GetGenericTypes(MaskType.Normal)}"))
                        {
                            if (obj.IsTypelessStruct())
                            {
                                if (anyHasRecordTypes
                                    || totalPassedLength == null
                                    || totalPassedLength.Value == 0)
                                {
                                    args.Add($"bytes: stream.RemainingMemory");
                                }
                                else if (obj.IsVariableLengthStruct())
                                {
                                    args.Add($"bytes: stream.RemainingMemory.Slice(0, finalPos - stream.Position)");
                                }
                                else
                                {
                                    args.Add($"bytes: stream.RemainingMemory.Slice(0, 0x{totalPassedLength:X})");
                                }
                            }
                            else
                            {
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Record:
                                        args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractRecordMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})");
                                        break;
                                    case ObjectType.Group:
                                        args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractGroupMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})");
                                        break;
                                    case ObjectType.Subrecord:
                                        args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})");
                                        break;
                                    case ObjectType.Mod:
                                        args.AddPassArg($"stream");
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            if (obj.GetObjectType() == ObjectType.Mod)
                            {
                                args.AddPassArg("modKey");
                                args.AddPassArg("shouldDispose");
                            }
                            else
                            {
                                args.AddPassArg("package");
                            }
                        }
                        if (obj.IsTypelessStruct())
                        {
                            fg.AppendLine($"int offset = stream.Position;");
                        }
                        else
                        {
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Subrecord:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.Subrecord(stream.RemainingSpan).TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Record:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorRecord(stream.RemainingSpan).TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Group:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.Group(stream.RemainingSpan).TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Mod:
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }

                        // Parse struct section ending positions
                        string structPassedAccessor = null;
                        int? structPassedLen = 0;
                        await foreach (var lengths in IteratePassedLengths(
                            obj,
                            forOverlay: true,
                            includeBaseClass: true,
                            passedLenPrefix: "ret."))
                        {
                            if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen)) continue;
                            var data = lengths.Field.GetFieldData();
                            if (lengths.Field is CustomLogic) continue;
                            switch (data.BinaryOverlayFallback)
                            {
                                case BinaryGenerationType.Normal:
                                case BinaryGenerationType.Custom:
                                    break;
                                default:
                                    continue;
                            }
                            if (!data.HasTrigger)
                            {
                                structPassedLen = lengths.CurLength;
                                structPassedAccessor = lengths.CurAccessor;
                            }
                        }

                        // Parse ending positions 
                        await foreach (var lengths in IteratePassedLengths(obj, forOverlay: true, passedLenPrefix: "ret."))
                        {
                            if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen)) continue;
                            var data = lengths.Field.GetFieldData();
                            switch (data.BinaryOverlayFallback)
                            {
                                case BinaryGenerationType.Normal:
                                case BinaryGenerationType.Custom:
                                    break;
                                default:
                                    continue;
                            }
                            if (data.HasTrigger) continue;
                            var amount = await typeGen.GetPassedAmount(obj, lengths.Field);
                            if (amount != null) continue;
                            if (lengths.Field is CustomLogic) continue;
                            switch (data.BinaryOverlayFallback)
                            {
                                case BinaryGenerationType.Custom:
                                    fg.AppendLine($"ret.Custom{lengths.Field.Name}EndPos();");
                                    break;
                                case BinaryGenerationType.NoGeneration:
                                    break;
                                case BinaryGenerationType.Normal:
                                    await typeGen.GenerateWrapperUnknownLengthParse(
                                        fg,
                                        obj,
                                        lengths.Field,
                                        lengths.PassedLength,
                                        lengths.PassedAccessor);
                                    break;
                            }
                        }

                        if (anyHasRecordTypes)
                        {
                            if (obj.GetObjectType() != ObjectType.Mod
                                && !obj.IsTypelessStruct())
                            {
                                if (structPassedAccessor != null)
                                {
                                    switch (obj.GetObjectType())
                                    {
                                        case ObjectType.Subrecord:
                                            fg.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
                                            break;
                                        case ObjectType.Record:
                                            fg.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.TypeAndLengthLength;");
                                            break;
                                        case ObjectType.Group:
                                            fg.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.TypeAndLengthLength;");
                                            break;
                                        case ObjectType.Mod:
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                }
                                using (var args = new ArgsWrapper(fg,
                                    $"ret.CustomFactoryEnd"))
                                {
                                    args.AddPassArg($"stream");
                                    args.AddPassArg($"finalPos");
                                    args.AddPassArg($"offset");
                                }
                            }

                            string call;
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Subrecord:
                                case ObjectType.Record:
                                    if (obj.IsTypelessStruct())
                                    {
                                        call = $"ret.{nameof(BinaryOverlay.FillTypelessSubrecordTypes)}";
                                    }
                                    else
                                    {
                                        call = $"ret.{nameof(BinaryOverlay.FillSubrecordTypes)}";
                                    }
                                    break;
                                case ObjectType.Group:
                                    var grupLoqui = await obj.GetGroupLoquiType();
                                    if (grupLoqui.TargetObjectGeneration != null && await grupLoqui.TargetObjectGeneration.IsMajorRecord())
                                    {
                                        call = $"ret.{nameof(BinaryOverlay.FillMajorRecords)}";
                                    }
                                    else
                                    {
                                        call = $"ret.{nameof(BinaryOverlay.FillGroupRecordsForWrapper)}";
                                    }
                                    break;
                                case ObjectType.Mod:
                                    call = $"{nameof(BinaryOverlay)}.{nameof(BinaryOverlay.FillModTypes)}";
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            using (var args = new ArgsWrapper(fg,
                                $"{call}"))
                            {
                                args.Add($"stream: stream");
                                if (obj.GetObjectType() != ObjectType.Mod)
                                {
                                    if (obj.IsTypelessStruct())
                                    {
                                        args.Add($"finalPos: stream.Length");
                                    }
                                    else
                                    {
                                        args.AddPassArg($"finalPos");
                                    }
                                    args.Add($"offset: offset");
                                    args.AddPassArg($"recordTypeConverter");
                                }
                                else
                                {
                                    args.Add("package: ret._package");
                                }
                                args.Add($"fill: ret.FillRecordType");
                            }
                        }
                        else
                        {

                            if (obj.IsTypelessStruct())
                            {
                                var breaks = obj.Fields.WhereCastable<TypeGeneration, BreakType>().ToList();
                                if (breaks.Count > 0)
                                {
                                    int breakIndex = 0;
                                    await foreach (var lengths in IteratePassedLengths(obj,
                                        forOverlay: true,
                                        includeBaseClass: true,
                                        passedLenPrefix: "ret."))
                                    {
                                        if (lengths.Field is BreakType breakType)
                                        {
                                            fg.AppendLine($"if (ret._data.Length <= {lengths.PassedAccessor})");
                                            using (new BraceWrapper(fg))
                                            {
                                                fg.AppendLine($"ret.{VersioningModule.VersioningFieldName} |= {obj.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                            }
                                        }
                                    }
                                    // Not advancing stream position, but only because breaks only occur in situations
                                    // that stream position doesn't matter
                                }
                                else if (structPassedAccessor != null)
                                {
                                    fg.AppendLine($"stream.Position += {structPassedAccessor};");
                                }
                            }
                            else
                            {
                                string headerAddition = null;
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Record:
                                        headerAddition = $" + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.HeaderLength";
                                        break;
                                    case ObjectType.Group:
                                        headerAddition = $" + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.HeaderLength";
                                        break;
                                    case ObjectType.Subrecord:
                                        headerAddition = $" + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength";
                                        break;
                                    case ObjectType.Mod:
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                var breaks = obj.Fields.WhereCastable<TypeGeneration, BreakType>().ToList();
                                if (breaks.Count > 0)
                                {
                                    int breakIndex = 0;
                                    await foreach (var lengths in IteratePassedLengths(obj,
                                        forOverlay: true,
                                        includeBaseClass: true,
                                        passedLenPrefix: "ret."))
                                    {
                                        if (lengths.Field is BreakType breakType)
                                        {
                                            fg.AppendLine($"if (ret._data.Length <= {lengths.PassedAccessor})");
                                            using (new BraceWrapper(fg))
                                            {
                                                fg.AppendLine($"ret.{VersioningModule.VersioningFieldName} |= {obj.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                            }
                                        }
                                    }
                                    // Not advancing stream position, but only because breaks only occur in situations
                                    // that stream position doesn't matter
                                }
                                else if (totalPassedLength != null)
                                {
                                    fg.AppendLine($"stream.Position += 0x{totalPassedLength.Value:X}{headerAddition};");
                                }
                            }
                            using (var args = new ArgsWrapper(fg,
                                $"ret.CustomFactoryEnd"))
                            {
                                args.AddPassArg($"stream");
                                args.Add($"finalPos: stream.Length");
                                args.AddPassArg($"offset");
                            }
                        }

                        // Parse ending positions 
                        await foreach (var lengths in IteratePassedLengths(obj, forOverlay: true, passedLenPrefix: "ret."))
                        {
                            if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen)) continue;
                            var data = lengths.Field.GetFieldData();
                            switch (data.BinaryOverlayFallback)
                            {
                                case BinaryGenerationType.Normal:
                                case BinaryGenerationType.Custom:
                                    break;
                                default:
                                    continue;
                            }
                            if (lengths.Field is DataType)
                            {
                                await typeGen.GenerateWrapperUnknownLengthParse(
                                    fg,
                                    obj,
                                    lengths.Field,
                                    lengths.PassedLength,
                                    lengths.PassedAccessor);
                            }
                        }

                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            foreach (var field in obj.IterateFields())
                            {
                                if (!(field is GroupType group)) continue;
                                if (!((bool)group.CustomData[Mutagen.Bethesda.Internals.Constants.EdidLinked])) continue;
                                using (var args = new ArgsWrapper(fg,
                                    $"{nameof(UtilityTranslation)}.{nameof(UtilityTranslation.FillEdidLinkCache)}<{group.GetGroupTarget().GetTypeName(LoquiInterfaceType.IGetter)}>"))
                                {
                                    args.Add("mod: ret");
                                    args.Add($"recordType: {group.GetGroupTarget().GetTriggeringSource()}");
                                    args.Add("package: ret._package");
                                }
                            }
                        }
                        if (objData.CustomBinaryEnd != CustomEnd.Off)
                        {
                            using (var args = new ArgsWrapper(fg,
                                "ret.CustomEnd"))
                            {
                                if (obj.GetObjectType() == ObjectType.Record)
                                {
                                    args.Add("stream: origStream");
                                }
                                else
                                {
                                    args.AddPassArg("stream");
                                }
                                args.Add("finalPos: stream.Length");
                                args.AddPassArg("offset");
                            }
                        }
                        fg.AppendLine("return ret;");
                    }
                    fg.AppendLine();

                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        using (var args = new FunctionWrapper(fg,
                            $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                        {
                            args.Add($"ReadOnlyMemorySlice<byte> slice");
                            args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                            args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                        }
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"return {obj.Name}Factory"))
                            {
                                args.Add($"stream: new {nameof(OverlayStream)}(slice, package)");
                                args.AddPassArg("package");
                                if (obj.IsVariableLengthStruct())
                                {
                                    args.Add($"finalPos: slice.Length");
                                }
                                args.AddPassArg("recordTypeConverter");
                            }
                        }
                    }
                }
                fg.AppendLine();

                if (HasRecordTypeFields(obj))
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public{await obj.FunctionOverride(async b => HasRecordTypeFields(b))}{nameof(ParseResult)} FillRecordType"))
                    {
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? nameof(IBinaryReadStream) : nameof(OverlayStream))} stream");
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                        args.Add($"int offset");
                        args.Add("RecordType type");
                        args.Add("int? lastParsed");
                        if (obj.GetObjectType() != ObjectType.Mod)
                        {
                            args.Add("Dictionary<RecordType, int>? recordParseCount");
                        }
                        args.Add("RecordTypeConverter? recordTypeConverter = null");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"type = recordTypeConverter.ConvertToStandard(type);");
                        fg.AppendLine("switch (type.TypeInt)");
                        using (new BraceWrapper(fg))
                        {
                            var fields = new List<(int, int, TypeGeneration Field)>();
                            foreach (var field in obj.IterateFieldIndices(
                                expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                                nonIntegrated: true))
                            {
                                var fieldData = field.Field.GetFieldData();
                                if (!fieldData.HasTrigger
                                    || fieldData.GenerationTypes.Count() == 0) continue;
                                if (fieldData.BinaryOverlayFallback == BinaryGenerationType.NoGeneration) continue;
                                if (field.Field.Derivative && fieldData.BinaryOverlayFallback != BinaryGenerationType.Custom) continue;
                                if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + field.Field);
                                }

                                if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                                fields.Add(field);
                            }

                            var doubleUsages = new Dictionary<RecordType, List<(int, int, TypeGeneration)>>();
                            foreach (var field in fields)
                            {
                                var fieldData = field.Field.GetFieldData();
                                if (fieldData.GenerationTypes.Count() > 1) continue;
                                foreach (var gen in fieldData.GenerationTypes)
                                {
                                    if (gen.Key.Count() > 1) continue;
                                    LoquiType loqui = gen.Value as LoquiType;
                                    if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;
                                    doubleUsages.TryCreateValue(gen.Key.First()).Add(field);
                                }
                            }
                            foreach (var item in doubleUsages.ToList())
                            {
                                if (item.Value.Count <= 1)
                                {
                                    doubleUsages.Remove(item.Key);
                                }
                            }

                            foreach (var field in fields)
                            {
                                var fieldData = field.Field.GetFieldData();
                                if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + field.Field);
                                }
                                foreach (var gen in fieldData.GenerationTypes)
                                {
                                    LoquiType loqui = gen.Value as LoquiType;
                                    if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;

                                    List<(int, int, TypeGeneration Field)> doubles = null;
                                    if (gen.Key.Count() == 1)
                                    {
                                        if (doubleUsages.TryGetValue(gen.Key.First(), out doubles))
                                        {
                                            // Means we handled earlier, break out
                                            if (doubles.Count == 0) continue;
                                        }
                                    }

                                    foreach (var trigger in gen.Key)
                                    {
                                        fg.AppendLine($"case RecordTypeInts.{trigger.CheckedType}:");
                                    }
                                    using (new BraceWrapper(fg))
                                    {
                                        if (doubles == null)
                                        {
                                            await GenerateLastParsedShortCircuit(
                                                obj: obj,
                                                fg: fg,
                                                field: field,
                                                doublesPotential: false,
                                                nextRecAccessor: "type",
                                                toDo: async () =>
                                                {
                                                    string recConverter = "recordTypeConverter";
                                                    if (fieldData?.RecordTypeConverter != null
                                                        && fieldData.RecordTypeConverter.FromConversions.Count > 0)
                                                    {
                                                        recConverter = $"{obj.RegistrationName}.{field.Field.Name}Converter";
                                                    }
                                                    await generator.GenerateWrapperRecordTypeParse(
                                                        fg: fg,
                                                        objGen: obj,
                                                        typeGen: gen.Value,
                                                        locationAccessor: "(stream.Position - offset)",
                                                        packageAccessor: "_package",
                                                        converterAccessor: recConverter);
                                                    if (obj.GetObjectType() == ObjectType.Mod
                                                        && field.Field.Name == "ModHeader")
                                                    {
                                                        using (var args = new ArgsWrapper(fg,
                                                            $"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo"))
                                                        {
                                                            args.Add(subFg =>
                                                            {
                                                                subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                                using (new DepthWrapper(subFg))
                                                                {
                                                                    subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                                    using (new BraceWrapper(subFg) { AppendParenthesis = true })
                                                                    {
                                                                        subFg.AppendLine("Master = master.Master,");
                                                                        subFg.AppendLine("FileSize = master.FileSize,");
                                                                    }
                                                                }
                                                            });
                                                        }
                                                    }
                                                });
                                        }
                                        else
                                        {
                                            fg.AppendLine($"switch (recordParseCount?.TryCreateValue(type) ?? 0)");
                                            using (new BraceWrapper(fg))
                                            {
                                                int count = 0;
                                                foreach (var doublesField in doubles)
                                                {
                                                    if (!this.TryGetTypeGeneration(doublesField.Field.GetType(), out var doubleGen))
                                                    {
                                                        throw new ArgumentException("Unsupported type generator: " + doublesField.Field);
                                                    }
                                                    var doublesFieldData = doublesField.Field.GetFieldData();
                                                    fg.AppendLine($"case {count++}:");
                                                    using (new DepthWrapper(fg))
                                                    {
                                                        await GenerateLastParsedShortCircuit(
                                                            obj: obj,
                                                            fg: fg,
                                                            field: doublesField,
                                                            doublesPotential: true,
                                                            nextRecAccessor: "type",
                                                            toDo: async () =>
                                                            {
                                                                string recConverter = "recordTypeConverter";
                                                                if (doublesFieldData.RecordTypeConverter != null
                                                                    && doublesFieldData.RecordTypeConverter.FromConversions.Count > 0)
                                                                {
                                                                    recConverter = $"{obj.RegistrationName}.{doublesField.Field.Name}Converter";
                                                                }
                                                                await doubleGen.GenerateWrapperRecordTypeParse(
                                                                    fg: fg,
                                                                    objGen: obj,
                                                                    typeGen: doublesField.Field,
                                                                    locationAccessor: "(stream.Position - offset)",
                                                                    packageAccessor: "_package",
                                                                    converterAccessor: recConverter);
                                                                if (obj.GetObjectType() == ObjectType.Mod
                                                                    && doublesField.Field.Name == "ModHeader")
                                                                {
                                                                    using (var args = new ArgsWrapper(fg,
                                                                        $"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo"))
                                                                    {
                                                                        args.Add(subFg =>
                                                                        {
                                                                            subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                                            using (new DepthWrapper(subFg))
                                                                            {
                                                                                subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                                                using (new BraceWrapper(subFg) { AppendParenthesis = true })
                                                                                {
                                                                                    subFg.AppendLine("Master = master.Master,");
                                                                                    subFg.AppendLine("FileSize = master.FileSize,");
                                                                                }
                                                                            }
                                                                        });
                                                                    }
                                                                }
                                                            });
                                                    }
                                                }
                                                fg.AppendLine($"default:");
                                                using (new DepthWrapper(fg))
                                                {
                                                    fg.AppendLine($"throw new NotImplementedException();");
                                                }
                                            }
                                            doubles.Clear();
                                        }
                                    }
                                }
                            }
                            var endMarkerType = obj.GetObjectData().EndMarkerType;
                            if (endMarkerType.HasValue)
                            {
                                fg.AppendLine($"case RecordTypeInts.{endMarkerType}: // End Marker");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine($"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.ReadSubrecordFrame(stream);");
                                    fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                }
                            }
                            fg.AppendLine("default:");
                            using (new DepthWrapper(fg))
                            {
                                if (obj.GetObjectData().CustomRecordFallback)
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"return CustomRecordFallback"))
                                    {
                                        args.AddPassArg("stream");
                                        args.AddPassArg("finalPos");
                                        args.AddPassArg("offset");
                                        args.AddPassArg("type");
                                        args.AddPassArg("lastParsed");
                                        if (obj.GetObjectData().BaseRecordTypeConverter?.FromConversions.Count > 0)
                                        {
                                            args.Add($"recordTypeConverter: {obj.RegistrationName}.BaseConverter");
                                        }
                                    }
                                }
                                else if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b)))
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        "return base.FillRecordType"))
                                    {
                                        args.AddPassArg("stream");
                                        args.AddPassArg("finalPos");
                                        args.AddPassArg("offset");
                                        args.AddPassArg("type");
                                        args.AddPassArg("lastParsed");
                                        args.AddPassArg("recordParseCount");
                                        if (obj.GetObjectData().BaseRecordTypeConverter?.FromConversions.Count > 0)
                                        {
                                            args.Add($"recordTypeConverter: {obj.RegistrationName}.BaseConverter");
                                        }
                                    }
                                }
                                else
                                {
                                    var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                    if (obj.GetObjectType() == ObjectType.Subrecord)
                                    {
                                        fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                    }
                                    else if (failOnUnknown)
                                    {
                                        fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                                    }
                                    else
                                    {
                                        fg.AppendLine($"return default(int?);");
                                    }
                                }
                            }
                        }
                    }
                }

                await obj.GenerateToStringCode(fg);
            }
            fg.AppendLine();
        }

        public override void CustomMainWriteMixInPreLoad(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            fg.AppendLine("var modKey = item.ModKey;");
        }

        public override void ReplaceTypeAssociation<Target, Replacement>()
        {
        }

        public struct PassedLengths
        {
            public TypeGeneration Field;
            public string PassedAccessor;
            public int? PassedLength;
            public string CurAccessor;
            public int? CurLength;
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
            foreach (var field in fields)
            {
                lengths.Field = field;
                if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen))
                {
                    if (!field.IntegrateField) continue;
                    throw new NotImplementedException();
                }
                void processLen(int? expectedLen)
                {
                    lengths.PassedLength = lengths.CurLength;
                    lengths.PassedAccessor = lengths.CurAccessor;
                    if (expectedLen == null)
                    {
                        lengths.CurLength = null;
                        lastUnknownField = field;
                        lengths.CurAccessor = $"{passedLenPrefix}{lastUnknownField.Name}EndingPos";
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
                        if (lastUnknownField == null)
                        {
                            lengths.CurAccessor = $"0x{ lengths.CurLength:X}";
                        }
                        else
                        {
                            lengths.CurAccessor = $"{passedLenPrefix}{lastUnknownField.Name}EndingPos + 0x{lengths.CurLength:X}";
                        }
                    }
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
