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
    public class FormLinkXmlTranslationGeneration : PrimitiveXmlTranslationGeneration<FormKey>
    {
        public override string TypeName(TypeGeneration typeGen)
        {
            FormLinkType type = typeGen as FormLinkType;
            switch (type.FormIDType)
            {
                case FormLinkType.FormIDTypeEnum.Normal:
                    return base.TypeName(typeGen);
                case FormLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
            }
        }

        protected override string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
        {
            FormLinkType type = typeGen as FormLinkType;
            switch (type.FormIDType)
            {
                case FormLinkType.FormIDTypeEnum.Normal:
                    return $"{itemAccessor.PropertyOrDirectAccess}.FormKey";
                case FormLinkType.FormIDTypeEnum.EDIDChars:
                    return $"{itemAccessor.PropertyOrDirectAccess}.EDID";
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
            FormLinkType linkType = typeGen as FormLinkType;
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
            FormLinkType linkType = typeGen as FormLinkType;
            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.TypeName(typeGen)}XmlTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = $"{itemAccessor}.{(linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "EDID")}",
                    TypeOverride = linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "RecordType",
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = $"{XmlTranslationModule.XElementLine.GetParameterName(objGen)}: {frameAccessor}".Single(),
                });
        }
    }
}
