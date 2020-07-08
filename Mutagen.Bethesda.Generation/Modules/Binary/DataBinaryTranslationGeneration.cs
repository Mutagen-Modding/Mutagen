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
    public class DataBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
        public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

        public override async Task GenerateCopyIn(
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
            Accessor translationAccessor,
            Accessor converterAccessor,
            bool inline)
        {
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
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? passedLength,
            string passedLengthAccesor,
            DataType _)
        {
            DataType dataType = typeGen as DataType;

            fg.AppendLine($"private int? _{dataType.GetFieldData().RecordType}Location;");
            fg.AppendLine($"public {objGen.ObjectName}.{dataType.EnumName} {dataType.StateName} {{ get; private set; }}");
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Custom:
                    await this.Module.CustomLogic.GenerateWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        passedLength,
                        passedLengthAccesor);
                    break;
                default:
                    break;
            }

            var lengths = await this.Module.IteratePassedLengths(
                    objGen,
                    dataType.SubFields,
                    forOverlay: true)
                .ToListAsync();
            TypeGeneration lastVersionedField = null;
            foreach (var field in dataType.IterateFieldsWithMeta())
            {
                if (!field.Field.Enabled) continue;
                if (!this.Module.TryGetTypeGeneration(field.Field.GetType(), out var subTypeGen)) continue;
                using (new RegionWrapper(fg, field.Field.Name)
                {
                    AppendExtraLine = false,
                    SkipIfOnlyOneLine = true
                })
                {
                    var fieldData = field.Field.GetFieldData();
                    var length = lengths.FirstOrDefault(l => l.Field == field.Field);
                    if (length.Field == null)
                    {
                        throw new ArgumentException();
                    }

                    var passIn = length.PassedAccessor;
                    if (passIn == null)
                    {
                        passIn = $"_{dataType.GetFieldData().RecordType}Location!.Value";
                    } 
                    else if (passIn == null 
                        || length.PassedType == BinaryTranslationModule.PassedType.Direct)
                    {
                        passIn = $"_{dataType.GetFieldData().RecordType}Location!.Value + {passIn}";
                    }

                    await subTypeGen.GenerateWrapperFields(
                        fg,
                        objGen,
                        field.Field,
                        dataAccessor,
                        length.PassedLength,
                        passIn,
                        data: dataType);
                    if (fieldData.HasVersioning)
                    {
                        VersioningModule.AddVersionOffset(fg, field.Field, length.FieldLength.Value, lastVersionedField, $"_package.MajorRecord!.FormVersion!.Value");
                        lastVersionedField = field.Field;
                    }
                    if (length.CurLength == null)
                    {
                        fg.AppendLine($"protected int {length.Field.Name}EndingPos;");
                        if (fieldData.BinaryOverlayFallback == BinaryGenerationType.Custom)
                        {
                            fg.AppendLine($"partial void Custom{length.Field.Name}EndPos();");
                        }
                    }
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
            fg.AppendLine($"_{dataType.GetFieldData().RecordType}Location = {locationAccessor} + _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
            if (dataType.HasBeenSet)
            {
                fg.AppendLine($"this.{dataType.StateName} = {objGen.ObjectName}.{dataType.EnumName}.Has;");
            }
            bool generatedStart = false;
            var lengths = await this.Module.IteratePassedLengths(
                    objGen,
                    dataType.SubFields,
                    forOverlay: true)
                .ToListAsync();
            foreach (var item in dataType.IterateFieldsWithMeta())
            {
                if (!this.Module.TryGetTypeGeneration(item.Field.GetType(), out var typeGen)) continue;
                var length = lengths.FirstOrDefault(l => l.Field == item.Field);
                if (length.Field == null)
                {
                    throw new ArgumentException();
                }
                if (item.BreakIndex != -1)
                {
                    if (!generatedStart)
                    {
                        generatedStart = true;
                        fg.AppendLine($"var subLen = _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.Subrecord(_data.Slice({locationAccessor})).ContentLength;");
                    }
                    fg.AppendLine($"if (subLen <= {length.PassedAccessor})");
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
                        fg.AppendLine($"var subLen = _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.Subrecord(_data.Slice({locationAccessor})).ContentLength;");
                    }
                }
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

        public override async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => null;

        public static void GenerateWrapperExtraMembers(FileGeneration fg, DataType dataType, ObjectGeneration objGen, TypeGeneration typeGen, string posAccessor)
        {
            var fieldData = typeGen.GetFieldData();
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
            if (fieldData.HasVersioning)
            {
                extraChecks.Append(VersioningModule.GetVersionIfCheck(fieldData, "_package.MajorRecord!.FormVersion!.Value"));
            }
            fg.AppendLine($"private int _{typeGen.Name}Location => {posAccessor};");
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    fg.AppendLine($"private bool _{typeGen.Name}_IsSet => _{dataType.GetFieldData().RecordType}Location.HasValue{(extraChecks.Length > 0 ? $" && {extraChecks}" : null)};");
                    break;
                case BinaryGenerationType.Custom:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override async Task GenerateWrapperUnknownLengthParse(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            int? passedLength,
            string passedLengthAccessor)
        {
            var dataType = typeGen as DataType;
            var lengths = await this.Module.IteratePassedLengths(
                    objGen,
                    dataType.SubFields,
                    forOverlay: true)
                .ToListAsync();
            foreach (var field in dataType.IterateFieldsWithMeta())
            {
                if (!this.Module.TryGetTypeGeneration(field.Field.GetType(), out var subTypeGen)) continue;
                var data = field.Field.GetFieldData();
                switch (data.BinaryOverlayFallback)
                {
                    case BinaryGenerationType.Normal:
                    case BinaryGenerationType.Custom:
                        break;
                    default:
                        continue;
                }
                if (data.HasTrigger) continue;
                var amount = await subTypeGen.GetPassedAmount(objGen, field.Field);
                if (amount != null) continue;
                if (field.Field is CustomLogic) continue;
                switch (data.BinaryOverlayFallback)
                {
                    case BinaryGenerationType.Custom:
                        fg.AppendLine($"ret.Custom{field.Field.Name}EndPos();");
                        break;
                    case BinaryGenerationType.NoGeneration:
                        break;
                    case BinaryGenerationType.Normal:
                        var length = lengths.FirstOrDefault(l => l.Field == field.Field);
                        if (length.Field == null)
                        {
                            throw new ArgumentException();
                        }
                        await subTypeGen.GenerateWrapperUnknownLengthParse(
                            fg,
                            objGen,
                            field.Field,
                            length.PassedLength,
                            $"ret._{dataType.GetFieldData().RecordType}Location!.Value + {length.PassedAccessor}");
                        break;
                }
            }
        }
    }
}
