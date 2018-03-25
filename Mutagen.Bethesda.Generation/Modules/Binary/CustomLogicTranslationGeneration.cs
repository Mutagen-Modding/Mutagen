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
        public override bool ShouldGenerateWrite(TypeGeneration typeGen)
        {
            return true;
        }

        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            return true;
        }

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string readerAccessor, Accessor itemAccessor, string doMaskAccessor, string maskAccessor)
        {
            CustomLogicTranslationGeneration.GenerateFill(
                fg: fg,
                field: typeGen,
                frameAccessor: readerAccessor);
        }

        public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen, string readerAccessor, Accessor retAccessor, string doMaskAccessor, string maskAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string doMaskAccessor, string maskAccessor)
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
                $"static partial void FillBinary_{field.Name}_Custom{obj.Mask_GenericClause(MaskType.Error)}",
                wheres: obj.GenericTypes_ErrorMaskWheres)
            {
                SemiColon = true
            })
            {
                args.Add($"{nameof(MutagenFrame)} frame");
                args.Add($"{obj.ObjectName} item");
                if (field.HasIndex)
                {
                    args.Add($"int fieldIndex");
                }
                args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
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
                args.Add($"{obj.ObjectName} item");
                if (field.HasIndex)
                {
                    args.Add($"int fieldIndex");
                }
                args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
            }
            fg.AppendLine();
            using (var args = new FunctionWrapper(fg,
                $"public static void WriteBinary_{field.Name}{obj.Mask_GenericClause(MaskType.Error)}",
                wheres: obj.GenericTypes_ErrorMaskWheres))
            {
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.ObjectName} item");
                if (field.HasIndex)
                {
                    args.Add($"int fieldIndex");
                }
                args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("try");
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
                        }
                        args.Add($"errorMask: errorMask");
                    }
                }
                fg.AppendLine("catch (Exception ex)");
                fg.AppendLine("when (errorMask != null)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("errorMask().Overall = ex;");
                }
            }
            fg.AppendLine();
        }

        public static void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration obj,
            TypeGeneration field,
            string writerAccessor)
        {
            using (var args = new ArgsWrapper(fg,
                $"{obj.ObjectName}.WriteBinary_{field.Name}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add("item: item");
                if (field.HasIndex)
                {
                    args.Add($"fieldIndex: (int){field.IndexEnumName}");
                }
                args.Add("errorMask: errorMask");
            }
        }

        public static void GenerateFill(
            FileGeneration fg,
            TypeGeneration field,
            string frameAccessor)
        {
            var data = field.GetFieldData();
            fg.AppendLine("try");
            using (new BraceWrapper(fg))
            {
                if (data.HasTrigger)
                {
                    fg.AppendLine($"using (var subFrame = {frameAccessor}.Spawn(Constants.SUBRECORD_LENGTH + contentLength, snapToFinalPosition: false))");
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
                            args.Add($"fieldIndex: (int){field.IndexEnumName}");
                        }
                        args.Add($"errorMask: errorMask");
                    }
                }
            }
            fg.AppendLine("catch (Exception ex)");
            fg.AppendLine("when (errorMask != null)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("errorMask().Overall = ex;");
            }
        }
    }
}
