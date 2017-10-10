using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Binary;

namespace Mutagen.Generation
{
    public class BinaryTranslationModule : TranslationModule<BinaryTranslationGeneration>
    {
        public override string Namespace => "Mutagen.Binary.";

        public override string ModuleNickname => "Binary";

        public BinaryTranslationModule(LoquiGenerator gen)
            : base(gen)
        {
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
            this._typeGenerations[typeof(UInt8NullType)] = new PrimitiveBinaryTranslationGeneration<byte?>();
            this._typeGenerations[typeof(UInt8Type)] = new PrimitiveBinaryTranslationGeneration<byte>();
            this._typeGenerations[typeof(UInt16NullType)] = new PrimitiveBinaryTranslationGeneration<ushort?>();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>();
            this._typeGenerations[typeof(UInt32NullType)] = new PrimitiveBinaryTranslationGeneration<uint?>();
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>();
            this._typeGenerations[typeof(UInt64NullType)] = new PrimitiveBinaryTranslationGeneration<ulong?>();
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>();
            this._typeGenerations[typeof(FormIDType)] = new PrimitiveBinaryTranslationGeneration<FormID>();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new ByteArrayTranslationGeneration();
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI("BinaryWriter writer"),
                readerAPI: new MethodAPI("BinaryReader reader"));
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

        public override void PostLoad(ObjectGeneration obj)
        {
            foreach (var gen in _typeGenerations.Values)
            {
                gen.Module = this;
                gen.MaskModule = this.Gen.MaskModule;
            }
        }

        public override IEnumerable<string> RequiredUsingStatements()
        {
            yield return "Mutagen.Binary";
        }

        public override IEnumerable<string> Interfaces(ObjectGeneration obj)
        {
            yield break;
        }

        private void ConvertFromStreamOut(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var writer = new BinaryWriter(stream))");
            using (new BraceWrapper(fg))
            {
                internalToDo("writer");
            }
        }

        private void ConvertFromStreamIn(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("using (var reader = new BinaryReader(stream))");
            using (new BraceWrapper(fg))
            {
                internalToDo("reader");
            }
        }

        public override void GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            base.GenerateInClass(obj, fg);
            GenerateCreateExtras(obj, fg);
        }

        public override void GenerateInCommonExt(ObjectGeneration obj, FileGeneration fg)
        {
            base.GenerateInCommonExt(obj, fg);
            GenerateWriteExtras(obj, fg);
        }

        private bool HasRecordTypeFields(ObjectGeneration obj)
        {
            foreach (var field in obj.Fields)
            {
                if (field.TryGetFieldData(out var data)
                    && data.TriggeringRecordAccessor != null) return true;
            }
            return false;
        }

        private bool HasEmbeddedFields(ObjectGeneration obj)
        {
            foreach (var field in obj.Fields)
            {
                if (!field.TryGetFieldData(out var data)
                    || data.TriggeringRecordAccessor == null) return true;
            }
            return false;
        }

        private void GenerateCreateExtras(ObjectGeneration obj, FileGeneration fg)
        {
            if (!obj.Abstract)
            {
                ObjectType objType = obj.GetObjectType();
                if (objType != ObjectType.Struct)
                {
                    using (var args = new FunctionWrapper(fg,
                        $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal{obj.Mask_GenericClause(MaskType.Error)}",
                        wheres: obj.GenericTypes_ErrorMaskWheres))
                    {
                        args.Add("BinaryReader reader");
                        args.Add("bool doMasks");
                        args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                    }
                    using (new BraceWrapper(fg))
                    {
                        switch (objType)
                        {
                            case ObjectType.Record:
                            case ObjectType.Subrecord:
                                RecordType? mutaData = obj.GetTriggeringRecordType();
                                string funcName;
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Struct:
                                        funcName = "GetSubrecord";
                                        break;
                                    case ObjectType.Subrecord:
                                        funcName = "ParseSubrecord";
                                        break;
                                    case ObjectType.Record:
                                        funcName = "ParseRecord";
                                        break;
                                    case ObjectType.Group:
                                    case ObjectType.Mod:
                                    default:
                                        throw new NotImplementedException();
                                }
                                using (var args = new ArgsWrapper(fg,
                                    $"var finalPosition = {nameof(HeaderTranslation)}.{funcName}"))
                                {
                                    args.Add("reader");
                                    args.Add($"{obj.RegistrationName}.{mutaData.Value.HeaderName}");
                                }
                                break;
                            case ObjectType.Group:
                                using (var args = new ArgsWrapper(fg,
                                    $"var finalPosition = {nameof(HeaderTranslation)}.ParseGroup"))
                                {
                                    args.Add("reader");
                                }
                                break;
                            case ObjectType.Mod:
                                fg.AppendLine($"var finalPosition = reader.BaseStream.Length;");
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        using (var args = new ArgsWrapper(fg,
                            $"return Create_{ModuleNickname}_Internal"))
                        {
                            args.Add("reader: reader");
                            args.Add("doMasks: doMasks");
                            args.Add("finalPosition: finalPosition");
                            args.Add("errorMask: errorMask");
                        }
                    }
                    fg.AppendLine();
                }

                using (var args = new FunctionWrapper(fg,
                    $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add("BinaryReader reader");
                    args.Add("bool doMasks");
                    if (objType != ObjectType.Struct)
                    {
                        args.Add("long finalPosition");
                    }
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"var ret = new {obj.Name}{obj.GenericTypes}();");
                    fg.AppendLine("try");
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"Fill_{ModuleNickname}"))
                        {
                            args.Add("item: ret");
                            args.Add("reader: reader");
                            args.Add("doMasks: doMasks");
                            args.Add("errorMask: errorMask");
                        }
                        if (HasRecordTypeFields(obj))
                        {
                            if (objType != ObjectType.Struct)
                            {
                                fg.AppendLine($"while (reader.BaseStream.Position < finalPosition)");
                            }
                            else
                            {
                                fg.AppendLine($"while (true)");
                            }
                            using (new BraceWrapper(fg))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"Fill_{ModuleNickname}_RecordTypes"))
                                {
                                    args.Add("item: ret");
                                    args.Add("reader: reader");
                                    args.Add("doMasks: doMasks");
                                    args.Add("errorMask: errorMask");
                                }
                            }
                        }
                        if (objType != ObjectType.Struct)
                        {
                            fg.AppendLine($"if (reader.BaseStream.Position != finalPosition)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("reader.BaseStream.Position = finalPosition;");
                                fg.AppendLine("throw new ArgumentException(\"Read more bytes than allocated\");");
                            }
                        }
                    }
                    fg.AppendLine("catch (Exception ex)");
                    fg.AppendLine("when (doMasks)");
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
                    $"protected static void Fill_{ModuleNickname}{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("BinaryReader reader");
                    args.Add("bool doMasks");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{obj.BaseClass.Name}.Fill_{ModuleNickname}"))
                        {
                            args.Add("item: item");
                            args.Add("reader: reader");
                            args.Add("doMasks: doMasks");
                            args.Add("errorMask: errorMask");
                        }
                    }
                    foreach (var field in obj.Fields)
                    {
                        if (field.TryGetFieldData(out var data)
                            && data.TriggeringRecordAccessor != null) continue;
                        if (field.Derivative && !data.CustomBinary) continue;
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }
                        GenerateFillSnippet(obj, fg, field, generator);
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"protected static void Fill_{ModuleNickname}_RecordTypes{obj.Mask_GenericClause(MaskType.Error)}",
                    wheres: obj.GenericTypes_ErrorMaskWheres))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("BinaryReader reader");
                    args.Add("bool doMasks");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    var mutaObjType = obj.GetObjectType();
                    string funcName;
                    switch (mutaObjType)
                    {
                        case ObjectType.Struct:
                        case ObjectType.Subrecord:
                        case ObjectType.Record:
                            funcName = $"ReadNextSubRecordType";
                            break;
                        case ObjectType.Group:
                            funcName = $"ReadNextRecordType";
                            break;
                        case ObjectType.Mod:
                            funcName = $"ReadNextType";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"var nextRecordType = {nameof(HeaderTranslation)}.{funcName}"))
                    {
                        args.Add("reader: reader");
                        args.Add("contentLength: out var subLength");
                    }
                    fg.AppendLine("switch (nextRecordType.Type)");
                    using (new BraceWrapper(fg))
                    {
                        foreach (var field in obj.IterateFields())
                        {
                            if (!field.Field.TryGetFieldData(out var data)
                                || data.TriggeringRecordAccessor == null
                                || !data.RecordType.HasValue) continue;
                            if (field.Field.Derivative && !data.CustomBinary) continue;
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }

                            if (generator.ShouldGenerateCopyIn(field.Field))
                            {
                                fg.AppendLine($"case \"{data.TriggeringRecordType.Value.Type}\":");
                                GenerateFillSnippet(obj, fg, field.Field, generator);
                                fg.AppendLine("break;");
                            }
                        }
                        fg.AppendLine($"default:");
                        using (new DepthWrapper(fg))
                        {
                            bool first = true;
                            // Generic options
                            foreach (var field in obj.IterateFields())
                            {
                                if (!field.Field.TryGetFieldData(out var data)
                                    || data.TriggeringRecordAccessor == null
                                    || data.RecordType.HasValue) continue;
                                if (field.Field.Derivative && !data.CustomBinary) continue;
                                if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + field.Field);
                                }

                                if (generator.ShouldGenerateCopyIn(field.Field))
                                {
                                    fg.AppendLine($"if{(first ? "" : " else")} (nextRecordType.Equals({data.TriggeringRecordAccessor}))");
                                    using (new BraceWrapper(fg))
                                    {
                                        GenerateFillSnippet(obj, fg, field.Field, generator, needBrace: false);
                                        fg.AppendLine("break;");
                                    }
                                }
                            }

                            // Default case
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Struct:
                                case ObjectType.Subrecord:
                                    fg.AppendLine($"reader.BaseStream.Position -= Constants.SUBRECORD_LENGTH;");
                                    break;
                                case ObjectType.Record:
                                    fg.AppendLine($"reader.BaseStream.Position -= Constants.RECORD_LENGTH;");
                                    break;
                                case ObjectType.Group:
                                case ObjectType.Mod:
                                default:
                                    break;
                            }
                            if (obj.HasBaseObject && obj.BaseClassTrail().Any((b) => HasRecordTypeFields(b)))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"{obj.BaseClass.Name}.Fill_{ModuleNickname}_RecordTypes"))
                                {
                                    args.Add("item: item");
                                    args.Add("reader: reader");
                                    args.Add("doMasks: doMasks");
                                    args.Add($"errorMask: errorMask");
                                    fg.AppendLine("break;");
                                }
                            }
                            else
                            {
                                fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {reader.BaseStream.Position}\");");
                            }
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        private void GenerateFillSnippet(ObjectGeneration obj, FileGeneration fg, TypeGeneration field, BinaryTranslationGeneration generator, bool needBrace = true)
        {
            var data = field.GetFieldData();
            if (data.CustomBinary)
            {
                fg.AppendLine($"FillBinary{field.Name}(reader, item);");
                return;
            }
            using (new BraceWrapper(fg, doIt: needBrace))
            {
                var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                fg.AppendLine($"{maskType} subMask;");
                generator.GenerateCopyIn(
                    fg: fg,
                    objGen: obj,
                    typeGen: field,
                    readerAccessor: "reader",
                    itemAccessor: new Accessor()
                    {
                        DirectAccess = $"item.{field.ProtectedName}",
                        PropertyAccess = field.Notifying == NotifyingOption.None ? null : $"item.{field.ProtectedProperty}"
                    },
                    doMaskAccessor: "doMasks",
                    maskAccessor: $"subMask");
                fg.AppendLine("if (doMasks && subMask != null)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"errorMask().{field.Name} = subMask;");
                }
            }
        }

        private void ConvertFromPathOut(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("using (var writer = new BinaryWriter(fileStream))");
                using (new BraceWrapper(fg))
                {
                    internalToDo("writer");
                }
            }
        }

        private void ConvertFromPathIn(FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("using (var reader = new BinaryReader(fileStream))");
                using (new BraceWrapper(fg))
                {
                    internalToDo("reader");
                }
            }
        }

        protected override void GenerateCopyInSnippet(ObjectGeneration obj, FileGeneration fg, bool usingErrorMask)
        {
            using (var args = new ArgsWrapper(fg,
                $"LoquiBinaryTranslation<{obj.ObjectName}, {(usingErrorMask ? obj.Mask(MaskType.Error): obj.Mask_GenericAssumed(MaskType.Error))}>.Instance.CopyIn"))
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
                args.Add("reader: reader");
                args.Add("doMasks: doMasks");
                args.Add($"errorMask: doMasks ? () => errMaskRet ?? (errMaskRet = new {obj.Mask(MaskType.Error)}()) : default(Func<{obj.Mask(MaskType.Error)}>)");
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
                    args.Add($"record: {obj.RegistrationName}.{obj.GetRecordType().HeaderName}");
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
                        args.Add($"doMasks: doMasks");
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
                            args.Add($"doMasks: doMasks");
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
                        args.Add($"doMasks: doMasks");
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
                            args.Add($"doMasks: doMasks");
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
                    args.Add($"{obj.Getter_InterfaceStr} item");
                    args.Add("BinaryWriter writer");
                    args.Add("bool doMasks");
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
                                args.Add("doMasks: doMasks");
                                args.Add("errorMask: errorMask");
                            }
                        }
                    }
                    foreach (var field in obj.Fields)
                    {
                        if (field.Derivative) continue;
                        if (field.TryGetFieldData(out var data)
                            && data.TriggeringRecordAccessor != null) continue;
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }
                        using (new BraceWrapper(fg))
                        {
                            var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                            fg.AppendLine($"{maskType} subMask;");
                            generator.GenerateWrite(
                                fg: fg,
                                objGen: obj,
                                typeGen: field,
                                writerAccessor: "writer",
                                itemAccessor: $"item.{field.Name}",
                                doMaskAccessor: "doMasks",
                                maskAccessor: $"subMask");
                            using (var args = new ArgsWrapper(fg,
                                $"ErrorMask.HandleErrorMask"))
                            {
                                args.Add("errorMask");
                                args.Add("doMasks");
                                args.Add($"(int){field.IndexEnumName}");
                                args.Add("subMask");
                            }
                        }
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
                    args.Add($"{obj.Getter_InterfaceStr} item");
                    args.Add("BinaryWriter writer");
                    args.Add("bool doMasks");
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
                                args.Add("doMasks: doMasks");
                                args.Add($"errorMask: errorMask");
                            }
                        }
                    }
                    foreach (var field in obj.IterateFields())
                    {
                        if (field.Field.Derivative) continue;
                        if (!field.Field.TryGetFieldData(out var data)
                            || data.TriggeringRecordAccessor == null) continue;
                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }

                        if (!generator.ShouldGenerateWrite(field.Field)) continue;

                        using (new BraceWrapper(fg))
                        {
                            var maskType = this.Gen.MaskModule.GetMaskModule(field.Field.GetType()).GetErrorMaskTypeStr(field.Field);
                            fg.AppendLine($"{maskType} subMask;");
                            generator.GenerateWrite(
                                fg: fg,
                                objGen: obj,
                                typeGen: field.Field,
                                writerAccessor: "writer",
                                itemAccessor: $"item.{field.Field.Name}",
                                doMaskAccessor: "doMasks",
                                maskAccessor: $"subMask");
                            using (var args = new ArgsWrapper(fg,
                                $"ErrorMask.HandleErrorMask"))
                            {
                                args.Add("errorMask");
                                args.Add("doMasks");
                                args.Add($"(int){field.Field.IndexEnumName}");
                                args.Add("subMask");
                            }
                        }
                    }
                }
                fg.AppendLine();
            }
        }
    }
}
