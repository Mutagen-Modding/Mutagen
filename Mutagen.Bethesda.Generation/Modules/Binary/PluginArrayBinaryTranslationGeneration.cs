using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using EnumType = Mutagen.Bethesda.Generation.Fields.EnumType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class PluginArrayBinaryTranslationGeneration : PluginListBinaryTranslationGeneration
{
    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        Accessor structDataAccessor,  
        Accessor recordDataAccessor, 
        int? currentPosition,
        string passedLengthAccessor,
        DataType dataType = null)
    {
        ArrayType arr = typeGen as ArrayType;
        var data = arr.GetFieldData();
        if (data.BinaryOverlayFallback != BinaryGenerationType.Normal)
        {
            await base.GenerateWrapperFields(sb, objGen, typeGen, structDataAccessor, recordDataAccessor, currentPosition, passedLengthAccessor, dataType);
            return;
        }
        var subGen = this.Module.GetTypeGeneration(arr.SubTypeGeneration.GetType());
        if (data.OverflowRecordType.HasValue)
        {
            OverflowGenerationHelper.GenerateWrapperOverflowMember(sb, typeGen);
        }
        if (data.HasTrigger)
        {
            sb.AppendLine($"private int? _{typeGen.Name}Location;");
        }
        if (arr.FixedSize.HasValue)
        {
            var useFixedDefaultVariable = arr.FixedSize.HasValue && data.HasTrigger && !arr.Nullable;
            if (useFixedDefaultVariable)
            {
                sb.AppendLine($"private readonly static {typeGen.TypeName(getter: true)} _default{typeGen.Name} = ArrayExt.Create({arr.FixedSize}, {arr.SubTypeGeneration.GetDefault(getter: false)});");
            }
            if (arr.SubTypeGeneration is EnumType e)
            {
                sb.AppendLine($"public {arr.ListTypeName(getter: true, internalInterface: true)} {typeGen.Name} => {nameof(BinaryOverlayArrayHelper)}.{nameof(BinaryOverlayArrayHelper.EnumSliceFromFixedSize)}<{arr.SubTypeGeneration.TypeName(getter: true)}>({structDataAccessor}.Slice({passedLengthAccessor ?? "0x0"}), amount: {arr.FixedSize.Value}, enumLength: {e.ByteLength});");
            }
            else if (arr.SubTypeGeneration is Loqui.Generation.FloatType f
                     && data.HasTrigger)
            {
                sb.AppendLine($"public {typeGen.TypeName(getter: true)}{typeGen.NullChar} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {nameof(BinaryOverlayArrayHelper)}.{nameof(BinaryOverlayArrayHelper.FloatSliceFromFixedSize)}(HeaderTranslation.ExtractSubrecordMemory({recordDataAccessor}, _{typeGen.Name}Location.Value, _package.MetaData.Constants), amount: {arr.FixedSize.Value}) : {(useFixedDefaultVariable ? $"_default{typeGen.Name}" : typeGen.GetDefault(getter: true))};");
            }
            else if (arr.SubTypeGeneration is FormLinkType fl
                     && data.HasTrigger)
            {
                sb.AppendLine($"public {typeGen.TypeName(getter: true)}{typeGen.NullChar} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {nameof(BinaryOverlayArrayHelper)}.{nameof(BinaryOverlayArrayHelper.FormLinkSliceFromFixedSize)}<{fl.LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>(HeaderTranslation.ExtractSubrecordMemory({recordDataAccessor}, _{typeGen.Name}Location.Value, _package.MetaData.Constants{(data.OverflowRecordType.HasValue ? $", {nameof(TypedParseParams)}.{nameof(TypedParseParams.FromLengthOverride)}(_{typeGen.Name}LengthOverride)" : null)}), amount: {arr.FixedSize.Value}, masterReferences: _package.MetaData.MasterReferences) : {(useFixedDefaultVariable ? $"_default{typeGen.Name}" : typeGen.GetDefault(getter: true))};");
            }
            else 
            {
                throw new NotImplementedException();
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        ArrayType arr = typeGen as ArrayType;
        if (arr.FixedSize.HasValue)
        {
            if (arr.Nullable)
            {
                throw new NotImplementedException();
            }
            else if (arr.SubTypeGeneration is EnumType e)
            {
                return arr.FixedSize.Value * e.ByteLength;
            }
            else if (arr.SubTypeGeneration is LoquiType loqui
                     && this.Module.TryGetTypeGeneration(loqui.GetType(), out var loquiGen))
            {
                return arr.FixedSize.Value * await loquiGen.GetPassedAmount(objGen, loqui);
            }
            throw new NotImplementedException();
        }
        return null;
    }

    public override async Task GenerateWrapperRecordTypeParse(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor locationAccessor, Accessor packageAccessor, Accessor converterAccessor)
    {
        ArrayType arr = typeGen as ArrayType;
        var data = typeGen.GetFieldData();
        switch (data.BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
                return;
            case BinaryGenerationType.Custom:
                using (var args = sb.Call(
                           $"{typeGen.Name}CustomParse"))
                {
                    args.AddPassArg($"stream");
                    args.AddPassArg($"finalPos");
                    args.AddPassArg($"offset");
                    args.AddPassArg($"type");
                    args.AddPassArg($"lastParsed");
                }
                return;
            default:
                throw new NotImplementedException();
        }

        if (!arr.FixedSize.HasValue) throw new NotImplementedException();
        if (data.HasTrigger)
        {
            sb.AppendLine($"_{typeGen.Name}Location = {locationAccessor};");
        }
        OverflowGenerationHelper.GenerateWrapperOverflowParse(sb, typeGen, data);
    }

    public override async Task GenerateCopyIn(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor nodeAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationMaskAccessor)
    {
        var arr = typeGen as ArrayType;
        var data = typeGen.GetFieldData();
        var subData = arr.SubTypeGeneration.GetFieldData();
        if (!arr.FixedSize.HasValue || arr.SubTypeGeneration is not Loqui.Generation.FloatType)
        {
            await base.GenerateCopyIn(sb, objGen, typeGen, nodeAccessor, itemAccessor, errorMaskAccessor, translationMaskAccessor);
            return;
        }
        if (!this.Module.TryGetTypeGeneration(arr.SubTypeGeneration.GetType(), out var gen))
        {
            throw new NotImplementedException();
        }
        if (data.HasTrigger && !subData.HasTrigger)
        {
            sb.AppendLine($"frame.Position += frame.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }
        sb.AppendLine($"{itemAccessor} = {nameof(BinaryOverlayArrayHelper)}.{nameof(BinaryOverlayArrayHelper.FloatSliceFromFixedSize)}({nodeAccessor}.ReadBytes({await gen.ExpectedLength(objGen, arr.SubTypeGeneration)} * {arr.FixedSize}), {arr.FixedSize}).ToArray();");
    }
}