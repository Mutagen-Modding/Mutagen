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
            this._typeGenerations[typeof(StringType)] = new PrimitiveBinaryTranslationGeneration<string>(nullable: true) { CanBeNotNullable = false };
            this._typeGenerations[typeof(UInt8NullType)] = new PrimitiveBinaryTranslationGeneration<byte?>();
            this._typeGenerations[typeof(UInt8Type)] = new PrimitiveBinaryTranslationGeneration<byte>();
            this._typeGenerations[typeof(UInt16NullType)] = new PrimitiveBinaryTranslationGeneration<ushort?>();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>();
            this._typeGenerations[typeof(UInt32NullType)] = new PrimitiveBinaryTranslationGeneration<uint?>();
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>();
            this._typeGenerations[typeof(UInt64NullType)] = new PrimitiveBinaryTranslationGeneration<ulong?>();
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new PrimitiveBinaryTranslationGeneration<byte[]>(typeName: "ByteArray", nullable: true);
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

        public override void Load()
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

        private void GenerateCreateExtras(ObjectGeneration obj, FileGeneration fg)
        {
            using (var args = new FunctionWrapper(fg,
                $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal"))
            {
                args.Add("BinaryReader reader");
                args.Add("bool doMasks");
                args.Add($"Func<{obj.ErrorMask}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var ret = new {obj.Name}{obj.GenericTypes}();");
                fg.AppendLine($"throw new NotImplementedException();");
                fg.AppendLine("try");
                using (new BraceWrapper(fg))
                {
                }
                fg.AppendLine("catch (Exception ex)");
                fg.AppendLine("when (doMasks)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("errorMask().Overall = ex;");
                }
                fg.AppendLine("return ret;");
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
