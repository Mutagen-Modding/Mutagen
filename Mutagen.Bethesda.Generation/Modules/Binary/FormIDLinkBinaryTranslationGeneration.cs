using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
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
                    return base.Typename(typeGen);
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
            }
        }

        protected override IEnumerable<string> AdditionalWriteParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string maskAccessor)
        {
            yield return "masterReferences: masterReferences";
        }

        protected override IEnumerable<string> AdditionalCopyInParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, Accessor itemAccessor, string maskAccessor)
        {
            yield return "masterReferences: masterReferences";
        }

        protected override IEnumerable<string> AdditionalCopyInRetParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, string retAccessor, Accessor outItemAccessor, string maskAccessor)
        {
            yield return "masterReferences: masterReferences";
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            bool squashedRepeatedList,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    using (var args = new ArgsWrapper(fg,
                        $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
                    {
                        args.Add(nodeAccessor);
                        args.Add($"errorMask: {maskAccessor}");
                        args.Add($"item: out {outItemAccessor.DirectAccess}");
                        foreach (var arg in AdditionalCopyInRetParameters(
                            fg: fg,
                            objGen: objGen,
                            typeGen: typeGen,
                            nodeAccessor: nodeAccessor,
                            retAccessor: retAccessor,
                            outItemAccessor: outItemAccessor,
                            maskAccessor: maskAccessor))
                        {
                            args.Add(arg);
                        }
                    }
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    fg.AppendLine($"{maskAccessor} = null;");
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
            string frameAccessor, 
            Accessor itemAccessor,
            string maskAccessor,
            string translationAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine($"{frameAccessor}.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                    MaskAccessor = maskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : ".Spawn(snapToFinalPosition: false)")}"
                        .Single()
                        .AndWhen("masterReferences: masterReferences", () => linkType.FormIDType == FormIDLinkType.FormIDTypeEnum.Normal)
                        .ToArray()
                });
        }

        public override void GenerateWrite(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen, 
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    base.GenerateWrite(fg, objGen, typeGen, writerAccessor, itemAccessor, maskAccessor,
                        translationMaskAccessor: translationMaskAccessor);
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
                    using (var args = new ArgsWrapper(fg,
                        $"{this.Namespace}RecordTypeBinaryTranslation.Instance.Write"))
                    {
                        args.Add($"writer: {writerAccessor}");
                        args.Add($"item: {ItemWriteAccess(itemAccessor)}");
                        if (typeGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        }
                        args.Add($"errorMask: {maskAccessor}");
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
