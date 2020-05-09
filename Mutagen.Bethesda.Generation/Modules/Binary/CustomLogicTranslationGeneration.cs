using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class CustomLogicTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool DoErrorMasks => true;
        public const bool DoErrorMasksStatic = false;

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldGenerateWrite(TypeGeneration typeGen)
        {
            return true;
        }

        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            return true;
        }

        public override async Task GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            GenerateFill(
                fg: fg,
                field: typeGen,
                frameAccessor: readerAccessor,
                isAsync: false);
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor)
        {
            this.GenerateWrite(
                fg: fg,
                obj: objGen,
                field: typeGen,
                writerAccessor: writerAccessor);
        }

        public static void GenerateCreatePartialMethods(
            FileGeneration fg,
            ObjectGeneration obj,
            TypeGeneration field,
            bool isAsync)
        {
            if (!isAsync)
            {
                using (var args = new FunctionWrapper(fg,
                    $"static partial void FillBinary{field.Name}Custom")
                {
                    SemiColon = true
                })
                {
                    args.Add($"{nameof(MutagenFrame)} frame");
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    if (DoErrorMasksStatic)
                    {
                        args.Add($"ErrorMaskBuilder errorMask");
                    }
                }
                fg.AppendLine();

                using (var args = new FunctionWrapper(fg,
                    $"public static void FillBinary{field.Name}CustomPublic"))
                {
                    args.Add($"{nameof(MutagenFrame)} frame");
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    if (DoErrorMasksStatic)
                    {
                        args.Add($"ErrorMaskBuilder errorMask");
                    }
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"FillBinary{field.Name}Custom"))
                    {
                        args.AddPassArg($"frame");
                        args.AddPassArg($"item");
                        if (DoErrorMasksStatic)
                        {
                            args.Add($"errorMask: errorMask");
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        public static void GenerateWritePartialMethods(
            FileGeneration fg,
            ObjectGeneration obj,
            TypeGeneration field,
            bool isAsync)
        {
            using (var args = new FunctionWrapper(fg,
                $"static partial void WriteBinary{field.Name}Custom{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                args.SemiColon = true;
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
                if (DoErrorMasksStatic)
                {
                    args.Add($"ErrorMaskBuilder errorMask");
                }
            }
            fg.AppendLine();
            using (var args = new FunctionWrapper(fg,
                $"public static void WriteBinary{field.Name}{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
                if (DoErrorMasksStatic)
                {
                    args.Add($"ErrorMaskBuilder errorMask");
                }
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"WriteBinary{field.Name}Custom"))
                {
                    args.Add("writer: writer");
                    args.Add("item: item");
                    if (DoErrorMasksStatic)
                    {
                        args.Add($"errorMask: errorMask");
                    }
                }
            }
            fg.AppendLine();
        }

        public void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration obj,
            TypeGeneration field,
            Accessor writerAccessor)
        {
            using (var args = new ArgsWrapper(fg,
                $"{this.Module.TranslationWriteClass(obj)}.WriteBinary{field.Name}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add("item: item");
                if (DoErrorMasksStatic)
                {
                    args.Add("errorMask: errorMask");
                }
            }
        }

        public void GenerateFill(
            FileGeneration fg,
            TypeGeneration field,
            Accessor frameAccessor,
            bool isAsync)
        {
            var data = field.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{Loqui.Generation.Utility.Await(isAsync)}{this.Module.TranslationCreateClass(field.ObjectGen)}.FillBinary{field.Name}CustomPublic"))
            {
                args.Add($"frame: {(data.HasTrigger ? $"{frameAccessor}.SpawnWithLength(frame.{nameof(MutagenFrame.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)} + contentLength)" : frameAccessor)}");
                args.Add("item: item");
                if (DoErrorMasksStatic)
                {
                    args.Add("errorMask: errorMask");
                }
            }
        }

        public async Task GenerateForCustomFlagWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition,
            DataType dataType = null)
        {
            var fieldData = typeGen.GetFieldData();
            var gen = this.Module.GetTypeGeneration(typeGen.GetType());
            string loc;
            if (fieldData.HasTrigger)
            {
                using (var args = new ArgsWrapper(fg,
                    $"partial void {typeGen.Name}CustomParse"))
                {
                    args.Add($"{nameof(BinaryMemoryReadStream)} stream");
                    args.Add($"long finalPos");
                    args.Add($"int offset");
                }
                if (typeGen.HasBeenSet && !typeGen.CanBeNullable(getter: true))
                {
                    fg.AppendLine($"public bool {typeGen.Name}_IsSet => Get{typeGen.Name}IsSetCustom();");
                }
                loc = $"_{typeGen.Name}Location.Value";
            }
            else if (dataType != null)
            {
                loc = $"_{typeGen.Name}Location";
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
            }
            else
            {
                loc = $"0x{currentPosition:X}";
            }
            using (var args = new ArgsWrapper(fg,
                $"public {typeGen.TypeName(getter: true)}{(typeGen.HasBeenSet && typeGen.CanBeNullable(getter: true) ? "?" : null)} {typeGen.Name} => Get{typeGen.Name}Custom"))
            {
                if (!fieldData.HasTrigger && dataType == null)
                {
                    args.Add($"location: {loc}");
                }
            }
            if (!fieldData.HasTrigger)
            {
                currentPosition += fieldData.Length ?? await gen.ExpectedLength(objGen, typeGen);
            }
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor, 
            int? passedLength,
            string passedLengthAccessor,
            DataType data = null)
        {
            using (var args = new ArgsWrapper(fg,
                $"partial void {(typeGen.Name == null ? typeGen.GetFieldData().RecordType?.ToString() : typeGen.Name)}CustomParse"))
            {
                args.Add($"{nameof(BinaryMemoryReadStream)} stream");
                args.Add($"int offset");
            }
        }

        public override async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor locationAccessor, 
            Accessor packageAccessor, 
            Accessor converterAccessor)
        {
            using (var args = new ArgsWrapper(fg,
                $"{(typeGen.Name == null ? typeGen.GetFieldData().RecordType?.ToString() : typeGen.Name)}CustomParse"))
            {
                args.Add("stream");
                args.Add("offset");
            }
        }

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            CustomLogic custom = typeGen as CustomLogic;
            var data = typeGen.GetFieldData();
            return data.Length;
        }
    }
}
