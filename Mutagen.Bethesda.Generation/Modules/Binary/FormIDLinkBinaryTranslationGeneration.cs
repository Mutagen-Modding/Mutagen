using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<RawFormID>
    {
        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string nodeAccessor, 
            Accessor retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    using (var args = new ArgsWrapper(fg,
                        $"{retAccessor.DirectAccess}{this.Namespace}{this.typeName}BinaryTranslation.Instance.Parse",
                        suffixLine: $".Bubble((o) => new {linkType.TypeName}(o))"))
                    {
                        args.Add(nodeAccessor);
                        args.Add($"doMasks: {doMaskAccessor}");
                        args.Add($"errorMask: out {maskAccessor}");
                        foreach (var arg in AdditionCopyInRetParameters(
                            fg: fg,
                            objGen: objGen,
                            typeGen: typeGen,
                            nodeAccessor: nodeAccessor,
                            retAccessor: retAccessor,
                            doMaskAccessor: doMaskAccessor,
                            maskAccessor: maskAccessor))
                        {
                            args.Add(arg);
                        }
                    }
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    fg.AppendLine($"{maskAccessor} = null;");
                    fg.AppendLine($"return TryGet<{linkType.TypeName}>.Succeed(new {linkType.TypeName}(HeaderTranslation.GetNextRecordType(r)));");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
