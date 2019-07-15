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
    public class FormIDLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
        public FormIDLinkBinaryTranslationGeneration()
        {
            this.AdditionalWriteParams.Add(AdditionalParam);
            this.AdditionalCopyInParams.Add(AdditionalParam);
            this.AdditionalCopyInRetParams.Add(AdditionalParam);
            this.PreferDirectTranslation = false;
        }

        protected override string ItemWriteAccess(Accessor itemAccessor)
        {
            return itemAccessor.PropertyOrDirectAccess;
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return false;
        }

        public override string Typename(TypeGeneration typeGen)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    return "FormLink";
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
            }
        }

        private static TryGet<string> AdditionalParam(
           ObjectGeneration objGen,
           TypeGeneration typeGen)
        {
            return TryGet<string>.Succeed("masterReferences: masterReferences");
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
                fg.AppendLine("r.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
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
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    fg.AppendLine($"{errorMaskAccessor} = null;");
                    fg.AppendLine($"{outItemAccessor.DirectAccess} = new {linkType.TypeName}(HeaderTranslation.ReadNextRecordType(r.Reader));");
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
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(MetaDataConstants.SubConstants)}.{nameof(IRecordConstants.HeaderLength)};");
            }

             TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}"
                        .Single()
                        .AndWhen("masterReferences: masterReferences", () => linkType.FormIDType == FormIDLinkType.FormIDTypeEnum.Normal)
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
            Accessor translationMaskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    base.GenerateWrite(fg, objGen, typeGen, writerAccessor, itemAccessor, errorMaskAccessor,
                        translationMaskAccessor: translationMaskAccessor);
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
                    using (var args = new ArgsWrapper(fg,
                        $"{this.Namespace}RecordTypeBinaryTranslation.Instance.Write"))
                    {
                        args.Add($"writer: {writerAccessor}");
                        args.Add($"item: {ItemWriteAccess(itemAccessor)}");
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
                            args.Add($"header: recordTypeConverter.Convert({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                            args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
