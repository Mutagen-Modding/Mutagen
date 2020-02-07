using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkXmlTranslationGeneration : PrimitiveXmlTranslationGeneration<FormKey>
    {
        public override string TypeName(TypeGeneration typeGen)
        {
            FormIDLinkType type = typeGen as FormIDLinkType;
            switch (type.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    return base.TypeName(typeGen);
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
            }
        }

        protected override string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
        {
            FormIDLinkType type = typeGen as FormIDLinkType;
            switch (type.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    return $"{itemAccessor.PropertyOrDirectAccess}?.FormKey";
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    return $"{itemAccessor.PropertyOrDirectAccess}?.EDID";
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}{this.TypeName(typeGen)}XmlTranslation.Instance.Parse",
                $".Bubble((o) => new {linkType.TypeName(getter: false)}(o.Value))"))
            {
                args.Add(nodeAccessor.DirectAccess);
                args.Add($"item: out {outItemAccessor}");
                args.Add($"nullable: {Nullable.ToString().ToLower()}");
                args.Add($"errorMask: out {errorMaskAccessor}");
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor frameAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.TypeName(typeGen)}XmlTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = $"{itemAccessor}.{(linkType.FormIDType == FormIDLinkType.FormIDTypeEnum.Normal ? "FormKey" : "EDID")}",
                    TypeOverride = linkType.FormIDType == FormIDLinkType.FormIDTypeEnum.Normal ? "FormKey" : "RecordType",
                    DefaultOverride = linkType.FormIDType == FormIDLinkType.FormIDTypeEnum.Normal ? "FormKey.Null" : "RecordType.Null",
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = $"{XmlTranslationModule.XElementLine.GetParameterName(objGen)}: {frameAccessor}".Single(),
                });
        }
    }
}
