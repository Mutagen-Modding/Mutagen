using Loqui.Generation;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using FloatType = Mutagen.Bethesda.Generation.Fields.FloatType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class FloatBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<float>
{
    public FloatBinaryTranslationGeneration()
        : base(expectedLen: 4, typeName: "Float")
    {
        PreferDirectTranslation = false;
        CustomRead = ReadFloat;
        CustomWrite = WriteFloat;
        AdditionalWriteParams.Add(AdditionalMultWriteParam);
        AdditionalWriteParams.Add(AdditionalDivisorWriteParam);
        AdditionalCopyInParams.Add(AdditionalMultReadParam);
        AdditionalCopyInParams.Add(AdditionalDivisorReadParam);
        AdditionalCopyInRetParams.Add(AdditionalMultReadParam);
        AdditionalCopyInRetParams.Add(AdditionalDivisorReadParam);
    }

    private static TryGet<string> AdditionalMultReadParam(
        ObjectGeneration objGen,
        TypeGeneration typeGen)
    {
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType == null
            && floatType.HasTransformations)
        {
            return TryGet<string>.Succeed($"multiplier: {floatType.MultiplierString}");
        }
        return TryGet<string>.Failure;
    }

    private static TryGet<string> AdditionalDivisorReadParam(
        ObjectGeneration objGen,
        TypeGeneration typeGen)
    {
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType == null
            && floatType.HasTransformations)
        {
            return TryGet<string>.Succeed($"divisor: {floatType.DivisorString}");
        }
        return TryGet<string>.Failure;
    }

    private static TryGet<string> AdditionalMultWriteParam(
        ObjectGeneration objGen,
        TypeGeneration typeGen)
    {
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType == null
            && floatType.HasTransformations)
        {
            // Intentionally flipped
            return TryGet<string>.Succeed($"divisor: {floatType.MultiplierString}");
        }
        return TryGet<string>.Failure;
    }

    private static TryGet<string> AdditionalDivisorWriteParam(
        ObjectGeneration objGen,
        TypeGeneration typeGen)
    {
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType == null
            && floatType.HasTransformations)
        {
            // Intentionally flipped
            return TryGet<string>.Succeed($"multiplier: {floatType.DivisorString}");
        }
        return TryGet<string>.Failure;
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        if (typeGen.GetFieldData().Binary != BinaryGenerationType.Normal) return await base.ExpectedLength(objGen, typeGen);
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType.HasValue)
        {
            return floatType.IntegerType switch
            {
                FloatIntegerType.UInt => 4,
                FloatIntegerType.UShort => 2,
                FloatIntegerType.Byte => 1,
                _ => throw new NotImplementedException(),
            };
        }
        else
        {
            return 4;
        }
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType.HasValue)
        {
            return $"{GetTranslatorInstance(typeGen, getter: true)}.GetFloat({dataAccessor}, {nameof(FloatIntegerType)}.{floatType.IntegerType}, multiplier: {floatType.MultiplierString}, divisor: {floatType.DivisorString})";
        }
        else if (floatType.HasMultiplier && floatType.HasDivisor)
        {
            return $"{dataAccessor}.Float() * {(float)floatType.Multiplier}f / {(float)floatType.Divisor}f";
        }
        else if (floatType.HasMultiplier)
        {
            return $"{dataAccessor}.Float() * {(float)floatType.Multiplier}f";
        }
        else if (floatType.HasDivisor)
        {
            return $"{dataAccessor}.Float() / {(float)floatType.Divisor}f";
        }
        else
        {
            return $"{dataAccessor}.Float()";
        }
    }

    bool ReadFloat(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item)
    {
        var floatType = typeGen as FloatType;
        if (floatType.IntegerType.HasValue)
        {
            using (var args = sb.Call(
                       $"{item} = {GetTranslatorInstance(typeGen, getter: true)}.Parse"))
            {
                args.Add($"reader: {reader}");
                args.Add($"integerType: {nameof(FloatIntegerType)}.{floatType.IntegerType}");
                if (floatType.HasTransformations)
                {
                    args.Add($"multiplier: {floatType.MultiplierString}");
                    args.Add($"divisor: {floatType.DivisorString}");
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    bool WriteFloat(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item)
    {
        var floatType = typeGen as FloatType;
        var data = floatType.GetFieldData();
        if (floatType.IntegerType.HasValue)
        {
            using (var args = sb.Call(
                       $"{GetTranslatorInstance(typeGen, getter: true)}.Write"))
            {
                args.Add($"writer: {writer}");
                args.Add($"item: {item}");
                args.Add($"integerType: {nameof(FloatIntegerType)}.{floatType.IntegerType}");
                if (floatType.HasTransformations)
                {
                    // Intentionally flipped
                    args.Add($"multiplier: {floatType.DivisorString}");
                    args.Add($"divisor: {floatType.MultiplierString}");
                }
                if (data.RecordType.HasValue
                    && data.HandleTrigger)
                {
                    args.Add($"header: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}