using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class FormLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
        public FormLinkBinaryTranslationGeneration()
            : base(expectedLen: 4)
        {
            this.PreferDirectTranslation = false;
        }

        protected override string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
        {
            return itemAccessor.PropertyOrDirectAccess;
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return false;
        }

        public override string Typename(TypeGeneration typeGen)
        {
            FormLinkType linkType = typeGen as FormLinkType;
            switch (linkType.FormIDType)
            {
                case FormLinkType.FormIDTypeEnum.Normal:
                    return "FormLink";
                case FormLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            FormLinkType linkType = typeGen as FormLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
                fg.AppendLine("r.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }
            switch (linkType.FormIDType)
            {
                case FormLinkType.FormIDTypeEnum.Normal:
                    using (var args = new ArgsWrapper(fg,
                        $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
                    {
                        args.Add(nodeAccessor.DirectAccess);
                        if (this.DoErrorMasks)
                        {
                            args.Add($"errorMask: {errorMaskAccessor}");
                        }
                        args.Add($"item: out {outItemAccessor.DirectAccess}");
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
                    fg.AppendLine($"{errorMaskAccessor} = null;");
                    fg.AppendLine($"{outItemAccessor.DirectAccess} = new {linkType.TypeName(getter: false)}(HeaderTranslation.ReadNextRecordType(r.Reader));");
                    fg.AppendLine($"return true;");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor frameAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            FormLinkType linkType = typeGen as FormLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = $"{itemAccessor}.{(linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "EDID")}",
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    TypeOverride = linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "RecordType",
                    DefaultOverride = linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey.Null" : "RecordType.Null",
                    ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}"
                        .Single()
                        .ToArray(),
                    SkipErrorMask = !this.DoErrorMasks,
                });
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
            FormLinkType linkType = typeGen as FormLinkType;
            var data = typeGen.GetFieldData();
            switch (linkType.FormIDType)
            {
                case FormLinkType.FormIDTypeEnum.Normal:
                    if (CustomWrite != null)
                    {
                        CustomWrite(fg, writerAccessor, itemAccessor);
                    }
                    else if (data.HasTrigger || !PreferDirectTranslation)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Write{(typeGen.HasBeenSet ? "Nullable" : null)}"))
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
                                args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                            }
                        }
                    }
                    else
                    {
                        fg.AppendLine($"{writerAccessor.DirectAccess}.Write({itemAccessor.DirectAccess});");
                    }
                    break;
                case FormLinkType.FormIDTypeEnum.EDIDChars:
                    using (var args = new ArgsWrapper(fg,
                        $"{this.Namespace}RecordTypeBinaryTranslation.Instance.Write{(typeGen.HasBeenSet ? "Nullable" : null)}"))
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
                    return $"new {linkType.DirectTypeName(getter: true, internalInterface: true)}(FormKey.Factory({packageAccessor}.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian({dataAccessor})))";
                case FormLinkType.FormIDTypeEnum.EDIDChars:
                    return $"new EDIDLink<{linkType.LoquiType.TypeName(getter: true, internalInterface: true)}>(new RecordType(BinaryPrimitives.ReadInt32LittleEndian({dataAccessor})))";
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateWrapperFields(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, int? currentPosition, DataType dataType = null)
        {
            var data = typeGen.GetFieldData();
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
            }
            FormLinkType linkType = typeGen as FormLinkType;
            
            if (data.RecordType.HasValue)
            {
                if (dataType != null)
                {
                    throw new ArgumentException();
                }
                dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {GenerateForTypicalWrapper(objGen, typeGen, dataAccessor, "_package")} : {linkType.DirectTypeName(getter: true)}.Empty;");
            }
            else
            {
                if (this.ExpectedLength(objGen, typeGen) == null)
                {
                    throw new NotImplementedException();
                }
                if (dataType == null)
                {
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength(objGen, typeGen).Value})", "_package")};");
                }
                else
                {
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice(_{typeGen.Name}Location, {this.ExpectedLength(objGen, typeGen).Value})", "_package")} : {linkType.DirectTypeName(getter: true)}.Empty;");
                }
            }
        }
    }
}
