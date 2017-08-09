using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class LoquiBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public string ModNickname;

        public LoquiBinaryTranslationGeneration(string modNickname)
        {
            this.ModNickname = modNickname;
        }

        public override void GenerateWrite(
            FileGeneration fg,
            TypeGeneration typeGen,
            string writerAccessor,
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{loquiGen.TargetObjectGeneration.ExtCommonName}.Write_{ModNickname}"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {itemAccessor}");
                    args.Add($"doMasks: {doMaskAccessor}");
                    args.Add($"errorMask: out {loquiGen.ErrorMaskItemString} loquiMask");
                }
                fg.AppendLine($"{maskAccessor} = loquiMask == null ? null : new MaskItem<Exception, {loquiGen.ErrorMaskItemString}>(null, loquiMask);");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            var loquiGen = typeGen as LoquiType;
            return loquiGen.SingletonType != LoquiType.SingletonLevel.Singleton || loquiGen.InterfaceType != LoquiInterfaceType.IGetter;
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            TypeGeneration typeGen,
            string readerAccessor,
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                if (loquiGen.SingletonType == LoquiType.SingletonLevel.Singleton)
                {
                    if (loquiGen.InterfaceType == LoquiInterfaceType.IGetter) return;
                    using (var args = new ArgsWrapper(fg,
                        $"var tmp = {loquiGen.TargetObjectGeneration.Name}.Create_{ModNickname}"))
                    {
                        args.Add($"reader: {readerAccessor}");
                        args.Add($"doMasks: {doMaskAccessor}");
                        args.Add($"errorMask: out {loquiGen.ErrorMaskItemString} createMask");
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"{loquiGen.TargetObjectGeneration.ExtCommonName}.CopyFieldsFrom"))
                    {
                        args.Add($"item: {itemAccessor}");
                        args.Add("rhs: tmp");
                        args.Add("def: null");
                        args.Add("cmds: null");
                        args.Add("copyMask: null");
                        args.Add($"doErrorMask: {doMaskAccessor}");
                        args.Add($"errorMask: out {loquiGen.ErrorMaskItemString} copyMask");
                    }
                    fg.AppendLine($"var loquiMask = {loquiGen.ErrorMaskItemString}.Combine(createMask, copyMask);");
                    fg.AppendLine($"{maskAccessor} = loquiMask == null ? null : new MaskItem<Exception, {loquiGen.ErrorMaskItemString}>(null, loquiMask);");
                }
                else
                {
                    GenerateCopyInRet(fg, typeGen, readerAccessor, null, doMaskAccessor, maskAccessor);
                    fg.AppendLine("if (tryGet.Succeeded)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"{itemAccessor} = tryGet.Value;");
                    }
                }
            }
            else
            {
                UnsafeXmlTranslationGeneration unsafeXml = new UnsafeXmlTranslationGeneration()
                {
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.ErrorMaskItemString}>"
                };
                unsafeXml.GenerateCopyIn(
                    fg: fg,
                    typeGen: typeGen,
                    nodeAccessor: readerAccessor,
                    itemAccessor: itemAccessor,
                    doMaskAccessor: doMaskAccessor,
                    maskAccessor: "var unsafeMask");
                fg.AppendLine($"{maskAccessor} = unsafeMask == null ? null : new MaskItem<Exception, object>(null, unsafeMask);");
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            TypeGeneration typeGen,
            string readerAccessor,
            string retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                fg.AppendLine($"{loquiGen.TargetObjectGeneration.ErrorMask} loquiMask;");
                using (var args = new ArgsWrapper(fg,
                    $"TryGet<{typeGen.TypeName}> tryGet = TryGet<{typeGen.TypeName}>.Succeed(({typeGen.TypeName}){loquiGen.TargetObjectGeneration.Name}.Create_{this.ModNickname}"))
                {
                    args.Add($"reader: {readerAccessor}");
                    args.Add($"doMasks: {doMaskAccessor}");
                    args.Add($"errorMask: out loquiMask)");
                }
                fg.AppendLine($"{maskAccessor} = loquiMask == null ? null : new MaskItem<Exception, {loquiGen.ErrorMaskItemString}>(null, loquiMask);");
                if (retAccessor != null)
                {
                    fg.AppendLine($"{retAccessor}tryGet;");
                }
            }
            else
            {
                UnsafeXmlTranslationGeneration unsafeXml = new UnsafeXmlTranslationGeneration()
                {
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.ErrorMaskItemString}>"
                };
                unsafeXml.GenerateCopyInRet(
                    fg: fg,
                    typeGen: typeGen,
                    nodeAccessor: readerAccessor,
                    retAccessor: retAccessor,
                    doMaskAccessor: doMaskAccessor,
                    maskAccessor: maskAccessor);
            }
        }
    }
}
