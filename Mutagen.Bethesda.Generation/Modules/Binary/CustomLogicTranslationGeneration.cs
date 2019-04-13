using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class CustomLogicTranslationGeneration : BinaryTranslationGeneration
    {
        public override string GetTranslatorInstance(TypeGeneration typeGen)
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

        public override void GenerateCopyIn(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            CustomLogicTranslationGeneration.GenerateFill(
                fg: fg,
                field: typeGen,
                frameAccessor: readerAccessor);
        }

        public override void GenerateCopyInRet(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration targetGen, 
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            bool squashedRepeatedList,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
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
            Accessor translationMaskAccessor)
        {
            CustomLogicTranslationGeneration.GenerateWrite(
                fg: fg,
                obj: objGen,
                field: typeGen,
                writerAccessor: writerAccessor);
        }

        public static void GeneratePartialMethods(
            FileGeneration fg,
            ObjectGeneration obj,
            TypeGeneration field)
        {
            using (var args = new FunctionWrapper(fg,
                $"static partial void FillBinary_{field.Name}_Custom")
            {
                SemiColon = true
            })
            {
                args.Add($"{nameof(MutagenFrame)} frame");
                args.Add($"{obj.ObjectName} item");
                args.Add($"MasterReferences masterReferences");
                args.Add($"ErrorMaskBuilder errorMask");
            }
            fg.AppendLine();
            using (var args = new FunctionWrapper(fg,
                $"static partial void WriteBinary_{field.Name}_Custom")
            {
                SemiColon = true
            })
            {
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.ObjectName} item");
                args.Add($"MasterReferences masterReferences");
                args.Add($"ErrorMaskBuilder errorMask");
            }
            fg.AppendLine();
            using (var args = new FunctionWrapper(fg,
                $"public static void WriteBinary_{field.Name}"))
            {
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.ObjectName} item");
                args.Add($"MasterReferences masterReferences");
                args.Add($"ErrorMaskBuilder errorMask");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"WriteBinary_{field.Name}_Custom"))
                {
                    args.Add("writer: writer");
                    args.Add("item: item");
                    args.Add($"masterReferences: masterReferences");
                    args.Add($"errorMask: errorMask");
                }
            }
            fg.AppendLine();
        }

        public static void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration obj,
            TypeGeneration field,
            Accessor writerAccessor)
        {
            using (var args = new ArgsWrapper(fg,
                $"{obj.ObjectName}.WriteBinary_{field.Name}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add("item: item");
                args.Add($"masterReferences: masterReferences");
                args.Add("errorMask: errorMask");
            }
        }

        public static void GenerateFill(
            FileGeneration fg,
            TypeGeneration field,
            Accessor frameAccessor)
        {
            var data = field.GetFieldData();
            if (data.HasTrigger)
            {
                fg.AppendLine($"using (var subFrame = {frameAccessor}.SpawnWithLength(Mutagen.Bethesda.Constants.SUBRECORD_LENGTH + contentLength, snapToFinalPosition: false))");
            }
            using (new BraceWrapper(fg, doIt: data.HasTrigger))
            {
                using (var args = new ArgsWrapper(fg,
                    $"FillBinary_{field.Name}_Custom"))
                {
                    args.Add($"frame: {(data.HasTrigger ? "subFrame" : frameAccessor)}");
                    args.Add("item: item");
                    args.Add($"masterReferences: masterReferences");
                    args.Add($"errorMask: errorMask");
                }
            }
        }
    }
}
