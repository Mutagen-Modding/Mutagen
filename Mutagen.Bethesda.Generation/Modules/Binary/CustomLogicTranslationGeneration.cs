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
        public override bool DoErrorMasks => true;
        public const bool DoErrorMasksStatic = true;

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

        public override void GenerateCopyIn(
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
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
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
                    args.Add($"{obj.ObjectName} item");
                    args.Add($"MasterReferences masterReferences");
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
                    args.Add($"{obj.ObjectName} item");
                    args.Add($"MasterReferences masterReferences");
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
                        args.Add($"frame: frame");
                        args.Add($"item: item");
                        args.Add($"masterReferences: masterReferences");
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
                $"static partial void WriteBinary{field.Name}Custom{obj.GetGenericTypes(MaskType.Normal)}",
                    obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics).ToArray())
            {
                SemiColon = true
            })
            {
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.Interface(getter: true, internalInterface: obj.HasInternalInterface)} item");
                args.Add($"MasterReferences masterReferences");
                if (DoErrorMasksStatic)
                {
                    args.Add($"ErrorMaskBuilder errorMask");
                }
            }
            fg.AppendLine();
            using (var args = new FunctionWrapper(fg,
                $"public static void WriteBinary{field.Name}{obj.GetGenericTypes(MaskType.Normal)}",
                obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics).ToArray()))
            {
                args.Add($"{nameof(MutagenWriter)} writer");
                args.Add($"{obj.Interface(getter: true, internalInterface: obj.HasInternalInterface)} item");
                args.Add($"MasterReferences masterReferences");
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
                    args.Add($"masterReferences: masterReferences");
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
                args.Add($"masterReferences: masterReferences");
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
                args.Add($"frame: {(data.HasTrigger ? $"{frameAccessor}.SpawnWithLength(Mutagen.Bethesda.Constants.SUBRECORD_LENGTH + contentLength)" : frameAccessor)}");
                args.Add("item: item");
                args.Add($"masterReferences: masterReferences");
                if (DoErrorMasksStatic)
                {
                    args.Add("errorMask: errorMask");
                }
            }
        }

        public void GenerateFillForWrapper(
            FileGeneration fg,
            TypeGeneration field,
            Accessor dataAccessor,
            bool doMasters)
        {
            var data = field.GetFieldData();
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{field.Name}Location;");
                fg.AppendLine($"public bool {field.Name}_IsSet => _{field.Name}Location.HasValue;");
            }
            using (var args = new ArgsWrapper(fg,
                $"public {field.TypeName(getter: true)} {field.Name} => Get{field.Name}Custom"))
            {
                args.Add($"span: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{field.Name}Location.Value, _meta)");
                if (doMasters)
                {
                    args.Add($"masterReferences: _masterReferences");
                }
            }
        }

        public override void GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor, 
            int passedLength, 
            DataType data = null)
        {
            var fieldData = typeGen.GetFieldData();
            if (fieldData.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
            }
        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            CustomLogic custom = typeGen as CustomLogic;
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            if (!data.RecordType.HasValue)
            {
                return custom.ExpectedLength.Value;
            }
            return 0;
        }
    }
}
