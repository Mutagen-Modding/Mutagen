using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Generation;

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
                return $"{itemAccessor}.FormKey";
            case FormLinkType.FormIDTypeEnum.EDIDChars:
                return $"{itemAccessor}.EDID";
            default:
                throw new NotImplementedException();
        }
    }

    public override void GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor, 
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        FormLinkType linkType = typeGen as FormLinkType;
        MaskGenerationUtility.WrapErrorFieldIndexPush(sb,
            () =>
            {
                if (itemAccessor.IsAssignment)
                {
                    using (var args = new ArgsWrapper(sb,
                               $"{itemAccessor} = {(linkType.FormIDType == FormLinkType.FormIDTypeEnum.Normal ? "FormKey" : "RecordType")}XmlTranslation.Instance.Parse"))
                    {
                        args.AddPassArg("node");
                        args.AddPassArg("errorMask");
                    }
                }
                else
                {
                    using (var args = new FunctionWrapper(sb,
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