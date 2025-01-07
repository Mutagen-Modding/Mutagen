using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Mutagen.Bethesda.Generation.Fields;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class PrimitiveBinaryTranslationGeneration<T> : BinaryTranslationGeneration
{
    private readonly int? _expectedLength;
    private readonly string _typeName;
    protected bool? nullable;
    public bool Nullable => nullable ?? false || typeof(T).GetName().EndsWith("?");
    public bool PreferDirectTranslation = true;
    public delegate bool CustomReadAction(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item);
    public CustomReadAction? CustomRead;
    public delegate bool CustomWriteAction(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item);
    public CustomWriteAction? CustomWrite;
    public delegate bool CustomWrapperAction(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor data, Accessor passedLen);
    public CustomWrapperAction? CustomWrapper;
    public override bool NeedsNamespacePrefix => false;
    public override string Namespace => "Mutagen.Bethesda.Translations.Binary.";
    public virtual bool NeedsGenerics => true;

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        return $"{Typename(typeGen)}BinaryTranslation{(NeedsGenerics ? $"<{Module.ReaderClass}, {Module.WriterClass}>" : null)}.Instance";
    }

    public PrimitiveBinaryTranslationGeneration(int? expectedLen, string? typeName = null, bool? nullable = null)
    {
        _expectedLength = expectedLen;
        this.nullable = nullable;
        _typeName = typeName ?? typeof(T).GetName().Replace("?", string.Empty);
    }

    protected virtual string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
    {
        return $"{itemAccessor}";
    }

    public virtual string Typename(TypeGeneration typeGen) => _typeName;

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
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        if (CustomWrite != null)
        {
            if (CustomWrite(sb, objGen, typeGen, writerAccessor, itemAccessor)) return;
        }
        if (data.HasTrigger || !PreferDirectTranslation)
        {
            using (var args = sb.Call(
                       $"{this.NamespacePrefix}{this.GetTranslatorInstance(typeGen, getter: true)}.Write{(typeGen.Nullable ? "Nullable" : null)}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {ItemWriteAccess(typeGen, itemAccessor)}");
                if (this.DoErrorMasks)
                {
                    if (typeGen.HasIndex)
                    {
                        args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                    }
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                if (data.RecordType.HasValue
                    && data.HandleTrigger)
                {
                    args.Add($"header: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                }
                foreach (var writeParam in this.AdditionalWriteParams)
                {
                    var get = writeParam(
                        objGen: objGen,
                        typeGen: typeGen);
                    if (get.Failed) continue;
                    args.Add(get.Value);
                }
            }
        }
        else if (typeGen.GetFieldData().Length > 1)
        {
            sb.AppendLine($"{writerAccessor}.Write({itemAccessor}, length: {typeGen.GetFieldData().Length});");
        }
        else
        {
            sb.AppendLine($"{writerAccessor}.Write({itemAccessor});");
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
        var fieldData = typeGen.GetFieldData();
        if (fieldData.HasTrigger)
        {
            sb.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }

        bool hasCustom = false;
        List<string> extraArgs = new List<string>();
        extraArgs.Add($"reader: {frameAccessor}{(fieldData.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}");
        foreach (var writeParam in this.AdditionalCopyInParams)
        {
            var get = writeParam(
                objGen: objGen,
                typeGen: typeGen);
            if (get.Failed) continue;
            extraArgs.Add(get.Value);
            hasCustom = true;
        }

        if (CustomRead != null)
        {
            if (CustomRead(sb, objGen, typeGen, frameAccessor, itemAccessor)) return;
        }
        if (PreferDirectTranslation && !hasCustom)
        {
            sb.AppendLine($"{itemAccessor} = {frameAccessor}.Read{_typeName}();");
        }
        else
        {
            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = sb,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.NamespacePrefix}{this.GetTranslatorInstance(typeGen, getter: true)}",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    AsyncMode = AsyncMode.Off,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = extraArgs.ToArray(),
                    SkipErrorMask = !this.DoErrorMasks
                });
        }
    }

    public override async Task GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration targetGen,
        TypeGeneration typeGen,
        Accessor nodeAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
        if (inline)
        {
            sb.AppendLine($"transl: {this.GetTranslatorInstance(typeGen, getter: false)}.Parse");
        }
        else
        {
            var data = typeGen.GetFieldData();
            if (data.RecordType.HasValue)
            {
                if (inline)
                {
                    throw new NotImplementedException();
                }
                sb.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = sb.Call(
                       $"{outItemAccessor} = {this.NamespacePrefix}{this.GetTranslatorInstance(typeGen, getter: true)}.Parse"))
            {
                args.Add(nodeAccessor.Access);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                foreach (var writeParam in this.AdditionalCopyInRetParams)
                {
                    var get = writeParam(
                        objGen: objGen,
                        typeGen: typeGen);
                    if (get.Failed) continue;
                    args.Add(get.Value);
                }
            }
            sb.AppendLine("return true;");
        }
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        return $"BinaryPrimitives.Read{typeGen.TypeName(getter: true)}LittleEndian({dataAccessor})";
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor structDataAccessor,  
        Accessor recordDataAccessor, 
        int? currentPosition,
        string passedLengthAccessor,
        DataType? dataType = null)
    {
        passedLengthAccessor ??= "0x0";
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
        }
        if (data.RecordType.HasValue)
        {
            if (dataType != null) throw new ArgumentException();
            recordDataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}({recordDataAccessor}, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)})";
            sb.AppendLine($"public {typeGen.OverrideStr}{typeGen.TypeName(getter: true)}{typeGen.NullChar} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {GenerateForTypicalWrapper(objGen, typeGen, recordDataAccessor, "_package")} : {typeGen.GetDefault(getter: true)};");
        }
        else
        {
            var expectedLen = await this.ExpectedLength(objGen, typeGen);
            if (this.CustomWrapper != null)
            {
                if (CustomWrapper(sb, objGen, typeGen, structDataAccessor, passedLengthAccessor)) return;
            }
            if (dataType == null)
            {
                if (typeGen.Nullable)
                {
                    if (!typeGen.CanBeNullable(getter: true))
                    {
                        throw new NotImplementedException();
                        //sb.AppendLine($"public bool {typeGen.Name}_IsSet => {dataAccessor}.Length >= {(currentPosition + this.ExpectedLength(objGen, typeGen).Value)};");
                        //sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength(objGen, typeGen).Value})", "_package")};");
                    }
                    else
                    {
                        string passed = int.TryParse(passedLengthAccessor.TrimStart("0x"), System.Globalization.NumberStyles.HexNumber, null, out var passedInt) ? (passedInt + expectedLen.Value).ToString() : $"({passedLengthAccessor} + {expectedLen.Value})";
                        sb.AppendLine($"public {typeGen.TypeName(getter: true)}? {typeGen.Name} => {structDataAccessor}.Length >= {passed} ? {GenerateForTypicalWrapper(objGen, typeGen, $"{structDataAccessor}.Slice({passedLengthAccessor}{(expectedLen != null ? $", 0x{expectedLen.Value:X}" : null)})", "_package")} : {typeGen.GetDefault(getter: true)};");
                    }
                }
                else if (data.IsAfterBreak)
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {structDataAccessor}.Length <= {passedLengthAccessor} ? default : {GenerateForTypicalWrapper(objGen, typeGen, $"{structDataAccessor}.Slice({passedLengthAccessor}{(expectedLen != null ? $", 0x{expectedLen.Value:X}" : null)})", "_package")};");
                }
                else
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{structDataAccessor}.Slice({passedLengthAccessor}{(expectedLen != null ? $", 0x{expectedLen.Value:X}" : null)})", "_package")};");
                }
            }
            else
            {
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
                sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {GenerateForTypicalWrapper(objGen, typeGen, $"{recordDataAccessor}.Slice(_{typeGen.Name}Location{(expectedLen.HasValue ? $", {expectedLen.Value}" : null)})", "_package")} : {typeGen.GetDefault(getter: true)};");
            }
        }
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return typeGen.GetFieldData().Length ?? this._expectedLength;
    }
}