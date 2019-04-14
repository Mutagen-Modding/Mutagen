using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkXmlTranslationGeneration : PrimitiveXmlTranslationGeneration<FormKey>
    {
        protected override string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
        {
            return $"{itemAccessor.PropertyOrDirectAccess}?.FormKey";
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
                $"{retAccessor.DirectAccess}{this.TypeName}XmlTranslation.Instance.Parse",
                $".Bubble((o) => new {linkType.TypeName}(o.Value))"))
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
            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.TypeName}XmlTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = $"{XmlTranslationModule.XElementLine.GetParameterName(objGen)}: {frameAccessor}".Single(),
                });
        }
    }
}
