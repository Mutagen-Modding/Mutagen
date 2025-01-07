using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class ByteBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<byte>
{
    public ByteBinaryTranslationGeneration() 
        : base(expectedLen: 1)
    {
        CustomRead = (sb, o, t, reader, item) =>
        {
            sb.AppendLine($"{item} = {reader}.ReadUInt8();");
            return true;
        };
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor structDataAccessor,  
        Accessor recordDataAccessor, 
        int? currentPosition,
        string passedLengthAccessor,
        DataType dataType)
    {
        var data = typeGen.GetFieldData();
        switch (data.BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
                return;
            case BinaryGenerationType.Custom:
                await this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                    sb,
                    objGen,
                    typeGen,
                    currentPosition,
                    passedLengthAccessor,
                    dataType);
                return;
            default:
                throw new NotImplementedException();
        }
        if (data.HasTrigger)
        {
            sb.AppendLine($"private int? _{typeGen.Name}Location;");
            if (typeGen.CanBeNullable(getter: true))
            {
                recordDataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}({recordDataAccessor}, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)})";
                sb.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {recordDataAccessor}[0] : default(Byte{(typeGen.Nullable ? "?" : null)});");
            }
            else
            {
                sb.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
                if (dataType != null) throw new ArgumentException();
                recordDataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}({recordDataAccessor}, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)})";
                sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {recordDataAccessor}[0] : default(Byte{(typeGen.Nullable ? "?" : null)});");
            }
        }
        else
        {
            if (dataType == null)
            {
                if (data.IsAfterBreak)
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {structDataAccessor}.Length <= {passedLengthAccessor} ? default : {structDataAccessor}.Span[{passedLengthAccessor ?? "0x0"}];");   
                }
                else
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {structDataAccessor}.Span[{passedLengthAccessor ?? "0x0"}];");   
                }
            }
            else
            {
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
                sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {recordDataAccessor}.Span[_{typeGen.Name}Location] : default;");
            }
        }
    }

    public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
    {
        return $"{dataAccessor}[0]";
    }
}