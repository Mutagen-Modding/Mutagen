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
                    return $"{itemAccessor.PropertyOrDirectAccess}.FormKey{(typeGen.HasBeenSet ? ".Value" : null)}";
                case FormLinkType.FormIDTypeEnum.EDIDChars:
                    return $"{itemAccessor.PropertyOrDirectAccess}.EDID";
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
            Accessor translationMaskAccessor)
        {
            FormLinkType linkType = typeGen as FormLinkType;
            MaskGenerationUtility.WrapErrorFieldIndexPush(fg,
                () =>
                {
                    if (itemAccessor.DirectIsAssignment)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"{itemAccessor}.{(linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "EDID")} = {(linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "RecordType")}XmlTranslation.Instance.Parse"))
                        {
                            args.AddPassArg("node");
                            args.AddPassArg("errorMask");
                        }
                    }
                    else
                    {
                        using (var args = new FunctionWrapper(fg,
                            itemAccessor.Assign($"new {linkType.DirectTypeName(getter: false)}")))
                        {
                            args.Add(subFg =>
                            {
                                using (var subArgs = new FunctionWrapper(subFg,
                                    $"FormKeyXmlTranslation.Instance.Parse"))
                                {
                                    subArgs.AddPassArg("node");
                                    subArgs.AddPassArg("errorMask");
                                }
                            });
                        }
                    }
                },
                indexAccessor: typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                errorMaskAccessor: errorMaskAccessor,
                doIt: typeGen.HasIndex);
        }
    }
}
