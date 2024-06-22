using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class FormLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
{
    public override bool NeedsGenerics => false;

    public FormLinkBinaryTranslationGeneration()
        : base(expectedLen: 4)
    {
        this.PreferDirectTranslation = false;
    }

    protected override string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
    {
        return itemAccessor.Access;
    }

    public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return false;
    }

    public override string Typename(TypeGeneration typeGen)
    {
        FormLinkType linkType = typeGen as FormLinkType;
        return linkType.FormIDType switch
        {
            FormLinkType.FormIDTypeEnum.Normal => "FormLink",
            FormLinkType.FormIDTypeEnum.EDIDChars => "RecordType",
            _ => throw new NotImplementedException(),
        };
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
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        if (inline)
        {
            sb.AppendLine($"transl: {this.GetTranslatorInstance(typeGen, getter: false)}.Parse");
            return;
        }
        if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
        FormLinkType linkType = typeGen as FormLinkType;
        var data = typeGen.GetFieldData();
        if (data.RecordType.HasValue)
        {
            if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
            sb.AppendLine("r.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
        }
        switch (linkType.FormIDType)
        {
            case FormLinkType.FormIDTypeEnum.Normal:
                using (var args = sb.Call(
                           $"{retAccessor}{this.NamespacePrefix}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
                {
                    args.Add(nodeAccessor.Access);
                    if (this.DoErrorMasks)
                    {
                        args.Add($"errorMask: {errorMaskAccessor}");
                    }
                    args.Add($"item: out {outItemAccessor}");
                    foreach (var writeParam in this.AdditionalCopyInRetParams)
                    {
                        var get = writeParam(
                            objGen: objGen,
                            typeGen: typeGen);
                        if (get.Failed) continue;
                        args.Add(get.Value);
                    }
                }
                break;
            case FormLinkType.FormIDTypeEnum.EDIDChars:
                sb.AppendLine($"{errorMaskAccessor} = null;");
                sb.AppendLine($"{outItemAccessor} = new {linkType.TypeName(getter: false)}(HeaderTranslation.ReadNextRecordType(r.Reader));");
                sb.AppendLine($"return true;");
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor, 
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor)
    {
        FormLinkType linkType = typeGen as FormLinkType;
        var data = typeGen.GetFieldData();
        if (data.RecordType.HasValue)
        {
            sb.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }

        using (var args = sb.Call(
                   $"{itemAccessor}.SetTo"))
        {
            args.Add(subFg =>
            {
                using (var args2 = subFg.Function(
                           $"{this.NamespacePrefix}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
                {
                    args2.Add("reader: frame");
                    if (linkType.MaxIsNone)
                    {
                        args2.Add("maxIsNone: true");
                    }
                }
            });
        }
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
        FormLinkType linkType = typeGen as FormLinkType;
        var data = typeGen.GetFieldData();
        switch (linkType.FormIDType)
        {
            case FormLinkType.FormIDTypeEnum.Normal:
                if (CustomWrite != null)
                {
                    if (CustomWrite(sb, objGen, typeGen, writerAccessor, itemAccessor)) return;
                }
                if (data.HasTrigger || !PreferDirectTranslation)
                {
                    using (var args = sb.Call(
                               $"{this.NamespacePrefix}{this.Typename(typeGen)}BinaryTranslation.Instance.Write{(typeGen.Nullable ? "Nullable" : null)}"))
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
                    }
                }
                else
                {
                    sb.AppendLine($"{writerAccessor}.Write({itemAccessor});");
                }
                break;
            case FormLinkType.FormIDTypeEnum.EDIDChars:
                using (var args = sb.Call(
                           $"{this.NamespacePrefix}RecordTypeBinaryTranslation.Instance.Write{(typeGen.Nullable ? "Nullable" : null)}"))
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
                    if (data.RecordType.HasValue)
                    {
                    }
                }
                break;
            default:
                break;
        }
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        FormLinkType linkType = typeGen as FormLinkType;
        switch (linkType.FormIDType)
        {
            case FormLinkType.FormIDTypeEnum.Normal:
                return $"FormLinkBinaryTranslation.Instance.{(typeGen.Nullable ? "Nullable" : string.Empty)}OverlayFactory<{linkType.LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>({packageAccessor}, {dataAccessor}{(linkType.MaxIsNone ? ", maxIsNull: true" : null)})";
            case FormLinkType.FormIDTypeEnum.EDIDChars:
                return $"new EDIDLink<{linkType.LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>(new RecordType(BinaryPrimitives.ReadInt32LittleEndian({dataAccessor})))";
            default:
                throw new NotImplementedException();
        }
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
        FormLinkType linkType = typeGen as FormLinkType;
            
        if (data.RecordType.HasValue)
        {
            if (dataType != null) throw new ArgumentException();
            sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {nameof(FormKeyBinaryTranslation)}.Instance.{nameof(FormKeyBinaryTranslation.OverlayNullableHelper)}<{linkType.GenericString}>(_{typeGen.Name}Location, _package, {recordDataAccessor}{(linkType.MaxIsNone ? ", maxIsNull: true" : null)});");
        }
        else
        {
            if (await this.ExpectedLength(objGen, typeGen) == null)
            {
                throw new NotImplementedException();
            }
            if (dataType == null)
            {
                if (data.IsAfterBreak)
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {structDataAccessor}.Length <= {passedLengthAccessor} ? {linkType.DirectTypeName(getter: true)}.Null : {GenerateForTypicalWrapper(objGen, typeGen, $"{structDataAccessor}.Span.Slice({passedLengthAccessor ?? "0x0"}, 0x{(await this.ExpectedLength(objGen, typeGen)).Value:X})", "_package")};");
                }
                else
                {
                    sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{structDataAccessor}.Span.Slice({passedLengthAccessor ?? "0x0"}, 0x{(await this.ExpectedLength(objGen, typeGen)).Value:X})", "_package")};");
                }
            }
            else
            {
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
                sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => FormLinkBinaryTranslation.Instance.OverlayFactory<{linkType.LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>(_package, {recordDataAccessor}.Span.Slice(_{typeGen.Name}Location, 0x{(await this.ExpectedLength(objGen, typeGen)).Value:X}), isSet: _{typeGen.Name}_IsSet{(linkType.MaxIsNone ? ", maxIsNull: true" : null)});");
            }
        }
    }
}