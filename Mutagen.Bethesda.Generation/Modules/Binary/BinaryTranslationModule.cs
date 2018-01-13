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

namespace Mutagen.Bethesda.Generation
{
    public class BinaryTranslationModule : TranslationModule<BinaryTranslationGeneration>
    {
        public override string Namespace => "Mutagen.Bethesda.Binary.";

        public override string ModuleNickname => "Binary";

        public BinaryTranslationModule(LoquiGenerator gen)
            : base(gen)
        {
            this.ExportWithIGetter = false;
            this._typeGenerations[typeof(LoquiType)] = new LoquiBinaryTranslationGeneration(ModuleNickname);
            this._typeGenerations[typeof(BoolNullType)] = new PrimitiveBinaryTranslationGeneration<bool?>();
            this._typeGenerations[typeof(BoolType)] = new PrimitiveBinaryTranslationGeneration<bool>();
            this._typeGenerations[typeof(CharNullType)] = new PrimitiveBinaryTranslationGeneration<char?>();
            this._typeGenerations[typeof(CharType)] = new PrimitiveBinaryTranslationGeneration<char>();
            this._typeGenerations[typeof(DateTimeNullType)] = new PrimitiveBinaryTranslationGeneration<DateTime?>();
            this._typeGenerations[typeof(DateTimeType)] = new PrimitiveBinaryTranslationGeneration<DateTime>();
            this._typeGenerations[typeof(DoubleNullType)] = new PrimitiveBinaryTranslationGeneration<double?>();
            this._typeGenerations[typeof(DoubleType)] = new PrimitiveBinaryTranslationGeneration<double>();
            this._typeGenerations[typeof(EnumType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(EnumNullType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(FloatNullType)] = new PrimitiveBinaryTranslationGeneration<float?>("Float");
            this._typeGenerations[typeof(FloatType)] = new PrimitiveBinaryTranslationGeneration<float>("Float");
            this._typeGenerations[typeof(Int8NullType)] = new PrimitiveBinaryTranslationGeneration<sbyte?>("Int8");
            this._typeGenerations[typeof(Int8Type)] = new PrimitiveBinaryTranslationGeneration<sbyte>("Int8");
            this._typeGenerations[typeof(Int16NullType)] = new PrimitiveBinaryTranslationGeneration<short?>();
            this._typeGenerations[typeof(Int16Type)] = new PrimitiveBinaryTranslationGeneration<short>();
            this._typeGenerations[typeof(Int32NullType)] = new PrimitiveBinaryTranslationGeneration<int?>();
            this._typeGenerations[typeof(Int32Type)] = new PrimitiveBinaryTranslationGeneration<int>();
            this._typeGenerations[typeof(Int64NullType)] = new PrimitiveBinaryTranslationGeneration<long?>();
            this._typeGenerations[typeof(Int64Type)] = new PrimitiveBinaryTranslationGeneration<long>();
            this._typeGenerations[typeof(StringType)] = new StringBinaryTranslationGeneration();
            this._typeGenerations[typeof(FilePathType)] = new FilePathBinaryTranslationGeneration();
            this._typeGenerations[typeof(UInt8NullType)] = new PrimitiveBinaryTranslationGeneration<byte?>();
            this._typeGenerations[typeof(UInt8Type)] = new PrimitiveBinaryTranslationGeneration<byte>();
            this._typeGenerations[typeof(UInt16NullType)] = new PrimitiveBinaryTranslationGeneration<ushort?>();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>();
            this._typeGenerations[typeof(UInt32NullType)] = new PrimitiveBinaryTranslationGeneration<uint?>();
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>();
            this._typeGenerations[typeof(UInt64NullType)] = new PrimitiveBinaryTranslationGeneration<ulong?>();
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>();
            this._typeGenerations[typeof(RawFormIDType)] = new PrimitiveBinaryTranslationGeneration<RawFormID>();
            this._typeGenerations[typeof(FormIDLinkType)] = new FormIDLinkBinaryTranslationGeneration();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new ByteArrayTranslationGeneration();
            this._typeGenerations[typeof(BufferType)] = new BufferBinaryTranslationGeneration();
            this._typeGenerations[typeof(DataType)] = new DataBinaryTranslationModule();
            this._typeGenerations[typeof(ColorType)] = new PrimitiveBinaryTranslationGeneration<Color>();
            this._typeGenerations[typeof(SpecialParseType)] = new SpecialParseTranslationGeneration();
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI(
                    majorAPI: new string[] { "MutagenWriter writer" },
                    optionalAPI: null,
                    customAPI: new CustomMethodAPI[] { CustomMethodAPI.Private($"{nameof(RecordTypeConverter)} recordTypeConverter", "null") }),
                readerAPI: new MethodAPI(
                    majorAPI: new string[] { "MutagenFrame frame" },
                    optionalAPI: null,
                    customAPI: new CustomMethodAPI[] { CustomMethodAPI.Private($"{nameof(RecordTypeConverter)} recordTypeConverter", "null") }));
            this.MinorAPIs.Add(
                new TranslationModuleAPI(new MethodAPI("string path"))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromPathOut,
                        ConvertFromPathIn)
                });
            this.MinorAPIs.Add(
                new TranslationModuleAPI(new MethodAPI("Stream stream"))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromStreamOut,
                        ConvertFromStreamIn)
                });
        }

        public override async Task PostLoad(ObjectGeneration obj)
        {
            foreach (var gen in _typeGenerations.Values)
            {
                gen.Module = this;
                gen.MaskModule = this.Gen.MaskModule;
            }
        }

        public override IEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
        {
            return base.RequiredUsingStatements(obj).And("Mutagen.Bethesda.Binary");
        }

        public override IEnumerable<string> Interfaces(ObjectGeneration obj)
        {
            yield break;
        }

        private void ConvertFromStreamOut(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var writer = new MutagenWriter(stream))");
            using (new BraceWrapper(fg))
            {
                internalToDo("writer");
            }
        }

        private void ConvertFromStreamIn(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var reader = new MutagenReader(stream))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var frame = new MutagenFrame(reader);");
                internalToDo("frame");
            }
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            GenerateCustomPartials(obj, fg);
            await GenerateCreateExtras(obj, fg);
        }

        private void GenerateCustomPartials(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields())
            {
                if (!field.TryGetFieldData(out var mutaData)) continue;
                if (!mutaData.CustomBinary) continue;
                using (var args = new FunctionWrapper(fg,
                    $"static partial void FillBinary_{field.Name}_Custom{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres)
                {
                    SemiColon = true
                })
                {
                    args.Add($"{nameof(MutagenFrame)} frame");
                    args.Add($"{obj.InterfaceStr} item");
                    if (field.HasIndex)
                    {
                        args.Add($"int fieldIndex");
                        args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                    }
                    else
                    {
                        args.Add("bool doMasks");
                        args.Add($"out {obj.Mask(MaskType.Error)} errorMask");
                    }
                }
                fg.AppendLine();
                using (var args = new FunctionWrapper(fg,
                    $"static partial void WriteBinary_{field.Name}_Custom{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres)
                {
                    SemiColon = true
                })
                {
                    args.Add($"{nameof(MutagenWriter)} writer");
                    args.Add($"{obj.Getter_InterfaceStr} item");
                    if (field.HasIndex)
                    {
                        args.Add($"int fieldIndex");
                        args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                    }
                    else
                    {
                        args.Add("bool doMasks");
                        args.Add($"out {obj.Mask(MaskType.Error)} errorMask");
                    }
                }
                fg.AppendLine();
                using (var args = new FunctionWrapper(fg,
                    $"public static void WriteBinary_{field.Name}{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add($"{nameof(MutagenWriter)} writer");
                    args.Add($"{obj.Getter_InterfaceStr} item");
                    if (field.HasIndex)
                    {
                        args.Add($"int fieldIndex");
                        args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                    }
                    else
                    {
                        args.Add("bool doMasks");
                        args.Add($"out {obj.Mask(MaskType.Error)} errorMask");
                    }
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"WriteBinary_{field.Name}_Custom"))
                    {
                        args.Add("writer: writer");
                        args.Add("item: item");
                        if (field.HasIndex)
                        {
                            args.Add($"fieldIndex: fieldIndex");
                            args.Add($"errorMask: errorMask");
                        }
                        else
                        {
                            args.Add("doMasks: doMasks");
                            args.Add($"errorMask: out errorMask");
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        public override async Task GenerateInCommonExt(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInCommonExt(obj, fg);
            GenerateWriteExtras(obj, fg);
        }

        private bool HasRecordTypeFields(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (field.TryGetFieldData(out var data)
                    && data.HasTrigger) return true;
            }
            return false;
        }

        private bool HasEmbeddedFields(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (field is SetMarkerType) continue;
                if (!field.TryGetFieldData(out var data)
                    || !data.HasTrigger) return true;
            }
            return false;
        }

        private async Task GenerateCreateExtras(ObjectGeneration obj, FileGeneration fg)
        {
            bool typelessStruct = obj.GetObjectType() == ObjectType.Subrecord && !obj.HasRecordType();
            if (!obj.Abstract)
            {
                ObjectType objType = obj.GetObjectType();

                using (var args = new FunctionWrapper(fg,
                    $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add("MutagenFrame frame");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                    args.Add($"{nameof(RecordTypeConverter)} recordTypeConverter");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"var ret = new {obj.Name}{obj.GenericTypes}();");
                    fg.AppendLine("try");
                    using (new BraceWrapper(fg))
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
                                            $"frame = frame.Spawn({nameof(HeaderTranslation)}.ParseSubrecord",
                                            suffixLine: ")"))
                                        {
                                            args.Add("frame");
                                            args.Add($"{obj.GetTriggeringSource()}");
                                        }
                                    }
                                    break;
                                case ObjectType.Record:
                                    using (var args = new ArgsWrapper(fg,
                                        $"frame = frame.Spawn({nameof(HeaderTranslation)}.ParseRecord",
                                        suffixLine: ")"))
                                    {
                                        args.Add("frame");
                                        args.Add($"{obj.GetTriggeringSource()}");
                                    }
                                    break;
                                case ObjectType.Group:
                                    using (var args = new ArgsWrapper(fg,
                                        $"frame = frame.Spawn({nameof(HeaderTranslation)}.ParseGroup",
                                        suffixLine: ")"))
                                    {
                                        args.Add("frame");
                                    }
                                    break;
                                case ObjectType.Mod:
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        fg.AppendLine("using (frame)");
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"Fill_{ModuleNickname}_Structs"))
                            {
                                args.Add("item: ret");
                                args.Add("frame: frame");
                                args.Add("errorMask: errorMask");
                            }
                            if (HasRecordTypeFields(obj))
                            {
                                if (typelessStruct)
                                {
                                    fg.AppendLine($"{obj.FieldIndexName}? lastParsed = null;");
                                }
                                fg.AppendLine($"while (!frame.Complete)");
                                using (new BraceWrapper(fg))
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"var parsed = Fill_{ModuleNickname}_RecordTypes"))
                                    {
                                        args.Add("item: ret");
                                        args.Add("frame: frame");
                                        if (typelessStruct)
                                        {
                                            args.Add("lastParsed: lastParsed");
                                        }
                                        args.Add("errorMask: errorMask");
                                        args.Add($"recordTypeConverter: recordTypeConverter");
                                    }
                                    fg.AppendLine("if (parsed.Failed) break;");
                                    if (typelessStruct)
                                    {
                                        fg.AppendLine("lastParsed = parsed.Value;");
                                    }
                                }
                            }
                        }
                        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
                        {
                            if (!(field is DataType dataType)) continue;
                            List<TypeGeneration> affectedFields = new List<TypeGeneration>();
                            foreach (var subField in dataType.IterateFieldsWithMeta())
                            {
                                if (!subField.EncounteredBreaks.Any()
                                    && subField.Range == null)
                                {
                                    continue;
                                }
                                affectedFields.Add(subField.Field);
                            }
                            if (affectedFields.Count == 0) continue;
                            fg.AppendLine($"if (ret.{dataType.StateName} != default({dataType.EnumName}))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("Action unsubAction = () =>");
                                using (new BraceWrapper(fg) { AppendSemicolon = true })
                                {
                                    foreach (var subField in affectedFields)
                                    {
                                        fg.AppendLine($"ret.{subField.Property}.Unsubscribe(DataTypeStateSubber);");
                                    }
                                    fg.AppendLine($"ret.{dataType.StateName} = default({dataType.EnumName});");
                                }
                                foreach (var subField in affectedFields)
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"ret.{subField.Property}.Subscribe"))
                                    {
                                        args.Add($"owner: DataTypeStateSubber");
                                        args.Add("callback: unsubAction");
                                        args.Add("fireInitial: false");
                                    }
                                }
                            }
                        }
                    }
                    fg.AppendLine("catch (Exception ex)");
                    fg.AppendLine("when (errorMask != null)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("errorMask().Overall = ex;");
                    }
                    fg.AppendLine("return ret;");
                }
                fg.AppendLine();
            }

            if ((!obj.Abstract && obj.BaseClassTrail().All((b) => b.Abstract)) || HasEmbeddedFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"protected static void Fill_{ModuleNickname}_Structs{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("MutagenFrame frame");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{obj.BaseClass.Name}.Fill_{ModuleNickname}_Structs"))
                        {
                            args.Add("item: item");
                            args.Add("frame: frame");
                            args.Add("errorMask: errorMask");
                        }
                    }
                    foreach (var field in obj.IterateFields(
                        nonIntegrated: true,
                        expandSets: SetMarkerType.ExpandSets.False))
                    {
                        if (field is SetMarkerType) continue;
                        if (field.TryGetFieldData(out var data)
                            && data.HasTrigger) continue;
                        if (field.Derivative && !data.CustomBinary) continue;
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }
                        fg.AppendLine($"if (frame.Complete) return;");
                        GenerateFillSnippet(obj, fg, field, generator, "frame");
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"protected static TryGet<{obj.FieldIndexName}?> Fill_{ModuleNickname}_RecordTypes{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("MutagenFrame frame");
                    if (typelessStruct)
                    {
                        args.Add($"{obj.FieldIndexName}? lastParsed");
                    }
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                    args.Add($"{nameof(RecordTypeConverter)} recordTypeConverter = null");
                }
                using (new BraceWrapper(fg))
                {
                    var mutaObjType = obj.GetObjectType();
                    string funcName;
                    switch (mutaObjType)
                    {
                        case ObjectType.Subrecord:
                        case ObjectType.Record:
                            funcName = $"GetNextSubRecordType";
                            break;
                        case ObjectType.Group:
                            funcName = $"GetNextRecordType";
                            break;
                        case ObjectType.Mod:
                            funcName = $"GetNextType";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"var nextRecordType = {nameof(HeaderTranslation)}.{funcName}"))
                    {
                        args.Add("frame: frame");
                        args.Add("contentLength: out var contentLength");
                        args.Add("recordTypeConverter: recordTypeConverter");
                    }
                    fg.AppendLine("switch (nextRecordType.Type)");
                    using (new BraceWrapper(fg))
                    {
                        foreach (var field in obj.IterateFieldIndices(
                            expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                            nonIntegrated: true))
                        {
                            if (!field.Field.TryGetFieldData(out var data)
                                || !data.HasTrigger
                                || data.TriggeringRecordTypes.Count == 0) continue;
                            if (field.Field.Derivative && !data.CustomBinary) continue;
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }

                            if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                            var dataSet = field.Field as DataType;
                            foreach (var gen in data.GenerationTypes)
                            {
                                if (gen.Value is LoquiType loqui)
                                {
                                    if (loqui.TargetObjectGeneration?.Abstract ?? false) continue;
                                }
                                foreach (var trigger in gen.Key)
                                {
                                    fg.AppendLine($"case \"{trigger.Type}\":");
                                }
                                using (new DepthWrapper(fg))
                                {
                                    if (typelessStruct && data.IsTriggerForObject)
                                    {
                                        if (dataSet != null)
                                        {
                                            fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= {dataSet.SubFields.Last().IndexEnumName}) return TryGet<{obj.FieldIndexName}?>.Failure;");
                                        }
                                        else if (field.Field is SpecialParseType)
                                        {
                                            var objFields = obj.IterateFieldIndices(nonIntegrated: false).ToList();
                                            var nextField = objFields.FirstOrDefault((i) => i.InternalIndex > field.InternalIndex);
                                            var prevField = objFields.LastOrDefault((i) => i.InternalIndex < field.InternalIndex);
                                            if (nextField.Field != null)
                                            {
                                                fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= {nextField.Field.IndexEnumName}) return TryGet<{obj.FieldIndexName}?>.Failure;");
                                            }
                                            else if (prevField.Field != null)
                                            {
                                                fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= {prevField.Field.IndexEnumName}) return TryGet<{obj.FieldIndexName}?>.Failure;");
                                            }
                                        }
                                        else
                                        {
                                            fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= {field.Field.IndexEnumName}) return TryGet<{obj.FieldIndexName}?>.Failure;");
                                        }
                                    }

                                    GenerateFillSnippet(obj, fg, gen.Value, generator, "frame");
                                    if (dataSet != null)
                                    {
                                        fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Succeed({dataSet.SubFields.Last().IndexEnumName});");
                                    }
                                    else if (field.Field is SpecialParseType special)
                                    {
                                        fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Succeed(lastParsed);");
                                    }
                                    else
                                    {
                                        fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Succeed({field.Field.IndexEnumName});");
                                    }
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
                                if (!field.Field.TryGetFieldData(out var data)
                                    || !data.HasTrigger
                                    || data.TriggeringRecordTypes.Count > 0) continue;
                                if (field.Field.Derivative && !data.CustomBinary) continue;
                                if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + field.Field);
                                }

                                if (generator.ShouldGenerateCopyIn(field.Field))
                                {
                                    using (var args = new IfWrapper(fg, ANDs: true, first: first))
                                    {
                                        foreach (var trigger in data.TriggeringRecordAccessors)
                                        {
                                            args.Checks.Add($"nextRecordType.Equals({trigger})");
                                        }
                                    }
                                    first = false;
                                    using (new BraceWrapper(fg))
                                    {
                                        GenerateFillSnippet(obj, fg, field.Field, generator, "frame");
                                        fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Failure;");
                                    }
                                }
                            }

                            // Default case
                            if (obj.HasBaseObject && obj.BaseClassTrail().Any((b) => HasRecordTypeFields(b)))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return {obj.BaseClass.Name}.Fill_{ModuleNickname}_RecordTypes",
                                    suffixLine: $".Bubble((i) => {obj.ExtCommonName}.ConvertFieldIndex(i))"))
                                {
                                    args.Add("item: item");
                                    args.Add("frame: frame");
                                    args.Add($"errorMask: errorMask");
                                }
                            }
                            else
                            {
                                var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                if (mutaObjType == ObjectType.Subrecord)
                                {
                                    fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Failure;");
                                }
                                else if (failOnUnknown)
                                {
                                    fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                                }
                                else
                                {
                                    fg.AppendLine($"errorMask().Warnings.Add($\"Unexpected header {{nextRecordType.Type}} at position {{frame.Position}}\");");
                                    string addString;
                                    switch (obj.GetObjectType())
                                    {
                                        case ObjectType.Mod:
                                            addString = null;
                                            break;
                                        case ObjectType.Subrecord:
                                        case ObjectType.Record:
                                            addString = " + Constants.SUBRECORD_LENGTH";
                                            break;
                                        case ObjectType.Group:
                                            addString = " + Constants.RECORD_LENGTH";
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                    fg.AppendLine($"frame.Position += contentLength{addString};");
                                    fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Succeed(null);");
                                }
                            }
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        private void GenerateFillSnippet(ObjectGeneration obj, FileGeneration fg, TypeGeneration field, BinaryTranslationGeneration generator, string frameAccessor)
        {
            if (field is DataType set)
            {
                fg.AppendLine($"{frameAccessor}.Position += Constants.SUBRECORD_LENGTH;");
                fg.AppendLine($"using (var dataFrame = {frameAccessor}.Spawn(contentLength))");
                using (new BraceWrapper(fg))
                {
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
                                fg.AppendLine($"item.{set.StateName} |= {set.EnumName}.Break{subField.BreakIndex};");
                                fg.AppendLine($"return TryGet<{obj.FieldIndexName}?>.Succeed({set.SubFields.TryGet(subField.FieldIndex - 1)?.IndexEnumName ?? "null"});");
                            }
                        }
                        if (subField.Range != null && !isInRange)
                        {
                            isInRange = true;
                            fg.AppendLine($"if (dataFrame.TotalLength > {subField.Range.DataSetSizeMin})");
                            fg.AppendLine("{");
                            fg.Depth++;
                            fg.AppendLine($"item.{set.StateName} |= {set.EnumName}.Range{subField.RangeIndex};");
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
                }
                return;
            }

            var data = field.GetFieldData();
            if (data.CustomBinary)
            {
                if (data.HasTrigger)
                {
                    fg.AppendLine($"using (var subFrame = {frameAccessor}.Spawn(Constants.SUBRECORD_LENGTH + contentLength))");
                }
                using (new BraceWrapper(fg, doIt: data.HasTrigger))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"FillBinary_{field.Name}_Custom"))
                    {
                        args.Add($"frame: {(data.HasTrigger ? "subFrame" : frameAccessor)}");
                        args.Add("item: item");
                        if (field.HasIndex)
                        {
                            if (field.IntegrateField)
                            {
                                args.Add($"fieldIndex: (int){field.IndexEnumName}");
                            }
                            args.Add($"errorMask: errorMask");
                        }
                        else
                        {
                            args.Add("doMasks: doMasks");
                            args.Add($"errorMask: out errorMask");
                        }
                    }
                }
                return;
            }
            generator.GenerateCopyIn(
                fg: fg,
                objGen: obj,
                typeGen: field,
                readerAccessor: frameAccessor,
                itemAccessor: new Accessor()
                {
                    DirectAccess = $"item.{field.ProtectedName}",
                    PropertyAccess = field.Bare ? null : $"item.{field.ProtectedProperty}"
                },
                doMaskAccessor: "doMasks",
                maskAccessor: $"errorMask");
        }

        private void ConvertFromPathOut(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var writer = new MutagenWriter(path))");
            using (new BraceWrapper(fg))
            {
                internalToDo("writer");
            }
        }

        private void ConvertFromPathIn(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var reader = new MutagenReader(path))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("var frame = new MutagenFrame(reader);");
                internalToDo("frame");
            }
        }

        protected override void GenerateCopyInSnippet(ObjectGeneration obj, FileGeneration fg, bool usingErrorMask)
        {
            using (var args = new ArgsWrapper(fg,
                $"LoquiBinaryTranslation<{obj.ObjectName}, {(usingErrorMask ? obj.Mask(MaskType.Error) : obj.Mask_GenericAssumed(MaskType.Error))}>.Instance.CopyIn"))
            using (new DepthWrapper(fg))
            {
                foreach (var item in this.MainAPI.ReaderPassArgs)
                {
                    args.Add(item);
                }
                args.Add($"item: this");
                args.Add($"skipProtected: true");
                if (usingErrorMask)
                {
                    args.Add($"doMasks: true");
                    args.Add($"mask: out errorMask");
                }
                else
                {
                    args.Add($"doMasks: false");
                    args.Add($"mask: out var errorMask");
                }
                args.Add($"cmds: cmds");
            }
        }

        protected override void GenerateCreateSnippet(ObjectGeneration obj, FileGeneration fg)
        {
            fg.AppendLine($"{obj.Mask(MaskType.Error)} errMaskRet = null;");
            using (var args = new ArgsWrapper(fg,
                $"var ret = Create_{ModuleNickname}_Internal"))
            {
                args.Add("frame: frame");
                args.Add($"errorMask: doMasks ? () => errMaskRet ?? (errMaskRet = new {obj.Mask(MaskType.Error)}()) : default(Func<{obj.Mask(MaskType.Error)}>)");
                args.Add($"recordTypeConverter: recordTypeConverter");
            }
            fg.AppendLine($"return (ret, errMaskRet);");
        }

        protected override void GenerateWriteSnippet(ObjectGeneration obj, FileGeneration fg)
        {
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
                        $"Write_{ModuleNickname}_Embedded"))
                    {
                        args.Add($"item: item");
                        args.Add($"writer: writer");
                        args.Add($"errorMask: errorMask");
                    }
                }
                else
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                    if (firstBase != null)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{firstBase.ExtCommonName}.Write_{ModuleNickname}_Embedded"))
                        {
                            args.Add($"item: item");
                            args.Add($"writer: writer");
                            args.Add($"errorMask: errorMask");
                        }
                    }
                }
                if (HasRecordTypeFields(obj))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"Write_{ModuleNickname}_RecordTypes"))
                    {
                        args.Add($"item: item");
                        args.Add($"writer: writer");
                        args.Add($"recordTypeConverter: recordTypeConverter");
                        args.Add($"errorMask: errorMask");
                    }
                }
                else
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasRecordTypeFields(b));
                    if (firstBase != null)
                    {
                        using (var args = new ArgsWrapper(fg,
                        $"{firstBase.ExtCommonName}.Write_{ModuleNickname}_RecordTypes"))
                        {
                            args.Add($"item: item");
                            args.Add($"writer: writer");
                            args.Add($"recordTypeConverter: recordTypeConverter");
                            args.Add($"errorMask: errorMask");
                        }
                    }
                }
            }
        }

        private void GenerateWriteExtras(ObjectGeneration obj, FileGeneration fg)
        {
            if (HasEmbeddedFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void Write_{ModuleNickname}_Embedded{obj.GenericTypes_ErrMask}",
                    wheres: obj.GenerateWhereClauses().And(obj.GenericTypes_ErrorMaskWheres).ToArray()))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("MutagenWriter writer");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasBaseObject)
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{firstBase.ExtCommonName}.Write_{ModuleNickname}_Embedded"))
                            {
                                args.Add("item: item");
                                args.Add("writer: writer");
                                args.Add("errorMask: errorMask");
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.False))
                    {
                        if (field.TryGetFieldData(out var data)
                            && data.HasTrigger) continue;
                        if (field.Derivative && !data.CustomBinary) continue;
                        var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                        if (data.CustomBinary)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{obj.ObjectName}.WriteBinary_{field.Name}"))
                            {
                                args.Add("writer: writer");
                                args.Add("item: item");
                                args.Add($"fieldIndex: (int){field.IndexEnumName}");
                                args.Add("errorMask: errorMask");
                            }
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
                            doMaskAccessor: null,
                            maskAccessor: "errorMask");
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static void Write_{ModuleNickname}_RecordTypes{obj.GenericTypes_ErrMask}",
                    wheres: obj.GenerateWhereClauses().And(obj.GenericTypes_ErrorMaskWheres).ToArray()))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("MutagenWriter writer");
                    args.Add("RecordTypeConverter recordTypeConverter");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasBaseObject)
                    {
                        var firstBase = obj.BaseClassTrail().FirstOrDefault((f) => HasRecordTypeFields(f));
                        if (firstBase != null)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{firstBase.ExtCommonName}.Write_{ModuleNickname}_RecordTypes"))
                            {
                                args.Add($"item: item");
                                args.Add("writer: writer");
                                args.Add("recordTypeConverter: recordTypeConverter");
                                args.Add($"errorMask: errorMask");
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                    {
                        if (!field.TryGetFieldData(out var data)
                            || !data.HasTrigger) continue;
                        if (field.Derivative && !data.CustomBinary) continue;
                        if (data.CustomBinary)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{obj.ObjectName}.WriteBinary_{field.Name}"))
                            {
                                args.Add("writer: writer");
                                args.Add("item: item");
                                args.Add($"fieldIndex: (int){field.IndexEnumName}");
                                args.Add("errorMask: errorMask");
                            }
                            continue;
                        }
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }

                        if (field is DataType dataType)
                        {
                            fg.AppendLine($"using (HeaderExport.ExportSubRecordHeader(writer, {obj.RecordTypeHeaderName(data.RecordType.Value)}))");
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
                                    if (subData.CustomBinary)
                                    {
                                        using (var args = new ArgsWrapper(fg,
                                            $"{obj.ObjectName}.WriteBinary_{subField.Field.Name}"))
                                        {
                                            args.Add("writer: writer");
                                            args.Add("item: item");
                                            args.Add($"fieldIndex: (int){subField.Field.IndexEnumName}");
                                            args.Add("errorMask: errorMask");
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
                                        itemAccessor: new Accessor(subField.Field, "item."),
                                        doMaskAccessor: "errorMask != null",
                                        maskAccessor: $"errorMask");
                                }
                                for (int i = 0; i < dataType.BreakIndices.Count; i++)
                                {
                                    fg.Depth--;
                                    fg.AppendLine("}");
                                }
                            }
                        }
                        else
                        {
                            if (!generator.ShouldGenerateWrite(field)) continue;
                            generator.GenerateWrite(
                                fg: fg,
                                objGen: obj,
                                typeGen: field,
                                writerAccessor: "writer",
                                itemAccessor: new Accessor(field, "item."),
                                doMaskAccessor: "errorMask != null",
                                maskAccessor: $"errorMask");
                        }
                    }
                }
                fg.AppendLine();
            }
        }
    }
}
