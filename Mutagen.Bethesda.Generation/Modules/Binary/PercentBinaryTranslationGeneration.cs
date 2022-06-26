using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using PercentType = Mutagen.Bethesda.Generation.Fields.PercentType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class PercentBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<double>
{
    public PercentBinaryTranslationGeneration()
        : base(expectedLen: null, typeName: "Percent")
    {
        PreferDirectTranslation = false;
        this.CustomRead = ReadPercent;
        this.CustomWrite = WritePercent;
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        if (typeGen.GetFieldData().Binary != BinaryGenerationType.Normal) return await base.ExpectedLength(objGen, typeGen);
        var percType = typeGen as PercentType;
        switch (percType.IntegerType)
        {
            case FloatIntegerType.UInt:
                return 4;
            case FloatIntegerType.UShort:
                return 2;
            case FloatIntegerType.Byte:
                return 1;
            default:
                throw new NotImplementedException();
        }
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        var percType = typeGen as PercentType;
        return $"{nameof(PercentBinaryTranslation)}.GetPercent({dataAccessor}, {nameof(FloatIntegerType)}.{percType.IntegerType})";
    }

    bool ReadPercent(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item)
    {
        var percType = typeGen as PercentType;
        using (var args = sb.Call(
                   $"{item} = {nameof(PercentBinaryTranslation)}.Parse"))
        {
            args.Add($"reader: {reader}");
            args.Add($"integerType: {nameof(FloatIntegerType)}.{percType.IntegerType}");
        }
        return true;
    }

    bool WritePercent(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item)
    {
        var percType = typeGen as PercentType;
        var data = percType.GetFieldData();
        using (var args = sb.Call(
                   $"{nameof(PercentBinaryTranslation)}.Write"))
        {
            args.Add($"writer: {writer}");
            args.Add($"item: {item}");
            args.Add($"integerType: {nameof(FloatIntegerType)}.{percType.IntegerType}");
            if (data.RecordType.HasValue
                && data.HandleTrigger)
            {
                args.Add($"header: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
            }
        }
        return true;
    }
}