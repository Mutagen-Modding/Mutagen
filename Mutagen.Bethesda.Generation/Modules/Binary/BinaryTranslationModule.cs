using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using System.IO;
using System.Windows.Media;
using Noggog;
using Loqui.Internal;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public enum BinaryGenerationType
    {
        Normal,
        DoNothing,
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

        public string BinaryWrapperClassName(ObjectGeneration obj) => $"{obj.Name}BinaryWrapper";
        public string BinaryWrapperClassName(LoquiType loqui) => $"{loqui.TargetObjectGeneration.Name}BinaryWrapper{loqui.GenericTypes(getter: true)}";
        public string BinaryWrapperClass(ObjectGeneration obj) => $"{BinaryWrapperClassName(obj)}{obj.GetGenericTypes(MaskType.Normal)}";

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
            this._typeGenerations[typeof(FormIDLinkType)] = new FormIDLinkBinaryTranslationGeneration();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ArrayType)] = new ArrayBinaryTranslationGeneration();
            this._typeGenerations[typeof(LoquiListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(DictType)] = new DictBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new ByteArrayBinaryTranslationGeneration();
            this._typeGenerations[typeof(BufferType)] = new BufferBinaryTranslationGeneration();
            this._typeGenerations[typeof(DataType)] = new DataBinaryTranslationGeneration();
            this._typeGenerations[typeof(ColorType)] = new ColorBinaryTranslationGeneration()
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(SpecialParseType)] = new SpecialParseTranslationGeneration();
            this._typeGenerations[typeof(ZeroType)] = new ZeroBinaryTranslationGeneration();
            this._typeGenerations[typeof(CustomLogic)] = new CustomLogicTranslationGeneration();
            APILine[] modAPILines = new APILine[]
            {
                new APILine(
                    nicknameKey: "GroupMask",
                    resolutionString: "GroupMask importMask = null",
                    when: (obj) => obj.GetObjectType() == ObjectType.Mod)
            };
            APILine masterRefs = new APILine(
                nicknameKey: "MasterReferences",
                resolutionString: "MasterReferences masterReferences",
                when: (obj) => obj.GetObjectType() != ObjectType.Mod);
            var modKey = new APILine(
                nicknameKey: "ModKey",
                resolutionString: "ModKey modKey",
                when: (obj) => obj.GetObjectType() == ObjectType.Mod);
            var modKeyOptional = new APILine(
                nicknameKey: "ModKeyOptional",
                resolutionString: "ModKey? modKeyOverride = null",
                when: (obj) => obj.GetObjectType() == ObjectType.Mod);
            var recTypeConverter = new APILine(
                "RecordTypeConverter",
                $"{nameof(RecordTypeConverter)} recordTypeConverter");
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine("MutagenWriter", "MutagenWriter writer") },
                    optionalAPI: modAPILines,
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(masterRefs),
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(recTypeConverter, "null")
                    }),
                readerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine("MutagenFrame", "MutagenFrame frame") },
                    optionalAPI: modAPILines,
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(masterRefs),
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(recTypeConverter, "null")
                    }));
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Path", "string path") },
                        customAPI: new CustomMethodAPI[]
                        {
                            CustomMethodAPI.FactoryPublic(masterRefs),
                        },
                        optionalAPI: modKeyOptional.And(modAPILines).ToArray()))
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
                            CustomMethodAPI.FactoryPublic(masterRefs),
                            CustomMethodAPI.FactoryPublic(modKey),
                        },
                        optionalAPI: modAPILines))
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
            foreach (var gen in _typeGenerations.Values)
            {
                gen.Module = this;
                gen.MaskModule = this.Gen.MaskModule;
            }
            obj.RequiredNamespaces.Add("ReactiveUI");
            obj.RequiredNamespaces.Add("System.Reactive.Disposables");
            obj.RequiredNamespaces.Add("System.Reactive.Linq");
        }

        public override async Task<IEnumerable<string>> RequiredUsingStatements(ObjectGeneration obj)
        {
            return (await base.RequiredUsingStatements(obj))
                .And("Mutagen.Bethesda.Binary")
                .And("System.Buffers.Binary");
        }

        private void ConvertFromStreamOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var writer = new MutagenWriter(stream, meta: item.GameMode, dispose: false))");
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationModuleAPI.Direction.Writer).ToArray());
            }
        }

        private void ConvertFromStreamIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(stream, {nameof(GameMode)}.{obj.GetObjectData().GameMode}))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var frame = new MutagenFrame(reader);");
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationModuleAPI.Direction.Reader).ToArray());
            }
        }

        public override async Task GenerateInTranslationWriteClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateCustomWritePartials(obj, fg);
            GenerateCustomBinaryEndWritePartial(obj, fg);
            GenerateWriteExtras(obj, fg);
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
            GenerateCustomCreatePartials(obj, fg);
            GenerateCustomBinaryEndCreatePartial(obj, fg);
            await base.GenerateInTranslationCreateClass(obj, fg);
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            await GenerateBinaryWrapperCreates(obj, fg);
        }

        private void GenerateCustomWritePartials(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields(nonIntegrated: true))
            {
                if (!field.TryGetFieldData(out var mutaData)) continue;
                if (mutaData.Binary != BinaryGenerationType.Custom && !(field is CustomLogic)) continue;
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
                if (!field.TryGetFieldData(out var mutaData)) continue;
                if (mutaData.Binary != BinaryGenerationType.Custom && !(field is CustomLogic)) continue;
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
                if (field.TryGetFieldData(out var data)
                    && data.HasTrigger)
                {
                    yield return field;
                }
            }
        }

        private IEnumerable<TypeGeneration> GetEmbeddedFields(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (field is SetMarkerType) continue;
                if (!field.TryGetFieldData(out var data)
                    || !data.HasTrigger)
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
                fg.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => {(obj.Abstract ? "throw new ArgumentException()" : obj.GetTriggeringSource())};");
            }

            if ((!obj.Abstract && obj.BaseClassTrail().All((b) => b.Abstract)) || HasEmbeddedFields(obj))
            {
                var async = HasAsyncStructs(obj, self: true);
                using (var args = new FunctionWrapper(fg,
                    $"protected static {Loqui.Generation.Utility.TaskReturn(async)} Fill{ModuleNickname}Structs"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add("MutagenFrame frame");
                    args.Add($"MasterReferences masterReferences");
                    args.Add($"ErrorMaskBuilder errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{Loqui.Generation.Utility.Await(async)}{obj.BaseClass.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class, MaskType.Normal)}.Fill{ModuleNickname}Structs"))
                        {
                            args.Add("item: item");
                            args.Add("frame: frame");
                            args.Add("masterReferences: masterReferences");
                            args.Add("errorMask: errorMask");
                        }
                    }
                    foreach (var field in obj.IterateFields(
                        nonIntegrated: true,
                        expandSets: SetMarkerType.ExpandSets.False))
                    {
                        if (field is SetMarkerType) continue;
                        if (field.TryGetFieldData(out var fieldData)
                            && fieldData.HasTrigger) continue;
                        if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                        if (fieldData.Binary == BinaryGenerationType.DoNothing) continue;
                        if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }
                        if (field.HasBeenSet)
                        {
                            fg.AppendLine($"if (frame.Complete) return;");
                        }
                        GenerateFillSnippet(obj, fg, field, generator, "frame");
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"protected static {Loqui.Generation.Utility.TaskWrap("TryGet<int?>", HasAsyncRecords(obj, self: true))} Fill{ModuleNickname}RecordTypes"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add("MutagenFrame frame");
                    if (typelessStruct)
                    {
                        args.Add($"int? lastParsed");
                    }
                    args.Add("RecordType nextRecordType");
                    args.Add("int contentLength");
                    args.Add("MasterReferences masterReferences");
                    args.Add($"ErrorMaskBuilder errorMask");
                    if (data.ObjectType == ObjectType.Mod)
                    {
                        args.Add($"GroupMask importMask");
                    }
                    args.Add($"{nameof(RecordTypeConverter)} recordTypeConverter = null");
                }
                using (new BraceWrapper(fg))
                {
                    var mutaObjType = obj.GetObjectType();
                    fg.AppendLine($"nextRecordType = recordTypeConverter.ConvertToStandard(nextRecordType);");
                    fg.AppendLine("switch (nextRecordType.TypeInt)");
                    using (new BraceWrapper(fg))
                    {
                        foreach (var field in obj.IterateFieldIndices(
                            expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                            nonIntegrated: true))
                        {
                            if (!field.Field.TryGetFieldData(out var fieldData)
                                || !fieldData.HasTrigger
                                || fieldData.TriggeringRecordTypes.Count == 0) continue;
                            if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                            if (field.Field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }

                            if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                            foreach (var gen in fieldData.GenerationTypes)
                            {
                                LoquiType loqui = gen.Value as LoquiType;
                                if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;
                                foreach (var trigger in gen.Key)
                                {
                                    fg.AppendLine($"case 0x{trigger.TypeInt.ToString("X")}: // {trigger.Type}");
                                }
                                using (new BraceWrapper(fg))
                                {
                                    await GenerateLastParsedShortCircuit(
                                        obj: obj,
                                        fg: fg,
                                        field: field,
                                        fieldData: fieldData,
                                        toDo: async () =>
                                        {
                                            if (fieldData.Binary != BinaryGenerationType.DoNothing)
                                            {
                                                var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                                if (groupMask)
                                                {
                                                    fg.AppendLine($"if (importMask?.{field.Field.Name} ?? true)");
                                                }
                                                using (new BraceWrapper(fg, doIt: groupMask))
                                                {
                                                    GenerateFillSnippet(obj, fg, gen.Value, generator, "frame");
                                                }
                                                if (groupMask)
                                                {
                                                    fg.AppendLine("else");
                                                    using (new BraceWrapper(fg))
                                                    {
                                                        fg.AppendLine("frame.Position += contentLength;");
                                                    }
                                                }
                                            }
                                        });
                                }
                            }
                        }
                        fg.AppendLine($"default:");
                        using (new DepthWrapper(fg))
                        {
                            bool first = true;
                            // Generic options
                            foreach (var field in obj.IterateFieldIndices())
                            {
                                if (!field.Field.TryGetFieldData(out var fieldData)
                                    || !fieldData.HasTrigger
                                    || fieldData.TriggeringRecordTypes.Count > 0) continue;
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
                                            args.Checks.Add($"nextRecordType.Equals({trigger})");
                                        }
                                    }
                                    first = false;
                                    using (new BraceWrapper(fg))
                                    {
                                        GenerateFillSnippet(obj, fg, field.Field, generator, "frame");
                                        fg.AppendLine($"return TryGet<int?>.Failure;");
                                    }
                                }
                            }

                            // Default case
                            if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasRecordTypeFields(b)))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return {Loqui.Generation.Utility.Await(HasAsyncRecords(obj, self: false))}{obj.BaseClass.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class, MaskType.Normal)}.Fill{ModuleNickname}RecordTypes"))
                                {
                                    args.Add("item: item");
                                    args.Add("frame: frame");
                                    if (obj.BaseClass.IsTypelessStruct())
                                    {
                                        args.Add($"lastParsed: lastParsed");
                                    }
                                    args.Add("nextRecordType: nextRecordType");
                                    args.Add("contentLength: contentLength");
                                    if (data.BaseRecordTypeConverter?.FromConversions.Count > 0)
                                    {
                                        args.Add($"recordTypeConverter: recordTypeConverter.Combine({obj.RegistrationName}.BaseConverter)");
                                    }
                                    else
                                    {
                                        args.Add("recordTypeConverter: recordTypeConverter");
                                    }
                                    args.Add($"masterReferences: masterReferences");
                                    args.Add($"errorMask: errorMask");
                                }
                            }
                            else
                            {
                                var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                if (mutaObjType == ObjectType.Subrecord)
                                {
                                    fg.AppendLine($"return TryGet<int?>.Failure;");
                                }
                                else if (failOnUnknown)
                                {
                                    fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                                }
                                else
                                {
                                    fg.AppendLine($"errorMask?.ReportWarning($\"Unexpected header {{nextRecordType.Type}} at position {{frame.Position}}\");");
                                    string addString;
                                    switch (obj.GetObjectType())
                                    {
                                        case ObjectType.Mod:
                                            addString = null;
                                            break;
                                        case ObjectType.Subrecord:
                                        case ObjectType.Record:
                                            addString = $" + frame.{nameof(MutagenFrame.MetaData)}.{nameof(MetaDataConstants.SubConstants)}.{nameof(MetaDataConstants.SubConstants.HeaderLength)}";
                                            break;
                                        case ObjectType.Group:
                                            addString = $" + frame.{nameof(MutagenFrame.MetaData)}.{nameof(MetaDataConstants.MajorConstants)}.{nameof(MetaDataConstants.SubConstants.HeaderLength)}";
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                    fg.AppendLine($"frame.Position += contentLength{addString};");
                                    fg.AppendLine($"return TryGet<int?>.Succeed(null);");
                                }
                            }
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        private async Task GenerateBinaryWrapperCreates(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static {obj.Interface(getter: true, internalInterface: true)} {CreateFromPrefix}{ModuleNickname}Wrapper"))
            {
                args.Add($"ReadOnlyMemorySlice<byte> bytes");
                args.Add($"ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryWrapperClass(obj)}.{obj.Name}Factory"))
                {
                    args.Add($"new {nameof(BinaryMemoryReadStream)}(bytes)");
                    args.AddPassArg("modKey");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static {obj.Interface(getter: true, internalInterface: true)} {CreateFromPrefix}{ModuleNickname}Wrapper"))
            {
                args.Add($"string path");
                args.Add($"ModKey? modKeyOverride = null");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {CreateFromPrefix}{ModuleNickname}Wrapper"))
                {
                    args.Add($"stream: new {nameof(BinaryReadStream)}(path)");
                    args.Add("modKey: modKeyOverride ?? ModKey.Factory(Path.GetFileName(path))");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static {obj.Interface(getter: true, internalInterface: true)} {CreateFromPrefix}{ModuleNickname}Wrapper"))
            {
                args.Add($"{nameof(IBinaryReadStream)} stream");
                args.Add($"ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryWrapperClass(obj)}.{obj.Name}Factory"))
                {
                    args.AddPassArg($"stream");
                    args.AddPassArg("modKey");
                }
            }
            fg.AppendLine();
        }

        private static async Task GenerateLastParsedShortCircuit(
            ObjectGeneration obj,
            FileGeneration fg,
            (int PublicIndex, int InternalIndex, TypeGeneration Field) field,
            MutagenFieldData fieldData,
            Func<Task> toDo)
        {
            var dataSet = field.Field as DataType;
            var typelessStruct = obj.IsTypelessStruct();
            if (typelessStruct && fieldData.IsTriggerForObject)
            {
                if (dataSet != null)
                {
                    fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){dataSet.SubFields.Last().IndexEnumName}) return TryGet<int?>.Failure;");
                }
                else if (field.Field is SpecialParseType
                    || field.Field is CustomLogic)
                {
                    var objFields = obj.IterateFieldIndices(nonIntegrated: false).ToList();
                    var nextField = objFields.FirstOrDefault((i) => i.InternalIndex > field.InternalIndex);
                    var prevField = objFields.LastOrDefault((i) => i.InternalIndex < field.InternalIndex);
                    if (nextField.Field != null)
                    {
                        fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){nextField.Field.IndexEnumName}) return TryGet<int?>.Failure;");
                    }
                    else if (prevField.Field != null)
                    {
                        fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){prevField.Field.IndexEnumName}) return TryGet<int?>.Failure;");
                    }
                }
                else
                {
                    fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){field.Field.IndexEnumName}) return TryGet<int?>.Failure;");
                }
            }
            await toDo();
            if (dataSet != null)
            {
                fg.AppendLine($"return TryGet<int?>.Succeed((int){dataSet.SubFields.Last(f => f.IntegrateField).IndexEnumName});");
            }
            else if (field.Field is SpecialParseType
                || field.Field is CustomLogic)
            {
                fg.AppendLine($"return TryGet<int?>.Succeed({(typelessStruct ? "lastParsed" : "null")});");
            }
            else
            {
                fg.AppendLine($"return TryGet<int?>.Succeed((int){field.Field.IndexEnumName});");
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
                args.Add("MasterReferences masterReferences");
                args.Add($"ErrorMaskBuilder errorMask");
            }
            using (var args = new FunctionWrapper(fg,
                $"public static void CustomBinaryEndExportInternal"))
            {
                args.Add("MutagenWriter writer");
                args.Add($"{obj.Interface(internalInterface: true, getter: true)} obj");
                args.Add("MasterReferences masterReferences");
                args.Add($"ErrorMaskBuilder errorMask");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"CustomBinaryEndExport"))
                {
                    args.Add($"writer: writer");
                    args.Add($"obj: obj");
                    args.Add($"masterReferences: masterReferences");
                    args.Add($"errorMask: errorMask");
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
                    args.Add("MasterReferences masterReferences");
                    args.Add($"ErrorMaskBuilder errorMask");
                }
                using (var args = new FunctionWrapper(fg,
                    $"public static void CustomBinaryEndImportPublic"))
                {
                    args.Add("MutagenFrame frame");
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} obj");
                    args.Add("MasterReferences masterReferences");
                    args.Add($"ErrorMaskBuilder errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"CustomBinaryEndImport"))
                    {
                        args.Add("frame: frame");
                        args.Add($"obj: obj");
                        args.Add("masterReferences: masterReferences");
                        args.Add($"errorMask: errorMask");
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

        private void GenerateFillSnippet(ObjectGeneration obj, FileGeneration fg, TypeGeneration field, BinaryTranslationGeneration generator, string frameAccessor)
        {
            if (field is DataType set)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(MetaDataConstants.SubConstants)}.{nameof(RecordConstants.HeaderLength)};");
                fg.AppendLine($"var dataFrame = {frameAccessor}.SpawnWithLength(contentLength);");
                fg.AppendLine($"if (!dataFrame.Complete)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"item.{set.StateName} = {obj.ObjectName}.{set.EnumName}.Has;");
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
                                var prevField = set.SubFields.TryGet(i);
                                if (!prevField?.IntegrateField ?? true) continue;
                                enumName = prevField.IndexEnumName;
                                break;
                            }
                            if (enumName != null)
                            {
                                enumName = $"(int){enumName}";
                            }
                            fg.AppendLine($"return TryGet<int?>.Succeed({enumName ?? "null"});");
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
                    GenerateFillSnippet(obj, fg, subField.Field, subGenerator, "dataFrame");
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
            if (data.Binary == BinaryGenerationType.Custom)
            {
                CustomLogic.GenerateFill(
                    fg,
                    field,
                    frameAccessor,
                    isAsync: false);
                return;
            }
            generator.GenerateCopyIn(
                fg: fg,
                objGen: obj,
                typeGen: field,
                readerAccessor: frameAccessor,
                itemAccessor: new Accessor(field, "item."),
                translationAccessor: null,
                errorMaskAccessor: $"errorMask");
        }

        private void ConvertFromPathOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var memStream = new MemoryTributary())");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"using (var writer = new MutagenWriter(memStream, dispose: false, meta: {nameof(MetaDataConstants)}.{nameof(MetaDataConstants.Get)}(item.GameMode)))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var modKey = modKeyOverride ?? ModKey.Factory(Path.GetFileName(path));");
                    internalToDo(this.MainAPI.PublicMembers(obj, TranslationModuleAPI.Direction.Writer).ToArray());
                }
                fg.AppendLine("using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"memStream.Position = 0;");
                    fg.AppendLine($"memStream.CopyTo(fs);");
                }
            }
        }

        private void ConvertFromPathIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(path, {nameof(GameMode)}.{obj.GetObjectData().GameMode}))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var frame = new MutagenFrame(reader);");
                fg.AppendLine("var modKey = modKeyOverride ?? ModKey.Factory(Path.GetFileName(path));");
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationModuleAPI.Direction.Reader).ToArray());
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
                        $"var nextRecord = HeaderTranslation.GetNext{(obj.GetObjectType() == ObjectType.Subrecord ? "Sub" : null)}RecordType"))
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
                                args.Add($"frame: frame.SpawnWithLength(customLen + frame.{nameof(MutagenFrame.MetaData)}.{nameof(MetaDataConstants.SubConstants)}.{nameof(MetaDataConstants.SubConstants.HeaderLength)})");
                                args.Add("recordType: nextRecord");
                                args.Add("recordTypeConverter: recordTypeConverter");
                                args.Add("masterReferences: masterReferences");
                                args.Add("errorMask: errorMask");
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
                fg.AppendLine($"var masterReferences = new MasterReferences({accessor}.ModHeader.MasterReferences, modKey);");
            }

            if (await obj.IsMajorRecord())
            {
                bool async = this.HasAsync(obj, self: true);
                using (var args = new ArgsWrapper(fg,
                    $"{Loqui.Generation.Utility.Await(async)}Utility{(async ? "Async" : null)}Translation.MajorRecordParse<{obj.Interface(getter: false, internalInterface: true)}>"))
                {
                    args.Add($"record: {accessor}");
                    args.Add($"frame: frame");
                    args.Add($"errorMask: errorMask");
                    args.Add($"recType: RecordType");
                    args.Add($"recordTypeConverter: recordTypeConverter");
                    args.Add($"masterReferences: masterReferences");
                    args.Add($"fillStructs: FillBinaryStructs");
                    args.Add($"fillTyped: FillBinaryRecordTypes");
                }
                if (data.CustomBinaryEnd != CustomEnd.Off)
                {
                    fg.AppendLine("try");
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{Loqui.Generation.Utility.Await(data.CustomBinaryEnd == CustomEnd.Async)}{this.TranslationCreateClass(obj)}.CustomBinaryEndImport{(await this.AsyncImport(obj) ? null : "Public")}"))
                        {
                            args.Add("frame: frame");
                            args.Add($"obj: {accessor}");
                            args.Add("masterReferences: masterReferences");
                            args.Add("errorMask: errorMask");
                        }
                    }
                    fg.AppendLine("catch (Exception ex)");
                    fg.AppendLine("when (errorMask != null)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("errorMask.ReportException(ex);");
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
                                using (var args = new ArgsWrapper(fg,
                                    $"frame = frame.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseSubrecord",
                                    suffixLine: ")"))
                                {
                                    args.Add("frame.Reader");
                                    args.Add($"recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(obj.GetRecordType())})");
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
                    case ObjectType.Record:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.{(typelessStruct ? "Typeless" : string.Empty)}RecordParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.Add("frame: frame");
                            args.Add($"setFinal: {(obj.TryGetRecordType(out var recType) ? "true" : "false")}");
                            args.Add("masterReferences: masterReferences");
                            args.Add("errorMask: errorMask");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj))
                            {
                                args.Add($"fillTyped: Fill{ModuleNickname}RecordTypes");
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
                            args.Add("masterReferences: masterReferences");
                            args.Add("errorMask: errorMask");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj))
                            {
                                args.Add($"fillTyped: Fill{ModuleNickname}RecordTypes");
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
                            args.Add("masterReferences: masterReferences");
                            args.Add("errorMask: errorMask");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj))
                            {
                                args.Add($"fillTyped: Fill{ModuleNickname}RecordTypes");
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
                        args.Add("errorMask: errorMask");
                        args.Add("masterReferences: masterReferences");
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
                    $"using (HeaderExport.ExportHeader",
                    ")",
                    semiColon: false))
                {
                    args.Add("writer: writer");
                    args.Add($"record: {obj.RecordTypeHeaderName(obj.GetRecordType())}");
                    args.Add($"type: {nameof(ObjectType)}.{obj.GetObjectType()}");
                }
            }
            using (new BraceWrapper(fg, doIt: hasRecType))
            {
                if (HasEmbeddedFields(obj))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"Write_Embedded"))
                    {
                        args.Add($"item: item");
                        args.Add($"writer: writer");
                        args.Add($"errorMask: errorMask");
                        args.Add($"masterReferences: masterReferences");
                    }
                }
                else
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                    if (firstBase != null)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{this.TranslationWriteClass(firstBase)}.Write_Embedded"))
                        {
                            args.Add($"item: item");
                            args.Add($"writer: writer");
                            args.Add($"errorMask: errorMask");
                            args.Add($"masterReferences: masterReferences");
                        }
                    }
                }
                if (HasRecordTypeFields(obj))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"Write_RecordTypes"))
                    {
                        args.Add($"item: item");
                        args.Add($"writer: writer");
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            args.Add($"importMask: importMask");
                            args.Add($"modKey: modKey");
                        }
                        args.Add($"recordTypeConverter: recordTypeConverter");
                        args.Add($"errorMask: errorMask");
                        if (obj.GetObjectType() != ObjectType.Mod)
                        {
                            args.Add($"masterReferences: masterReferences");
                        }
                    }
                }
                else
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasRecordTypeFields(b));
                    if (firstBase != null)
                    {
                        using (var args = new ArgsWrapper(fg,
                        $"{this.TranslationWriteClass(firstBase)}.Write_RecordTypes"))
                        {
                            args.Add($"item: item");
                            args.Add($"writer: writer");
                            args.Add($"recordTypeConverter: recordTypeConverter");
                            args.Add($"errorMask: errorMask");
                            args.Add($"masterReferences: masterReferences");
                        }
                    }
                }
            }
            if (data.CustomBinaryEnd != CustomEnd.Off)
            {
                using (var args = new ArgsWrapper(fg,
                    $"CustomBinaryEndExportInternal"))
                {
                    args.Add("writer: writer");
                    args.Add("obj: item");
                    args.Add("errorMask: errorMask");
                    args.Add($"masterReferences: masterReferences");
                }
            }
        }

        private void GenerateWriteExtras(ObjectGeneration obj, FileGeneration fg)
        {
            var data = obj.GetObjectData();
            if (HasEmbeddedFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void Write_Embedded{obj.GetGenericTypes(MaskType.Normal)}"))
                {
                    args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                    args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                    args.Add("MutagenWriter writer");
                    args.Add($"ErrorMaskBuilder errorMask");
                    args.Add($"MasterReferences masterReferences");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasLoquiBaseObject)
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{TranslationWriteClass(firstBase)}.Write_Embedded"))
                            {
                                args.Add("item: item");
                                args.Add("writer: writer");
                                args.Add("errorMask: errorMask");
                                args.Add("masterReferences: masterReferences");
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.False))
                    {
                        if (field.TryGetFieldData(out var fieldData)
                            && fieldData.HasTrigger) continue;
                        if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                        if (fieldData.Binary == BinaryGenerationType.DoNothing) continue;
                        if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                        List<string> conditions = new List<string>();
                        if (field.HasBeenSet)
                        {
                            conditions.Add($"{field.HasBeenSetAccessor(new Accessor(field, "item."))}");
                        }
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
                                throw new ArgumentException("Unsupported type generator: " + field);
                            }
                            generator.GenerateWrite(
                                fg: fg,
                                objGen: obj,
                                typeGen: field,
                                writerAccessor: "writer",
                                itemAccessor: new Accessor(field, "item."),
                                translationAccessor: null,
                                errorMaskAccessor: "errorMask");
                        }
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void Write_RecordTypes{obj.GetGenericTypes(MaskType.Normal)}"))
                {
                    args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                    args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                    args.Add("MutagenWriter writer");
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        args.Add($"GroupMask importMask");
                        args.Add($"ModKey modKey");
                    }
                    args.Add("RecordTypeConverter recordTypeConverter");
                    args.Add($"ErrorMaskBuilder errorMask");
                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        args.Add($"MasterReferences masterReferences");
                    }
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        fg.AppendLine($"MasterReferences masterReferences = new MasterReferences(item.ModHeader.MasterReferences, modKey);");
                    }
                    if (obj.HasLoquiBaseObject)
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((f) => HasRecordTypeFields(f));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{TranslationWriteClass(firstBase)}.Write_RecordTypes"))
                            {
                                args.Add($"item: item");
                                args.Add("writer: writer");
                                if (data.BaseRecordTypeConverter?.FromConversions.Count > 0)
                                {
                                    args.Add($"recordTypeConverter: recordTypeConverter.Combine({obj.RegistrationName}.BaseConverter)");
                                }
                                else
                                {
                                    args.Add("recordTypeConverter: recordTypeConverter");
                                }
                                args.Add($"errorMask: errorMask");
                                args.Add($"masterReferences: masterReferences");
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                    {
                        if (!field.TryGetFieldData(out var fieldData)
                            || !fieldData.HasTrigger) continue;
                        if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
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
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }

                        var accessor = new Accessor(field, "item.");
                        if (field is DataType dataType)
                        {
                            fg.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Has))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"using (HeaderExport.ExportSubRecordHeader(writer, recordTypeConverter.ConvertToCustom({obj.RecordTypeHeaderName(fieldData.RecordType.Value)})))");
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
                                        if (subData.Binary == BinaryGenerationType.Custom)
                                        {
                                            using (var args = new ArgsWrapper(fg,
                                                $"{TranslationWriteClass(obj)}.WriteBinary{subField.Field.Name}"))
                                            {
                                                args.Add("writer: writer");
                                                args.Add("item: item");
                                                args.Add("errorMask: errorMask");
                                                args.Add($"masterReferences: masterReferences");
                                            }
                                            continue;
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
                                            itemAccessor: new Accessor(subField.Field, "item."),
                                            errorMaskAccessor: $"errorMask");
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
                            if (!generator.ShouldGenerateWrite(field)) continue;
                            if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                            if (fieldData.Binary == BinaryGenerationType.DoNothing) continue;
                            bool doIf = true;
                            if (field is LoquiType loqui
                                && loqui.TargetObjectGeneration?.GetObjectType() == ObjectType.Group
                                && obj.GetObjectType() == ObjectType.Mod)
                            {
                                fg.AppendLine($"if (importMask?.{field.Name} ?? true)");
                            }
                            else if (field.HasBeenSet)
                            {
                                fg.AppendLine($"if ({field.HasBeenSetAccessor(accessor)})");
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
                                    errorMaskAccessor: $"errorMask");
                            }
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        protected async Task GenerateImportWrapper(ObjectGeneration obj, FileGeneration fg)
        {
            var dataAccessor = new Accessor("_data");
            var packageAccessor = new Accessor("_package");
            var metaAccessor = new Accessor("_package.Meta");
            var objData = obj.GetObjectData();
            var needsMasters = await obj.GetNeedsMasters();
            var anyHasRecordTypes = (await obj.EntireClassTree()).Any(c => HasRecordTypeFields(c));

            using (var args = new ClassWrapper(fg, $"{BinaryWrapperClass(obj)}"))
            {
                args.Partial = true;
                args.BaseClass = obj.HasLoquiBaseObject ? BinaryWrapperClass(obj.BaseClass) : (obj.GetObjectType() == ObjectType.Mod ? null : nameof(BinaryWrapper));
                args.Interfaces.Add(obj.Interface(getter: true, internalInterface: true));
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
                    fg.AppendLine($"void IModGetter.WriteToBinary(string path, ModKey? modKey) => this.WriteToBinary(path, modKey, importMask: null);");
                    fg.AppendLine($"Task IModGetter.WriteToBinaryAsync(string path, ModKey? modKey) => this.WriteToBinaryAsync(path, modKey);");
                    fg.AppendLine($"void IModGetter.WriteToBinaryParallel(string path, ModKey? modKey) => this.WriteToBinaryParallel(path, modKey);");
                    fg.AppendLine($"IReadOnlyList<{nameof(IMasterReferenceGetter)}> {nameof(IModGetter)}.MasterReferences => this.ModHeader.MasterReferences;");
                }

                if (obj.GetObjectType() == ObjectType.Mod
                    || (await LinkModule.HasLinks(obj, includeBaseClass: false) != LinkModule.LinkCase.No))
                {
                    fg.AppendLine($"public{await obj.FunctionOverride(async (o) => (await LinkModule.HasLinks(o, includeBaseClass: false)) != LinkModule.LinkCase.No)}IEnumerable<{nameof(ILinkGetter)}> Links => {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.GetLinks(this);");
                }

                if (await MajorRecordEnumerationModule.HasMajorRecordsInTree(obj, includeBaseClass: false) != MajorRecordEnumerationModule.Case.No)
                {
                    MajorRecordEnumerationModule.GenerateClassImplementation(fg, onlyGetter: true);
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
                    fg.AppendLine($"private readonly {nameof(BinaryWrapperFactoryPackage)} _package = new {nameof(BinaryWrapperFactoryPackage)}({nameof(GameMode)}.{obj.GetObjectData().GameMode});");
                    fg.AppendLine($"private readonly {nameof(IBinaryReadStream)} _data;");
                }

                fg.AppendLine();

                int? passedLength = 0;
                // Get passed length of all base classes, not including major record
                if (obj.HasLoquiBaseObject)
                {
                    foreach (var field in obj.BaseClass.IterateFields(
                        expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                        nonIntegrated: true,
                        includeBaseClass: true))
                    {
                        if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen)) continue;
                        var data = field.GetFieldData();
                        switch (data.BinaryWrapperFallback)
                        {
                            case BinaryGenerationType.Custom:
                                passedLength += CustomLogic.ExpectedLength(obj, field).Value;
                                continue;
                            case BinaryGenerationType.Normal:
                                break;
                            default:
                                continue;
                        }
                        passedLength += typeGen.GetPassedAmount(obj, field);
                    }
                }

                foreach (var field in obj.IterateFields(
                    expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                    nonIntegrated: true))
                {
                    if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen)) continue;
                    using (new RegionWrapper(fg, field.Name)
                    {
                        AppendExtraLine = false,
                        SkipIfOnlyOneLine = true
                    })
                    {
                        var data = field.GetFieldData();
                        switch (data.BinaryWrapperFallback)
                        {
                            case BinaryGenerationType.DoNothing:
                            case BinaryGenerationType.NoGeneration:
                                continue;
                            default:
                                break;
                        }
                        typeGen.GenerateWrapperFields(
                            fg,
                            obj,
                            field,
                            dataAccessor,
                            passedLength);
                        passedLength += typeGen.GetPassedAmount(obj, field);
                    }
                }

                // Get complete passed length of all fields
                passedLength = 0;
                foreach (var field in obj.IterateFields(
                    expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                    nonIntegrated: true,
                    includeBaseClass: true))
                {
                    if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen)) continue;
                    var data = field.GetFieldData();
                    switch (data.BinaryWrapperFallback)
                    {
                        case BinaryGenerationType.Normal:
                        case BinaryGenerationType.Custom:
                            break;
                        default:
                            continue;
                    }
                    passedLength += typeGen.GetPassedAmount(obj, field);
                }

                using (var args = new ArgsWrapper(fg,
                    $"partial void CustomCtor"))
                {
                    args.Add($"{nameof(IBinaryReadStream)} stream");
                    args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                    args.Add($"int offset");
                }
                if (objData.CustomBinaryEnd != CustomEnd.Off)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"partial void CustomEnd"))
                    {
                        args.Add($"{nameof(IBinaryReadStream)} stream");
                        args.Add($"int finalPos");
                        args.Add($"int offset");
                    }
                }
                fg.AppendLine();

                using (var args = new FunctionWrapper(fg,
                    $"protected {BinaryWrapperClassName(obj)}"))
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        args.Add($"{nameof(IBinaryReadStream)} stream");
                        args.Add("ModKey modKey");
                    }
                    else
                    {
                        args.Add($"ReadOnlyMemorySlice<byte> bytes");
                        args.Add($"{nameof(BinaryWrapperFactoryPackage)} package");
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
                }
                fg.AppendLine();

                if (!obj.Abstract)
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        using (var args = new FunctionWrapper(fg,
                            $"public static {this.BinaryWrapperClass(obj)} {obj.Name}Factory"))
                        {
                            args.Add($"ReadOnlyMemorySlice<byte> data");
                            args.Add("ModKey modKey");
                        }
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"return {obj.Name}Factory"))
                            {
                                args.Add($"stream: new {nameof(BinaryMemoryReadStream)}(data)");
                                args.AddPassArg("modKey");
                            }
                        }
                        fg.AppendLine();
                    }

                    using (var args = new FunctionWrapper(fg,
                        $"public static {this.BinaryWrapperClass(obj)} {obj.Name}Factory"))
                    {
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            args.Add($"{nameof(IBinaryReadStream)} stream");
                            args.Add("ModKey modKey");
                        }
                        else
                        {
                            args.Add($"{nameof(BinaryMemoryReadStream)} stream");
                            args.Add($"{nameof(BinaryWrapperFactoryPackage)} package");
                            args.Add($"{nameof(RecordTypeConverter)} recordTypeConverter = null");
                        }
                    }
                    using (new BraceWrapper(fg))
                    {
                        if (await obj.IsMajorRecord())
                        {
                            fg.AppendLine($"stream = {nameof(UtilityTranslation)}.{nameof(UtilityTranslation.DecompressStream)}(stream, package.Meta);");
                        }
                        if (obj.TryGetCustomRecordTypeTriggers(out var customLogicTriggers))
                        {
                            fg.AppendLine($"var nextRecord = recordTypeConverter.ConvertToCustom(package.Meta.Get{(obj.GetObjectType() == ObjectType.Subrecord ? "SubRecord" : "MajorRecord")}(stream).RecordType);");
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
                            $"var ret = new {BinaryWrapperClassName(obj)}{obj.GetGenericTypes(MaskType.Normal)}"))
                        {
                            if (obj.IsTypelessStruct())
                            {
                                if (anyHasRecordTypes)
                                {
                                    args.Add($"bytes: stream.RemainingMemory");
                                }
                                else
                                {
                                    args.Add($"bytes: stream.RemainingMemory.Slice(0, {passedLength})");
                                }
                            }
                            else
                            {
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Record:
                                        args.Add($"bytes: HeaderTranslation.ExtractRecordWrapperMemory(stream.RemainingMemory, package.Meta)");
                                        break;
                                    case ObjectType.Group:
                                        args.Add($"bytes: HeaderTranslation.ExtractGroupWrapperMemory(stream.RemainingMemory, package.Meta)");
                                        break;
                                    case ObjectType.Subrecord:
                                        args.Add($"bytes: HeaderTranslation.ExtractSubrecordWrapperMemory(stream.RemainingMemory, package.Meta)");
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
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + package.Meta.SubRecord(stream.RemainingSpan).TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.Meta.SubConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Record:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + package.Meta.MajorRecord(stream.RemainingSpan).TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.Meta.MajorConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Group:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + package.Meta.Group(stream.RemainingSpan).TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.Meta.GroupConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Mod:
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        if (anyHasRecordTypes)
                        {
                            if (obj.GetObjectType() != ObjectType.Mod
                                && !obj.IsTypelessStruct())
                            {
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Subrecord:
                                        fg.AppendLine($"stream.Position += 0x{passedLength.Value.ToString("X")} + package.Meta.SubConstants.TypeAndLengthLength;");
                                        break;
                                    case ObjectType.Record:
                                        fg.AppendLine($"stream.Position += 0x{passedLength.Value.ToString("X")} + package.Meta.MajorConstants.TypeAndLengthLength;");
                                        break;
                                    case ObjectType.Group:
                                        fg.AppendLine($"stream.Position += 0x{passedLength.Value.ToString("X")} + package.Meta.GroupConstants.TypeAndLengthLength;");
                                        break;
                                    case ObjectType.Mod:
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                using (var args = new ArgsWrapper(fg,
                                    $"ret.CustomCtor"))
                                {
                                    args.AddPassArg($"stream");
                                    args.AddPassArg($"finalPos");
                                    args.AddPassArg($"offset");
                                }
                            }
                            else
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"ret.CustomCtor"))
                                {
                                    args.AddPassArg($"stream");
                                    args.Add($"finalPos: stream.Length");
                                    args.Add($"offset: 0");
                                }
                            }
                            string call;
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Subrecord:
                                case ObjectType.Record:
                                    if (obj.IsTypelessStruct())
                                    {
                                        call = $"ret.{nameof(BinaryWrapper.FillTypelessSubrecordTypes)}";
                                    }
                                    else
                                    {
                                        call = $"ret.{nameof(BinaryWrapper.FillSubrecordTypes)}";
                                    }
                                    break;
                                case ObjectType.Group:
                                    var grupLoqui = await obj.GetGroupLoquiType();
                                    if (grupLoqui.TargetObjectGeneration != null && await grupLoqui.TargetObjectGeneration.IsMajorRecord())
                                    {
                                        call = $"ret.{nameof(BinaryWrapper.FillMajorRecords)}";
                                    }
                                    else
                                    {
                                        call = $"ret.{nameof(BinaryWrapper.FillGroupRecordsForWrapper)}";
                                    }
                                    break;
                                case ObjectType.Mod:
                                    call = $"{nameof(BinaryWrapper)}.{nameof(BinaryWrapper.FillModTypes)}";
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
                            if (!obj.IsTypelessStruct())
                            {
                                string headerAddition = null;
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Record:
                                        headerAddition = $" + package.Meta.MajorConstants.HeaderLength";
                                        break;
                                    case ObjectType.Group:
                                        headerAddition = $" + package.Meta.GroupConstants.HeaderLength";
                                        break;
                                    case ObjectType.Subrecord:
                                        headerAddition = $" + package.Meta.SubConstants.HeaderLength";
                                        break;
                                    case ObjectType.Mod:
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                fg.AppendLine($"stream.Position += 0x{(passedLength).Value.ToString("X")}{headerAddition};");
                            }
                            using (var args = new ArgsWrapper(fg,
                                $"ret.CustomCtor"))
                            {
                                args.AddPassArg($"stream");
                                args.Add($"finalPos: stream.Length");
                                args.AddPassArg($"offset");
                            }
                        }
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            foreach (var field in obj.IterateFields())
                            {
                                if (!(field is GroupType group)) continue;
                                if (!((bool)group.CustomData[Mutagen.Bethesda.Constants.EdidLinked])) continue;
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
                                args.AddPassArg("stream");
                                args.Add("finalPos: stream.Length");
                                args.AddPassArg("offset");
                            }
                        }
                        fg.AppendLine("return ret;");
                    }
                    fg.AppendLine();
                }

                if (HasRecordTypeFields(obj))
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public{await obj.FunctionOverride(async b => HasRecordTypeFields(b))}TryGet<int?> FillRecordType"))
                    {
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? nameof(IBinaryReadStream) : nameof(BinaryMemoryReadStream))} stream");
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                        args.Add($"int offset");
                        args.Add("RecordType type");
                        args.Add("int? lastParsed");
                        args.Add("RecordTypeConverter recordTypeConverter");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"type = recordTypeConverter.ConvertToStandard(type);");
                        fg.AppendLine("switch (type.TypeInt)");
                        using (new BraceWrapper(fg))
                        {
                            foreach (var field in obj.IterateFieldIndices(
                                expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                                nonIntegrated: true))
                            {
                                if (!field.Field.TryGetFieldData(out var fieldData)
                                    || !fieldData.HasTrigger
                                    || fieldData.TriggeringRecordTypes.Count == 0) continue;
                                if (fieldData.BinaryWrapperFallback == BinaryGenerationType.NoGeneration) continue;
                                if (field.Field.Derivative && fieldData.BinaryWrapperFallback != BinaryGenerationType.Custom) continue;
                                if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + field.Field);
                                }

                                if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                                var dataSet = field.Field as DataType;
                                foreach (var gen in fieldData.GenerationTypes)
                                {
                                    LoquiType loqui = gen.Value as LoquiType;
                                    if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;
                                    foreach (var trigger in gen.Key)
                                    {
                                        fg.AppendLine($"case 0x{trigger.TypeInt.ToString("X")}: // {trigger.Type}");
                                    }
                                    using (new BraceWrapper(fg))
                                    {
                                        await GenerateLastParsedShortCircuit(
                                            obj: obj,
                                            fg: fg,
                                            field: field,
                                            fieldData: fieldData,
                                            toDo: async () =>
                                            {
                                                string recConverter = "null";
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
                                                        "_package.MasterReferences = new MasterReferences"))
                                                    {
                                                        args.Add((subFg) =>
                                                        {
                                                            subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                            using (new DepthWrapper(subFg))
                                                            {
                                                                subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                                using (new BraceWrapper(subFg)
                                                                {
                                                                    AppendParenthesis = true
                                                                })
                                                                {
                                                                    subFg.AppendLine("Master = master.Master,");
                                                                    subFg.AppendLine("FileSize = master.FileSize,");
                                                                    subFg.AppendLine(" FileSize_IsSet = master.FileSize_IsSet");
                                                                }
                                                                subFg.AppendLine(".ToList()");
                                                            }
                                                        });
                                                        args.Add("this.ModKey");
                                                    }
                                                }
                                            });
                                    }
                                }
                            }
                            fg.AppendLine("default:");
                            using (new DepthWrapper(fg))
                            {
                                if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b)))
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        "return base.FillRecordType"))
                                    {
                                        args.AddPassArg("stream");
                                        args.AddPassArg("finalPos");
                                        args.AddPassArg("offset");
                                        args.AddPassArg("type");
                                        args.AddPassArg("lastParsed");
                                        if (obj.GetObjectData().BaseRecordTypeConverter?.FromConversions.Count > 0)
                                        {
                                            args.Add($"recordTypeConverter: recordTypeConverter.Combine({obj.RegistrationName}.BaseConverter)");
                                        }
                                        else
                                        {
                                            args.AddPassArg("recordTypeConverter");
                                        }
                                    }
                                }
                                else
                                {
                                    var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                    if (obj.GetObjectType() == ObjectType.Subrecord)
                                    {
                                        fg.AppendLine($"return TryGet<int?>.Failure;");
                                    }
                                    else if (failOnUnknown)
                                    {
                                        fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                                    }
                                    else
                                    {
                                        fg.AppendLine($"return TryGet<int?>.Succeed(null);");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            if (maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class, MaskType.Normal))
            {
                await GenerateCreateExtras(obj, fg);
            }
            await base.GenerateInCommon(obj, fg, maskTypes);
        }
    }
}