using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;

namespace Mutagen.Generation
{
    public abstract class BinaryTranslationModule : TranslationModule<BinaryTranslationGeneration>
    {
        public override string Namespace => "Mutagen.Binary.";

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

        private bool HasRecordTypeFields(ObjectGeneration obj)
        {
            foreach (var field in obj.Fields)
            {
                if (field.TryGetFieldData(out var data)
                    && data.RecordType.HasValue) return true;
            }
            return false;
        }

        private void GenerateCreateExtras(ObjectGeneration obj, FileGeneration fg)
        {
            ObjectType objType = obj.GetObjectType();
            using (var args = new FunctionWrapper(fg,
                $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal"))
            {
                args.Add("BinaryReader reader");
                args.Add("bool doMasks");
                args.Add($"Func<{obj.ErrorMask}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                switch (objType)
                {
                    case ObjectType.Subrecord:
                    case ObjectType.Record:
                        RecordType? mutaData = obj.GetTriggeringRecordType();
                        using (var args = new ArgsWrapper(fg,
                            $"var finalPosition = HeaderTranslation.Parse{(objType == ObjectType.Subrecord ? "Subrecord" : "Record")}"))
                        {
                            args.Add("reader");
                            args.Add(mutaData.Value.HeaderName);
                        }
                        break;
                    case ObjectType.Mod:
                        fg.AppendLine($"var finalPosition = reader.BaseStream.Length;");
                        break;
                    case ObjectType.Group:
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

            using (var args = new FunctionWrapper(fg,
                $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal"))
            {
                args.Add("BinaryReader reader");
                args.Add("bool doMasks");
                args.Add("long finalPosition");
                args.Add($"Func<{obj.ErrorMask}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var ret = new {obj.Name}{obj.GenericTypes}();");
                fg.AppendLine("try");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.Fields)
                    {
                        if (field.TryGetFieldData(out var data)
                            && data.RecordType.HasValue) continue;
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }
                        using (new BraceWrapper(fg))
                        {
                            var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                            fg.AppendLine($"{maskType} subMask;");
                            generator.GenerateCopyIn(
                                fg: fg,
                                typeGen: field,
                                readerAccessor: "reader",
                                itemAccessor: new Accessor()
                                {
                                    DirectAccess = $"ret.{field.ProtectedName}",
                                    PropertyAccess = field.Notifying == NotifyingOption.None ? null : $"ret.{field.ProtectedProperty}"
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
                    if (HasRecordTypeFields(obj))
                    {
                        fg.AppendLine($"while (reader.BaseStream.Position < finalPosition)");
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"Fill_{ModuleNickname}_Internal"))
                            {
                                args.Add("item: ret");
                                args.Add("reader: reader");
                                args.Add("doMasks: doMasks");
                                args.Add("errorMask: errorMask");
                            }
                        }
                    }
                    fg.AppendLine($"if (reader.BaseStream.Position != finalPosition)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("reader.BaseStream.Position = finalPosition;");
                        fg.AppendLine("throw new ArgumentException(\"Read more bytes than allocated\");");
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

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                $"protected static void Fill_{ModuleNickname}_Internal"))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add("BinaryReader reader");
                    args.Add("bool doMasks");
                    args.Add($"Func<{obj.ErrorMask}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"var nextRecordType = HeaderTranslation.ReadNextSubRecordType"))
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
                                || !data.RecordType.HasValue) continue;
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }

                            if (generator.ShouldGenerateCopyIn(field.Field))
                            {
                                fg.AppendLine($"case \"{data.RecordType.Value.Type}\":");
                                using (new BraceWrapper(fg))
                                {
                                    var maskType = this.Gen.MaskModule.GetMaskModule(field.Field.GetType()).GetErrorMaskTypeStr(field.Field);
                                    fg.AppendLine($"{maskType} subMask;");
                                    generator.GenerateCopyIn(
                                        fg: fg,
                                        typeGen: field.Field,
                                        readerAccessor: "reader",
                                        itemAccessor: new Accessor()
                                        {
                                            DirectAccess = $"item.{field.Field.ProtectedName}",
                                            PropertyAccess = field.Field.Notifying == NotifyingOption.None ? null : $"item.{field.Field.ProtectedProperty}"
                                        },
                                        doMaskAccessor: "doMasks",
                                        maskAccessor: $"subMask");
                                    fg.AppendLine("if (doMasks && subMask != null)");
                                    using (new BraceWrapper(fg))
                                    {
                                        fg.AppendLine($"errorMask().{field.Field.Name} = subMask;");
                                    }
                                    fg.AppendLine("break;");
                                }
                            }
                        }
                        fg.AppendLine($"default:");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"throw new ArgumentException($\"Unexpected header {{nextRecordType.Type}} at position {{reader.BaseStream.Position}}\");");
                        }
                    }
                }
                fg.AppendLine();
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
                $"LoquiBinaryTranslation<{obj.ObjectName}, {obj.ErrorMask}>.Instance.CopyIn"))
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
                    args.Add($"mask: out {obj.ErrorMask} errorMask");
                }
                args.Add($"cmds: cmds");
            }
        }

        protected override void GenerateCreateSnippet(ObjectGeneration obj, FileGeneration fg)
        {
            fg.AppendLine($"{obj.ErrorMask} errMaskRet = null;");
            using (var args = new ArgsWrapper(fg,
                $"var ret = Create_{ModuleNickname}_Internal"))
            {
                args.Add("reader: reader");
                args.Add("doMasks: doMasks");
                args.Add($"errorMask: doMasks ? () => errMaskRet ?? (errMaskRet = new {obj.ErrorMask}()) : default(Func<{obj.ErrorMask}>)");
            }
            fg.AppendLine($"errorMask = errMaskRet;");
            fg.AppendLine($"return ret;");
        }

        protected override void GenerateWriteSnippet(ObjectGeneration obj, FileGeneration fg)
        {
            fg.AppendLine($"throw new NotImplementedException();");
        }
    }
}
