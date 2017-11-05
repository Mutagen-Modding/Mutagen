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
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                using (var args = new ArgsWrapper(fg,
                    $"LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Write"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                    args.Add($"doMasks: {doMaskAccessor}");
                    if (loquiGen.HasIndex)
                    {
                        args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        args.Add($"errorMask: {maskAccessor}");
                    }
                    else
                    {
                        args.Add($"errorMask: out {maskAccessor}");
                    }
                }
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
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string readerAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                if (loquiGen.SingletonType == LoquiType.SingletonLevel.Singleton)
                {
                    if (loquiGen.InterfaceType == LoquiInterfaceType.IGetter) return;
                    fg.AppendLine($"{readerAccessor}.Position -= Constants.SUBRECORD_LENGTH;");
                    using (var args = new ArgsWrapper(fg,
                        $"var tmp = {loquiGen.TargetObjectGeneration.Name}.Create_{ModNickname}"))
                    {
                        args.Add($"reader: {readerAccessor}");
                        args.Add($"doMasks: {doMaskAccessor}");
                        args.Add($"errorMask: out {loquiGen.MaskItemString(MaskType.Error)} createMask");
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"{loquiGen.TargetObjectGeneration.ExtCommonName}.CopyFieldsFrom"))
                    {
                        args.Add($"item: {itemAccessor.DirectAccess}");
                        args.Add("rhs: tmp");
                        args.Add("def: null");
                        args.Add("cmds: null");
                        args.Add("copyMask: null");
                        args.Add($"doErrorMask: {doMaskAccessor}");
                        args.Add($"errorMask: out {loquiGen.MaskItemString(MaskType.Error)} copyMask");
                    }
                    fg.AppendLine($"var loquiMask = {loquiGen.MaskItemString(MaskType.Error)}.Combine(createMask, copyMask);");
                    fg.AppendLine($"{maskAccessor} = loquiMask == null ? null : new MaskItem<Exception, {loquiGen.MaskItemString(MaskType.Error)}>(null, loquiMask);");
                }
                else
                {
                    var data = loquiGen.GetFieldData();
                    using (var args = new ArgsWrapper(fg,
                        $"var {typeGen.Name}tryGet = LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Parse"))
                    {
                        args.Add($"frame: {readerAccessor}");
                        args.Add($"doMasks: {doMaskAccessor}");
                        if (loquiGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                            args.Add($"errorMask: {maskAccessor}");
                        }
                        else
                        {
                            args.Add($"errorMask: out {maskAccessor}");
                        }
                    }
                    if (itemAccessor.PropertyAccess != null)
                    {
                        fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}({typeGen.Name}tryGet);");
                    }
                    else
                    {
                        fg.AppendLine($"if ({typeGen.Name}tryGet.Succeeded)");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"{itemAccessor.DirectAccess} = {typeGen.Name}tryGet.Value;");
                        }
                    }
                }
            }
            else
            {
                UnsafeXmlTranslationGeneration unsafeXml = new UnsafeXmlTranslationGeneration()
                {
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.MaskItemString(MaskType.Error)}>"
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
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string readerAccessor,
            string retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{retAccessor}LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Parse"))
                {
                    args.Add($"frame: {readerAccessor}{(loquiGen.TargetObjectGeneration.GetObjectType() == ObjectType.Subrecord ? ".Spawn(snapToFinalPosition: false)" : null)}");
                    args.Add($"doMasks: {doMaskAccessor}");
                    if (loquiGen.HasIndex)
                    {
                        args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        args.Add($"errorMask: {maskAccessor}");
                    }
                    else
                    {
                        args.Add($"errorMask: out {maskAccessor}");
                    }
                }
            }
            else
            {
                UnsafeXmlTranslationGeneration unsafeXml = new UnsafeXmlTranslationGeneration()
                {
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.MaskItemString(MaskType.Error)}>"
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
