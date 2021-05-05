using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Pex.Binary.Translations;

namespace Mutagen.Bethesda.Generation.Modules.Pex
{
    public class PexModule : BinaryTranslationModule
    {
        public override string ReaderClass => nameof(PexReader);
        public override string ReaderMemberName => "reader";
        public override string WriterClass => nameof(PexWriter);
        public override string WriterMemberName => "writer";
        public override string ModuleNickname => "Pex";
        public override string Namespace => "Mutagen.Bethesda.Pex.Translations.";
        public override string TranslationTerm => "Binary";

        public PexModule(LoquiGenerator gen)
            : base(gen)
        {
            this._typeGenerations[typeof(DateTimeType)] = new DateTimePexTranslationGeneration();
            this._typeGenerations[typeof(LoquiType)] = new LoquiBinaryTranslationGeneration(TranslationTerm);
            this._typeGenerations[typeof(ListType)] = new PexListBinaryTranslationGeneration();
            this._typeGenerations[typeof(StringType)] = new PexStringBinaryTranslationGeneration();
            this._typeGenerations[typeof(EnumType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(Int8Type)] = new SByteBinaryTranslationGeneration();
            this._typeGenerations[typeof(Int16Type)] = new PrimitiveBinaryTranslationGeneration<short>(expectedLen: 2);
            this._typeGenerations[typeof(Int32Type)] = new PrimitiveBinaryTranslationGeneration<int>(expectedLen: 4);
            this._typeGenerations[typeof(Int64Type)] = new PrimitiveBinaryTranslationGeneration<long>(expectedLen: 8);
            this._typeGenerations[typeof(UInt8Type)] = new ByteBinaryTranslationGeneration();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>(expectedLen: 2);
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>(expectedLen: 4);
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>(expectedLen: 8);
            this._typeGenerations[typeof(BoolType)] = new BooleanBinaryTranslationGeneration();
            this._typeGenerations[typeof(FloatType)] = new FloatBinaryTranslationGeneration();
            this._typeGenerations[typeof(CustomLogic)] = new CustomLogicTranslationGeneration();
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine(WriterClass, $"{WriterClass} {WriterMemberName}") },
                    optionalAPI: Array.Empty<APILine>(),
                    customAPI: Array.Empty<CustomMethodAPI>()),
                readerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine(ReaderClass, $"{ReaderClass} {ReaderMemberName}") },
                    optionalAPI: Array.Empty<APILine>(),
                    customAPI: Array.Empty<CustomMethodAPI>()));
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Path", $"string path") },
                        customAPI: Array.Empty<CustomMethodAPI>(),
                        optionalAPI: Array.Empty<APILine>()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromPathOut,
                        ConvertFromPathIn),
                    When = (o, d) => o.Name.Contains("PexFile")
                });
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Stream", $"Stream stream") },
                        customAPI: Array.Empty<CustomMethodAPI>(),
                        optionalAPI: Array.Empty<APILine>()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromStreamOut,
                        ConvertFromStreamIn),
                    When = (o, d) => o.Name.Contains("PexFile")
                });
            this.DoErrorMasks = false;
            this.TranslationMaskParameter = false;
        }

        public override bool GenerateAbstractCreates => false;

        protected override bool GenerateMainCreate(ObjectGeneration obj)
        {
            return !obj.Name.Contains("PexFile");
        }

        protected override bool GenerateMainWrite(ObjectGeneration obj)
        {
            return !obj.Name.Contains("PexFile");
        }

        public override void ReplaceTypeAssociation<Target, Replacement>()
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
        }

        public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
        {
            await foreach (var item in base.RequiredUsingStatements(obj))
            {
                yield return item;
            }
            yield return "System.IO";
            yield return "Mutagen.Bethesda.Pex";
            yield return "Mutagen.Bethesda.Pex.Binary.Translations";
        }

        private GameCategory GetGameCategory(ProtocolGeneration proto)
        {
            return Enum.Parse<GameCategory>(proto.DefaultNamespace.Split('.')[2]);
        }

        private void ConvertFromPathOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);");
            fg.AppendLine($"using var {WriterMemberName} = new {nameof(PexWriter)}(new {nameof(BinaryWriteStream)}(stream, isLittleEndian: {(GetGameCategory(obj.ProtoGen).IsBigEndian() ? "false" : "true")}));");
            internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
        }

        private void ConvertFromPathIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);");
            fg.AppendLine($"var {ReaderMemberName} = new {nameof(PexReader)}(new {nameof(BinaryReadStream)}(stream, isLittleEndian: {(GetGameCategory(obj.ProtoGen).IsBigEndian() ? "false" : "true")}), new());");
            internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
        }

        private void ConvertFromStreamOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"using var {WriterMemberName} = new {nameof(PexWriter)}(new {nameof(BinaryWriteStream)}(stream, isLittleEndian: {(GetGameCategory(obj.ProtoGen).IsBigEndian() ? "false" : "true")}));");
            internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
        }

        private void ConvertFromStreamIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine($"var {ReaderMemberName} = new {nameof(PexReader)}(new {nameof(BinaryReadStream)}(stream, isLittleEndian: {(GetGameCategory(obj.ProtoGen).IsBigEndian() ? "false" : "true")}), new());");
            internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
        }

        protected override async Task GenerateCopyInSnippet(ObjectGeneration obj, FileGeneration fg, Accessor accessor)
        {
            if (obj.Name == "PexFile") return;
            foreach (var field in obj.IterateFields(nonIntegrated: true))
            {
                if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                {
                    if (!field.IntegrateField) continue;
                    throw new ArgumentException("Unsupported type generator: " + field);
                }
                var fieldData = field.GetFieldData();
                switch (fieldData.Binary)
                {
                    case BinaryGenerationType.Normal:
                        break;
                    case BinaryGenerationType.NoGeneration:
                        continue;
                    case BinaryGenerationType.Custom:
                        CustomLogic.GenerateFill(
                            fg: fg,
                            field: field,
                            frameAccessor: ReaderMemberName,
                            isAsync: false);
                        continue;
                    default:
                        throw new NotImplementedException();
                }
                GenerateInstructionHeaderRead(obj, field, fg);
                await generator.GenerateCopyIn(
                    fg: fg,
                    objGen: obj,
                    typeGen: field,
                    readerAccessor: this.ReaderMemberName,
                    itemAccessor: Accessor.FromType(field, "item"),
                    translationAccessor: null,
                    errorMaskAccessor: null);
            }
        }

        protected override async Task GenerateNewSnippet(ObjectGeneration obj, FileGeneration fg)
        {
            fg.AppendLine($"var ret = new {obj.Name}();");
        }

        protected override async Task GenerateWriteSnippet(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.Name == "PexFile") return;

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

            foreach (var field in obj.IterateFields(nonIntegrated: true))
            {
                if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                {
                    if (!field.IntegrateField) continue;
                    throw new ArgumentException("Unsupported type generator: " + field);
                }
                var fieldData = field.GetFieldData();
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
                GenerateInstructionHeaderWrite(obj, field, fg);
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

        protected bool IsInstructionObject(ObjectGeneration obj)
        {
            return obj.Name.Contains("Instruction");
        }

        protected bool IsVariableObject(ObjectGeneration obj)
        {
            return IsInstructionObject(obj) || (obj.BaseClass?.Name.Equals("AVariable") ?? false);
        }

        protected void GenerateInstructionHeaderRead(ObjectGeneration obj, TypeGeneration type, FileGeneration fg)
        {
            if (!IsVariableObject(obj)) return;
            VariableType varType;
            switch (type)
            {
                case StringType str:
                    fg.AppendLine($"{ReaderMemberName}.EnsureVariableType({nameof(VariableType)}.{VariableType.String}, {nameof(VariableType)}.{VariableType.Identifier});");
                    return;
                case Int32Type _:
                    varType = VariableType.Integer;
                    break;
                case FloatType _:
                    varType = VariableType.Float;
                    break;
                case BoolType _:
                    varType = VariableType.Bool;
                    break;
                default:
                    return;
            }
            fg.AppendLine($"{ReaderMemberName}.EnsureVariableType({nameof(VariableType)}.{varType});");
        }

        protected void GenerateInstructionHeaderWrite(ObjectGeneration obj, TypeGeneration type, FileGeneration fg)
        {
            if (!IsInstructionObject(obj)) return;
            VariableType varType;
            switch (type)
            {
                case StringType str:
                    varType = str.Name.StartsWith("Identifier") ? VariableType.Identifier : VariableType.String;
                    break;
                case Int32Type _:
                    varType = VariableType.Integer;
                    break;
                case FloatType _:
                    varType = VariableType.Float;
                    break;
                case BoolType _:
                    varType = VariableType.Bool;
                    break;
                default:
                    return;
            }
            fg.AppendLine($"{this.WriterMemberName}.Write((byte){nameof(VariableType)}.{varType});");
        }
    }
}
