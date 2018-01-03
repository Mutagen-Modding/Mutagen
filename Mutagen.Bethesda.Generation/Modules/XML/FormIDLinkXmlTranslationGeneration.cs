using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkXmlTranslationGeneration : PrimitiveXmlTranslationGeneration<RawFormID>
    {
        protected override string ItemWriteAccess(Accessor itemAccessor)
        {
            return $"{itemAccessor.DirectAccess}?.FormID";
        }

        public override void GenerateCopyInRet(
            FileGeneration fg, 
            TypeGeneration typeGen,
            string nodeAccessor,
            Accessor retAccessor, 
            string doMaskAccessor, 
            string maskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}{this.TypeName}XmlTranslation.Instance.Parse",
                $".Bubble((o) => new {linkType.TypeName}(o.Value))"))
            {
                args.Add(nodeAccessor);
                args.Add($"nullable: {Nullable.ToString().ToLower()}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
            }
        }
    }
}
