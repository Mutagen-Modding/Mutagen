using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class DataBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
        public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
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
            Accessor translationAccessor)
        {
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
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor, 
            int passedLength,
            DataType _)
        {
            DataType dataType = typeGen as DataType;
            
            fg.AppendLine($"private int? _{dataType.GetFieldData().RecordType}Location;");
            fg.AppendLine($"public {objGen.ObjectName}.{dataType.EnumName} {dataType.StateName} {{ get; private set; }}");
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Custom:
                    this.Module.CustomLogic.GenerateWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        passedLength);
                    break;
                default:
                    break;
            }

            var dataPassedLength = 0;
            foreach (var field in dataType.IterateFieldsWithMeta())
            {
                if (!this.Module.TryGetTypeGeneration(field.Field.GetType(), out var subTypeGen)) continue;
                using (new RegionWrapper(fg, field.Field.Name)
                {
                    AppendExtraLine = false,
                    SkipIfOnlyOneLine = true
                })
                {
                    subTypeGen.GenerateWrapperFields(
                        fg,
                        objGen,
                        field.Field,
                        dataAccessor,
                        dataPassedLength,
                        data: dataType);
                    dataPassedLength += subTypeGen.GetPassedAmount(objGen, field.Field);
                }
            }
        }

        public override async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration field, 
            Accessor locationAccessor,
            Accessor packageAccessor,
            Accessor converterAccessor)
        {
            DataType dataType = field as DataType;
            switch (field.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.DoNothing:
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    await this.Module.CustomLogic.GenerateWrapperRecordTypeParse(
                        fg,
                        objGen,
                        field,
                        locationAccessor,
                        packageAccessor,
                        converterAccessor);
                    return;
                default:
                    break;
            }
            fg.AppendLine($"_{dataType.GetFieldData().RecordType}Location = (ushort){locationAccessor} + _package.Meta.SubConstants.TypeAndLengthLength;");
            fg.AppendLine($"this.{dataType.StateName} = {objGen.ObjectName}.{dataType.EnumName}.Has;");
            bool generatedStart = false;
            var passedLen = 0;
            foreach (var item in dataType.IterateFieldsWithMeta())
            {
                if (!this.Module.TryGetTypeGeneration(item.Field.GetType(), out var typeGen)) continue;
                if (item.BreakIndex != -1)
                {
                    if (!generatedStart)
                    {
                        generatedStart = true;
                        fg.AppendLine($"var subLen = _package.Meta.SubRecord(_data.Slice({locationAccessor})).RecordLength;");
                    }
                    fg.AppendLine($"if (subLen <= 0x{passedLen.ToString("X")})");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"this.{dataType.StateName} |= {objGen.ObjectName}.{dataType.EnumName}.Break{item.BreakIndex};");
                    }
                }
                if (item.RangeIndex != -1)
                {
                    if (!generatedStart)
                    {
                        generatedStart = true;
                        fg.AppendLine($"var subLen = _package.Meta.SubRecord(_data.Slice({locationAccessor})).RecordLength;");
                    }
                }
                passedLen += typeGen.GetPassedAmount(objGen, item.Field);
            }
            for (int i = 0; i < dataType.RangeIndices.Count; i++)
            {
                var range = dataType.RangeIndices[i];
                fg.AppendLine($"if (subLen > {range.DataSetSizeMin})");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"this.{dataType.StateName} |= {objGen.ObjectName}.{dataType.EnumName}.Range{i};");
                }
            }
        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => null;

        public static void GenerateWrapperExtraMembers(FileGeneration fg, DataType dataType, ObjectGeneration objGen, TypeGeneration typeGen, int pos)
        {
            var dataMeta = dataType.IterateFieldsWithMeta().First(item => item.Field == typeGen);
            StringBuilder extraChecks = new StringBuilder();
            if (dataMeta.EncounteredBreaks.Any())
            {
                var breakIndex = dataMeta.EncounteredBreaks.Last();
                extraChecks.Append($"!{dataType.StateName}.HasFlag({objGen.Name}.{dataType.EnumName}.Break{breakIndex})");
            }
            if (dataMeta.RangeIndex != -1)
            {
                extraChecks.Append($"{dataType.StateName}.HasFlag({objGen.Name}.{dataType.EnumName}.Range{dataMeta.RangeIndex})");
            }
            fg.AppendLine($"private int _{typeGen.Name}Location => _{dataType.GetFieldData().RecordType}Location.Value + 0x{pos.ToString("X")};");
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    fg.AppendLine($"private bool _{typeGen.Name}_IsSet => _{dataType.GetFieldData().RecordType}Location.HasValue{(extraChecks.Length > 0 ? $" && {extraChecks}" : null)};");
                    break;
                case BinaryGenerationType.Custom:
                    fg.AppendLine($"private bool _{typeGen.Name}_IsSet => Get{typeGen.Name}IsSetCustom();");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
