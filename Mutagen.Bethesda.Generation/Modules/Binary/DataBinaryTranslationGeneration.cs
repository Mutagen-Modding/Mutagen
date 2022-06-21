using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class DataBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
    public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
    }

    public override void GenerateCopyInRet(
        StructuredStringBuilder sb,
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

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
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
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor structDataAccessor,  
        Accessor recordDataAccessor, 
        int? passedLength,
        string passedLengthAccesor,
        DataType _)
    {
        DataType dataType = typeGen as DataType;

        sb.AppendLine($"private {nameof(RangeInt32)}? _{dataType.GetFieldData().RecordType}Location;");
        sb.AppendLine($"public {objGen.ObjectName}.{dataType.EnumName} {dataType.StateName} {{ get; private set; }}");
        switch (typeGen.GetFieldData().BinaryOverlayFallback)
        {
            case BinaryGenerationType.Custom:
                await this.Module.CustomLogic.GenerateWrapperFields(
                    sb,
                    objGen,
                    typeGen,
                    structDataAccessor,
                    recordDataAccessor,
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
            using (sb.Region(field.Field.Name, appendExtraLine: false, skipIfOnlyOneLine: true))
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
                    passIn = $"_{dataType.GetFieldData().RecordType}Location!.Value.{nameof(RangeInt32.Min)}";
                } 
                else if (passIn == null 
                         || length.PassedType == BinaryTranslationModule.PassedType.Direct)
                {
                    passIn = $"_{dataType.GetFieldData().RecordType}Location!.Value.{nameof(RangeInt32.Min)} + {passIn}";
                }

                await subTypeGen.GenerateWrapperFields(
                    sb,
                    objGen,
                    field.Field,
                    structDataAccessor,
                    recordDataAccessor,
                    length.PassedLength,
                    passIn,
                    data: dataType);
                if (fieldData.HasVersioning)
                {
                    VersioningModule.AddVersionOffset(sb, field.Field, length.FieldLength.Value, lastVersionedField, $"_package.FormVersion!.FormVersion!.Value");
                    lastVersionedField = field.Field;
                }
                if (length.CurLength == null)
                {
                    if (fieldData.BinaryOverlayFallback == BinaryGenerationType.Custom)
                    {
                        sb.AppendLine($"partial void Custom{length.Field.Name}EndPos();");
                    }
                    else
                    {
                        sb.AppendLine($"protected int {length.Field.Name}EndingPos;");
                    }
                }
            }
        }
    }

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb,
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
                    sb,
                    objGen,
                    field,
                    locationAccessor,
                    packageAccessor,
                    converterAccessor);
                return;
            default:
                break;
        }
        sb.AppendLine($"_{dataType.GetFieldData().RecordType}Location = new({locationAccessor} + _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength, finalPos - offset - 1);");
        if (dataType.Nullable)
        {
            sb.AppendLine($"this.{dataType.StateName} = {objGen.ObjectName}.{dataType.EnumName}.Has;");
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
                    sb.AppendLine($"var subLen = _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubrecordHeader(_recordData.Slice({locationAccessor})).ContentLength;");
                }
                sb.AppendLine($"if (subLen <= {length.PassedAccessor})");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"this.{dataType.StateName} |= {objGen.ObjectName}.{dataType.EnumName}.Break{item.BreakIndex};");
                }
            }
            if (item.RangeIndex != -1)
            {
                if (!generatedStart)
                {
                    generatedStart = true;
                    sb.AppendLine($"var subLen = _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.Subrecord(_data.Slice({locationAccessor})).ContentLength;");
                }
            }
        }
        for (int i = 0; i < dataType.RangeIndices.Count; i++)
        {
            var range = dataType.RangeIndices[i];
            sb.AppendLine($"if (subLen > {range.DataSetSizeMin})");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"this.{dataType.StateName} |= {objGen.ObjectName}.{dataType.EnumName}.Range{i};");
            }
        }
    }

    public override async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => null;

    public static void GenerateWrapperExtraMembers(StructuredStringBuilder sb, DataType dataType, ObjectGeneration objGen, TypeGeneration typeGen, string posAccessor)
    {
        var fieldData = typeGen.GetFieldData();
        var dataMeta = dataType.IterateFieldsWithMeta().First(item => item.Field == typeGen);
        List<string> extraChecks = new List<string>();
        if (dataMeta.EncounteredBreaks.Any())
        {
            var breakIndex = dataMeta.EncounteredBreaks.Last();
            extraChecks.Add($"!{dataType.StateName}.HasFlag({objGen.Name}.{dataType.EnumName}.Break{breakIndex})");
        }
        if (dataMeta.RangeIndex != -1)
        {
            extraChecks.Add($"{dataType.StateName}.HasFlag({objGen.Name}.{dataType.EnumName}.Range{dataMeta.RangeIndex})");
        }
        if (fieldData.HasVersioning)
        {
            extraChecks.Add(VersioningModule.GetVersionIfCheck(fieldData, "_package.FormVersion!.FormVersion!.Value"));
        }
        sb.AppendLine($"private int _{typeGen.Name}Location => {posAccessor};");
        switch (typeGen.GetFieldData().BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                sb.AppendLine($"private bool _{typeGen.Name}_IsSet => _{dataType.GetFieldData().RecordType}Location.HasValue{(extraChecks.Count > 0 ? $" && {string.Join(" && ", extraChecks)}" : null)};");
                break;
            case BinaryGenerationType.Custom:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public override async Task GenerateWrapperUnknownLengthParse(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor, 
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
                    sb.AppendLine($"ret.Custom{field.Field.Name}EndPos();");
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
                        sb,
                        objGen,
                        field.Field,
                        dataAccessor,
                        length.PassedLength,
                        $"ret._{dataType.GetFieldData().RecordType}Location!.Value.{nameof(RangeInt32.Min)} + {length.PassedAccessor}");
                    break;
            }
        }
    }
}