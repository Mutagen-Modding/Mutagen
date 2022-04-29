using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Noggog;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class ByteArrayBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<byte[]>
{
    public ByteArrayBinaryTranslationGeneration()
        : base(nullable: true,
            expectedLen: null,
            typeName: "ByteArray")
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
        var data = typeGen.GetFieldData();
        using (var args = new ArgsWrapper(sb,
                   $"{this.NamespacePrefix}{GetTranslatorInstance(typeGen, getter: true)}.Write"))
        {
            args.Add($"writer: {writerAccessor}");
            args.Add($"item: {itemAccessor}");
            if (this.DoErrorMasks)
            {
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"errorMask: {errorMaskAccessor}");
            }
            if (data.RecordType.HasValue)
            {
                args.Add($"header: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
            }
            if (data.OverflowRecordType.HasValue)
            {
                args.Add($"overflowRecord: {objGen.RecordTypeHeaderName(data.OverflowRecordType.Value)}");
            }
        }
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        if (data.HasTrigger)
        {
            sb.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }

        string framePass;
        if (data.HasTrigger)
        {
            framePass = $"{frameAccessor}.SpawnWithLength(contentLength)";
        }
        else if (data.Length.HasValue)
        {
            framePass = $"{frameAccessor}.SpawnWithLength({data.Length.Value})";
        }
        else
        {
            framePass = frameAccessor.ToString();
        }

        TranslationGeneration.WrapParseCall(
            new TranslationWrapParseArgs()
            {
                FG = sb,
                TypeGen = typeGen,
                TranslatorLine = $"{this.NamespacePrefix}{GetTranslatorInstance(typeGen, getter: true)}",
                MaskAccessor = errorMaskAccessor,
                ItemAccessor = itemAccessor,
                IndexAccessor = typeGen.IndexEnumInt,
                ExtraArgs = $"reader: {framePass}".AsEnumerable(),
                SkipErrorMask = !this.DoErrorMasks
            });
    }

    public override void GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration targetGen,
        TypeGeneration typeGen,
        Accessor nodeAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        if (inline)
        {
            sb.AppendLine($"transl: {this.GetTranslatorInstance(typeGen, getter: false)}.Parse");
            return;
        }
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        using (var args = new ArgsWrapper(sb,
                   $"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}{this.NamespacePrefix}{GetTranslatorInstance(typeGen, getter: true)}.Parse",
                   suffixLine: Loqui.Generation.Utility.ConfigAwait(asyncMode)))
        {
            args.Add(nodeAccessor.Access);
            if (this.DoErrorMasks)
            {
                args.Add($"errorMask: out {errorMaskAccessor}");
            }
            if (asyncMode == AsyncMode.Off)
            {
                args.Add($"item: out {outItemAccessor}");
            }
            if (data.HasTrigger)
            {
                args.Add($"length: subLength");
            }
            else
            {
                args.Add($"length: {data.Length.Value}");
            }
        }
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        int? currentPosition,
        string passedLengthAccessor,
        DataType dataType = null)
    {
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
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
                    dataAccessor,
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
        }
        if (data.RecordType.HasValue)
        {
            if (dataType != null) throw new ArgumentException();
            if (data.OverflowRecordType.HasValue)
            {
                sb.AppendLine($"private int? _{typeGen.Name}LengthOverride;");
                using (var args = new ArgsWrapper(sb,
                           $"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => {nameof(PluginUtilityTranslation)}.{nameof(PluginUtilityTranslation.ReadByteArrayWithOverflow)}"))
                {
                    args.Add(dataAccessor.ToString());
                    args.Add($"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}");
                    args.Add($"_{typeGen.Name}Location");
                    args.Add($"_{typeGen.Name}LengthOverride");
                }
            }
            else
            {
                sb.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}(_data, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}) : {(typeGen.Nullable ? $"default(ReadOnlyMemorySlice<byte>?)" : "Array.Empty<byte>()")};");
            }
        }
        else
        {
            if (dataType == null)
            {
                if (typeGen.Nullable)
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => {dataAccessor}.Length >= {(currentPosition + (await this.ExpectedLength(objGen, typeGen)).Value)} ? {dataAccessor}.Span.Slice({passedLengthAccessor ?? "0x0"}, {data.Length.Value}).ToArray() : default(ReadOnlyMemorySlice<byte>?);");
                }
                else if (data.Length.HasValue)
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => {dataAccessor}.Span.Slice({passedLengthAccessor ?? "0x0"}, 0x{data.Length.Value:X}).ToArray();");
                }
                else
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => {dataAccessor}.Span{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}.ToArray();");
                }
            }
            else
            {
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
                sb.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {dataAccessor}.Span.Slice(_{typeGen.Name}Location, {(await this.ExpectedLength(objGen, typeGen)).Value}).ToArray() : default(ReadOnlyMemorySlice<byte>{(typeGen.Nullable ? "?" : null)});");
            }
        }
    }

    public override async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        if (!data.RecordType.HasValue)
        {
            if (data.Length.HasValue)
            {
                return checked((int)data.Length.Value);
            }
            return null;
        }
        else
        {
            return 0;
        }
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        ByteArrayType bType = typeGen as ByteArrayType;
        return bType.Length;
    }

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor locationAccessor,
        Accessor packageAccessor,
        Accessor converterAccessor)
    {
        var data = typeGen.GetFieldData();
        await base.GenerateWrapperRecordTypeParse(sb, objGen, typeGen, locationAccessor, packageAccessor, converterAccessor);
        if (data.OverflowRecordType.HasValue
            && data.BinaryOverlayFallback != BinaryGenerationType.Custom)
        {
            sb.AppendLine($"_{typeGen.Name}LengthOverride = lastParsed.{nameof(PreviousParse.LengthOverride)};");
            sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.LengthOverride)}.HasValue)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"stream.Position += lastParsed.{nameof(PreviousParse.LengthOverride)}.Value;");
            }
        }
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        return dataAccessor.ToString();
    }
}